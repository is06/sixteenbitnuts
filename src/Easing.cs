using Microsoft.Xna.Framework;

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
        public static float SmoothStepArch4(float t) => ReverseScale(Scale(Arch2(t), t), t);
        public static float BellCurve6(float t) => SmoothStopArch3(t) * SmoothStartArch3(t);

        // Complex functions

        public static float NormalizedBezier2(float a, float t)
        {
            return Calc.Bezier2(0, a, 1, t);
        }

        public static float NormalizedBezier3(float b, float c, float t)
        {
            float s = 1 - t;
            float t2 = t * t;
            float s2 = s * s;
            float t3 = t2 * t;

            return (3 * b * s2 * t)
                + (3 * c * s * t2)
                + t3;
        }

        public static float NormalizedBezier5(float b, float c, float d, float e, float t)
        {
            float s = 1 - t;
            float t2 = t * t;
            float s2 = s * s;
            float t3 = t2 * t;
            float s3 = s2 * s;
            float t4 = t2 * t2;
            float s4 = s2 * s2;
            float t5 = t3 * t2;

            return (5 * b * s4 * t)
                + (15 * c * s3 * t2)
                + (15 * d * s2 * t3)
                + (5 * e * s * t4)
                + t5;
        }

        public static float NormalizedBezier7(float b, float c, float d, float e, float f, float g, float t)
        {
            float s = 1 - t;
            float t2 = t * t;
            float s2 = s * s;
            float t3 = t2 * t;
            float s3 = s2 * s;
            float t4 = t2 * t2;
            float s4 = s2 * s2;
            float t5 = t3 * t2;
            float s5 = s3 * s2;
            float t6 = t3 * t3;
            float s6 = s3 * s3;
            float t7 = t3 * t2 * t2;

            return (7 * b * s6 * t)
                + (21 * c * s5 * t2)
                + (35 * d * s4 * t3)
                + (35 * e * s3 * t4)
                + (21 * f * s2 * t5)
                + (7 * g * s * t6)
                + t7;
        }

        // Utilities
        private static float Flip(float t) => 1 - t;
        private static float Scale(float a, float t) => t * a;
        private static float ReverseScale(float a, float t) => (1 - t) * a;
        private static float Crossfade(float a, float b, float x) => a + x * (b - a);
    }
}
