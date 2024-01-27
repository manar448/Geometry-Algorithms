using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            //if number of points = 3 that is the polygon
            if (points.Count <= 3)
            {
                outPoints = new List<Point>(points); ;
            }

            else
            {
                // Find the point with the lowest Y-axis (leftmost-bottommost point)
                Point first_point = points[0];
                for (int i = 1; i < points.Count; i++)
                {
                    if (Compare_Points(points[i], first_point) < 0)
                        first_point = points[i];
                }

                // sort point about their angle
                List<Point> sortedPoints = new List<Point>();
                List<KeyValuePair<Point, double>> valuePairs = new List<KeyValuePair<Point, double>>();
                for (int i = 0; i < points.Count; i++)
                {
                    valuePairs.Add(new KeyValuePair<Point, double>(points[i], Math.Atan2(first_point.Y - points[i].Y, first_point.X - points[i].X) * 180 / Math.PI));
                }
                //frome large to small in x.
                valuePairs = valuePairs.OrderByDescending(x => x.Value).ToList();
                for (int i = valuePairs.Count - 1; i >= 0; i--)
                {
                    sortedPoints.Add(valuePairs[i].Key);
                }

                Stack<Point> convexHull = new Stack<Point>();
                convexHull.Push(sortedPoints[0]);
                convexHull.Push(sortedPoints[1]);

                // the remaining points
                for (int i = 2; i < sortedPoints.Count; i++)
                {
                    // Remove points from the stack while the current point is making a clockwise turn
                    while ((convexHull.Count >= 2) && !Orientation(convexHull.ElementAt(1), convexHull.Peek(), sortedPoints[i]))
                    {
                        convexHull.Pop();
                    }

                    convexHull.Push(sortedPoints[i]);
                }

                outPoints = convexHull.ToList();

                // check orientation of the last two points with first point in convex hull
                if (!Orientation(outPoints[0], outPoints[outPoints.Count - 1], outPoints[outPoints.Count - 2]))
                {
                    outPoints.RemoveAt(outPoints.Count - 1);

                }
            }
        }

        static bool Orientation(Point p1, Point p2, Point p3)
        {
            // Is CounterClockwise
            int crossProduct = (int)((p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y));
            return crossProduct > 0;
        }       

        private static int Compare_Points(Point point_1, Point point_2)
        {
            // return 1 if point_1 large, return -1 if point_2 large
            if ((point_1.Y < point_2.Y))
            {
                return -1;
            }
            if ((point_1.Y > point_2.Y))
            {
                return 1;
            }
            return 0;
        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}