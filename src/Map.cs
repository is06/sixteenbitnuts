using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SixteenBitNuts.Editor;
using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts
{
    public delegate void CollisionHandler(Collider collider, string colliderName, IMapElement element, CollisionSide side);

    /// <summary>
    /// Class representing an in-game map
    /// </summary>
    [Serializable]
    public class Map : Scene, ISerializable
    {
        private const float TRANSITION_SPEED = 0.03f;
        private const float NEAR_ELEMENT_THRESHOLD = 100f;

        public bool ShowMapEditor = false; // F1
        public bool ShowSectionEditor = false; // F2
        public bool ShowDebug = false; // F3

        #region Fields

        // Map
        private int currentSectionIndex;
        private int nextSectionIndex;

        // Edit and debug
        private bool keyMapEditModePressed;
        private bool keySectionEditModePressed;
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
        public string? MusicName { get; set; }
        public Landscape? Landscape { get; set; }
        public Hud Hud { get; }
        public Dictionary<int, MapSection> Sections => sections;
        public MapSection CurrentMapSection => sections[currentSectionIndex];
        public Player? Player
        {
            get
            {
                return player;
            }
            protected set
            {
                player = value;
                if (player != null)
                {
                    colliders["player"] = player;
                }
            }
        }
        public Camera Camera { get; private set; }
        public int EntityLastIndex { get; set; }
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
        public Size Size
        {
            get
            {
                float minTop = 0f;
                float minLeft = 0f;
                float maxRight = 0f;
                float maxBottom = 0f;

                foreach (var section in Sections)
                {
                    if (section.Value.Bounds.Top < minTop)
                        minTop = section.Value.Bounds.Top;

                    if (section.Value.Bounds.Left < minLeft)
                        minLeft = section.Value.Bounds.Left;

                    if (section.Value.Bounds.Right > maxRight)
                        maxRight = section.Value.Bounds.Right;

                    if (section.Value.Bounds.Bottom > maxBottom)
                        maxBottom = section.Value.Bounds.Bottom;
                }

                return new Size(maxRight - minLeft, maxBottom - minTop);
            }
        }

        #endregion

        #region Components

        private Player? player;
        protected readonly Dictionary<int, MapSection> sections;
        protected Dictionary<string, Collider> colliders;
        protected MapSectionEditor sectionEditor;
        protected EntityGenerator generator;

        private readonly MapEditor mapEditor;
        private readonly TransitionGuide transitionGuide;

        #endregion

        #region Event handlers

        public event CollisionHandler? OnColliderCollidesWithElement;

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
            colliders = new Dictionary<string, Collider>();
            generator = new EntityGenerator(this);
            Hud = new Hud(game);

            // Load map descriptor
            LoadFromFile("Content/Descriptors/Maps/" + name + ".map");

            // Initialization of the toolbar only after loading a map
            sectionEditor.InitializeToolbar();

            // Play the specified music if any
            if (MusicName is string musicName)
            {
                Game.AudioManager?.PlayMusic(musicName);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Performs calculations for the map
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ShowMapEditor)
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
                Player?.UpdateHitBoxes();

                // Hud
                Hud.Update(gameTime);

                #endregion

                #region Collision detection

                var nearElements = new List<IMapElement>();
                if (!ShowSectionEditor)
                {
                    // First collision detection pass (all section elements)
                    foreach (var element in CurrentMapSection.Elements)
                    {
                        {
                            // If element has destroying flag: remove it from the list
                            if (element is Entity entity && entity.IsDestroying)
                            {
                                CurrentMapSection.Entities.Remove(entity.Name);
                            }
                        }

                        {
                            // We take only elements that are at below a certain distance from the player
                            if (Player != null && Vector2.Distance(Player.Position, element.Position) <= NEAR_ELEMENT_THRESHOLD)
                            {
                                if (element is Entity entity && entity.IsDestroying)
                                {
                                    continue;
                                }
                                nearElements.Add(element);
                            }
                        }

                        element.DebugColor = Color.LimeGreen;
                    }

                    // Second collision detection pass (near elements)
                    foreach (var element in nearElements)
                    {
                        element.DebugColor = Color.Orange;
                    }
                }

                if (!ShowSectionEditor)
                {
                    bool playerIsIntersectingWithObstacle = false;

                    foreach (var collider in colliders)
                    {
                        if (!collider.Value.IsCollisionEnabled)
                            continue;

                        foreach (var element in CollisionManager.GetResolutionElementsFromHitBox(collider.Value.HitBox, collider.Value.PreviousFrameHitBox, nearElements))
                        {
                            element.DebugColor = Color.Red;

                            if (collider.Value.HitBox.Intersects(element.HitBox))
                            {
                                // Detect the collision side of the obstacle
                                CollisionSide side = CollisionManager.GetCollisionSide(
                                    moving: collider.Value.PreviousFrameHitBox,
                                    stopped: element.HitBox,
                                    movingVelocity: collider.Value.Velocity
                                );

                                // Entity collision event
                                {
                                    if (element is Entity entity)
                                    {
                                        if (element is MusicParamTrigger musicParamTrigger)
                                        {
                                            musicParamTrigger.Trigger();
                                        }

                                        OnColliderCollidesWithElement?.Invoke(collider.Value, collider.Key, entity, side);
                                    }
                                }

                                // Player-only collision correction
                                if (collider.Value is Player && element.IsObstacle)
                                {
                                    playerIsIntersectingWithObstacle = true;

                                    // Correct position to prevent intersection and block player
                                    collider.Value.Position = CollisionManager.GetCorrectedPosition(collider.Value.HitBox, element.HitBox, side);

                                    {
                                        if (Player is PlatformerPlayer player)
                                        {
                                            // Hit the ground
                                            player.IsTouchingTheGround = false;
                                            if (side == CollisionSide.Top)
                                            {
                                                player.IsTouchingTheGround = true;
                                                OnColliderCollidesWithElement?.Invoke(player, collider.Key, element, side);

                                                if (player.IsFalling)
                                                {
                                                    player.IsFalling = false;
                                                    player.WasOnPlatform = true;
                                                }
                                            }

                                            // Hit the ceiling
                                            player.IsTouchingTheCeiling = false;
                                            if (side == CollisionSide.Bottom)
                                            {
                                                player.IsTouchingTheCeiling = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    {
                        if (Player is PlatformerPlayer player)
                        {
                            // No ground under player's feet: falling
                            if (!playerIsIntersectingWithObstacle && player.WasOnPlatform && !player.IsJumping)
                            {
                                player.WasOnPlatform = false;
                                player.IsFalling = true;
                                player.IsTouchingTheGround = false;
                            }
                        }
                    }
                }

                #endregion

                #region Design and debug commands

                if (!keyDebugViewPressed && Keyboard.GetState().IsKeyDown(Keys.F3))
                {
                    keyDebugViewPressed = true;
                    ShowDebug = !ShowDebug;
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

                    if (ShowSectionEditor)
                    {
                        ShowSectionEditor = false;
                        if (Player != null) Player.IsControllable = true;
                    }
                    else
                    {
                        ShowSectionEditor = true;
                        if (Player != null) Player.IsControllable = false;
                    }
                }
                if (Keyboard.GetState().IsKeyUp(Keys.F2))
                {
                    keySectionEditModePressed = false;
                }

                if (ShowSectionEditor)
                {
                    sectionEditor.Update();

                    if (Keyboard.GetState().IsKeyDown(Keys.F12))
                    {
                        SaveToDisk();
                    }
                }

                #endregion

                #region Section Transition

                if (!ShowSectionEditor && !Camera.IsMovingToNextSection)
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

            if (ShowDebug)
            {
                foreach (var collider in colliders)
                {
                    collider.Value.UpdateDebugHitBoxes();
                }
            }

            #region Map Editor

            if (!keyMapEditModePressed && Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                keyMapEditModePressed = true;
                ShowMapEditor = !ShowMapEditor;
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
            if (ShowMapEditor)
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
                        foreach (KeyValuePair<int, MapSection> section in sections)
                        {
                            section.Value.DrawBackground(layerTransform);
                        }

                        Player?.Draw(layerTransform);

                        foreach (KeyValuePair<int, MapSection> section in sections)
                        {
                            section.Value.DrawForeground(layerTransform);
                        }
                    }
                }
            }

            if (!ShowMapEditor && ShowSectionEditor)
            {
                sectionEditor.Draw();
            }

            base.Draw();
        }

        public override void OutGameDraw()
        {
            base.OutGameDraw();

            Hud.Draw(Matrix.Identity);
        }

        /// <summary>
        /// Draw debug info for in-game elements
        /// </summary>
        public override void DebugDraw()
        {
            if (!ShowMapEditor && ShowDebug)
            {
                foreach (var section in sections)
                {
                    section.Value.DebugDraw(Camera.Transform);
                }
                foreach (var collider in colliders)
                {
                    collider.Value.DebugDraw(Camera.Transform);
                }
            }

            base.DebugDraw();
        }

        /// <summary>
        /// Draw all design UI elements (HD graphics)
        /// </summary>
        public override void UIDraw()
        {
            if (!ShowMapEditor && ShowSectionEditor)
            {
                sectionEditor.UIDraw();
            }
            if (ShowMapEditor)
            {
                mapEditor.UIDraw();
            }
        }

        public void LoadSectionFromIndex(int index)
        {
            currentSectionIndex = index;
            ShowMapEditor = false;

            if (Player != null && CurrentMapSection.DefaultSpawnPoint != null)
            {
                Player.Position = CurrentMapSection.DefaultSpawnPoint.Position;
            }
        }

        public void SaveToDisk()
        {
            MapWriter.SaveToFile(MapFileMode.Text, this);
            MapWriter.SaveToFile(MapFileMode.Binary, this);
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

        public Dictionary<string, IEntity> GetEntitiesFromTag(string tag)
        {
            var result = new Dictionary<string, IEntity>();

            foreach (var entity in CurrentMapSection.Entities)
            {
                if (entity.Value.Tag == tag)
                {
                    result.Add(entity.Key, entity.Value);
                }
            }

            return result;
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
            Tileset? tileset = null;

            foreach (string line in lines)
            {
                string[] components = line.Split(' ');

                switch (components[0])
                {
                    case "mu":
                        // Music
                        MusicName = components[1];
                        break;
                    case "bg":
                        // Background
                        Landscape = new Landscape(this, components[1]);
                        break;
                    case "ly":
                        // Background layer
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
                            Texture = Game.Content.Load<Texture2D>("Graphics/Landscapes/" + Landscape.Name + "/" + components[2]),
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
                                components[5]
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
                    case "ts":
                        // Tileset marker (every following ti marker are in the tileset section)
                        tileset = Game.TilesetService.Get(components[1]);
                        sections[sectionIndex].TilesetSections.Add(new TilesetSection() { Tileset = tileset });
                        break;
                    case "ti":
                        // Tile
                        int elementId = int.Parse(components[1]);
                        var position = new Vector2()
                        {
                            X = int.Parse(components[2]),
                            Y = int.Parse(components[3])
                        };
                        if (tileset?.GetSizeFromId(elementId) is Size size && tileset?.GetTypeFromId(elementId) is TileType type)
                        {
                            var tileToAdd = new Tile(this, tileset, elementId, position, size, type);

                            if (tileset != null)
                            {
                                foreach (var tilesetGroup in tileset.Groups)
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
                            }

                            if (tileset?.GetLayerFromId(elementId) == TileLayer.Background)
                                sections[sectionIndex].BackgroundTiles.Add(tileToAdd);
                            else
                                sections[sectionIndex].ForegroundTiles.Add(tileToAdd);
                        }
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
                case "MusicParamTrigger":
                    sections[sectionIndex].Entities[name] = new MusicParamTrigger(this, name)
                    {
                        Position = position,
                        Size = new Size(
                            int.Parse(extraData[5]),
                            int.Parse(extraData[6])
                        ),
                        ParamName = extraData[7],
                        ParamValue = float.Parse(extraData[8], CultureInfo.InvariantCulture)
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
            ShowSectionEditor = true;
        }

        public override void EditLayout()
        {
            ShowMapEditor = true;
        }
    }
}
