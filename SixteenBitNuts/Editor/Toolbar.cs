using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts.Editor
{
    public class Toolbar
    {
        public MapSectionEditor Editor { get; private set; }
        public List<ToolbarButton> Buttons { get; }
        public System.Type SelectedButtonType { get; set; }
        public int SelectedTileId { get; set; }
        public string SelectedEntityId { get; set; }

        public Toolbar(MapSectionEditor editor)
        {
            Editor = editor;
            SelectedTileId = 4;

            Buttons = new List<ToolbarButton>();

            int position = 64;
            for (int i = 4; i <= 12; i++)
            {
                Buttons.Add(new TileToolbarButton(this)
                {
                    Id = i,
                    Position = new Vector2(position, 4),
                });
                position += 22;
            }
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
