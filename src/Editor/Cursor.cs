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
        Move = 3,
        Arrow = 4,
    }

    class Cursor
    {
        private readonly Map map;
        private readonly Camera camera;
        private readonly Texture2D[] textures = new Texture2D[5];
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
                var position = Position.ToVector2();

                position -= new Vector2(
                    map.Game.SectionEditorViewPort.X,
                    map.Game.SectionEditorViewPort.Y
                );

                position *= new Vector2(
                    (float)map.Game.InGameViewport.Width / map.Game.SectionEditorViewPort.Width,
                    (float)map.Game.InGameViewport.Height / map.Game.SectionEditorViewPort.Height
                );

                position += camera.CornerPosition;

                return position;
            }
        }

        public Cursor(Map map, Camera camera)
        {
            this.map = map;
            this.camera = camera;

            textures[(int)CursorType.Crosshair] = map.Game.Content.Load<Texture2D>("EngineGraphics/Editor/cursor_crosshair");
            textures[(int)CursorType.ResizeHorizontal] = map.Game.Content.Load<Texture2D>("EngineGraphics/Editor/cursor_resize_horizontal");
            textures[(int)CursorType.ResizeVertical] = map.Game.Content.Load<Texture2D>("EngineGraphics/Editor/cursor_resize_vertical");
            textures[(int)CursorType.Move] = map.Game.Content.Load<Texture2D>("EngineGraphics/Editor/cursor_move");
            textures[(int)CursorType.Arrow] = map.Game.Content.Load<Texture2D>("EngineGraphics/Editor/cursor_arrow");
        }

        public void Update()
        {
            position.X = Mouse.GetState().X;
            position.Y = Mouse.GetState().Y;
        }

        public void Draw()
        {
            map.Game.SpriteBatch?.Begin(samplerState: SamplerState.PointClamp);

            map.Game.SpriteBatch?.Draw(
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

            map.Game.SpriteBatch?.End();
        }
    }
}
