namespace SixteenBitNuts
{
    class Calc
    {
        public static sbyte Sign(int value)
        {
            return (sbyte)(value > 0 ? 1 : (value < 0 ? -1 : 0));
        }
    }
}
