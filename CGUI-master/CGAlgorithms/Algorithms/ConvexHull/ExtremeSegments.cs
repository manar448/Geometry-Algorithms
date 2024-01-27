using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
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
                //colinear ==> line uses to compare
                List<KeyValuePair<Point, Point>> list_of_colinear = new List<KeyValuePair<Point, Point>>();
                int left, right;
                double slope1 = 0, slope2 = 0;
                double distance_1, distance_2;
                Point point_1, point_2;
                for (int i = 0; i < points.Count; i++)
                {
                    for (int j = 0; j < points.Count; j++)
                    {
                        // if stop in the same point ==> skip this rotate
                        if (i == j)
                            continue;

                        left = 0;
                        right = 0;

                        // line (start ==> i, end ==> j)
                        Line line = new Line(points[i], points[j]);

                        // check the type of point (left, right)
                        for (int k = 0; k < points.Count; k++)
                        {
                            if (i != j && i != k && k != j)
                            {
                                Enums.TurnType type_of_point = HelperMethods.CheckTurn(line, points[k]);
                                if (type_of_point == Enums.TurnType.Left)
                                {
                                    left++;
                                }
                                else if (type_of_point == Enums.TurnType.Right)
                                {
                                    right++;
                                }
                            }
                        }

                        bool flag_coliner = false;

                        // if extreme line (all points in one side of this line)
                        if (left == points.Count - 2 || right == points.Count - 2)
                        {
                            if (!outPoints.Contains(points[i]))
                            {
                                outPoints.Add(points[i]);
                            }
                            if (!outPoints.Contains(points[j]))
                            {
                                outPoints.Add(points[j]);
                            }
                        }

                        else if (left == 0 || right == 0)
                        {
                            KeyValuePair<Point, Point> x = new KeyValuePair<Point, Point>(points[i], points[j]);
                            if (list_of_colinear.Count == 0)
                            {
                                list_of_colinear.Add(x);
                            }
                            else
                            {
                                // calculate slope and distance for each line 
                                for (int co_var = 0; co_var < list_of_colinear.Count; co_var++)
                                {
                                    point_1 = list_of_colinear[co_var].Key;
                                    point_2 = list_of_colinear[co_var].Value;
                                    slope1 = (point_2.Y - point_1.Y) / (point_2.X - point_1.X);
                                    slope2 = (points[i].Y - points[j].Y) / (points[i].X - points[j].X);

                                    if (slope2 == slope1)
                                    {
                                        flag_coliner = true;
                                        double diff_x = 0f, diff_y = 0f;
                                        diff_x = point_1.X - point_2.X;
                                        diff_y = point_1.Y - point_2.Y;
                                        distance_1 = Math.Sqrt(diff_x * diff_x + diff_y * diff_y);
                                        diff_x = points[i].X - points[j].X;
                                        diff_y = points[i].Y - points[j].Y;
                                        distance_2 = Math.Sqrt(diff_x * diff_x + diff_y * diff_y);
                                        if (distance_2 > distance_1)
                                        {
                                            list_of_colinear[co_var] = x;
                                        }
                                    }
                                }
                                if (!flag_coliner)
                                {
                                    list_of_colinear.Add(x);
                                }
                            }
                        }
                    }
                }

                foreach (KeyValuePair<Point, Point> c in list_of_colinear)
                {
                    if (!outPoints.Contains(c.Key))
                    {
                        outPoints.Add(c.Key);
                    }
                    if (!outPoints.Contains(c.Value))
                    {
                        outPoints.Add(c.Value);
                    }
                }
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
