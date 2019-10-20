﻿using Microsoft.Xna.Framework;
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
        private readonly Image frame;
        private readonly Texture2D gridTexture;
        private readonly Map map;
        private readonly Label cursorPosition;

        private readonly int gridZoom = 4;
        private bool isKeyAddPressed = false;

        public bool IsMovingSection { get; set; }
        public bool IsResizingSection { get; set; }
        public Cursor Cursor { get; set; }

        public MapEditor(Map map)
        {
            this.map = map;

            camera = new Camera(map, new Vector2(0, 0), new Viewport(0, 0, 480, 270))
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
            frame = new Image(map, new Vector2(0, 0), "Engine/editor/level_frame")
            {
                Color = Color.BlueViolet
            };
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
                    480,
                    270
                );

                int nextSectionIndex = sections.Count;
                sections.Add(nextSectionIndex, new MapSection(map, this, nextSectionIndex, bounds));

                // TODO: retrieve tileset from tileset factory
                var tileset = new Tileset(map.Game, "tileset3");

                // TODO: create a spawn point for this section
                map.Sections.Add(nextSectionIndex, new SixteenBitNuts.MapSection(map, bounds, tileset, "spawn02"));
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
            Cursor.Update();
        }

        public void Draw()
        {
            var gridOrigin = camera.ViewPort.Bounds.Location - camera.ViewPort.Bounds.Center - (new Point((int)camera.Transform.Translation.X, (int)camera.Transform.Translation.Y));
            var gridDestination = camera.ViewPort.Bounds.Location + camera.ViewPort.Bounds.Size - (new Point((int)camera.Transform.Translation.X, (int)camera.Transform.Translation.Y));

            map.Game.SpriteBatch.Begin(transformMatrix: camera.Transform);

            // Draw grid
            for (int i = gridOrigin.X / gridZoom; i < gridDestination.X / gridZoom; i++)
            {
                for (int j = gridOrigin.Y / gridZoom; j < gridDestination.Y / gridZoom; j++)
                {
                    map.Game.SpriteBatch.Draw(
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

            map.Game.SpriteBatch.End();

            map.Game.SpriteBatch.Begin();
            frame.Draw();
            cursorPosition.Draw();
            map.Game.SpriteBatch.End();
        }

        public void UIDraw()
        {
            map.Game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Cursor.Draw();
            map.Game.SpriteBatch.End();
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
            map.LoadSectionFromIndex(section.Index);
        }
    }
}
