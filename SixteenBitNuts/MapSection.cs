using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    /// <summary>
    /// Representation of a map section
    /// Contains the tileset and tile list
    /// </summary>
    public class MapSection
    {
        private Vector2[] transitionPoints;
        private readonly Map map;
        private readonly SpriteBatch spriteBatch;
        private readonly Texture2D transitionCornerTexture;
        private readonly string defaultSpawnPointName;

        #region Properties

        public Rectangle Bounds { get; set; }
        public Tileset Tileset { get; private set; }
        public List<Tile> Tiles { get; set; }
        public Dictionary<string, Entity> Entities { get; set; }
        public List<MapElement> Obstacles
        {
            get
            {
                List<MapElement> obstacles = new List<MapElement>();
                foreach (var tile in Tiles)
                {
                    if (tile.IsObstacle)
                    {
                        obstacles.Add(tile);
                    }
                }
                foreach (var entity in Entities)
                {
                    if (entity.Value.IsObstacle)
                    {
                        obstacles.Add(new MapElement()
                        {
                            IsObstacle = true,
                            Size = entity.Value.Size,
                            Position = entity.Value.Position
                        });
                    }
                }

                return obstacles;
            }
        }
        public SpawnPoint DefaultSpawnPoint
        {
            get
            {
                return (SpawnPoint)Entities[defaultSpawnPointName];
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MapSection(Map map, Rectangle bounds, Tileset tileset, string defaultSpawnPointName)
        {
            this.map = map;
            this.defaultSpawnPointName = defaultSpawnPointName;
            spriteBatch = new SpriteBatch(map.Graphics);
            transitionCornerTexture = map.Content.Load<Texture2D>("Engine/editor/transition_corner");

            // Properties
            Bounds = bounds;
            Tileset = tileset;
            Tiles = new List<Tile>();
            Entities = new Dictionary<string, Entity>();

            SetTransitionPoints(bounds);
        }


        /// <summary>
        /// Performs calculations for the map section
        /// </summary>
        public void Update()
        {
            foreach (KeyValuePair<string, Entity> pair in Entities)
            {
                pair.Value.Update();
            }
        }

        /// <summary>
        /// Draw all tiles
        /// </summary>
        public void Draw(Matrix transform)
        {
            foreach (Tile tile in Tiles)
            {
                tile.Draw(transform);
            }
            foreach (KeyValuePair<string, Entity> pair in Entities)
            {
                pair.Value.Draw(transform);
            }

            if (map.IsInSectionEditMode)
            {
                foreach (KeyValuePair<string, Entity> pair in Entities)
                {
                    pair.Value.EditorDraw(transform);
                }
            }
        }

        /// <summary>
        /// Draw debug info for in-game elements
        /// </summary>
        public void DebugDraw(Matrix transform)
        {
            foreach (Tile tile in Tiles)
            {
                tile.DebugDraw(transform);
            }
            foreach (KeyValuePair<string, Entity> pair in Entities)
            {
                pair.Value.DebugDraw(transform);
            }
            for (int i = 0; i < transitionPoints.Length; i++)
            {
                spriteBatch.Begin(transformMatrix: transform);
                spriteBatch.Draw(
                    transitionCornerTexture,
                    new Vector2(transitionPoints[i].X, transitionPoints[i].Y),
                    new Rectangle(0, 0, 16, 16),
                    Color.White
                );
                spriteBatch.End();
            }
        }

        public Vector2 GetNearestTransitionPointFrom(Vector2 position)
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

        public void UpdateAllPositions(Point positionOffset, Point size)
        {
            if (positionOffset == Point.Zero)
            {
                return;
            }

            // Move player if inside the section
            if (Bounds.Contains(map.Player.Position))
            {
                map.Player.Position = new Vector2(map.Player.Position.X + positionOffset.X, map.Player.Position.Y + positionOffset.Y);
            }

            // Move camera if inside the section
            if (Bounds.Contains(map.Camera.Position))
            {
                map.Camera.Position = new Vector2(map.Camera.Position.X + positionOffset.X, map.Camera.Position.Y + positionOffset.Y);
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
            foreach (KeyValuePair<string, Entity> entity in Entities)
            {
                entity.Value.Position = new Vector2(
                    entity.Value.Position.X + positionOffset.X,
                    entity.Value.Position.Y + positionOffset.Y
                );
            }
        }

        private void SetTransitionPoints(Rectangle bounds)
        {
            transitionPoints = new Vector2[] {
                new Vector2(bounds.X + 240, bounds.Y + 135),
                new Vector2(bounds.X + bounds.Width - 240, bounds.Y + 135),
                new Vector2(bounds.X + 240, bounds.Y + bounds.Height - 135),
                new Vector2(bounds.X + bounds.Width - 240, bounds.Y + bounds.Height - 135)
            };
        }
    }
}
