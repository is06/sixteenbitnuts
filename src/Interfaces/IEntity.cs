namespace SixteenBitNuts.Interfaces
{
    public interface IEntity : IMapElement
    {
        string Name { get; }
        string MapTextDescription { get; }
        bool IsDestroying { get; }
        bool IsCollectable { get; set; }
        bool IsBehindThePlayer { get; set; }
        bool IsVisibleInPreMainDisplay { get; }
        void Destroy();
    }
}
