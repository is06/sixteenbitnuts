using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts
{
    /// <summary>
    /// Representation of a map section
    /// Contains the tileset and tile list
    /// </summary>
    [Serializable]
    public class MapSection : ISerializable
    {
        private Vector2[]? transitionPoints;
        private readonly Texture2D transitionCornerTexture;
        private readonly string defaultSpawnPointName;

        #region Properties

        public Map Map { get; private set; }
        public Rectangle Bounds { get; set; }
        public Tileset Tileset { get; private set; }
        public List<ITile> Tiles { get; set; }
        public Dictionary<string, IEntity> Entities { get; set; }

        public List<IMapElement> Elements
        {
            get
            {
                var elements = new List<IMapElement>();
                foreach (var tile in Tiles)
                {
                    elements.Add(tile);
                }
                foreach (var entity in Entities)
                {
                    elements.Add(entity.Value);
                }

                return elements;
            }
        }

        public ISpawnPoint DefaultSpawnPoint
        {
            get
            {
                return (ISpawnPoint)Entities[defaultSpawnPointName];
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MapSection(Map map, Rectangle bounds, Tileset tileset, string defaultSpawnPointName)
        {
            Map = map;
            this.defaultSpawnPointName = defaultSpawnPointName;
            transitionCornerTexture = Map.Game.Content.Load<Texture2D>("Engine/editor/transition_corner");

            // Properties
            Bounds = bounds;
            Tileset = tileset;

            Tiles = new List<ITile>();
            Entities = new Dictionary<string, IEntity>();

            SetTransitionPoints(bounds);
        }


        /// <summary>
        /// Performs calculations for the map section
        /// </summary>
        public void Update(GameTime gameTime)
        {
            foreach (KeyValuePair<string, IEntity> pair in Entities)
            {
                pair.Value.Update(gameTime);
            }
        }

        /// <summary>
        /// Draw all tiles
        /// </summary>
        public void Draw(int layer)
        {
            // Main layer drawables
            if (layer == (int)LayerIndex.Main)
            {
                foreach (Tile tile in Tiles)
                {
                    tile.Draw();
                }
                foreach (KeyValuePair<string, IEntity> pair in Entities)
                {
                    pair.Value.Draw();
                }
                if (Map.IsInSectionEditMode)
                {
                    foreach (KeyValuePair<string, IEntity> pair in Entities)
                    {
                        pair.Value.EditorDraw();
                    }
                }
            }
        }

        /// <summary>
        /// Draw debug info for in-game elements
        /// </summary>
        public void DebugDraw()
        {
            foreach (Tile tile in Tiles)
            {
                tile.DebugDraw();
            }
            foreach (KeyValuePair<string, IEntity> pair in Entities)
            {
                pair.Value.DebugDraw();
            }
            if (transitionPoints != null)
            {
                for (int i = 0; i < transitionPoints.Length; i++)
                {
                    Map.Game.SpriteBatch?.Draw(
                        transitionCornerTexture,
                        new Vector2(transitionPoints[i].X, transitionPoints[i].Y),
                        new Rectangle(0, 0, 16, 16),
                        Color.White
                    );
                }
            }
        }

        public Vector2? GetNearestTransitionPointFrom(Vector2 position)
        {
            if (transitionPoints != null)
            {
                Vector2 nearestPoint = transitionPoints[0];
                float min = Vector2.Distance(transitionPoints[0], position);

                foreach (Vector2 point in transitionPoints)
                {
                    float distance = Vector2.Distance(point, position);

                    if (distance < min)
                    {
                        min = distance;
                        nearestPoint = point;
                    }
                }

                return nearestPoint;
            }

            return null;
        }

        public void UpdateAllPositions(Point positionOffset, Point size)
        {
            if (positionOffset == Point.Zero)
            {
                Bounds = new Rectangle(Bounds.X, Bounds.Y, size.X, size.Y);
                return;
            }

            // Move player if inside the section
            if (Map.Player != null && Bounds.Contains(Map.Player.Position))
            {
                Map.Player.Position = new Vector2(Map.Player.Position.X + positionOffset.X, Map.Player.Position.Y + positionOffset.Y);
            }

            // Move camera if inside the section
            if (Bounds.Contains(Map.Camera.Position))
            {
                Map.Camera.Position = new Vector2(Map.Camera.Position.X + positionOffset.X, Map.Camera.Position.Y + positionOffset.Y);
            }

            // Move bounds of map section
            Bounds = new Rectangle(Bounds.X + positionOffset.X, Bounds.Y + positionOffset.Y, size.X, size.Y);

            // Move transition points
            SetTransitionPoints(Bounds);

            // Move tiles in the section
            foreach (Tile tile in Tiles)
            {
                tile.Position = new Vector2(tile.Position.X + positionOffset.X, tile.Position.Y + positionOffset.Y);
            }

            // Move entities in the section
            foreach (KeyValuePair<string, IEntity> entity in Entities)
            {
                entity.Value.Position = new Vector2(
                    entity.Value.Position.X + positionOffset.X,
                    entity.Value.Position.Y + positionOffset.Y
                );
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("bounds.X", Bounds.X);
            info.AddValue("bounds.Y", Bounds.Y);
            info.AddValue("bounds.Width", Bounds.Width);
            info.AddValue("bounds.Height", Bounds.Height);
            info.AddValue("tileset", Tileset.Name);
            info.AddValue("defaultSpawnPoint", DefaultSpawnPoint.Name);
            info.AddValue("tiles", Tiles);
            info.AddValue("entities", Entities);
        }

        private void SetTransitionPoints(Rectangle bounds)
        {
            transitionPoints = new Vector2[] {
                new Vector2(bounds.X + Map.Game.InternalSize.Width / 2, bounds.Y + Map.Game.InternalSize.Height / 2),
                new Vector2(bounds.X + bounds.Width - Map.Game.InternalSize.Width / 2, bounds.Y + Map.Game.InternalSize.Height / 2),
                new Vector2(bounds.X + Map.Game.InternalSize.Width / 2, bounds.Y + bounds.Height - Map.Game.InternalSize.Height / 2),
                new Vector2(bounds.X + bounds.Width - Map.Game.InternalSize.Width / 2, bounds.Y + bounds.Height - Map.Game.InternalSize.Height / 2)
            };
        }
    }
}
