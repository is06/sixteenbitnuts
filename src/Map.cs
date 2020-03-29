using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SixteenBitNuts.Editor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts
{
    public delegate void CollisionHandler(Entity entity);

    /// <summary>
    /// Class representing an in-game map
    /// </summary>
    [Serializable]
    public class Map : Scene, ISerializable
    {
        private const float TRANSITION_SPEED = 0.03f;
        private const float NEAR_ELEMENT_THRESHOLD = 100f;

        #region Fields

        // Map
        private int currentSectionIndex;
        private int nextSectionIndex;

        // Edit and debug
        private bool isInMapEditMode;
        private bool keyMapEditModePressed;
        private bool isInSectionEditMode;
        private bool keySectionEditModePressed;
        protected bool isInDebugViewMode;
        private bool keyDebugViewPressed;

        // Transitions
        private float transitionDeltaLength;
        private float transitionProgression;
        private float transitionDeltaHyp;
        private float transitionDeltaX;
        private float transitionDeltaY;
        private bool transitionIsFinished;

        // Landscape
        private int minLandscapeLayerIndex;
        private int maxLandscapeLayerIndex;

        #endregion

        #region Properties

        public string Name { get; set; }
        public Landscape? Landscape { get; set; }
        public Dictionary<int, MapSection> Sections => sections;
        public MapSection CurrentMapSection => sections[currentSectionIndex];
        public Player? Player { get; protected set; }
        public Camera Camera { get; private set; }
        public int EntityLastIndex { get; set; }
        public bool IsInSectionEditMode => isInSectionEditMode;
        public Vector2 Gravity { get; protected set; }
        public override bool HasPreMainDisplayEffectDraws
        {
            get
            {
                foreach (var entity in CurrentMapSection.Entities)
                {
                    if (entity.Value.IsVisibleInPreMainDisplay)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

        #region Components

        protected readonly Dictionary<int, MapSection> sections;
        protected MapSectionEditor sectionEditor;

        private readonly MapEditor mapEditor;
        private readonly TransitionGuide transitionGuide;

        #endregion

        #region Event handlers

        public event CollisionHandler? OnPlayerCollidesWithEntity;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The reference to the game object</param>
        /// <param name="name">The name identifier of the map (used to load data from file)</param>
        public Map(Game game, string name) : base(game)
        {
            Name = name;

            Camera = new Camera(
                this,
                new Vector2(Game.InternalSize.Width / 2, Game.InternalSize.Height / 2),
                new Viewport(0, 0, (int)Game.InternalSize.Width, (int)Game.InternalSize.Height)
            );

            // Components
            sections = new Dictionary<int, MapSection>();
            transitionGuide = new TransitionGuide();
            mapEditor = new MapEditor(this);
            sectionEditor = new MapSectionEditor(this);

            // Load map descriptor
            LoadFromFile("Data/maps/" + name + ".map");

            // Initialization of the toolbar only after loading a map
            sectionEditor.InitializeToolbar();
        }

        

        /// <summary>
        /// Performs calculations for the map
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isInMapEditMode)
            {
                mapEditor.Update(gameTime);
            }
            else
            {
                #region Components positioning

                // Map Sections
                foreach (var section in sections)
                {
                    section.Value.Update(gameTime);
                }

                // Player
                Player?.Update(gameTime);
                Player?.ComputePhysics();
                Player?.UpdateHitBox();

                #endregion

                #region Collision detection

                var nearElements = new List<IMapElement>();
                if (!isInSectionEditMode)
                {
                    // First collision detection pass (all section elements)
                    foreach (var element in CurrentMapSection.Elements)
                    {
                        // If element has destroying flag: remove it from the list
                        if (element is Entity && element.IsDestroying)
                        {
                            CurrentMapSection.Entities.Remove(((Entity)element).Name);
                        }

                        element.DebugColor = Color.LimeGreen;
                        
                        // We take only elements that are at below a certain distance from the player
                        if (Player != null && Vector2.Distance(Player.Position, element.Position) <= NEAR_ELEMENT_THRESHOLD)
                        {
                            nearElements.Add(element);
                        }
                    }

                    // Second collision detection pass (near elements)
                    foreach (var element in nearElements)
                    {
                        element.DebugColor = Color.Orange;
                    }
                }

                if (!isInSectionEditMode)
                {
                    bool playerIsIntersectingWithObstacle = false;

                    if (Player != null)
                    {
                        foreach (var element in CollisionManager.GetResolutionElementsFromHitBox(Player.HitBox, Player.PreviousFrameHitBox, nearElements))
                        {
                            element.DebugColor = Color.Red;

                            // Player collisions
                            if (Player.HitBox.Intersects(element.HitBox))
                            {
                                // Detect the collision side of the obstacle
                                CollisionSide side = CollisionManager.GetCollisionSide(
                                    moving: Player.PreviousFrameHitBox,
                                    stopped: element.HitBox,
                                    movingVelocity: Player.Velocity
                                );

                                // Entity collision event
                                if (element is Entity entity)
                                {
                                    OnPlayerCollidesWithEntity?.Invoke(entity);
                                }

                                if (element.IsObstacle)
                                {
                                    playerIsIntersectingWithObstacle = true;

                                    // Correct position to prevent intersection and block player
                                    Player.Position = CollisionManager.GetCorrectedPosition(Player.HitBox, element.HitBox, side);

                                    if (Player is PlatformerPlayer hittingPlayer)
                                    {
                                        // Hit the ground
                                        hittingPlayer.IsTouchingTheGround = false;
                                        if (side == CollisionSide.Top)
                                        {
                                            hittingPlayer.IsTouchingTheGround = true;

                                            if (hittingPlayer.IsFalling)
                                            {
                                                hittingPlayer.IsFalling = false;
                                                hittingPlayer.WasOnPlatform = true;
                                            }
                                        }

                                        // Hit the ceiling
                                        hittingPlayer.IsTouchingTheCeiling = false;
                                        if (side == CollisionSide.Bottom)
                                        {
                                            hittingPlayer.IsTouchingTheCeiling = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (Player is PlatformerPlayer fallingPlayer)
                        {
                            // No ground under player's feet: falling
                            if (!playerIsIntersectingWithObstacle && fallingPlayer.WasOnPlatform && !fallingPlayer.IsJumping)
                            {
                                fallingPlayer.WasOnPlatform = false;
                                fallingPlayer.IsFalling = true;
                                fallingPlayer.IsTouchingTheGround = false;
                            } 
                        }
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
                        if (Player != null) Player.IsControllable = true;
                    }
                    else
                    {
                        isInSectionEditMode = true;
                        if (Player != null) Player.IsControllable = false;
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
                        MapWriter.SaveToFile(MapFileMode.Text, this);
                        MapWriter.SaveToFile(MapFileMode.Binary, this);
                    }
                }

                #endregion

                #region Section Transition

                if (!isInSectionEditMode && !Camera.IsMovingToNextSection)
                {
                    if (Player != null)
                    {
                        if (Player.HitBox.Right > CurrentMapSection.Bounds.Right ||
                        Player.HitBox.Left < CurrentMapSection.Bounds.Left)
                        {
                            transitionProgression = 0;
                            nextSectionIndex = GetTransitionNextSectionIndex();
                            if (nextSectionIndex != -1)
                            {
                                Player.IsControllable = false;
                                Camera.IsMovingToNextSection = true;
                                Camera.CanOverrideLimits = true;
                                
                                var nearestTransitionPoint = sections[nextSectionIndex].GetNearestTransitionPointFrom(Player.Position);
                                if (nearestTransitionPoint != null)
                                {
                                    Vector2 transitionTargetPosition = (Vector2)nearestTransitionPoint;
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
                    }
                }
                if (Camera.IsMovingToNextSection)
                {
                    // Compute positions
                    transitionGuide.Position = new Vector2(
                        transitionGuide.Position.X + transitionDeltaX,
                        transitionGuide.Position.Y + transitionDeltaY
                    );
                    if (Player != null)
                    {
                        Player.Position = new Vector2(
                            Player.Position.X + transitionDeltaX / 32,
                            Player.Position.Y + transitionDeltaY / 32
                        );
                    }
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
                    if (Player != null)
                    {
                        Player.IsControllable = true;
                    }

                    currentSectionIndex = nextSectionIndex;
                    nextSectionIndex = -1;

                    transitionGuide.Position = CurrentMapSection.Bounds.Center.ToVector2();
                    Camera.Update(gameTime);

                    transitionIsFinished = false;
                }

                #endregion

                #region Camera

                if (Player != null)
                {
                    Camera.Position = Player.Position - new Vector2(-8, -12);
                }
                Camera.Update(gameTime);
                if (!Camera.IsMovingToNextSection)
                {
                    transitionGuide.Position = Camera.Position;
                }

                #endregion
            }

            if (isInDebugViewMode)
            {
                Player?.UpdateDebugHitBoxes();
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
                for (int layer = minLandscapeLayerIndex; layer <= maxLandscapeLayerIndex; layer++)
                {
                    var layerTransform = Matrix.Identity;
                    var offsetX = 1.0f;
                    var offsetY = 1.0f;

                    if (Landscape != null && Landscape.Layers.ContainsKey(layer))
                    {
                        offsetX = Landscape.Layers[layer].TransformOffset.X / 100;
                        offsetY = Landscape.Layers[layer].TransformOffset.Y / 100;
                    }

                    layerTransform.Translation = new Vector3(
                        Camera.Transform.Translation.X * offsetX,
                        Camera.Transform.Translation.Y * offsetY,
                        0
                    );

                    if (Landscape != null && Landscape.Layers.ContainsKey(layer))
                    {
                        Landscape.Draw(layer, layerTransform);
                    }

                    if (layer == 0)
                    {
                        Player?.Draw(layerTransform);

                        foreach (KeyValuePair<int, MapSection> section in sections)
                        {
                            section.Value.Draw(layer, layerTransform);
                        }
                    }
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
                foreach (var section in sections)
                {
                    section.Value.DebugDraw(Camera.Transform);
                }
                Player?.DebugDraw(Camera.Transform);
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
            isInMapEditMode = false;

            if (Player != null && CurrentMapSection.DefaultSpawnPoint != null)
            {
                Player.Position = CurrentMapSection.DefaultSpawnPoint.Position;
            }
        }

        /// <summary>
        /// Add an entity to the current map section
        /// </summary>
        /// <param name="entity">The entity to add</param>
        protected void AddEntity(Entity entity)
        {
            CurrentMapSection.Entities.Add(entity.Name, entity);
        }

        /// <summary>
        /// Add an entity to a specific map section
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <param name="sectionIndex">The section index where to add the entity</param>
        protected void AddEntity(Entity entity, int sectionIndex)
        {
            sections[sectionIndex].Entities.Add(entity.Name, entity);
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
                        new Size(section.Value.Bounds.Size.X, section.Value.Bounds.Size.Y)
                    );
                    if (Player != null && Player.HitBox.Intersects(sectionHitBox))
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
                throw new GameException("Map file '" + Name + ".map' is empty");
            }

            int sectionIndex = -1;

            foreach (string line in lines)
            {
                string[] components = line.Split(' ');

                switch (components[0])
                {
                    case "bg":
                        Landscape = new Landscape(this, components[1]);
                        break;
                    case "ly":
                        var layerIndex = int.Parse(components[1]);
                        if (layerIndex > maxLandscapeLayerIndex) maxLandscapeLayerIndex = layerIndex;
                        if (layerIndex < minLandscapeLayerIndex) minLandscapeLayerIndex = layerIndex;
                        if (layerIndex == 0)
                        {
                            throw new GameException("Unable to use an index 0 for a landscape layer because it is reserved for main entities and tiles.");
                        }
                        Landscape?.Layers.Add(layerIndex, new LandscapeLayer()
                        {
                            Name = components[2],
                            Index = layerIndex,
                            Texture = Game.Content.Load<Texture2D>("Game/backgrounds/" + Landscape.Name + "/" + components[2]),
                            TransformOffset = new Vector2(int.Parse(components[3]), int.Parse(components[4])),
                        });
                        break;
                    case "se":
                        // Begin section
                        sectionIndex++;
                        try
                        {
                            var bounds = new Rectangle(
                                int.Parse(components[1]),
                                int.Parse(components[2]),
                                int.Parse(components[3]),
                                int.Parse(components[4])
                            );
                            sections[sectionIndex] = new MapSection(
                                this,
                                bounds,
                                Game.TilesetService.Get(components[5]),
                                components[6]
                            );
                            mapEditor.MapSectionContainers.Add(
                                sectionIndex,
                                new MapSectionContainer(this, mapEditor, sectionIndex, bounds)
                            );
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            throw new GameException("Missing a component of section descriptor line (" + e.Message + ")");
                        }
                        break;
                    case "en":
                        // Entities
                        LoadEntity(
                            type: components[1],
                            sectionIndex: sectionIndex,
                            name: components[2],
                            position: new Vector2(
                                int.Parse(components[3]),
                                int.Parse(components[4])
                            ),
                            extraData: components
                        );
                        break;
                    case "ti":
                        // Tile
                        int elementId = int.Parse(components[1]);
                        var position = new Vector2
                        {
                            X = int.Parse(components[2]),
                            Y = int.Parse(components[3])
                        };
                        var tileToAdd = new Tile(
                            this,
                            sections[sectionIndex].Tileset,
                            elementId,
                            position,
                            sections[sectionIndex].Tileset.GetSizeFromId(elementId),
                            sections[sectionIndex].Tileset.GetTypeFromId(elementId)
                        );
                        foreach (var tilesetGroup in CurrentMapSection.Tileset.Groups)
                        {
                            if (tilesetGroup.Value.Definitions != null)
                            {
                                foreach (var definition in tilesetGroup.Value.Definitions)
                                {
                                    if (definition.Value.TileIndex == elementId)
                                    {
                                        tileToAdd.GroupName = tilesetGroup.Value.Name;
                                        break;
                                    }
                                }
                            }
                        }
                        sections[sectionIndex].Tiles.Add(tileToAdd);
                        break;
                }
            }
        }

        
        protected virtual void LoadEntity(string type, int sectionIndex, string name, Vector2 position, string[] extraData)
        {
            switch (type)
            {
                case "SpawnPoint":
                    sections[sectionIndex].Entities[name] = new SpawnPoint(this, name)
                    {
                        Position = position
                    };
                    break;
                case "EventTrigger":
                    sections[sectionIndex].Entities[name] = new EventTrigger(this, name)
                    {
                        Position = position,
                        Size = new Size(
                            int.Parse(extraData[5]),
                            int.Parse(extraData[6])
                        )
                    };
                    break;
                case "Teleport":
                    sections[sectionIndex].Entities[name] = new Teleport(this, name)
                    {
                        Position = position,
                        Size = new Size(
                            int.Parse(extraData[5]),
                            int.Parse(extraData[6])
                        ),
                        DestinationPoint = new Vector2(
                            int.Parse(extraData[7]),
                            int.Parse(extraData[8])
                        )
                    };
                    break;
            }
        }

        /// <summary>
        /// Serialization of map
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("landscape", Landscape);
            info.AddValue("sections", sections);
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
