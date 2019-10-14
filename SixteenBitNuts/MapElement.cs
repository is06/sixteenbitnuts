﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public class MapElement
    {
        #region Constants

        private const int DEBUG_BOX_THICKNESS = 1;

        #endregion

        #region Properties

        public Color DebugColor { get; set; }
        public bool IsObstacle { get; set; }
        public bool IsPlatform { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public BoundingBox HitBox
        {
            get
            {
                return new BoundingBox
                {
                    Min = new Vector3(Position.X, Position.Y, 0),
                    Max = new Vector3(Position.X + Size.X, Position.Y + Size.Y, 0)
                };
            }
        }

        #endregion

        #region Fields

        private readonly Box debugHitBox;

        #endregion

        public MapElement(GraphicsDevice graphicsDevice)
        {
            debugHitBox = new Box(
                graphicsDevice,
                new Rectangle(Position.ToPoint(), Size.ToPoint()),
                DEBUG_BOX_THICKNESS,
                DebugColor
            );
        }

        public virtual void Update()
        {

        }

        public virtual void Draw(Matrix transform)
        {

        }

        public virtual void EditorDraw(Matrix transform)
        {

        }

        public virtual void DebugDraw(Matrix transform)
        {
            debugHitBox.Color = DebugColor;
            debugHitBox.Bounds = new Rectangle(Position.ToPoint(), Size.ToPoint());
            debugHitBox.Update();
            debugHitBox.Draw(transform);
        }
    }
}
