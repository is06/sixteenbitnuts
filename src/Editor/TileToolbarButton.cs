using Microsoft.Xna.Framework;

namespace SixteenBitNuts.Editor
{
    public class TileToolbarButton : ToolbarButton
    {
        private bool isGroup;

        public TileToolbarButton(Toolbar bar, bool isGroup) : base(bar)
        {
            this.isGroup = isGroup;
        }

        public override void Draw()
        {
            base.Draw();

            if (isGroup && GroupName != null)
            {
                var definitions = Toolbar.Editor.Map.CurrentMapSection.Tileset.Groups[GroupName].Definitions;
                if (definitions != null)
                {
                    Id = definitions[TilesetGroupDefinitionType.Single].TileIndex;
                }
            }

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
