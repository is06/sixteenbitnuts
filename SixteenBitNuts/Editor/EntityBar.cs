using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SixteenBitNuts.Editor
{
    class EntityBar
    {
        public MapSectionEditor Editor { get; private set; }
        public List<EntityBarButton> Buttons { get; }
        public int SelectedTileId { get; set; }

        public EntityBar(MapSectionEditor editor)
        {
            Editor = editor;
            SelectedTileId = 4;

            Buttons = new List<EntityBarButton>();

            int position = 64;
            for (int i = 4; i <= 12; i++)
            {
                Buttons.Add(new EntityBarButton(this, i, new Vector2(position, 4)));
                position += 22;
            }
        }

        public void Update()
        {
            foreach (EntityBarButton button in Buttons)
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
            foreach (EntityBarButton button in Buttons)
            {
                button.Draw();
            }
        }
    }
}
