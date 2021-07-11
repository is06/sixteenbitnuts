namespace SixteenBitNuts.Interfaces
{
    public interface IEntity : IMapElement
    {
        string Name { get; }
        string? Tag { get;  }
        string MapTextDescription { get; }
        bool IsGenerated { get; set; }
        bool IsDestroying { get; }
        bool IsCollectable { get; set; }
        bool IsBehindThePlayer { get; set; }
        bool IsVisibleInPreMainDisplay { get; }
        void Destroy();
    }
}
