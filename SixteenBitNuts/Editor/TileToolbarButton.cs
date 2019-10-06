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
                Position + new Vector2(2, 2),
                Toolbar.Editor.Map.CurrentMapSection.Tileset.GetSizeFromId(Id),
                Toolbar.Editor.Map.CurrentMapSection.Tileset.GetOffsetFromId(Id),
                0,
                Matrix.CreateScale(Toolbar.Editor.Map.Game.ScreenScale)
            );
        }
    }
}
