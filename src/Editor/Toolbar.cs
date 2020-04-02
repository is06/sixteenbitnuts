using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts.Editor
{
    public class Toolbar
    {
        public MapSectionEditor Editor { get; private set; }
        public List<ToolbarButton> Buttons { get; }
        public System.Type? SelectedButtonType { get; set; }
        public int SelectedTileId { get; set; }
        public Tileset? SelectedTileset { get; set; }
        public string? SelectedGroupName { get; set; }
        public string? SelectedEntityType { get; set; }

        public Toolbar(MapSectionEditor editor)
        {
            Editor = editor;
            SelectedTileId = 0;
            Buttons = new List<ToolbarButton>();

            int position = 16;

            foreach (var tileset in Editor.Map.Game.TilesetService.Tilesets)
            {
                foreach (var group in tileset.Value.Groups)
                {
                    Buttons.Add(new TileToolbarButton(this, isGroup: true)
                    {
                        Tileset = tileset.Value,
                        GroupName = group.Value.Name,
                        Position = new Vector2(position, 16),
                    });
                    position += 64;
                }
            }

            Buttons.Add(new EntityToolbarButton(this, "spawn", "spawn")
            {
                Id = 100,
                Position = new Vector2(position, 16),
            });
        }

        public void Update()
        {
            foreach (ToolbarButton button in Buttons)
            {
                if (button.Id == SelectedTileId)
                {
                    button.IsSelected = true;
                }
                else
                {
                    button.IsSelected = false;
                }
            }
        }

        public void Draw()
        {
            foreach (ToolbarButton button in Buttons)
            {
                button.Draw();
            }
        }
    }
}
