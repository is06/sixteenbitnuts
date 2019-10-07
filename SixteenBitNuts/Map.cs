using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SixteenBitNuts.Editor;
using System.IO;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    /// <summary>
    /// Class representing an in-game map
    /// </summary>
    public class Map : Scene
    {
        private const float TRANSITION_SPEED = 0.03f;

        #region Fields

        // Map
        private readonly string name;
        private int currentSectionIndex;
        private int nextSectionIndex;

        // Edit and debug
        private bool isInMapEditMode;
        private bool keyMapEditModePressed;
        private bool isInSectionEditMode;
        private bool keySectionEditModePressed;
        private bool isInDebugViewMode;
        private bool keyDebugViewPressed;

        // Transitions
        private float transitionDeltaLength;
        private float transitionProgression;
        private float transitionDeltaHyp;
        private float transitionDeltaX;
        private float transitionDeltaY;
        private bool transitionIsFinished;

        #endregion

        #region Properties

        public Dictionary<int, MapSection> Sections
        {
            get
            {
                return sections;
            }
        }
        public MapSection CurrentMapSection
        {
            get
            {
                return sections[currentSectionIndex];
            }
        }
        public Player Player { get; private set; }
        public Camera Camera { get; private set; }
        public int EntityLastIndex { get; set; }

        #endregion

        #region Components

        protected readonly Dictionary<int, MapSection> sections;
        protected MapSectionEditor sectionEditor;
        private readonly MapEditor mapEditor;
        private readonly TransitionGuide transitionGuide;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The reference to the game object</param>
        /// <param name="name">The name identifier of the map (used to load data from file)</param>
        public Map(Game game, string name) : base(game)
        {
            // Fields
            this.name = name;

            Camera = new Camera(this, new Vector2(240, 135), new Viewport(0, 0, 480, 270));

            // Components
            sections = new Dictionary<int, MapSection>();
            sectionEditor = new MapSectionEditor(this);
            transitionGuide = new TransitionGuide();

            // Load map descriptor
            LoadFromFile("Data/maps/" + name + ".map");

            Player = new Player(this, CurrentMapSection.Entities[CurrentMapSection.DefaultSpawnPointName].Position);

            mapEditor = new MapEditor(this);
        }

        /// <summary>
        /// Performs calculations for the map
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (isInMapEditMode)
            {
                mapEditor.Update();
            }
            else
            {
                #region Components positioning

                foreach (KeyValuePair<int, MapSection> pair in sections)
                {
                    pair.Value.Update();
                }

                // Player
                Player.Update();

                // Camera
                Camera.Position = Player.Position - new Vector2(-8, -12);
                Camera.Update();
                if (!Camera.IsMovingToNextSection)
                {
                    transitionGuide.Position = Camera.Position;
                }

                #endregion

                #region Player collision detection

                if (!isInSectionEditMode)
                {
                    // Get the nearest obstacles to test intersection
                    int nearestObstacleLimit = GetIntersectionCount(Player.NextFrameHitBox, sections[currentSectionIndex].Tiles);
                    if (nearestObstacleLimit >= 2)
                    {
                        nearestObstacleLimit -= 1;
                    }
                    List<Tile> nearestObstacles = GetNearestObstaclesFromHitBox(Player.DistanceBox, sections[currentSectionIndex].Tiles, nearestObstacleLimit);

                    foreach (Tile obstacle in nearestObstacles)
                    {
                        if (Player.HitBox.Intersects(obstacle.HitBox))
                        {
                            // Detect the collision side of the obstacle
                            CollisionSide side = CollisionManager.GetCollisionSide(
                                moving: Player.PreviousFrameHitBox,
                                stopped: obstacle.HitBox,
                                movingVelocity: Player.Velocity
                            );

                            // Change player fall state
                            if (Player.IsFalling && side == CollisionSide.Top)
                            {
                                Player.IsFalling = false;
                                Player.WasOnPlatform = true;
                            }

                            // Correct position to prevent intersection
                            Player.Position = CollisionManager.GetCorrectedPosition(Player.HitBox, obstacle.HitBox, side);
                            Player.UpdateHitBoxes();
                        }
                    }

                    // Check for intersection for ground existence
                    bool playerIsIntersectingWithNothing = true;
                    foreach (Tile tile in sections[currentSectionIndex].Tiles)
                    {
                        if (tile.IsObstacle && Player.HitBox.Intersects(tile.HitBox))
                        {
                            playerIsIntersectingWithNothing = false;
                            break;
                        }
                    }
                    // No ground under player's feet: falling
                    if (playerIsIntersectingWithNothing && Player.WasOnPlatform && !Player.IsJumping)
                    {
                        Player.WasOnPlatform = false;
                        Player.IsFalling = true;
                    }
                }

                #endregion

                #region Design and debug commands

                if (!keyDebugViewPressed && Keyboard.GetState().IsKeyDown(Keys.F3))
                {
                    keyDebugViewPressed = true;
                    isInDebugViewMode = !isInDebugViewMode;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.F3))
                {
                    keyDebugViewPressed = false;
                }

                #endregion

                #region MapSection Editor

                    if (!keySectionEditModePressed && Keyboard.GetState().IsKeyDown(Keys.F2))
                {
                    keySectionEditModePressed = true;

                    if (isInSectionEditMode)
                    {
                        isInSectionEditMode = false;
                        Player.IsControllable = true;
                    }
                    else
                    {
                        isInSectionEditMode = true;
                        Player.IsControllable = false;
                    }
                }
                if (Keyboard.GetState().IsKeyUp(Keys.F2))
                {
                    keySectionEditModePressed = false;
                }

                if (isInSectionEditMode)
                {
                    sectionEditor.Update();

                    if (Keyboard.GetState().IsKeyDown(Keys.F12))
                    {
                        SaveToFile("Data/maps/" + name + ".map");
                    }
                }

                #endregion

                #region Section Transition

                if (!isInSectionEditMode && !Camera.IsMovingToNextSection)
                {
                    if (Player.Right > CurrentMapSection.Bounds.Right ||
                        Player.Left < CurrentMapSection.Bounds.Left)
                    {
                        transitionProgression = 0;
                        nextSectionIndex = GetNextSectionIndex();
                        if (nextSectionIndex != -1)
                        {
                            Player.IsControllable = false;
                            Camera.IsMovingToNextSection = true;
                            Camera.CanOverrideLimits = true;
                            Vector2 transitionTargetPosition = sections[nextSectionIndex].GetNearestTransitionPointFrom(Player.Position);
                            Vector2 oppositeAngle = new Vector2(transitionTargetPosition.X, transitionGuide.Position.Y);

                            // Compute deltas
                            transitionDeltaLength = Vector2.Distance(transitionGuide.Position, transitionTargetPosition);
                            transitionDeltaHyp = transitionDeltaLength * TRANSITION_SPEED;

                            // Compute move deltas
                            float deltaX = Vector2.Distance(transitionGuide.Position, oppositeAngle);
                            float deltaY = Vector2.Distance(transitionTargetPosition, oppositeAngle);

                            if (transitionTargetPosition.X < transitionGuide.Position.X)
                                deltaX *= -1;
                            if (transitionTargetPosition.Y < transitionGuide.Position.Y)
                                deltaY *= -1;

                            transitionDeltaX = TRANSITION_SPEED * deltaX;
                            transitionDeltaY = TRANSITION_SPEED * deltaY;
                        }
                    }
                }
                if (Camera.IsMovingToNextSection)
                {
                    // Compute positions
                    transitionGuide.Position = new Vector2(
                        transitionGuide.Position.X + transitionDeltaX,
                        transitionGuide.Position.Y + transitionDeltaY
                    );
                    Player.Position = new Vector2(
                        Player.Position.X + transitionDeltaX / 32,
                        Player.Position.Y + transitionDeltaY / 32
                    );
                    Camera.Position = transitionGuide.Position;

                    transitionProgression += transitionDeltaHyp;

                    // Transition has to be finished
                    if (transitionProgression > transitionDeltaLength)
                    {
                        transitionIsFinished = true;
                    }
                }
                if (transitionIsFinished)
                {
                    Camera.IsMovingToNextSection = false;
                    Camera.CanOverrideLimits = false;
                    Player.IsControllable = true;

                    currentSectionIndex = nextSectionIndex;
                    nextSectionIndex = -1;

                    transitionGuide.Position = CurrentMapSection.Bounds.Center.ToVector2();
                    Camera.Update();

                    transitionIsFinished = false;
                }

                #endregion
            }

            #region Map Editor

            if (!keyMapEditModePressed && Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                keyMapEditModePressed = true;
                isInMapEditMode = !isInMapEditMode;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.F1))
            {
                keyMapEditModePressed = false;
            }

            #endregion
        }

        /// <summary>
        /// Draw all in-game elements for this map
        /// </summary>
        public override void Draw()
        {
            if (isInMapEditMode)
            {
                mapEditor.Draw();
            }
            else
            { 
                foreach (KeyValuePair<int, MapSection> pair in sections)
                {
                    pair.Value.Draw(Camera.Transform);
                }

                Player.Draw(Camera.Transform);
            }

            base.Draw();
        }

        /// <summary>
        /// Draw debug info for in-game elements
        /// </summary>
        public override void DebugDraw()
        {
            if (!isInMapEditMode && isInDebugViewMode)
            {
                foreach (KeyValuePair<int, MapSection> pair in sections)
                {
                    pair.Value.DebugDraw(Camera.Transform);
                }

                Player.DebugDraw(Camera.Transform);
            }

            base.DebugDraw();
        }

        /// <summary>
        /// Draw all design UI elements (HD graphics)
        /// </summary>
        public override void UIDraw(GameTime gameTime)
        {
            if (!isInMapEditMode && isInSectionEditMode)
            {
                sectionEditor.Draw();
            }
            if (isInMapEditMode)
            {
                mapEditor.UIDraw();
            }

            base.UIDraw(gameTime);
        }

        public void LoadSectionFromIndex(int index)
        {
            currentSectionIndex = index;
            Player.Position = CurrentMapSection.Entities[CurrentMapSection.DefaultSpawnPointName].Position;

            isInMapEditMode = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moving"></param>
        /// <param name="tiles"></param>
        /// <returns></returns>
        private int GetIntersectionCount(BoundingBox moving, List<Tile> tiles)
        {
            int count = 0;

            foreach (Tile tile in tiles)
            {
                if (tile.IsObstacle && moving.Intersects(tile.HitBox))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Get the list of the nearest tiles (1 or 2 at equal distance) from the specified hit box
        /// </summary>
        /// <param name="hitBox">The hitbox of the moving object</param>
        /// <param name="tiles">List of obstacles</param>
        /// <param name="limit">Limit count of nearest obstacles</param>
        /// <returns>The list of the nearest tiles from the specified hit box</returns>
        private List<Tile> GetNearestObstaclesFromHitBox(BoundingBox hitBox, List<Tile> tiles, int limit)
        {
            List<Tile> nearestTiles = new List<Tile>();
            List<float> distances = new List<float>();

            foreach (Tile obstacle in tiles)
            {
                distances.Add(CollisionManager.GetDistance(hitBox, obstacle.HitBox));
            }
            distances.Sort();

            foreach (float distance in distances.GetRange(0, limit))
            {
                foreach (Tile obstacle in tiles)
                {
                    if (distance == CollisionManager.GetDistance(hitBox, obstacle.HitBox))
                    {
                        nearestTiles.Add(obstacle);
                    }
                }
            }

            return nearestTiles;
        }

        private int GetNextSectionIndex()
        {
            foreach (KeyValuePair<int, MapSection> pair in sections)
            {
                if (pair.Value != CurrentMapSection)
                {
                    BoundingBox sectionHitBox = new BoundingBox()
                    {
                        Min = new Vector3(pair.Value.Bounds.X, pair.Value.Bounds.Y, 0),
                        Max = new Vector3(pair.Value.Bounds.X + pair.Value.Bounds.Width, pair.Value.Bounds.Y + pair.Value.Bounds.Height, 0)
                    };

                    if (Player.HitBox.Intersects(sectionHitBox))
                    {
                        return pair.Key;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Load map data from a file and parse it
        /// </summary>
        /// <param name="fileName">The file name of the map</param>
        private void LoadFromFile(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);

            if (lines.Length == 0)
            {
                throw new MapException("Map file '" + name + ".map' is empty");
            }

            int sectionIndex = -1;

            foreach (string line in lines)
            {
                string[] components = line.Split(' ');

                switch (components[0])
                {
                    case "se":
                        // Begin section
                        sectionIndex++;
                        sections[sectionIndex] = new MapSection(
                            this,
                            new Rectangle(
                                int.Parse(components[1]),
                                int.Parse(components[2]),
                                int.Parse(components[3]),
                                int.Parse(components[4])
                            ),
                            new Tileset(Game.GraphicsDevice, Game.Content, components[5]),
                            components[6]
                        );
                        break;
                    case "en":
                        // Entities
                        LoadEntity(components[1], sectionIndex, components[2], new Vector2(
                            int.Parse(components[3]),
                            int.Parse(components[4])
                        ));
                        break;
                    case "ti":
                        // Tile
                        int elementId = int.Parse(components[1]);
                        Vector2 position = new Vector2
                        {
                            X = int.Parse(components[2]),
                            Y = int.Parse(components[3])
                        };
                        float layer = float.Parse(components[4]);

                        sections[sectionIndex].Tiles.Add(new Tile(
                            sections[sectionIndex].Tileset,
                            elementId,
                            position,
                            sections[sectionIndex].Tileset.GetSizeFromId(elementId),
                            sections[sectionIndex].Tileset.GetTypeFromId(elementId),
                            layer
                        ));
                        break;
                }
            }
        }

        protected virtual void LoadEntity(string type, int sectionIndex, string name, Vector2 position)
        {
            switch (type)
            {
                case "spawn": // TODO: remove this identifier
                case "SixteenBitNuts.SpawnPoint":
                    sections[sectionIndex].Entities[name] = new SpawnPoint(this)
                    {
                        Position = position
                    };
                    break;
            }
        }

        private void SaveToFile(string filePath)
        {
            List<string> contents = new List<string>
            {
                "map " + name
            };

            foreach (KeyValuePair<int, MapSection> section in sections)
            {
                contents.Add(
                    "se " + section.Value.Bounds.X +
                    " " + section.Value.Bounds.Y +
                    " " + section.Value.Bounds.Width +
                    " " + section.Value.Bounds.Height +
                    " " + section.Value.Tileset.Name +
                    " " + section.Value.DefaultSpawnPointName
                );

                foreach (Tile tile in section.Value.Tiles)
                {
                    contents.Add(
                        "ti " + tile.Id +
                        " " + tile.Position.X +
                        " " + tile.Position.Y +
                        " 0"
                    );
                }
                foreach (KeyValuePair<string, Entity> entity in section.Value.Entities)
                {
                    contents.Add(
                        "en " + entity.Value.GetType() +
                        " " + entity.Key +
                        " " + entity.Value.Position.X +
                        " " + entity.Value.Position.Y
                    );
                }
            }

            File.Delete(filePath);
            File.AppendAllLines(filePath, contents);
        }

        public override void EditCurrentSection()
        {
            isInSectionEditMode = true;
        }

        public override void EditLayout()
        {
            isInMapEditMode = true;
        }
    }
}
