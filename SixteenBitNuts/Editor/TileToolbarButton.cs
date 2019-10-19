using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Editor
{
    public class TileToolbarButton : ToolbarButton
    {
        public TileToolbarButton(Toolbar bar, SpriteBatch spriteBatch) : base(bar, spriteBatch)
        {

        }

        public override void Draw()
        {
            base.Draw();

            Toolbar.Editor.Map.CurrentMapSection.Tileset.Draw(
                Position + new Vector2(2, 2),
                Toolbar.Editor.Map.CurrentMapSection.Tileset.GetSizeFromId(Id),
                Toolbar.Editor.Map.CurrentMapSection.Tileset.GetOffsetFromId(Id),
                new Vector2(3, 3)
            );
        }
    }
}
