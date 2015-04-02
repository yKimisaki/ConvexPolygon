
namespace Tonari.ConvexPolygon
{
	public class Triangle2D
    {
        public int[] Triangle;
		public float Area;

        public Triangle2D() { }

        private Triangle2D(Point2D a, Point2D b, Point2D c, int n)
		{
			Area = ((a.X - b.X) * (a.Y + b.Y) + (b.X - c.X) * (b.Y + c.Y) + (c.X - a.X) * (c.Y + a.Y)) * 0.5f;

			if (Area > 0)
				Triangle = new[] { 0, 2 + n, 1 + n };
			else
			{
				Area = -Area;
				Triangle = new[] { 0, 1 + n, 2 + n };
			}
		}

        public static bool TryCreate(Point2D a, Point2D b, Point2D c, int polygonNumber, out Triangle2D triangle)
		{
			if (a == b || b == c || c == a)
			{
				triangle = null;
				return false;
			}

			triangle = new Triangle2D(a, b, c, polygonNumber);
			return true;
		}
	}
}
