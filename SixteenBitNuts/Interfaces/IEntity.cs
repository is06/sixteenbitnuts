namespace SixteenBitNuts.Interfaces
{
    public interface IEntity : IMapElement
    {
        string Name { get; }
        void Destroy();
    }
}
