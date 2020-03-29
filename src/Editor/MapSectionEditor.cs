﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using SixteenBitNuts.Interfaces;

namespace SixteenBitNuts.Editor
{
    public class MapSectionEditor
    {
        private const int DEFAULT_GRID_SIZE = 16;

        private bool hasErasedAnEntity;

        public Map Map { get; private set; }
        public int GridSize { get; protected set; }

        protected Toolbar? toolbar;

        private readonly Cursor cursor;
        private readonly Texture2D gridTexture;
        private readonly Box frame;

        public MapSectionEditor(Map map)
        {
            Map = map;
            if (GridSize == 0)
            {
                GridSize = DEFAULT_GRID_SIZE;
            }
            cursor = new Cursor(map, map.Camera);
            gridTexture = map.Game.Content.Load<Texture2D>("Engine/editor/grid");
            frame = new Box(map.Game, new Rectangle(0, 0, (int)map.Game.WindowSize.Width, (int)map.Game.WindowSize.Height), (int)map.Game.ScreenScale * 2, Color.Green);
        }

        public void InitializeToolbar()
        {
            toolbar = new Toolbar(this);
        }

        public void Update()
        {
            toolbar?.Update();
            cursor.Update();
            frame.Update();

            #region Draw tile/entity

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                // Select a tile in the bar
                bool clickedOnBarElement = false;

                if (toolbar != null)
                {
                    foreach (ToolbarButton button in toolbar.Buttons)
                    {
                        if (button.HitBox.Contains(cursor.Position))
                        {
                            clickedOnBarElement = true;
                            toolbar.SelectedTileId = button.Id;
                            toolbar.SelectedGroupName = button.GroupName;
                            toolbar.SelectedButtonType = button.GetType();

                            if (button.GetType() == typeof(EntityToolbarButton))
                            {
                                toolbar.SelectedEntityType = ((EntityToolbarButton)button).Type;
                            }

                            break;
                        }
                    }
                }
                
                if (!clickedOnBarElement && toolbar != null)
                {
                    var drawerPosition = GetGridSnapedPosition();

                    // Draw a tile
                    if (toolbar.SelectedButtonType == typeof(TileToolbarButton) && !TileAlreadyAtPosition(drawerPosition))
                    {
                        // Draw the tile
                        Map.CurrentMapSection.Tiles.Add(new Tile(
                            Map,
                            Map.CurrentMapSection.Tileset,
                            toolbar.SelectedTileId,
                            drawerPosition,
                            Map.CurrentMapSection.Tileset.GetSizeFromId(toolbar.SelectedTileId),
                            Map.CurrentMapSection.Tileset.GetTypeFromId(toolbar.SelectedTileId)
                        )
                        {
                            GroupName = toolbar.SelectedGroupName
                        });

                        // Update the id of every tiles of the same group
                        UpdateTilesTypes();
                    }

                    // Draw an entity
                    if (toolbar.SelectedButtonType == typeof(EntityToolbarButton) && !EntityAlreadyAtPosition(drawerPosition))
                    {
                        if (toolbar.SelectedEntityType != null)
                        {
                            AddEntity(toolbar.SelectedEntityType, drawerPosition);
                        }
                    }
                }
            }

            #endregion

            #region Erase a tile/entity

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                Vector2 eraserPosition = GetGridSnapedPosition();

