using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Editor
{
    public class TileToolbarButton : ToolbarButton
    {
        private readonly bool isGroup;

        public TileToolbarButton(Toolbar toolbar, bool isGroup) : base(toolbar)
        {
            Type = ToolbarButtonType.Tile;
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

                tileset.Game.SpriteBatch?.Begin(transformMatrix: Matrix.Identity, samplerState: SamplerState.PointClamp);

                tileset.Draw(
                    Position + new Vector2(6, 6),
                    tileset.GetTileBoundFromId(Id),
                    new Vector2(3, 3)
                );

                tileset.Game.SpriteBatch?.End();
            }
        }
    }
}
