using Microsoft.Xna.Framework;
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

        public Map Map { get; private set; }
        public int GridSize { get; protected set; }

        protected Toolbar? toolbar;

        private readonly Cursor cursor;
        private readonly Texture2D gridTexture;
        private bool hasErasedAnEntity;
        private IEntity? selectedEntity;
        private Box? selectedEntityBox;
        private bool isMovingAnEntity;
        private bool hasPressedAnEntity;

        public MapSectionEditor(Map map)
        {
            Map = map;
            if (GridSize == 0) GridSize = DEFAULT_GRID_SIZE;
            cursor = new Cursor(map, map.Camera) { Type = CursorType.Arrow };
            gridTexture = map.Game.Content.Load<Texture2D>("EngineGraphics/Editor/grid");
        }

        public void InitializeToolbar()
        {
            toolbar = new Toolbar(this);
        }

        public void Update()
        {
            toolbar?.Update();
            cursor.Update();
            selectedEntityBox?.Update();

            #region Draw tile/entity

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                // Select a tile in the bar
                bool clickedOnBarElement = false;

                if (toolbar != null)
                {
                    foreach (ToolbarButton button in toolbar.Buttons)
                    {
                        // Click on a toolbar button
                        if (button.HitBox.Contains(cursor.Position))
                        {
                            cursor.Type = CursorType.Crosshair;

                            clickedOnBarElement = true;
                            toolbar.SelectedTileId = button.Id;
                            toolbar.SelectedGroupName = button.GroupName;
                            toolbar.SelectedButtonType = button.Type;
                            toolbar.SelectedTileset = button.Tileset;

                            if (button.Type == ToolbarButtonType.Tile)
                            {
                                selectedEntity = null;
                                selectedEntityBox = null;
                            }
                            if (button.Type == ToolbarButtonType.Entity && button is EntityToolbarButton toolbarButton)
                            {
                                selectedEntity = null;
                                selectedEntityBox = null;
                                toolbar.SelectedEntityType = toolbarButton.EntityType;
                            }
                            if (button.Type == ToolbarButtonType.Selection)
                            {
                                cursor.Type = CursorType.Arrow;
                            }

                            break;
                        }
                    }

                    if (!clickedOnBarElement)
                    {
                        var drawerPosition = GetGridSnapedPosition();

                        // Draw a tile
                        if (toolbar.SelectedButtonType == ToolbarButtonType.Tile && !TileAlreadyAtPosition(drawerPosition))
                        {
                            DrawSelectedTile(drawerPosition);
                        }

                        // Draw an entity
                        if (toolbar.SelectedButtonType == ToolbarButtonType.Entity && GetEntityAtPosition(drawerPosition) == null)
                        {
                            DrawSelectedEntity(drawerPosition);
                        }

                        // Select an entity
                        if (toolbar.SelectedButtonType == ToolbarButtonType.Selection)
                        {
                            hasPressedAnEntity = true;
                        }
                    }
                }

                if (selectedEntity != null && selectedEntity.HitBox.Contains(cursor.InGamePosition))
                {
                    isMovingAnEntity = true;
                }
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (toolbar != null)
                {
                    // Select an entity
                    if (hasPressedAnEntity && toolbar.SelectedButtonType == ToolbarButtonType.Selection)
                    {
                        SelectEntity(cursor.InGamePosition);
                        hasPressedAnEntity = false;
                    }
                }

                isMovingAnEntity = false;
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
                    foreach (Tile tile in Map.CurrentMapSection.ForegroundTiles)
                    {
                        if (tile.Position == eraserPosition)
                        {
                            Map.CurrentMapSection.ForegroundTiles.Remove(tile);
                            UpdateTilesTypes();
                            break;
                        }
                    }
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Delete) || Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                if (selectedEntity != null)
                {
                    Map.CurrentMapSection.Entities.Remove(selectedEntity.Name);
                    selectedEntity = null;
                    selectedEntityBox = null;
                }
            }

            if (hasErasedAnEntity && Mouse.GetState().RightButton == ButtonState.Released)
            {
                hasErasedAnEntity = false;
            }

            #endregion

            #region Move an entity

            if (isMovingAnEntity && selectedEntity != null && selectedEntityBox != null)
            {
                selectedEntity.Position = GetGridSnapedPosition();
                selectedEntityBox.Bounds = selectedEntity.HitBox.ToRectangle();
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

            // Draw entity selections
            selectedEntityBox?.Draw(Map.Camera.Transform);
        }

        public void UIDraw()
        {
            toolbar?.Draw();
            cursor.Draw();
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

        private void DrawSelectedTile(Vector2 drawerPosition)
        {
            if (toolbar != null)
            {
                var sizePoint = toolbar.SelectedTileset?.GetTileBoundFromId(toolbar.SelectedTileId).Size;
                if (sizePoint is Point point)
                {
                    var size = new Size(point.X, point.Y);
                    if (toolbar.SelectedTileset?.GetTypeFromId(toolbar.SelectedTileId) is TileType type)
                    {
                        var tileToAdd = new Tile(Map, toolbar.SelectedTileset, toolbar.SelectedTileId, drawerPosition, size, type)
                        {
                            GroupName = toolbar.SelectedGroupName
                        };

                        // Draw the tile
                        if (toolbar.SelectedTileset.GetLayerFromId(toolbar.SelectedTileId) == TileLayer.Background)
                            Map.CurrentMapSection.BackgroundTiles.Add(tileToAdd);
                        else
                            Map.CurrentMapSection.ForegroundTiles.Add(tileToAdd);

                        // Update the id of every tiles of the same group
                        UpdateTilesTypes();
                    }
                }
            }
        }

        private void DrawSelectedEntity(Vector2 drawerPosition)
        {
            if (toolbar != null && toolbar.SelectedEntityType != null)
            {
                AddEntity(toolbar.SelectedEntityType, drawerPosition);
            }
        }

        private void SelectEntity(Vector2 position)
        {
            if (GetEntityAtPosition(position) is IEntity entity)
            {
                selectedEntity = entity;
                selectedEntityBox = new Box(Map.Game, entity.HitBox.ToRectangle(), Color.OrangeRed);
            }
            else
            {
                selectedEntity = null;
                selectedEntityBox = null;
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
            foreach (var tile in Map.CurrentMapSection.ForegroundTiles)
            {
                if (tile.Position == position)
                {
                    return true;
                }
            }

            return false;
        }

        private IEntity? GetEntityAtPosition(Vector2 position)
        {
            foreach (var entity in Map.CurrentMapSection.Entities)
            {
                if (entity.Value.HitBox.Contains(position))
                {
                    return entity.Value;
                }
            }

            return null;
        }

        private void UpdateTilesTypes()
        {
            foreach (var element in Map.CurrentMapSection.Elements)
            {
                if (element is Tile tile && toolbar?.SelectedTileset is Tileset tileset)
                {
                    TilesetGroup? currentGroup = null;

                    foreach (var groupPair in tileset.Groups)
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
        }

        private PresenceSide GetTilePresenceSides(ITile tile, TilesetGroup group)
        {
            PresenceSide side = PresenceSide.None;

            foreach (var element in Map.CurrentMapSection.Elements)
            {
                if (element is Tile currentTile)
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
