using Microsoft.Xna.Framework;
using System;

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

    public static class DirectionHelper
    {
        public static string Identifier(this Direction direction)
        {
            return direction switch
            {
                Direction.Left => "left",
                Direction.Right => "right",
                Direction.TopRight => "topright",
                Direction.Top => "top",
                Direction.TopLeft => "topleft",
                Direction.BottomLeft => "bottomleft",
                Direction.Bottom => "bottom",
                Direction.BottomRight => "bottomright",
                _ => "none",
            };
        }

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

        /// <summary>
        /// Retrive the radian value of a given direction, -1 if the direction is none
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>Radian value</returns>
        public static double GetRadians(this Direction direction)
        {
            return direction switch
            {
                Direction.Left => Math.PI,
                Direction.Right => 0,
                Direction.TopRight => Math.PI / 4,
                Direction.Top => Math.PI / 2,
                Direction.TopLeft => 3 * Math.PI / 4,
                Direction.BottomLeft => 5 * Math.PI / 4,
                Direction.Bottom => 3 * Math.PI / 2,
                Direction.BottomRight => 7 * Math.PI / 4,
                _ => -1,
            };
        }

        public static Direction FromNormalizedHorizontal(int value)
        {
            if (value == 1)
            {
                return Direction.Right;
            }
            else if (value == -1)
            {
                return Direction.Left;
            }
            return Direction.None;
        }

        public static Direction FromNormalizedVector(Vector2 value)
        {
            if (value.X == 1)
            {
                if (value.Y == 1)
                {
                    return Direction.BottomRight;
                }
                else if (value.Y == -1)
                {
                    return Direction.TopRight;
                }
                else
                {
                    return Direction.Right;
                }
            }
            else if (value.X == -1)
            {
                if (value.Y == 1)
                {
                    return Direction.BottomLeft;
                }
                else if (value.Y == -1)
                {
                    return Direction.TopLeft;
                }
                else
                {
                    return Direction.Left;
                }
            }
            else
            {
                if (value.Y == 1)
                {
                    return Direction.Bottom;
                }
                else if (value.Y == -1)
                {
                    return Direction.Top;
                }
                else
                {
                    return Direction.None;
                }
            }
        }
    }
}
