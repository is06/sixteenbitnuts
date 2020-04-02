using Microsoft.Xna.Framework;

namespace SixteenBitNuts.Editor
{
    public class TileToolbarButton : ToolbarButton
    {
        private readonly bool isGroup;

        public TileToolbarButton(Toolbar bar, bool isGroup) : base(bar)
        {
            this.isGroup = isGroup;
        }

        public override void Draw()
        {
            base.Draw();

            if (Tileset is Tileset tileset)
            {
                if (isGroup && GroupName != null)
                {
                    var definitions = tileset.Groups[GroupName].Definitions;
                    if (definitions != null)
                    {
                        Id = definitions[TilesetGroupDefinitionType.Single].TileIndex;
                    }
                }

                tileset.Draw(
                    Position + new Vector2(6, 6),
                    tileset.GetSizeFromId(Id).ToVector2(),
                    tileset.GetOffsetFromId(Id),
                    new Vector2(3, 3),
                    Matrix.Identity
                );
            }
        }
    }
}
