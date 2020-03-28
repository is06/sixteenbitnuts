namespace SixteenBitNuts.Interfaces
{
    public interface IEntity : IMapElement
    {
        string Name { get; }
        string MapTextDescription { get; }
        bool IsVisibleInPreMainDisplay { get; }
        void Destroy();
    }
}
