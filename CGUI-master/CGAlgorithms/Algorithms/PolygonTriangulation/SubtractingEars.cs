using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class SubtractingEars : Algorithm
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
                outPolygons = polygons;
            }
            // TODO: Apply SubtractingEars on Polygon
            else
            {
                earPoint(points, outLines, outPoints, outPolygons);
            }

        }
        public void earPoint(List<CGUtilities.Point> points, List<CGUtilities.Line> outLines, List<CGUtilities.Point> outPoints, List<CGUtilities.Polygon> outPolygons)
        {
            List<Point> neighbors = new List<Point>();
            while (points.Count > 3)
            {
                //TODO: Iterate on each vertice of polygon
                for (int counter = 0; counter < points.Count; counter++)
                {
                    int checkEar = 1;
                    //TODO: Get check verticese 
                    getNeighbors(neighbors, points, counter);
                    Point prv = neighbors[0];
                    Point cur = points[counter];
                    Point nxt = neighbors[1];
                    //TODO: Check verticese Turn
                    Enums.TurnType checkTriangle = HelperMethods.CheckTurn(new Line(prv, cur), nxt);
                    if (checkTriangle == Enums.TurnType.Left)
                        continue;
                    //TODO: Check vertice of polygon with other verticese
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (points[i] == prv || points[i] == cur || points[i] == nxt)
                            continue;
                        Enums.PointInPolygon state = HelperMethods.PointInTriangle(points[i], prv, cur, nxt);
                        if (state == Enums.PointInPolygon.Inside || state == Enums.PointInPolygon.OnEdge)
                        {
                            checkEar = 0;
                            break;
                        }

                    }
                    //TODO: Remove ear and add new segement
                    if (checkEar == 1)
                    {
                        Line line = new Line(prv, nxt);
                        points.Remove(cur);
                        outLines.Add(line);
                        break;
                    }
                }
            }
            Line newLine = new Line(points[0], points[2]);
            outLines.Add(newLine);
        }
        public void getNeighbors(List<CGUtilities.Point> neighbors, List<CGUtilities.Point> points, int point)
        {
            neighbors.Clear();
            int verticeBefore = point - 1;
            int verticeAfter = point + 1;
            int verticePrevious = ((verticeBefore + points.Count) % points.Count);
            int verticeNext = verticeAfter % points.Count;
            Point prv = points[verticePrevious];
            Point next = points[verticeNext];
            neighbors.Add(prv);
            neighbors.Add(next);
        }
        public override string ToString()
        {
            return "Subtracting Ears";
        }
    }
}
