﻿namespace SixteenBitNuts
{
    public struct Size
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public float AspectRatio
        {
            get
            {
                return Width / (float)Height;
            }
        }
    }
}
