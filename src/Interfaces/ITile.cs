namespace SixteenBitNuts.Interfaces
{
    public interface ITile : IMapElement
    {
        int Id { get; set; }
        string? GroupName { get; set; }
        string MapTextDescription { get; }
    }
}
