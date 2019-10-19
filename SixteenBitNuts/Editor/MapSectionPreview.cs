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

        public MapSectionPreview(MapSection section, SpriteBatch spriteBatch)
        {
            this.section = section;
            this.spriteBatch = spriteBatch;

            pixel = new Texture2D(section.Map.Game.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            previewTilePositions = new List<Point>();
            UpdatePreviewTilesFromRealSection();
        }

        public void Draw()
        {
            foreach (var position in previewTilePositions)
            {
                spriteBatch.Draw(
                    pixel,
                    position.ToVector2() + Position.ToVector2(),
                    Color.RosyBrown
                );
            }
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
