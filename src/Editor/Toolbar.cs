using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts.Editor
{
    public class Toolbar
    {
        private const int TOOLBAR_POSITION_Y = 16;

        public MapSectionEditor Editor { get; private set; }
        public List<ToolbarButton> Buttons { get; }
        public ToolbarButtonType? SelectedButtonType { get; set; }
        public int SelectedTileId { get; set; }
        public Tileset? SelectedTileset { get; set; }
        public string? SelectedGroupName { get; set; }
        public string? SelectedEntityType { get; set; }
        
        private int nextButtonPosition = 16;

        public Toolbar(MapSectionEditor editor)
        {
            Editor = editor;
            SelectedTileId = 0;
            Buttons = new List<ToolbarButton>();

            AddButton(new SelectionToolbarButton(this) { Id = 900 });

            foreach (var tileset in Editor.Map.Game.TilesetService.Tilesets)
            {
                // Create every tileset groups defined by "gr" lines in tileset descriptor
                foreach (var group in tileset.Value.Groups)
                {
                    var button = new TileToolbarButton(this, isGroup: true)
                    {
                        Tileset = tileset.Value,
                        GroupName = group.Value.Name
                    };
                    AddButton(button);
                }
            }

            AddButton(new EntityToolbarButton(this, "spawn", "spawn"));
        }

        public void AddButton(ToolbarButton button)
        {
            button.Position = new Vector2(nextButtonPosition, TOOLBAR_POSITION_Y);
            Buttons.Add(button);
            nextButtonPosition += 64;
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
