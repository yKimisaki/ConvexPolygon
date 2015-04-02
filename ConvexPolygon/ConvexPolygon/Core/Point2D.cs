using System;

namespace Tonari.ConvexPolygon
{
    public class Point2D : IEquatable<Point2D>
    {
        public static Point2D Zero { get { return new Point2D(); } }

        public float X;
        public float Y;

        public bool Equals(Point2D other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return other.X == X && other.Y == Y;
        }

        public override int GetHashCode()
        {
            return (int)(X + Y);
        }

        public static Point2D operator +(Point2D a, Point2D b)
        {
            return new Point2D { X = a.X + b.X, Y = a.Y + b.Y };
        }

        public static Point2D operator -(Point2D a, Point2D b)
        {
            return new Point2D { X = a.X - b.X, Y = a.Y - b.Y };
        }

        public static Point2D operator *(float a, Point2D b)
        {
            return new Point2D { X = a * b.X, Y = a * b.Y };
        }

        public static Point2D operator /(Point2D a, float b)
        {
            return new Point2D { X = a.X / b, Y = a.Y / b };
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Point2D);
        }

        public static bool operator ==(Point2D a, Point2D b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            return (!ReferenceEquals(a, null)) ? a.Equals(b) : b.Equals(a);
        }

        public static bool operator !=(Point2D a, Point2D b)
        {
            return !(a == b);
        }
    }
}