                foreach (KeyValuePair<string, IEntity> entity in Map.CurrentMapSection.Entities)
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
                            UpdateTilesTypes();
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
                Map.Player?.MoveLeft(2f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Map.Player?.MoveRight(2f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Map.Player?.MoveUp(2f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Map.Player?.MoveDown(2f);
            }

            #endregion
        }

        public void Draw()
        {
            // Draw grid
            Map.Game.SpriteBatch?.Begin(transformMatrix: Map.Camera.Transform);
            for (int i = Map.CurrentMapSection.Bounds.X / GridSize; i < (Map.CurrentMapSection.Bounds.X + Map.CurrentMapSection.Bounds.Width) / GridSize + 1; i++)
            {
                for (int j = Map.CurrentMapSection.Bounds.Y / GridSize; j < (Map.CurrentMapSection.Bounds.Y + Map.CurrentMapSection.Bounds.Height) / GridSize + 1; j++)
                {
                    Map.Game.SpriteBatch?.Draw(
                        gridTexture,
                        new Vector2(i * GridSize, j * GridSize),
                        new Rectangle(0, 0, GridSize, GridSize),
                        Color.FromNonPremultiplied(50, 50, 50, 100),
                        0,
                        new Vector2(0, 0),
                        1,
                        SpriteEffects.None,
                        0
                    );
                }
            }
            Map.Game.SpriteBatch?.End();
        }

        public void UIDraw()
        {
            toolbar?.Draw();
            cursor.Draw();
            frame.Draw(Matrix.Identity);
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
                    Map.CurrentMapSection.Entities.Add(name, new SpawnPoint(Map, name)
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
            var position = cursor.InGamePosition;
            position.X = ((int)Math.Ceiling(position.X) / GridSize) * GridSize;
            position.Y = ((int)Math.Ceiling(position.Y) / GridSize) * GridSize;

            return position;
        }

        private bool TileAlreadyAtPosition(Vector2 position)
        {
            foreach (var tile in Map.CurrentMapSection.Tiles)
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
            foreach (KeyValuePair<string, IEntity> entity in Map.CurrentMapSection.Entities)
            {
                if (entity.Value.Position == position)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateTilesTypes()
        {
            foreach (var tile in Map.CurrentMapSection.Tiles)
            {
                TilesetGroup? currentGroup = null;

                foreach (var groupPair in Map.CurrentMapSection.Tileset.Groups)
                {
                    if (tile.GroupName == groupPair.Value.Name)
                    {
                        currentGroup = groupPair.Value;
                        break;
                    }
                }

                if (currentGroup is TilesetGroup group)
                {
                    var side = GetTilePresenceSides(tile, group);

                    if (group.Definitions != null)
                    {
                        if (side.HasFlag(PresenceSide.Bottom) && side.HasFlag(PresenceSide.Top))
                        {
                            if (side.HasFlag(PresenceSide.Left) && side.HasFlag(PresenceSide.Right))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.Center].TileIndex;
                            }
                            else if (side.HasFlag(PresenceSide.Left))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.Right].TileIndex;
                            }
                            else if (side.HasFlag(PresenceSide.Right))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.Left].TileIndex;
                            }
                            else
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.VerticalNarrowCenter].TileIndex;
                            }
                        }
                        else if (side.HasFlag(PresenceSide.Bottom))
                        {
                            if (side.HasFlag(PresenceSide.Left) && side.HasFlag(PresenceSide.Right))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.Top].TileIndex;
                            }
                            else if (side.HasFlag(PresenceSide.Left))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.TopRight].TileIndex;
                            }
                            else if (side.HasFlag(PresenceSide.Right))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.TopLeft].TileIndex;
                            }
                            else
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.VerticalNarrowTop].TileIndex;
                            }
                        }
                        else if (side.HasFlag(PresenceSide.Top))
                        {
                            if (side.HasFlag(PresenceSide.Left) && side.HasFlag(PresenceSide.Right))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.Bottom].TileIndex;
                            }
                            else if (side.HasFlag(PresenceSide.Left))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.BottomRight].TileIndex;
                            }
                            else if (side.HasFlag(PresenceSide.Right))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.BottomLeft].TileIndex;
                            }
                            else
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.VerticalNarrowBottom].TileIndex;
                            }
                        }
                        else
                        {
                            if (side.HasFlag(PresenceSide.Left) && side.HasFlag(PresenceSide.Right))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.HorizontalNarrowCenter].TileIndex;
                            }
                            else if (side.HasFlag(PresenceSide.Left))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.HorizontalNarrowRight].TileIndex;
                            }
                            else if (side.HasFlag(PresenceSide.Right))
                            {
                                tile.Id = group.Definitions[TilesetGroupDefinitionType.HorizontalNarrowLeft].TileIndex;
                            }
                        }
                    }
                }
            }
        }

        private PresenceSide GetTilePresenceSides(ITile tile, TilesetGroup group)
        {
            PresenceSide side = PresenceSide.None;

            foreach (var currentTile in Map.CurrentMapSection.Tiles)
            {
                if (currentTile.GroupName == group.Name)
                {
                    if (currentTile.Position.X == tile.Position.X && currentTile.Position.Y == tile.Position.Y - 16)
                    {
                        side |= PresenceSide.Top;
                    }
                    if (currentTile.Position.X == tile.Position.X && currentTile.Position.Y == tile.Position.Y + 16)
                    {
                        side |= PresenceSide.Bottom;
                    }
                    if (currentTile.Position.X == tile.Position.X - 16 && currentTile.Position.Y == tile.Position.Y)
                    {
                        side |= PresenceSide.Left;
                    }
                    if (currentTile.Position.X == tile.Position.X + 16 && currentTile.Position.Y == tile.Position.Y)
                    {
                        side |= PresenceSide.Right;
                    }
                }
            }

            return side;
        }
    }

    [Flags]
    public enum PresenceSide
    {
        None = 1,
        Top = 2,
        Left = 4,
        Right = 8,
        Bottom = 16,
    }
}
