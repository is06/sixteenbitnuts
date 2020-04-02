namespace SixteenBitNuts
{
    public struct TilesetSection
    {
        public Tileset Tileset { get; set; }

        public string MapTextDescription
        {
            get
            {
                return "ts " + Tileset.Name;
            }
        }

        public TilesetSection(Tileset tileset)
        {
            Tileset = tileset;
        }
    }
}