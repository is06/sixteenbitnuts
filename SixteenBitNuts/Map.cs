using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SixteenBitNuts.Editor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts
{
    public delegate void CollisionHandler(Entity entity);

    /// <summary>
    /// Class representing an in-game map
    /// </summary>
    public class Map : Scene
    {
        private const float TRANSITION_SPEED = 0.03f;
        private const float NEAR_ELEMENT_THRESHOLD = 100f;

        public static Matrix[] parallaxTransforms;

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

        // Death
        private readonly Timer deathTimer;

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
        public Player Player { get; protected set; }
        public Camera Camera { get; private set; }
        public int EntityLastIndex { get; set; }
        public bool IsInSectionEditMode
        {
            get
            {
                return isInSectionEditMode;
            }
        }

        #endregion

        #region Components

        protected readonly Dictionary<int, MapSection> sections;
        protected MapSectionEditor sectionEditor;
        private MapEditor mapEditor;
        private readonly TransitionGuide transitionGuide;
        private readonly Vector2[] layerOffsetFactors;
        private Landscape landscape;
        private Label debugPlayerPosition;

        #endregion

        #region Events

        public event CollisionHandler OnCollisionWithEntity;
        public event CollisionHandler OnAttackEntity;
        public event CollisionHandler OnDashWithEntity;

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

            Camera = new Camera(
                this,
                new Vector2(Game.InternalSize.Width / 2, Game.InternalSize.Height / 2),
                new Viewport(0, 0, Game.InternalSize.Width, Game.InternalSize.Height)
            );

            // Components
            sections = new Dictionary<int, MapSection>();
            transitionGuide = new TransitionGuide();
            deathTimer = new Timer { Duration = 2 };
            deathTimer.OnTimerFinished += DeathTimer_OnTimerFinished;

            // Load map descriptor
            LoadFromFile("Data/maps/" + name + ".map");

            // Overridable initializers
            InitPlayer();
            InitMapSectionEditor();
            InitMapEditor();

            // Layer transformations
            layerOffsetFactors = new Vector2[8];
            layerOffsetFactors[(int)LayerIndex.StaticBackground] = Vector2.Zero;
            layerOffsetFactors[(int)LayerIndex.Background4] = new Vector2(0.1f, 0.1f);
            layerOffsetFactors[(int)LayerIndex.Background3] = new Vector2(0.2f, 0.2f);
            layerOffsetFactors[(int)LayerIndex.Background2] = new Vector2(0.4f, 0.4f);
            layerOffsetFactors[(int)LayerIndex.Background1] = new Vector2(0.6f, 0.6f);
            layerOffsetFactors[(int)LayerIndex.Main] = new Vector2(1, 1);
            layerOffsetFactors[(int)LayerIndex.Foreground1] = new Vector2(1.4f, 1.4f);
            layerOffsetFactors[(int)LayerIndex.Foreground2] = new Vector2(1.7f, 1.7f);

            debugPlayerPosition = new Label(this)
            {
                Position = new Vector2(4, 4),
                Color = Color.White,
                IsVisible = true,
            };
        }

        protected virtual void InitPlayer()
        {
            Player = new Player(this, CurrentMapSection.DefaultSpawnPoint.Position);
        }

        protected virtual void InitMapEditor()
        {
            mapEditor = new MapEditor(this);
        }

        protected virtual void InitMapSectionEditor()
        {
            sectionEditor = new MapSectionEditor(this);
        }

        private void DeathTimer_OnTimerFinished()
        {
            Player.Position = CurrentMapSection.DefaultSpawnPoint.Position;
            Player.IsControllable = true;
        }

        /// <summary>
        /// Performs calculations for the map
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            deathTimer.Update(gameTime);

            if (isInMapEditMode)
            {
                mapEditor.Update(gameTime);
            }
            else
            {
                #region Components positioning

                foreach (KeyValuePair<int, MapSection> pair in sections)
                {
                    pair.Value.Update(gameTime);
                }

                // Player
                Player.Update(gameTime);

                // Camera
                Camera.Position = Player.Position - new Vector2(-8, -12);
                Camera.Update(gameTime);
                if (!Camera.IsMovingToNextSection)
                {
                    transitionGuide.Position = Camera.Position;
                }

                #endregion

                #region MapElement handle

                var nearElements = new List<IMapElement>();
                if (!isInSectionEditMode)
                {
                    // First collision detection pass:
                    // We take only elements that are at below a certain distance from the player
                    foreach (var element in CurrentMapSection.Elements)
                    {
                        // If element has destroying flag: remove it from the list
                        if (element is Entity && element.IsDestroying)
                        {
                            CurrentMapSection.Entities.Remove(((Entity)element).Name);
                        }

                        element.DebugColor = Color.LimeGreen;

                        if (Vector2.Distance(Player.Position, element.Position) <= NEAR_ELEMENT_THRESHOLD)
                        {
                            nearElements.Add(element);
                        }
                    }

                    // Second collision detection pass:
                    // We check for attacks
                    foreach (var element in nearElements)
                    {
                        element.DebugColor = Color.Orange;

                        // Attack collisions
                        if (Player.IsAttacking && Player.AttackBox.Intersects(element.HitBox))
                        {
                            if (element is Entity)
                            {
                                OnAttackEntity?.Invoke((Entity)element);
                            }
                        }
                    }
                }

                #endregion

                #region Collision detection

                if (!isInSectionEditMode)
                {
                    // Get the nearest obstacles to test intersection
                    int nearestElementLimit = nearElements.Aggregate(0, (limit, element) =>
                    {
                        if (Player.NextFrameHitBox.Intersects(element.HitBox))
                        {
                            limit++;
                        }

                        return limit;
                    });
                    if (nearestElementLimit >= 2)
                    {
                        nearestElementLimit -= 1;
                    }
                    var nearestElements = GetNearestElementsFromHitBox(Player.DistanceBox, nearElements, nearestElementLimit);

                    foreach (var element in nearestElements)
                    {
                        element.DebugColor = Color.Red;

                        // Player collisions
                        if (Player.HitBox.Intersects(element.HitBox))
                        {
                            // Entity collision event
                            if (element is Entity)
                            {
                                if (Player.IsDashFalling)
                                {
                                    OnDashWithEntity?.Invoke((Entity)element);
                                }
                                OnCollisionWithEntity?.Invoke((Entity)element);
                            }

                            // If player was dashing when colliding with some obstacle,
                            // player is controllable again
                            if (Player.IsDashFalling)
                            {
                                if (element.IsObstacle || element.IsPlatform)
                                {
                                    Player.IsDashing = false;
                                    Player.IsDashFalling = false;
                                    Player.IsControllable = true;
                                }
                            }

                            // Detect the collision side of the obstacle
                            if (element.IsObstacle)
                            {
                                CollisionSide side = CollisionManager.GetCollisionSide(
                                    moving: Player.PreviousFrameHitBox,
                                    stopped: element.HitBox,
                                    movingVelocity: Player.Velocity
                                );

                                // Change player fall state
                                if (Player.IsFalling && side == CollisionSide.Top)
                                {
                                    Player.IsFalling = false;
                                    Player.WasOnPlatform = true;
                                }

                                // Correct position to prevent intersection
                                Player.Position = CollisionManager.GetCorrectedPosition(Player.HitBox, element.HitBox, side);
                            }
                        }
                    }

                    // Check for intersection for ground existence
                    bool playerIsNotIntersectingWithObstacle = true;
                    foreach (var element in CurrentMapSection.Elements)
                    {
                        if (element.IsObstacle && Player.HitBox.Intersects(element.HitBox))
                        {
                            playerIsNotIntersectingWithObstacle = false;
                            break;
                        }
                    }
                    // No ground under player's feet: falling
                    if (playerIsNotIntersectingWithObstacle && Player.WasOnPlatform && !Player.IsJumping && !Player.IsDashing)
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
                        nextSectionIndex = GetTransitionNextSectionIndex();
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
                    Camera.Update(gameTime);

                    transitionIsFinished = false;
                }

                #endregion

                #region Player falls down

                if (Player.Position.Y >= CurrentMapSection.Bounds.Bottom)
                {
                    Player.IsControllable = false;
                    deathTimer.Active = true;
                }

                #endregion
            }

            if (isInDebugViewMode)
            {
                Player.UpdateDebugHitBoxes();

                debugPlayerPosition.Text = Player.Position.ToString();
                debugPlayerPosition.Update();
            }

            #region Map Editor

            if (!keyMapEditModePressed && Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                keyMapEditModePressed = true;
                isInMapEditMode = !isInMapEditMode;
                mapEditor.UpdateLayout();
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
                for (int layer = 0; layer < 8; layer++)
                {
                    var layerTransform = Matrix.Identity;
                    layerTransform.Translation = new Vector3(
                        Camera.Transform.Translation.X * layerOffsetFactors[layer].X,
                        Camera.Transform.Translation.Y * layerOffsetFactors[layer].Y,
                        0
                    );

                    Game.SpriteBatch.Begin(transformMatrix: layerTransform, samplerState: SamplerState.PointWrap);

                    if (landscape != null)
                    {
                        landscape.Draw(layer);
                    }

                    foreach (KeyValuePair<int, MapSection> section in sections)
                    {
                        section.Value.Draw(layer);
                    }

                    if (layer == (int)LayerIndex.Main)
                    {
                        Player.Draw();
                    }

                    Game.SpriteBatch.End();
                }
            }

            if (!isInMapEditMode && isInSectionEditMode)
            {
                sectionEditor.Draw();
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
                for (int layer = -4; layer <= 2; layer++)
                {
                    Game.SpriteBatch.Begin(transformMatrix: Camera.Transform);

                    foreach (var section in sections)
                    {
                        section.Value.DebugDraw();
                    }
                    if (layer == 0)
                    {
                        Player.DebugDraw();
                    }

                    Game.SpriteBatch.End();
                }

                Game.SpriteBatch.Begin();

                debugPlayerPosition.Draw();

                Game.SpriteBatch.End();
            }

            

            base.DebugDraw();
        }

        /// <summary>
        /// Draw all design UI elements (HD graphics)
        /// </summary>
        public override void UIDraw()
        {
            if (!isInMapEditMode && isInSectionEditMode)
            {
                sectionEditor.UIDraw();
            }
            if (isInMapEditMode)
            {
                mapEditor.UIDraw();
            }
        }

        public void LoadSectionFromIndex(int index)
        {
            currentSectionIndex = index;
            Player.Position = CurrentMapSection.DefaultSpawnPoint.Position;

            isInMapEditMode = false;
        }

        /// <summary>
        /// Get the list of the nearest elements (1 or 2 at equal distance) from the specified hit box
        /// </summary>
        /// <param name="hitBox">The hitbox of the moving object</param>
        /// <param name="elements">List of elements</param>
        /// <param name="limit">Limit count of nearest elements</param>
        /// <returns>The list of the nearest elements from the specified hit box</returns>
        private List<IMapElement> GetNearestElementsFromHitBox(HitBox hitBox, IEnumerable<IMapElement> elements, int limit)
        {
            var nearestElements = new List<IMapElement>();
            var distances = new List<float>();

            foreach (var element in elements)
            {
                distances.Add(CollisionManager.GetDistance(hitBox, element.HitBox));
            }
            distances.Sort();

            foreach (float distance in distances.GetRange(0, limit))
            {
                foreach (var element in elements)
                {
                    if (distance == CollisionManager.GetDistance(hitBox, element.HitBox))
                    {
                        nearestElements.Add(element);
                    }
                }
            }

            return nearestElements;
        }

        /// <summary>
        /// Return the index of the next section for the transition
        /// </summary>
        /// <returns>Integer representing the index</returns>
        private int GetTransitionNextSectionIndex()
        {
            foreach (var section in sections)
            {
                if (section.Value != CurrentMapSection)
                {
                    var sectionHitBox = new HitBox(
                        section.Value.Bounds.Location.ToVector2(),
                        section.Value.Bounds.Size.ToVector2()
                    );
                    if (Player.HitBox.Intersects(sectionHitBox))
                    {
                        return section.Key;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Load map data from a file and parse it
        /// </summary>
        /// <param name="fileName">The file name of the map</param>
        protected virtual void LoadFromFile(string fileName)
        {
            string[] lines;

            try
            {
                lines = File.ReadAllLines(fileName);
            }
            catch (DirectoryNotFoundException)
            {
                throw new GameException("Unable to find the map file " + fileName);
            }

            if (lines.Length == 0)
            {
                throw new GameException("Map file '" + name + ".map' is empty");
            }

            int sectionIndex = -1;

            foreach (string line in lines)
            {
                string[] components = line.Split(' ');

                switch (components[0])
                {
                    case "bg":
                        landscape = new Landscape(this)
                        {
                            Name = components[1]
                        };
                        break;
                    case "ly":
                        landscape.Layers.Add(new LandscapeLayer()
                        {
                            Name = components[2],
                            Index = (LayerIndex)int.Parse(components[1]),
                            Texture = Game.Content.Load<Texture2D>("Game/backgrounds/" + components[2])
                        });
                        break;
                    case "se":
                        // Begin section
                        sectionIndex++;
                        try
                        {
                            sections[sectionIndex] = new MapSection(
                                this,
                                new Rectangle(
                                    int.Parse(components[1]),
                                    int.Parse(components[2]),
                                    int.Parse(components[3]),
                                    int.Parse(components[4])
                                ),
                                new Tileset(Game, components[5]),
                                components[6]
                            );
                        }
                        catch (System.Exception)
                        {
                            throw new GameException("Section deleclaration doest not conform to correct syntax: se x y width height tileset_name default_spawn");
                        }
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

                        sections[sectionIndex].Tiles.Add(new Tile(
                            this,
                            sections[sectionIndex].Tileset,
                            elementId,
                            position,
                            sections[sectionIndex].Tileset.GetSizeFromId(elementId),
                            sections[sectionIndex].Tileset.GetTypeFromId(elementId)
                        ));
                        break;
                }
            }
        }

        protected virtual void LoadEntity(string type, int sectionIndex, string name, Vector2 position)
        {
            switch (type)
            {
                case "SpawnPoint":
                    sections[sectionIndex].Entities[name] = new SpawnPoint(this, name)
                    {
                        Position = position
                    };
                    break;
            }
        }

        private void SaveToFile(string filePath)
        {
            var contents = new List<string>
            {
                "map " + name,
                "bg " + landscape.Name,
            };

            foreach (var layer in landscape.Layers)
            {
                contents.Add("ly " + (int)layer.Index + " " + layer.Name);
            }

            foreach (var section in sections)
            {
                contents.Add(
                    "se " + section.Value.Bounds.X +
                    " " + section.Value.Bounds.Y +
                    " " + section.Value.Bounds.Width +
                    " " + section.Value.Bounds.Height +
                    " " + section.Value.Tileset.Name +
                    " " + section.Value.DefaultSpawnPoint.Name
                );

                foreach (var tile in section.Value.Tiles)
                {
                    contents.Add(
                        "ti " + tile.Id +
                        " " + tile.Position.X +
                        " " + tile.Position.Y +
                        " 0"
                    );
                }
                foreach (var entity in section.Value.Entities)
                {
                    contents.Add(
                        "en " + entity.Value.GetType().Name +
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
