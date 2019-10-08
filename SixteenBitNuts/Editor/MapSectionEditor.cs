using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SixteenBitNuts.Editor
{
    public class MapSectionEditor
    {
        #region Constants

        private const int GRID_SIZE = 16;

        #endregion

        #region Fields

        private bool hasErasedAnEntity;

        #endregion

        #region Properties

        public Map Map { get; private set; }

        #endregion

        #region Components

        protected Toolbar toolbar;

        private readonly Cursor cursor;
        private readonly Texture2D frameTexture;
        private readonly Texture2D gridTexture;
        private readonly SpriteBatch spriteBatch;

        #endregion

        public MapSectionEditor(Map map)
        {
            Map = map;

            toolbar = new Toolbar(this);
            cursor = new Cursor(map, map.Camera);
            frameTexture = map.Content.Load<Texture2D>("Engine/editor/frame");
            gridTexture = map.Content.Load<Texture2D>("Engine/editor/grid");
            spriteBatch = new SpriteBatch(map.Graphics);
        }

        public void Update()
        {
            toolbar.Update();
            cursor.Update();

            #region Draw

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                // Select a tile in the bar
                bool clickedOnBarElement = false;
                foreach (ToolbarButton button in toolbar.Buttons)
                {
                    if (button.HitBox.Contains(cursor.Position))
                    {
                        clickedOnBarElement = true;
                        toolbar.SelectedTileId = button.Id;
                        toolbar.SelectedButtonType = button.GetType();

                        if (button.GetType() == typeof(EntityToolbarButton))
                        {
                            toolbar.SelectedEntityType = ((EntityToolbarButton)button).Type;
                        }
                        
                        break;
                    }
                }

                
                if (!clickedOnBarElement)
                {
                    Vector2 drawerPosition = GetGridSnapedPosition();

                    // Draw a tile
                    if (toolbar.SelectedButtonType == typeof(TileToolbarButton) && !TileAlreadyAtPosition(drawerPosition))
                    {
                        Map.CurrentMapSection.Tiles.Add(new Tile(
                            Map.CurrentMapSection.Tileset,
                            toolbar.SelectedTileId,
                            drawerPosition,
                            Map.CurrentMapSection.Tileset.GetSizeFromId(toolbar.SelectedTileId),
                            Map.CurrentMapSection.Tileset.GetTypeFromId(toolbar.SelectedTileId),
                            0
                        ));
                    }

                    // Draw an entity
                    if (toolbar.SelectedButtonType == typeof(EntityToolbarButton) && !EntityAlreadyAtPosition(drawerPosition))
                    {
                        AddEntity(toolbar.SelectedEntityType, drawerPosition);
                    }
                }
            }

            #endregion

            #region Erase a tile

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                Vector2 eraserPosition = GetGridSnapedPosition();

                foreach (KeyValuePair<string, Entity> entity in Map.CurrentMapSection.Entities)
                {
                    if (entity.Value.Position == eraserPosition)
                    {
                        Map.CurrentMapSection.Entities.Remove(entity.Key);
                        hasErasedAnEntity = true;
                        break;
                    }
                }

                if (!hasErasedAnEntity)
                {
                    foreach (Tile tile in Map.CurrentMapSection.Tiles)
                    {
                        if (tile.Position == eraserPosition)
                        {
                            Map.CurrentMapSection.Tiles.Remove(tile);
                            break;
                        }
                    }
                }
            }

            if (hasErasedAnEntity && Mouse.GetState().RightButton == ButtonState.Released)
            {
                hasErasedAnEntity = false;
            }

            #endregion

            #region Move player

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Map.Player.MoveLeft(2f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Map.Player.MoveRight(2f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Map.Player.MoveUp(2f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Map.Player.MoveDown(2f);
            }

            #endregion
        }

        public void Draw()
        {
            // Draw grid
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Map.Camera.UITransform);
            for (int i = Map.CurrentMapSection.Bounds.X / GRID_SIZE; i < (Map.CurrentMapSection.Bounds.X + Map.CurrentMapSection.Bounds.Width) / GRID_SIZE + 1; i++)
            {
                for (int j = Map.CurrentMapSection.Bounds.Y / GRID_SIZE; j < (Map.CurrentMapSection.Bounds.Y + Map.CurrentMapSection.Bounds.Height) / GRID_SIZE + 1; j++)
                {
                    spriteBatch.Draw(
                        gridTexture,
                        new Vector2(i * (GRID_SIZE * Map.Game.ScreenScale), j * (GRID_SIZE * Map.Game.ScreenScale)),
                        new Rectangle(0, 0, GRID_SIZE, GRID_SIZE),
                        Color.FromNonPremultiplied(50, 50, 50, 100),
                        0,
                        new Vector2(0, 0),
                        3,
                        SpriteEffects.None,
                        0
                    );
                }
            }
            spriteBatch.End();

            toolbar.Draw();
            cursor.Draw();

            // Draw frame
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(
                frameTexture,
                new Vector2(0, 0),
                new Rectangle(0, 0, 480, 270),
                Color.LimeGreen,
                0,
                new Vector2(0, 0),
                Map.Game.ScreenScale,
                SpriteEffects.None,
                0
            );
            spriteBatch.End();
        }

        protected virtual void AddEntity(string entityType, Vector2 position)
        {
            string name = entityType;
            while (Map.CurrentMapSection.Entities.ContainsKey(name))
            {
                name = entityType + "_" + Map.EntityLastIndex;
                Map.EntityLastIndex++;
            }

            switch (entityType)
            {
                case "spawn":
                    Map.CurrentMapSection.Entities.Add(name, new SpawnPoint(Map)
                    {
                        Position = position
                    });
                    break;
                default:
                    break;
            }
        }

        private Vector2 GetGridSnapedPosition()
        {
            Vector2 position = cursor.InGamePosition;
            position.X = ((int)Math.Ceiling(position.X) / GRID_SIZE) * GRID_SIZE;
            position.Y = ((int)Math.Ceiling(position.Y) / GRID_SIZE) * GRID_SIZE;

            return position;
        }

        private bool TileAlreadyAtPosition(Vector2 position)
        {
            foreach (Tile tile in Map.CurrentMapSection.Tiles)
            {
                if (tile.Position == position)
                {
                    return true;
                }
            }

            return false;
        }

        private bool EntityAlreadyAtPosition(Vector2 position)
        {
            foreach (KeyValuePair<string, Entity> entity in Map.CurrentMapSection.Entities)
            {
                if (entity.Value.Position == position)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
