namespace SixteenBitNuts
{
    class MapDescriptorException : GameException
    {
        public MapDescriptorException(string message)
            : base("MapDescriptor error: " + message)
        {

        }
    }
}
