using System.Collections.Generic;

namespace SixteenBitNuts
{
    public struct TilesetGroup
    {
        public string Name;
        public bool IsAutoTilingEnabled;
        public Dictionary<TilesetGroupDefinitionType, TilesetGroupDefinition>? Definitions;
    }

    public struct TilesetGroupDefinition
    {
        public TilesetGroupDefinitionType? Type;
        public int TileIndex;

        public static TilesetGroupDefinitionType? GetTypeFromString(string type)
        {
            return type switch
            {
                "topleft" => TilesetGroupDefinitionType.TopLeft,
                "top" => TilesetGroupDefinitionType.Top,
                "topright" => TilesetGroupDefinitionType.TopRight,
                "left" => TilesetGroupDefinitionType.Left,
                "center" => TilesetGroupDefinitionType.Center,
                "right" => TilesetGroupDefinitionType.Right,
                "bottomleft" => TilesetGroupDefinitionType.BottomLeft,
                "bottom" => TilesetGroupDefinitionType.Bottom,
                "bottomright" => TilesetGroupDefinitionType.BottomRight,
                "cornertopleft" => TilesetGroupDefinitionType.CornerTopLeft,
                "cornertopright" => TilesetGroupDefinitionType.CornerTopRight,
                "cornerbottomleft" => TilesetGroupDefinitionType.CornerBottomLeft,
                "cornerbottomright" => TilesetGroupDefinitionType.CornerBottomRight,
                "hnarrowleft" => TilesetGroupDefinitionType.HorizontalNarrowLeft,
                "hnarrowcenter" => TilesetGroupDefinitionType.HorizontalNarrowCenter,
                "hnarrowright" => TilesetGroupDefinitionType.HorizontalNarrowRight,
                "vnarrowtop" => TilesetGroupDefinitionType.VerticalNarrowTop,
                "vnarrowcenter" => TilesetGroupDefinitionType.VerticalNarrowCenter,
                "vnarrowbottom" => TilesetGroupDefinitionType.VerticalNarrowBottom,
                "single" => TilesetGroupDefinitionType.Single,
                _ => null,
            };
        }
    }

    public enum TilesetGroupDefinitionType
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight,
        CornerTopLeft,
        CornerTopRight,
        CornerBottomLeft,
        CornerBottomRight,
        HorizontalNarrowLeft,
        HorizontalNarrowCenter,
        HorizontalNarrowRight,
        VerticalNarrowTop,
        VerticalNarrowCenter,
        VerticalNarrowBottom,
        Single
    }
}
