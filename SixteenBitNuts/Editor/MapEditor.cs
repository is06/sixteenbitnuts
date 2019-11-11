using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SixteenBitNuts.Editor
{
    class MapEditor
    {
        private const float CAMERA_SPEED = 2f;
        public const float SCALE = 16f;

        private readonly Camera camera;
        private readonly Dictionary<int, MapSection> sections;
        private readonly Box frame;
        private readonly Texture2D gridTexture;
        private readonly Label cursorPosition;

        private readonly int gridZoom = 4;
        private bool isKeyAddPressed = false;

        public Map Map { get; private set; }
        public bool IsMovingSection { get; set; }
        public bool IsResizingSection { get; set; }
        public Cursor Cursor { get; set; }

        public MapEditor(Map map)
        {
            Map = map;

            camera = new Camera(map, new Vector2(0, 0), new Viewport(0, 0, map.Game.InternalSize.Width, map.Game.InternalSize.Height))
            {
                CanOverrideLimits = true
            };
            sections = new Dictionary<int, MapSection>();
            cursorPosition = new Label(map)
            {
                IsVisible = true,
                Position = new Vector2(8, 256),
                Color = Color.White,
                Text = "0;0"
            };
            frame = new Box(map.Game, new Rectangle(0, 0, map.Game.InternalSize.Width, map.Game.InternalSize.Height), 2, Color.BlueViolet);
            gridTexture = map.Game.Content.Load<Texture2D>("Engine/editor/grid");

            foreach (var section in map.Sections)
            {
                sections[section.Key] = new MapSection(map, this, section.Key, section.Value.Bounds);
            }

            Cursor = new Cursor(map, camera);
        }

        public void Update(GameTime gameTime)
        {
            Cursor.Type = CursorType.Crosshair;

            #region Moving Camera

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                camera.MoveLeft(CAMERA_SPEED);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                camera.MoveRight(CAMERA_SPEED);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                camera.MoveUp(CAMERA_SPEED);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                camera.MoveDown(CAMERA_SPEED);
            }

            #endregion

            #region Add section

            if (!isKeyAddPressed && Keyboard.GetState().IsKeyDown(Keys.Add))
            {
                isKeyAddPressed = true;

                var bounds = new Rectangle(
                    (int)(Cursor.InGamePosition.X * SCALE),
                    (int)(Cursor.InGamePosition.Y * SCALE),
                    Map.Game.InternalSize.Width,
                    Map.Game.InternalSize.Height
                );

                int nextSectionIndex = sections.Count;
                sections.Add(nextSectionIndex, new MapSection(Map, this, nextSectionIndex, bounds));

                // TODO: retrieve tileset from tileset factory
                var tileset = new Tileset(Map.Game, "tileset3");

                // TODO: create a spawn point for this section
                Map.Sections.Add(nextSectionIndex, new SixteenBitNuts.MapSection(Map, bounds, tileset, "spawn02"));
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Add))
            {
                isKeyAddPressed = false;
            }

            #endregion

            foreach (var section in sections)
            {
                section.Value.Update(new Vector2(
                    Cursor.InGamePosition.X * SCALE,
                    Cursor.InGamePosition.Y * SCALE
                ));
            }

            cursorPosition.Text = (int)Cursor.InGamePosition.X * SCALE + ";" + (int)Cursor.InGamePosition.Y * SCALE;
            cursorPosition.Update();

            camera.Update(gameTime);
            frame.Update();
            Cursor.Update();
        }

        public void Draw()
        {
            var gridOrigin = camera.ViewPort.Bounds.Location - camera.ViewPort.Bounds.Center - (new Point((int)camera.Transform.Translation.X, (int)camera.Transform.Translation.Y));
            var gridDestination = camera.ViewPort.Bounds.Location + camera.ViewPort.Bounds.Size - (new Point((int)camera.Transform.Translation.X, (int)camera.Transform.Translation.Y));

            Map.Game.SpriteBatch.Begin(transformMatrix: camera.Transform);

            // Draw grid
            for (int i = gridOrigin.X / gridZoom; i < gridDestination.X / gridZoom; i++)
            {
                for (int j = gridOrigin.Y / gridZoom; j < gridDestination.Y / gridZoom; j++)
                {
                    Map.Game.SpriteBatch.Draw(
                        texture: gridTexture,
                        position: new Vector2(i * gridZoom, j * gridZoom),
                        sourceRectangle: new Rectangle(0, 0, 16, 16),
                        color: Color.FromNonPremultiplied(50, 50, 50, 255),
                        rotation: 0,
                        origin: new Vector2(0, 0),
                        scale: 1,
                        effects: SpriteEffects.None,
                        layerDepth: 0
                    );
                }
            }

            // Draw map sections
            foreach (var section in sections)
            {
                section.Value.Draw();
            }

            Map.Game.SpriteBatch.End();

            Map.Game.SpriteBatch.Begin();
            frame.Draw();
            cursorPosition.Draw();
            Map.Game.SpriteBatch.End();
        }

        public void UIDraw()
        {
            Map.Game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Cursor.Draw();
            Map.Game.SpriteBatch.End();
        }

        public void UpdateLayout()
        {
            foreach (var section in sections)
            {
                section.Value.UpdateLayout();
            }
        }

        public void LoadSection(MapSection section)
        {
            Map.LoadSectionFromIndex(section.Index);
        }
    }
}
