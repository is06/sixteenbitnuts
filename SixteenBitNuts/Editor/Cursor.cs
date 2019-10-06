using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts.Editor
{
    enum CursorType
    {
        Crosshair = 0,
        ResizeHorizontal = 1,
        ResizeVertical = 2,
        Move = 3
    }

    class Cursor
    {
        private readonly Map map;
        private readonly Camera camera;
        private readonly Texture2D[] textures = new Texture2D[4];
        private readonly SpriteBatch spriteBatch;
        private Point position;

        public CursorType Type { get; set; }

        public Point Position
        {
            get
            {
                return position;
            }
            private set
            {
                position = value;
            }
        }
        public Vector2 InGamePosition
        {
            get
            {
                return camera.CornerPosition + Position.ToVector2() * new Vector2(
                    (float)map.Game.InGameViewport.Width / map.Game.GraphicsDevice.Viewport.Width,
                    (float)map.Game.InGameViewport.Height / map.Game.GraphicsDevice.Viewport.Height
                );
            }
        }

        public Cursor(Map map, Camera camera)
        {
            this.map = map;
            this.camera = camera;
            spriteBatch = new SpriteBatch(map.Graphics);
            textures[0] = map.Content.Load<Texture2D>("Engine/editor/cursor_crosshair");
            textures[1] = map.Content.Load<Texture2D>("Engine/editor/cursor_resize_horizontal");
            textures[2] = map.Content.Load<Texture2D>("Engine/editor/cursor_resize_vertical");
            textures[3] = map.Content.Load<Texture2D>("Engine/editor/cursor_move");
        }

        public void Update()
        {
            position.X = Mouse.GetState().X;
            position.Y = Mouse.GetState().Y;
        }

        public void Draw()
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(
                texture: textures[(int)Type],
                position: Position.ToVector2(),
                sourceRectangle: new Rectangle(0, 0, 32, 32),
                color: Color.White,
                rotation: 0f,
                origin: new Vector2(16, 16),
                scale: 3f,
                effects: SpriteEffects.None,
                layerDepth: 0
            );
            spriteBatch.End();
        }
    }
}
