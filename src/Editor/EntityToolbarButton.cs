using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts.Editor
{
    public class EntityToolbarButton : IconableToolbarButton
    {
        public string EntityType { get; private set; }

        public EntityToolbarButton(Toolbar toolbar, string entityType, string? textureName) : base(toolbar)
        {
            Type = ToolbarButtonType.Entity;
            EntityType = entityType;

            iconTexture = entityType switch
            {
                "spawn" => toolbar.Editor.Map.Game.Content.Load<Texture2D>("Engine/editor/" + textureName),
                _ => null,
            };
        }
    }
}
