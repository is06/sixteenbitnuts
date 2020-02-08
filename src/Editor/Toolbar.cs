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
        public string? SelectedEntityType { get; set; }

        public Toolbar(MapSectionEditor editor)
        {
            Editor = editor;
            SelectedTileId = 4;
            Buttons = new List<ToolbarButton>();

            int position = 256;
            for (int i = 0; i <= 8; i++)
            {
                Buttons.Add(new TileToolbarButton(this)
                {
                    Id = i,
                    Position = new Vector2(position, 16),
                });
                position += 66;
            }

            Buttons.Add(new EntityToolbarButton(this, "spawn")
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
