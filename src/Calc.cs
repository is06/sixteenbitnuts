namespace SixteenBitNuts
{
    public static class Calc
    {
        public static float Bezier2(float a, float b, float c, float t)
        {
            float s = 1 - t;
            float s2 = s * s;
            float t2 = t * t;

            return (s2 * a) + (2 * b * s * t) + (t2 * c);
        }
    }
}
