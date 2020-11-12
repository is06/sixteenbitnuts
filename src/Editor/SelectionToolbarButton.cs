using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Editor
{
    public class SelectionToolbarButton : IconableToolbarButton
    {
        public SelectionToolbarButton(Toolbar toolbar) : base(toolbar)
        {
            Type = ToolbarButtonType.Selection;
            iconTexture = toolbar.Editor.Map.Game.Content.Load<Texture2D>("Engine/editor/selection_tool");
        }
    }
}
