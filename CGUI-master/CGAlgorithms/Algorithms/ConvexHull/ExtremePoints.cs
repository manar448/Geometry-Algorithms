using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // If the two points are equal
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

            List<Point> points_extream = new List<Point>(points);
            for (int i = 0; i < points.Count; i++)
            {
                bool outtt = false;
                for (int j = 0; j < points.Count; j++)
                {
                    for (int k = 0; k < points.Count; k++)
                    {
                        for (int l = 0; l < points.Count; l++)
                        {
                            if (i != j && i != k && i != l && j != k && j != l && k != l)
                            {
                                Enums.PointInPolygon type_point = HelperMethods.PointInTriangle(points[i], points[j], points[k], points[l]);
                                if (type_point == Enums.PointInPolygon.Inside || type_point == Enums.PointInPolygon.OnEdge)
                                {
                                    points_extream.Remove(points[i]);
                                    outtt = true;
                                    break;
                                }
                            }
                        }
                        if (outtt) break;

                    }
                    if (outtt) break;
                }
            }
            outPoints = new List<Point>(points_extream);
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
