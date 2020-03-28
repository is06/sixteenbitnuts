using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SixteenBitNuts.Editor
{
    class MapSectionPreview
    {
        public Point Position { get; set; }

        private readonly Texture2D pixel;
        private readonly MapSectionContainer section;
        private readonly List<Point> previewTilePositions;

        public MapSectionPreview(MapSectionContainer section)
        {
            this.section = section;

            pixel = new Texture2D(section.Map.Game.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            previewTilePositions = new List<Point>();
            UpdatePreviewTilesFromRealSection();
        }

        public void Draw(Matrix transform)
        {
            section.Map.Game.SpriteBatch?.Begin(transformMatrix: transform);

            foreach (var position in previewTilePositions)
            {
                section.Map.Game.SpriteBatch?.Draw(
                    pixel,
                    position.ToVector2() + Position.ToVector2(),
                    Color.RosyBrown
                );
            }

            section.Map.Game.SpriteBatch?.End();
        }

        public void UpdatePreviewTilesFromRealSection()
        {
            previewTilePositions.Clear();

            if (section.RealSection != null)
            {
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
}
