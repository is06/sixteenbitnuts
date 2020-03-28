using Microsoft.Xna.Framework;

namespace SixteenBitNuts.Editor
{
    public class TileToolbarButton : ToolbarButton
    {
        public TileToolbarButton(Toolbar bar) : base(bar)
        {

        }

        public override void Draw()
        {
            base.Draw();

            Toolbar.Editor.Map.CurrentMapSection.Tileset.Draw(
                Position + new Vector2(6, 6),
                Toolbar.Editor.Map.CurrentMapSection.Tileset.GetSizeFromId(Id).ToVector2(),
                Toolbar.Editor.Map.CurrentMapSection.Tileset.GetOffsetFromId(Id),
                new Vector2(3, 3),
                Matrix.Identity
            );
        }
    }
}
