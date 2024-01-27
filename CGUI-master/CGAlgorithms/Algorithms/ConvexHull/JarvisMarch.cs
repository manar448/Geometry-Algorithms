using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            //If the two points are equal.
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count;)
                {
                    if (points[i].X == points[j].X && points[i].Y == points[j].Y)
                    {
                        points.Remove(points[j]);
                    }
                    else
                    {
                        j++;
                    }
                }
            }

            //if number of points = 3 that is the poligon
            if (points.Count <= 3)
            {
                outPoints = new List<Point>(points);
            }
            else
            {
                double y_min = points[0].Y;
                Point smallest_point = new Point(points[0].X, points[0].Y);
                // smallest point about y-axis
                for (int i = 1; i < points.Count; i++)
                {
                    if (points[i].Y < y_min)
                    {
                        y_min = points[i].Y;
                        smallest_point = points[i];
                    }
                }

                // x,y of smallest point
                double x = smallest_point.X;
                double y = smallest_point.Y;

                // start point of polygon
                Point start_point = new Point(x, y);

                outPoints.Add(smallest_point);

                do
                {
                    smallest_point = outPoints[outPoints.Count - 1];
                    Point coliner = null;

                    for (int i = 0; i < points.Count; i++)
                    {
                        if (smallest_point == points[i])
                            continue;
                        int left = 0, right = 0;

                        // check the type of point (left, right)
                        for (int j = 0; j < points.Count; j++)
                        {
                            if (smallest_point != points[j] && i != j)
                            {
                                Enums.TurnType type = HelperMethods.CheckTurn(new Line(smallest_point, points[i]), points[j]);
                                if (type == Enums.TurnType.Left)
                                {
                                    left++;
                                }
                                else if (type == Enums.TurnType.Right)
                                {
                                    right++;
                                    break;
                                }
                            }
                        }

                        // if extreme line (all points in one side of this line)
                        if (left == points.Count - 2)
                        {
                            outPoints.Add(points[i]);
                            break;
                        }
                        else if (right == 0)
                        {
                            if (coliner == null)
                            {
                                coliner = points[i];
                            }
                            else
                            {
                                // calculate distance for each line
                                double distance_1 = Math.Sqrt(Math.Pow(smallest_point.X - coliner.X, 2) + Math.Pow(smallest_point.Y - coliner.Y, 2));
                                double distance_2 = Math.Sqrt(Math.Pow(smallest_point.X - points[i].X, 2) + Math.Pow(smallest_point.Y - points[i].Y, 2));
                                if (distance_2 > distance_1)
                                {
                                    coliner = points[i];
                                }
                            }
                        }
                    }
                    if (coliner != null)
                    {
                        outPoints.Add(coliner);
                    }
                    // If put the starting point twice in outpoints
                    if (outPoints[outPoints.Count - 1].X == start_point.X && outPoints[outPoints.Count - 1].Y == start_point.Y)
                    {
                        outPoints.Remove(outPoints[outPoints.Count - 1]);
                        break;
                    }
                } while (true);
            }
        }


        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
