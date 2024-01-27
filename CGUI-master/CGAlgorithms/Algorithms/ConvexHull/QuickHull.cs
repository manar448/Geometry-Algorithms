using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count == 1)
            {
                outPoints = points;
                outLines = null;
                outPolygons = null;

            }
            else
            {
                List<Point> convexHull = new List<Point>();

                int min_x = 0;
                int max_x = 0;
                for (int i = 1; i < points.Count; i++)
                {
                    if (points[i].X < points[min_x].X)
                        min_x = i;
                    if (points[i].X > points[max_x].X)
                        max_x = i;
                }

                convexHull.Add(points[min_x]);
                convexHull.Add(points[max_x]);
                Line linee = new Line(points[min_x], points[max_x]);
                Line line_revirse = new Line(points[max_x], points[min_x]);

               
                outLines.Add(linee);
                outLines.Add(line_revirse);

                List<Point> left_points = new List<Point>();
                List<Point> right_points = new List<Point>();

                // check the type of point (left, right)
                get_type_of_point(points, linee, left_points, right_points);

                Quick_Hull_Rec(left_points, linee, convexHull);
                Quick_Hull_Rec(right_points, line_revirse, convexHull);
                outPoints = convexHull;
            }
        }

        public void Quick_Hull_Rec(List<Point> points, Line line, List<Point> convexHull)
        {
            if (points.Count == 0)
            {
                return;
            }

            int index = -1;
            double maxDistance = 0;

            for (int i = 0; i < points.Count; i++)
            {
                double distance = Get_Distance(line, points[i]);
                if (distance > maxDistance)
                {
                    index = i;
                    maxDistance = distance;
                }
            }

            if (index == -1)
                return;

            convexHull.Insert(convexHull.IndexOf(line.End), points[index]);

            Line line_1 = new Line(line.Start, points[index]);
            List<Point> left_point_of_line1 = new List<Point>();
            List<Point> right_point_of_line1 = new List<Point>();
            get_type_of_point(points, line_1, left_point_of_line1, right_point_of_line1);
            Quick_Hull_Rec(left_point_of_line1, line_1, convexHull);

            Line line_2 = new Line(points[index], line.End);
            List<Point> left_point_of_line2 = new List<Point>();
            List<Point> right_point_of_line2 = new List<Point>();

            get_type_of_point(points, line_2, left_point_of_line2, right_point_of_line2);
            Quick_Hull_Rec(left_point_of_line2, line_2, convexHull);
        }
        private void get_type_of_point(List<Point> points, Line line, List<Point> left_points, List<Point> right_points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i] == line.Start || points[i] == line.End)
                {
                    continue;
                }
                Enums.TurnType type = HelperMethods.CheckTurn(line, points[i]);
                if (type == Enums.TurnType.Left)
                {
                    left_points.Add(points[i]);
                }
                if (type == Enums.TurnType.Right)
                {
                    right_points.Add(points[i]);
                }
            }
        }

        private double Get_Distance(Line line, Point c)
        {
            double d1 = Math.Sqrt(Math.Pow(line.Start.X - line.End.X, 2) + Math.Pow(line.Start.Y - line.End.Y, 2));
            double d2 = Math.Sqrt(Math.Pow(line.End.X - c.X, 2) + Math.Pow(line.End.Y - c.Y, 2));
            double d3 = Math.Sqrt(Math.Pow(line.Start.X - c.X, 2) + Math.Pow(line.Start.Y - c.Y, 2));
            double D = (d1 + d2 + d3) / 3;
            return D;
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}