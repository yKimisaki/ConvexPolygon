using System;

namespace Tonari.ConvexPolygon
{
    public class Area2D
    {
        public Point2D[] Apexes;
        public Line2D[] Edges;
        public int[] Triangles;
        public float Area;

        public Area2D() { }

        private Area2D(Point2D[] apexes, Line2D[] edges, int[] triangles, float area)
        {
            Apexes = apexes;
            Edges = edges;
            Triangles = triangles;
            Area = area;
        }

        public static bool TryCreate(out Area2D area, params Point2D[] points)
        {
            var length = points.Length;

            if (length < 3)
            {
                area = null;
                return false;
            }

            var cache = 0f;
            for (var i = 0; i < length; ++i)
            {
                if (points[i] == points[(i + 1) % length])
                {
                    var distinct = new Point2D[length - 1];
                    if (i == length - 1)
                        Array.Copy(points, 0, distinct, 0, length - 1);
                    else
                    {
                        Array.Copy(points, 0, distinct, 0, i + 1);
                        Array.Copy(points, i + 2, distinct, i + 1, length - (i + 2));
                    }
                    return TryCreate(out area, distinct);
                }

                var rotation = CheckRotation(points[i], points[(i + 1) % length], points[(i + 2) % length]);
                if (i != 0 && cache * rotation < 0)
                {
                    area = null;
                    return false;
                }
                cache = rotation;
            }
            if (cache < 0)
            {
                var sorted = new Point2D[length];
                sorted[0] = points[0];
                for (var i = 1; i < length; ++i)
                    sorted[i] = points[length - i];
                points = sorted;
            }

            var a = 0f;
            var edges = new Line2D[length];
            var triangles = new int[(length - 2) * 3];
            for (var i = 0; i < length; ++i)
            {
                Line2D.TryCreate(points[i], points[(i + 1) % length], out edges[i]);
                if (i < length - 2)
                {
                    Triangle2D t;
                    Triangle2D.TryCreate(points[0], points[i + 1], points[i + 2], i, out t);
                    a += t.Area;
                    for (var j = 0; j < 3; ++j)
                        triangles[i * 3 + j] = t.Triangle[j];
                }
            }

            area = new Area2D(points, edges, triangles, a);
            return true;
        }

        public bool TryCut(Line2D devider, Point2D contains, out Area2D cut)
        {
            Point2D begin, end;
            if (!TryGetIntersections(devider, out begin, out end))
            {
                cut = null;
                return false;
            }

            var side = CheckRotation(begin, end, contains);

            if (side == 0)
            {
                cut = null;
                return false;
            }

            bool first = false, previous = false;
            int beginIndex = -1, endIndex = -1;
            for (var i = 0; i < Apexes.Length; ++i)
            {
                var current = side * CheckRotation(begin, end, Apexes[i]) > 0;
                if (i == 0)
                {
                    first = previous = current;
                    continue;
                }

                if (current && !previous)
                    beginIndex = i;
                if (!current && previous)
                    endIndex = (i - 1) % Apexes.Length;

                previous = current;
            }
            if (first && !previous)
                beginIndex = 0;

            if (beginIndex == endIndex)
            {
                if (beginIndex != -1)
                    return TryCreate(out cut, begin, end, Apexes[beginIndex]);

                cut = null;
                return false;
            }

            if (beginIndex > endIndex)
                endIndex += Apexes.Length;
            var apexes = new Point2D[endIndex - beginIndex + 3];
            for (var i = 0; i < apexes.Length; ++i)
            {
                if (i == 0)
                {
                    apexes[i] = side > 0 ? begin : end;
                    continue;
                }
                if (i == 1)
                {
                    apexes[i] = side > 0 ? end : begin;
                    continue;
                }
                apexes[i] = Apexes[(beginIndex + i - 2) % Apexes.Length];
            }

            return TryCreate(out cut, apexes);
        }

        public bool TryGetIntersections(Line2D line, out Point2D begin, out Point2D end)
        {
            for (var i = 0; ; ++i)
            {
                if (i < Apexes.Length)
                {
                    if (!Edges[i].TryIntersect(line, out begin))
                    {
                        continue;
                    }

                    for (++i; ; ++i)
                    {
                        if (i < Apexes.Length)
                        {
                            if (Edges[i].TryIntersect(line, out end))
                                return true;
                        }
                        else
                        {
                            begin = end = Point2D.Zero;
                            return false;
                        }
                    }
                }
                else
                {
                    begin = end = Point2D.Zero;
                    return false;
                }
            }
        }

        public bool Contains(Point2D point)
        {
            var length = Apexes.Length;
            var cache = 0f;
            for (var i = 0; i < length; ++i)
            {
                var rotation = CheckRotation(Apexes[i], Apexes[(i + 1) % length], point);
                if (i != 0)
                {
                    if (rotation == 0)
                    {
                        var line = new Line2D(Apexes[i], Apexes[(i + 1) % length]);
                        if (!line.IsInRange(point))
                            return false;
                    }
                    else if (cache * rotation < 0)
                        return false;
                }
                cache = rotation;
            }
            return true;
        }

        private static float CheckRotation(Point2D begin, Point2D end, Point2D point)
        {
            var ebx = System.Math.Abs(end.X - begin.X) < Math.BigEpsilon ? 0f : (end.X - begin.X);
            var pby = System.Math.Abs(point.Y - begin.Y) < Math.BigEpsilon ? 0f : (point.Y - begin.Y);
            var pbx = System.Math.Abs(point.X - begin.X) < Math.BigEpsilon ? 0f : (point.X - begin.X);
            var eby = System.Math.Abs(end.Y - begin.Y) < Math.BigEpsilon ? 0f : (end.Y - begin.Y);
            return ebx * pby - pbx * eby;
        }

        public override string ToString()
        {
            string result = string.Empty;
            foreach (var a in Apexes)
                result += (a.ToString()) + " => ";
            return result;
        }
    }
}