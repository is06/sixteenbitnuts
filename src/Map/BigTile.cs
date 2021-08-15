using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts
{
    public class BigTile : MapElement, IBigTile
    {
        private readonly Texture2D texture;

        public BigTile(Map map, string textureName, HitBox hitBox) : base(map, hitBox)
        {
            texture = map.Game.Content.Load<Texture2D>("Graphics/Sprites/BigTiles/" + textureName);
        }

        public override void Draw(Matrix transform)
        {
            map.Game.SpriteBatch?.Begin(transformMatrix: transform, samplerState: SamplerState.PointClamp);

            map.Game.SpriteBatch?.Draw(
                texture: texture,
                destinationRectangle: new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height),
                color: Color.White
            );

            map.Game.SpriteBatch?.End();
        }

        public string MapTextDescription =>
            "bt " +
            texture.Name + " " +
            Position.X + " " + Position.Y + " " +
            HitBox.Width + " " + HitBox.Height + " " +
            (HitBox.Position.X - Position.X) + " " + (HitBox.Position.Y - Position.Y) + " " +
            (IsObstacle ? "1" : (IsPlatform ? "2" : "0"));
    }
}
