using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SixteenBitNuts.Editor
{
    class MapSectionPreview
    {
        public Point Position { get; set; }

        private readonly SpriteBatch spriteBatch;
        private readonly Texture2D pixel;
        private readonly MapSection section;
        private readonly List<Point> previewTilePositions;

        public MapSectionPreview(MapSection section)
        {
            this.section = section;

            pixel = new Texture2D(section.Map.Game.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch = new SpriteBatch(section.Map.Game.GraphicsDevice);

            previewTilePositions = new List<Point>();
            UpdatePreviewTilesFromRealSection();
        }

        public void Draw(Matrix transform)
        {
            spriteBatch.Begin(transformMatrix: transform);

            foreach (var position in previewTilePositions)
            {
                spriteBatch.Draw(
                    pixel,
                    position.ToVector2() + Position.ToVector2(),
                    Color.RosyBrown
                );
            }

            spriteBatch.End();
        }

        public void UpdatePreviewTilesFromRealSection()
        {
            previewTilePositions.Clear();

            foreach (var tile in section.RealSection.Tiles)
            {
                previewTilePositions.Add(new Point(
                    (int)((tile.Position.X / MapEditor.SCALE) - (section.RealSection.Bounds.Location.X / MapEditor.SCALE)),
                    (int)((tile.Position.Y / MapEditor.SCALE) - (section.RealSection.Bounds.Location.Y / MapEditor.SCALE))
                ));
            }
        }
    }
}
