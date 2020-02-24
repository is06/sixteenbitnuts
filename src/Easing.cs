namespace SixteenBitNuts
{
    public static class Easing
    {
        // Standard easing functions
        public static float Linear(float t) => t;
        public static float SmoothStart2(float t) => t * t;
        public static float SmoothStart3(float t) => t * t * t;
        public static float SmoothStart4(float t) => t * t * t * t;
        public static float SmoothStart5(float t) => t * t * t * t * t;
        public static float SmoothStop2(float t) => 1 - (1 - t) * (1 - t);
        public static float SmoothStop3(float t) => 1 - (1 - t) * (1 - t) * (1 - t);
        public static float SmoothStop4(float t) => 1 - (1 - t) * (1 - t) * (1 - t) * (1 - t);
        public static float SmoothStop5(float t) => 1 - (1 - t) * (1 - t) * (1 - t) * (1 - t) * (1 - t);
        public static float SmoothStep3(float t) => Crossfade(SmoothStart2(t), SmoothStop2(t), t);
        public static float SmoothStep6(float t) => Crossfade(SmoothStart5(t), SmoothStop5(t), t);
        public static float Hesitate6(float t) => Crossfade(SmoothStop5(t), SmoothStart5(t), t);
        public static float Arch2(float t) => Scale(Flip(t), t);
        public static float SmoothStartArch3(float t) => Scale(Arch2(t), t);
        public static float SmoothStopArch3(float t) => ReverseScale(Arch2(t), t);
        public static float BellCurve6(float t) => SmoothStop3(t) * SmoothStart3(t);

        // Complex functions
        public static float NormalizedBezier3(float b, float c, float t)
        {
            float s = 1.0f - t;
            float t2 = t * t;
            float s2 = s * s;
            float t3 = t2 * t;
            return (3.0f * b * s2 * t) + (3.0f * c * s * t2) + t3;
        }

        // Utilities
        private static float Flip(float t) => 1 - t;
        private static float Scale(float a, float t) => a * t;
        private static float ReverseScale(float a, float t) => (1 - t) * a;
        private static float Crossfade(float a, float b, float x) => a + x * (b - a);
    }
}
