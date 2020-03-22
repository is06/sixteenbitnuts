namespace SixteenBitNuts.Interfaces
{
    public interface ITile : IMapElement
    {
        int Id { get; }
        string MapTextDescription { get; }
    }
}
