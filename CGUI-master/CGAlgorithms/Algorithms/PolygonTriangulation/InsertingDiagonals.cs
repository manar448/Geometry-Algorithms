using CGAlgorithms.Algorithms.ConvexHull;
using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGUtilities.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class InsertingDiagonals : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            // TODO: Extract points from lines
            for (int i = 0; i < lines.Count; i++)
            {
                points.Add(lines[i].Start);
            }
            // TODO: Enter point, single line or Triangle
            if (points.Count <= 3)

            {
                outPoints = points;
                outLines = lines;
            }
            // TODO: Apply InsertingDiagonals on Polygon
            else
            {
                outLines = DecomposePolygon(points);
            }
        }
        private List<Line> DecomposePolygon(List<CGUtilities.Point> points)
        {
            //TODO: Base case
            if (points.Count <= 3)
            {
                return (new List<Line>());
            }
            else
            {
                //TODO: Check the orientatio
                int curr = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].X < points[curr].X)
                        curr = i;
                }
                int Current_str = (curr - 1 + points.Count) % points.Count;
                int next_str = (curr + 1 + points.Count) % points.Count;
                Line l = new Line(points[Current_str], points[next_str]);
                if (HelperMethods.CheckTurn(l, points[curr]) == Enums.TurnType.Left)
                    points.Reverse();

                //TODO: Call check Convex points
                curr = 0;
                while (true)
                {
                    if (!(checkConvex(points, curr)) == false) { break; }
                    curr++;
                }
                List<Line> outputList = new List<Line>();
                List<Point> subPoygon1 = new List<Point>();
                List<Point> subPoygon2 = new List<Point>();
                //TODO: Initial points
                int numOfPolygonVertices = points.Count;
                int verticeBefore = curr - 1;
                int verticeAfter = curr + 1;
                int previous_point = ((verticeBefore + numOfPolygonVertices) % numOfPolygonVertices);
                int next_point = (verticeAfter % numOfPolygonVertices);
                //TODO: Find subPolygon
                int element1, element2;
                int flage = getPoint(points, previous_point, next_point, curr);
                if (flage == -1)
                {
                    outputList.Add(new Line(points[previous_point], points[next_point]));
                    element1 = next_point;
                    element2 = previous_point;
                }
                else
                {
                    outputList.Add(new Line(points[curr], points[flage]));
                    element1 = curr;
                    element2 = flage;
                }
                //TODO: Decompose Polygon into two sub Polygons
                for (int i = Math.Max(element1, element2); i != Math.Min(element1, element2); i = (i + 1) % points.Count)
                    subPoygon1.Add(points[i]);
                for (int i = Math.Min(element1, element2); i != Math.Max(element1, element2); i = (i + 1) % points.Count)
                    subPoygon2.Add(points[i]);
                //TODO: Iterate the process.
                subPoygon1.Add(points[Math.Min(element1, element2)]); subPoygon2.Add(points[Math.Max(element1, element2)]);
                outputList.AddRange(DecomposePolygon(subPoygon1));
                outputList.AddRange(DecomposePolygon(subPoygon2));
                return outputList;
            }
        }
        public bool checkConvex(List<CGUtilities.Point> points, int verticeCurrent)
        {
            //TODO: Indexses of check Convex points
            int numOfPolygonVertices = points.Count;
            int verticeBefore = verticeCurrent - 1;
            int verticeAfter = verticeCurrent + 1;
            //TODO: Initialize start of check Convex points
            Point Current_str = points[verticeCurrent];
            Point previous_str = points[((verticeBefore + numOfPolygonVertices) % numOfPolygonVertices)];
            Point next_str = points[(verticeAfter % numOfPolygonVertices)];
            //TODO: Check Convex point
            Line checkLine = new Line(previous_str, Current_str);
            return (HelperMethods.CheckTurn(checkLine, next_str) == Enums.TurnType.Left) ? true : false;
        }
        public double similarity(Point str, Point end, Point cheakPoint)
        {
            double res = Math.Abs(((end.X - str.X) * (str.Y - cheakPoint.Y)) - ((str.X - cheakPoint.X) * (end.Y - str.Y)));
            double index = Math.Pow(end.X - str.X, 2) + Math.Pow(end.Y - str.Y, 2);
            res /= Math.Sqrt(index);
            return res;
        }
        private int getPoint(List<Point> p, int prv, int nxt, int curr)
        {
            double maxCheck = -1e6;
            int maxCurr = -1;
            for (int i = 0; i < p.Count; i++)
                if (HelperMethods.PointInTriangle(p[i], p[curr], p[prv], p[nxt]) == Enums.PointInPolygon.Inside)
                {
                    double dis = similarity(p[prv], p[nxt], p[i]);
                    if (dis > maxCheck)
                    {
                        maxCheck = dis;
                        maxCurr = i;
                    }
                }
            return maxCurr;
        }
        public override string ToString()
        {
            return "Inserting Diagonals";
        }
    }
}