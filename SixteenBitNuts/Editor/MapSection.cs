using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Editor
{
    enum ResizeSide
    {
        None,
        Left,
        Right,
        Top,
        Bottom
    }

    class MapSection
    {
        public Map Map { get; private set; }
        public int Index { get; set; }
        public SixteenBitNuts.MapSection RealSection
        {
            get
            {
                return Map.Sections[Index];
            }
        }

        private const int SCALE = 16;

        private readonly MapEditor editor;
        private bool isMoving;
        private bool canResize;
        private ResizeSide resizeSide;
        private bool mouseLeftButtonIsPressed;
        private bool mouseRightButtonIsPressed;
        private Point previousSectionPosition;
        private Rectangle bounds;
        private readonly Box box;
        private readonly MapSectionPreview preview;
        private readonly EditorLabel sizeLabel;
        private readonly EditorLabel positionLabel;

        public Rectangle ScreenBounds
        {
            get
            {
                return new Rectangle(bounds.X / SCALE, bounds.Y / SCALE, bounds.Width / SCALE, bounds.Height / SCALE);
            }
            private set
            {
                bounds = new Rectangle(value.X * SCALE, value.Y * SCALE, value.Width * SCALE, value.Height * SCALE);
            }
        }

        public MapSection(Map map, MapEditor editor, int index, Rectangle bounds)
        {
            Map = map;
            Index = index;
            this.editor = editor;
            this.bounds = bounds;

            previousSectionPosition = new Point(0, 0);

            box = new Box(map.Game, bounds, 1, Color.Ivory);
            preview = new MapSectionPreview(this);
            sizeLabel = new EditorLabel(map)
            {
                Text = "64x128",
                Color = Color.White,
                Position = box.Bounds.Location.ToVector2()
            };
            positionLabel = new EditorLabel(map)
            {
                Text = "64x128",
                Color = Color.White,
                Position = box.Bounds.Location.ToVector2()
            };
        }

        public void Update(Vector2 cursorInMapPosition)
        {
            box.Color = Color.Ivory;
            sizeLabel.IsVisible = false;
            positionLabel.IsVisible = false;

            #region Move

            if (!editor.IsResizingSection)
            {
                if (!canResize && !editor.IsMovingSection && bounds.Contains(cursorInMapPosition))
                {
                    box.Color = Color.Lime;
                    editor.Cursor.Type = CursorType.Move;
                    sizeLabel.IsVisible = false;
                    positionLabel.IsVisible = true;

                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        if (!isMoving)
                        {
                            previousSectionPosition = bounds.Location;
                            mouseLeftButtonIsPressed = true;
                        }
                        isMoving = true;
                        editor.IsMovingSection = true;
                    }
                }

                if (!canResize && isMoving)
                {
                    box.Color = Color.Lime;
                    positionLabel.IsVisible = true;
                    sizeLabel.IsVisible = false;
                    editor.Cursor.Type = CursorType.Move;

                    // Update bounds of the map section representation in this editor
                    bounds.Location = new Point(
                        ((int)(cursorInMapPosition.X - bounds.Width / 2) / SCALE) * SCALE,
                        ((int)(cursorInMapPosition.Y - bounds.Height / 2) / SCALE) * SCALE
                    );

                    // The mouse button is released: 
                    if (mouseLeftButtonIsPressed && Mouse.GetState().LeftButton == ButtonState.Released)
                    {
                        mouseLeftButtonIsPressed = false;
                        isMoving = false;
                        editor.IsMovingSection = false;

                        // Update all positions in the real map section
                        RealSection.UpdateAllPositions(bounds.Location - previousSectionPosition, bounds.Size);

                        preview.UpdatePreviewTilesFromRealSection();
                    }
                }
            }

            #endregion

            #region Resize

            if (!editor.IsMovingSection)
            {
                if (bounds.Contains(cursorInMapPosition))
                {
                    var leftSide = new Rectangle(bounds.X - 32, bounds.Y, 64, bounds.Height);
                    var rightSide = new Rectangle(bounds.X + bounds.Width - 32, bounds.Y, 64, bounds.Height);
                    var topSide = new Rectangle(bounds.X, bounds.Y - 32, bounds.Width, 64);
                    var bottomSide = new Rectangle(bounds.X, bounds.Y + bounds.Height - 32, bounds.Width, 64);

                    if (!editor.IsResizingSection)
                    {
                        // The cursor is inside the right handle
                        if (leftSide.Contains(cursorInMapPosition))
                        {
                            canResize = true;

                            box.Color = Color.Yellow;
                            editor.Cursor.Type = CursorType.ResizeHorizontal;
                            sizeLabel.IsVisible = true;
                            positionLabel.IsVisible = false;

                            // The mouse button is pressed inside the handle
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                            {
                                mouseLeftButtonIsPressed = true;
                                resizeSide = ResizeSide.Left;
                                editor.IsResizingSection = true;
                            }
                        }
                        else if (rightSide.Contains(cursorInMapPosition))
                        {
                            canResize = true;

                            box.Color = Color.Yellow;
                            editor.Cursor.Type = CursorType.ResizeHorizontal;
                            sizeLabel.IsVisible = true;
                            positionLabel.IsVisible = false;

                            // The mouse button is pressed inside the handle
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                            {
                                mouseLeftButtonIsPressed = true;
                                resizeSide = ResizeSide.Right;
                                editor.IsResizingSection = true;
                            }
                        }
                        else if (topSide.Contains(cursorInMapPosition))
                        {
                            canResize = true;

                            box.Color = Color.Yellow;
                            editor.Cursor.Type = CursorType.ResizeVertical;
                            sizeLabel.IsVisible = true;
                            positionLabel.IsVisible = false;

                            // The mouse button is pressed inside the handle
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                            {
                                mouseLeftButtonIsPressed = true;
                                resizeSide = ResizeSide.Top;
                                editor.IsResizingSection = true;
                            }
                        }
                        else if (bottomSide.Contains(cursorInMapPosition))
                        {
                            canResize = true;

                            box.Color = Color.Yellow;
                            editor.Cursor.Type = CursorType.ResizeVertical;
                            sizeLabel.IsVisible = true;
                            positionLabel.IsVisible = false;

                            // The mouse button is pressed inside the handle
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                            {
                                mouseLeftButtonIsPressed = true;
                                resizeSide = ResizeSide.Bottom;
                                editor.IsResizingSection = true;
                            }
                        }
                        else
                        {
                            canResize = false;
                        }
                    }   
                }

                if (resizeSide != ResizeSide.None)
                {
                    box.Color = Color.Yellow;
                    sizeLabel.IsVisible = true;
                    positionLabel.IsVisible = false;

                    if (resizeSide == ResizeSide.Left)
                    {
                        editor.Cursor.Type = CursorType.ResizeHorizontal;

                        int cursorPosition = ((int)cursorInMapPosition.X / SCALE) * SCALE;
                        int distance = bounds.X - cursorPosition;
                        int width = bounds.Width + distance;

                        if (width >= editor.Map.Game.InternalSize.Width)
                        {
                            bounds.X = cursorPosition;
                        }
                        if (width < editor.Map.Game.InternalSize.Width)
                        {
                            width = (int)editor.Map.Game.InternalSize.Width;
                        }

                        bounds.Width = width;
                    }
                    else if (resizeSide == ResizeSide.Right)
                    {
                        editor.Cursor.Type = CursorType.ResizeHorizontal;
                        int width = (((int)cursorInMapPosition.X / SCALE) * SCALE) - bounds.X;
                        if (width < editor.Map.Game.InternalSize.Width) width = (int)editor.Map.Game.InternalSize.Width;
                        bounds.Width = width;
                    }
                    if (resizeSide == ResizeSide.Top)
                    {
                        editor.Cursor.Type = CursorType.ResizeVertical;

                        int cursorPosition = ((int)cursorInMapPosition.Y / SCALE) * SCALE;
                        int distance = bounds.Y - cursorPosition;
                        int height = bounds.Height + distance;

                        if (height >= editor.Map.Game.InternalSize.Height)
                        {
                            bounds.Y = cursorPosition;
                        }
                        if (height < editor.Map.Game.InternalSize.Height)
                        {
                            height = (int)editor.Map.Game.InternalSize.Height;
                        }

                        bounds.Height = height;
                    }
                    else if (resizeSide == ResizeSide.Bottom)
                    {
                        editor.Cursor.Type = CursorType.ResizeVertical;
                        int height = (((int)cursorInMapPosition.Y / SCALE) * SCALE) - bounds.Y;
                        if (height < editor.Map.Game.InternalSize.Height) height = (int)editor.Map.Game.InternalSize.Height;
                        bounds.Height = height;
                    }

                    // The mouse button is released: 
                    if (mouseLeftButtonIsPressed && Mouse.GetState().LeftButton == ButtonState.Released)
                    {
                        mouseLeftButtonIsPressed = false;
                        resizeSide = ResizeSide.None;
                        editor.IsResizingSection = false;

                        // Update all positions in the real map section
                        RealSection.UpdateAllPositions(bounds.Location - previousSectionPosition, bounds.Size);

                        preview.UpdatePreviewTilesFromRealSection();
                    }
                }
            }

            #endregion

            #region Load section

            if (bounds.Contains(cursorInMapPosition)) {
                if (!mouseRightButtonIsPressed && Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    mouseRightButtonIsPressed = true;
                }
                if (mouseRightButtonIsPressed && Mouse.GetState().RightButton == ButtonState.Released)
                {
                    mouseRightButtonIsPressed = false;
                    editor.LoadSection(this);
                }
            }

            #endregion

            #region Labels

            positionLabel.Position = box.Bounds.Location.ToVector2() - new Vector2(0, 8);
            positionLabel.Text = bounds.Location.X + ";" + bounds.Location.Y;

            sizeLabel.Position = box.Bounds.Location.ToVector2() - new Vector2(0, 8);
            sizeLabel.Text = bounds.Size.X + "x" + bounds.Size.Y;

            #endregion

            box.Bounds = ScreenBounds;
            preview.Position = ScreenBounds.Location;
            box.Update();
        }

        public void Draw()
        {
            box.Draw();
            preview.Draw();
            sizeLabel.Draw();
            positionLabel.Draw();
        }

        public void UpdateLayout()
        {
            preview.UpdatePreviewTilesFromRealSection();
        }
    }
}
