namespace SixteenBitNuts
{
    public enum Direction : int
    {
        None = -1,
        Right = 0,
        TopRight = 1,
        Top = 2,
        TopLeft = 3,
        Left = 4,
        BottomLeft = 5,
        Bottom = 6,
        BottomRight = 7
    }

    public static class DirectionMethods
    {
        public static Direction Opposite(this Direction direction)
        {
            return direction switch
            {
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                Direction.TopRight => Direction.BottomLeft,
                Direction.Top => Direction.Bottom,
                Direction.TopLeft => Direction.BottomRight,
                Direction.BottomLeft => Direction.TopRight,
                Direction.Bottom => Direction.Top,
                Direction.BottomRight => Direction.TopLeft,
                _ => Direction.None,
            };
        }

        public static Direction HorizontalRound(this Direction direction)
        {
            return direction switch
            {
                Direction.BottomLeft => Direction.Left,
                Direction.BottomRight => Direction.Right,
                Direction.TopLeft => Direction.Left,
                Direction.TopRight => Direction.Right,
                _ => direction,
            };
        }

        public static sbyte HorizontalSign(this Direction direction)
        {
            return direction switch
            {
                Direction.Right => 1,
                Direction.Left => -1,
                _ => 0,
            };
        }
    }
}
