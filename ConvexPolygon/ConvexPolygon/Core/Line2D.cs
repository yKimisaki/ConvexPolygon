using System;

namespace Tonari.ConvexPolygon
{
	public class Line2D : IEquatable<Line2D>
    {
        public float A;
        public float B;
		public float C;
        
        public Point2D Begin;
        public Point2D End;
        
		public float Length;

        public Line2D() { }

        private Line2D(float a, float b, float c, Point2D begin, Point2D end, float length)
		{
			A = a;
			B = b;
			C = c;

			Begin = begin;
			End = end;

			Length = length;
		}

		private Line2D(float a, float b, float c)
		{
			A = a;
			B = b;
			C = c;

            Begin = Point2D.Zero;
            End = Point2D.Zero;

			Length = float.NaN;
		}

        internal Line2D(Point2D begin, Point2D end, bool isSegment = true)
		{
			float dx = end.X - begin.X;
			float dy = end.Y - begin.Y;

			A = dy;
			B = -dx;
			C = dx * begin.Y - dy * begin.X;

			Begin = begin;
			End = end;

            Length = isSegment ? (float)System.Math.Sqrt(dx * dx + dy * dy) : float.NaN;
		}

		public static bool TryCreate(float a, float b, float c, out Line2D line)
		{
			if (a == 0 && b == 0)
			{
				line = null;
				return false;
			}

			line = new Line2D(a, b, c);
			return true;
		}

        public static bool TryCreate(Point2D begin, Point2D end, out Line2D line, bool isSegment = true)
		{
			if (begin == end)
			{
				line = null;
				return false;
			}

			line = new Line2D(begin, end, isSegment);
			return true;
		}

		public static bool TryCreate(float beginX, float beginY, float endX, float endY, out Line2D line, bool isSegment = true)
		{
            return TryCreate(new Point2D { X = beginX, Y = beginY }, new Point2D { X = endX, Y = endY }, out line, isSegment);
		}

        private bool IsSegment { get { return !float.IsNaN(Length); } }
		private bool IsInvalid { get { return A == 0 && B == 0; } }

        public bool TryIntersect(Line2D target, out Point2D point)
		{
			if (IsInvalid || target.IsInvalid)
			{
                point = Point2D.Zero;
				return false;
			}

			float d = A * target.B - target.A * B;
            point = new Point2D
            {
					X = (B * target.C - target.B * this.C) / d,
					Y = (target.A * this.C - this.A * target.C) / d
            };
			if (IsSegment && !IsInRange(point))
				return false;
			if (target.IsSegment && !target.IsInRange(point))
				return false;

			return true;
		}

		public bool TryCreateBisector(out Line2D bisector)
		{
			if (!IsSegment)
			{
				bisector = null;
				return false;
			}

			return TryCreateBisector(Begin, End, out bisector);
		}

        public static bool TryCreateBisector(Point2D left, Point2D right, out Line2D bisector)
		{
			var cX = (left.X + right.X) * 0.5f;
			var cY = (left.Y + right.Y) * 0.5f;

            var begin = new Point2D { X = cX, Y = cY };
            var end = new Point2D { X = cX + (left.Y - right.Y), Y = cY + (right.X - left.X) };

			return TryCreate(begin, end, out bisector, false);
		}

        internal bool IsInRange(Point2D point)
		{
			var bx = System.Math.Abs(point.X - Begin.X) < Math.BigEpsilon ? 0f : point.X - Begin.X;
            var ex = System.Math.Abs(point.X - End.X) < Math.BigEpsilon ? 0f : point.X - End.X;
            var by = System.Math.Abs(point.Y - Begin.Y) < Math.BigEpsilon ? 0f : point.Y - Begin.Y;
            var ey = System.Math.Abs(point.Y - End.Y) < Math.BigEpsilon ? 0f : point.Y - End.Y;

			return bx * ex <= 0 && by * ey <= 0;
		}

		public bool Equals(Line2D other)
		{
			if (IsInvalid || other.IsInvalid)
				return false;

			if (IsSegment)
			{
				if (Begin == other.Begin && End == other.End)
					return true;
				else if (Begin == other.End && End == other.Begin)
					return true;
			}

			if (A != 0)
				return B / A == other.B / other.A && C / A == other.C / other.A;
			if (B != 0)
				return A / B == other.A / other.B && C / B == other.C / other.B;
		
			return false;
		}

		public override bool Equals(object obj)
		{
			var line = obj as Line2D;
			if (line == null)
				return false;

			return Equals(line);
		}

		public override int GetHashCode()
		{
			return (int)(A + B + C);
		}

		public override string ToString()
		{
			if (IsSegment)
				return string.Format("{0} -> {1}", Begin.ToString(), End.ToString());
			return string.Format("{0}x {1}y {2}", A, B, C);
		}

		public static bool operator ==(Line2D left, Line2D right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Line2D left, Line2D right)
		{
			return !left.Equals(right);
		}
	}
}
