using CGUtilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    public struct PointWithType
    {
        public Point Point { get; set; }
        public String Type { get; set; }

        public PointWithType(Point point, String type)
        {
            Point = point;
            Type = type;
        }
    }

    public struct EdgeWithHelper
    {
        public Line Edge { get; set; }
        public PointWithType Helper { get; set; }

        public EdgeWithHelper(Line edge, PointWithType helper)
        {
            Edge = edge;
            Helper = helper;
        }
    }


    class MonotonePartitioning : Algorithm
    {
        Point Min_X = new Point(double.MaxValue, double.MaxValue);
        int Index_Min_X;

        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            // sort points to turn Counter ClockWise
            lines = CounterClockwise(lines);

            List<PointWithType> classified_points = Extract_Points_and_Classify(lines);

            // sort classified points about y-axis
            classified_points = classified_points.OrderByDescending(p => p.Point.Y).ToList();

            List<EdgeWithHelper> helper = new List<EdgeWithHelper>();
            for (int i = 0; i < classified_points.Count; i++)
            {
                PointWithType current_point = classified_points[i];
                Line current_line = Find_Line_Start_End(lines, current_point.Point, "start");
                
                // Start
                if (classified_points[i].Type.Equals("Start"))
                {
                    //search for edge where current_point is the start point in it
                    EdgeWithHelper line_with_helper = new EdgeWithHelper(current_line, current_point);
                    helper.Add(line_with_helper);
                }
                // End
                else if (classified_points[i].Type.Equals("End")) 
                {
                    Line edge = Find_Line_Start_End(lines, current_point.Point, "End");
                    foreach (EdgeWithHelper line in helper)
                    {
                        EdgeWithHelper line_new_point = line;
                        if (line_new_point.Edge.Start == edge.Start && line_new_point.Edge.End == edge.End)
                        {
                            PointWithType h_vertex = line_new_point.Helper;
                            if (h_vertex.Type.Equals("Merge"))
                            {
                                outLines.Add(new Line(h_vertex.Point, current_point.Point));
                            }
                            helper.Remove(line); 
                            break;
                        }
                    }

                }
                // Split
                else if (classified_points[i].Type.Equals("Split")) 
                {
                    double closest_distance = double.MaxValue;
                    EdgeWithHelper line_to_left = helper[0];   // as initialize
                    //search for first edge on the left of current point in helper 
                    for (int r = 0; r < helper.Count; r++)
                    {
                        if (current_point.Point.X >= helper[r].Edge.Start.X && current_point.Point.X >= helper[r].Edge.End.X)
                        {
                            double distance = Math.Min(Math.Abs(current_point.Point.X - helper[r].Edge.Start.X), Math.Abs(current_point.Point.X - helper[r].Edge.End.X));
                            if (distance < closest_distance)
                            {
                                closest_distance = distance;
                                line_to_left = helper[r];
                            }
                        }
                    }
                    PointWithType h_vertex = line_to_left.Helper;   // Get Vertex from Closest Edge
                    outLines.Add(new Line(h_vertex.Point, current_point.Point));

                    // create new edges with helpers associated with the current point.
                    EdgeWithHelper line_with_helper = new EdgeWithHelper(line_to_left.Edge, current_point);
                    EdgeWithHelper line_with_helper_1 = new EdgeWithHelper(current_line, current_point);

                    // remove old edge from helper and add 2 new dges
                    EdgeWithHelper line_with_helper_re = line_to_left;
                    helper.Remove(line_with_helper_re);
                    helper.Add(line_with_helper);
                    helper.Add(line_with_helper_1);
                }
                // Merge
                else if (classified_points[i].Type.Equals("Merge")) 
                {
                    Line edge = Find_Line_Start_End(lines, current_point.Point, "End");
                    foreach (EdgeWithHelper line in helper)
                    {
                        if (line.Edge.Start == edge.Start && line.Edge.End == edge.End)
                        {
                            PointWithType h_vertex = line.Helper;
                            if (h_vertex.Type.Equals("Merge"))  
                            {
                                outLines.Add(new Line(h_vertex.Point, current_point.Point));
                            }
                            helper.Remove(line);
                            break;
                        }
                    }
                    //search for first edge on the left of current point in helper 
                    double closest_distance = double.MaxValue;
                    EdgeWithHelper line_to_left = helper[0];
                    for (int m = 0; m < helper.Count; m++)
                    {
                        if (current_point.Point.X >= helper[m].Edge.Start.X && current_point.Point.X >= helper[m].Edge.End.X)
                        {
                            double distance = Math.Min(Math.Abs(current_point.Point.X - helper[m].Edge.Start.X), Math.Abs(current_point.Point.X - helper[m].Edge.End.X));
                            if (distance < closest_distance)
                            {
                                closest_distance = distance;
                                line_to_left = helper[m];
                            }
                        }
                    }

                    PointWithType h_vertex_l = line_to_left.Helper;
                    if (h_vertex_l.Type.Equals("Merge"))
                    {
                        outLines.Add(new Line(h_vertex_l.Point, current_point.Point));
                    }
                    EdgeWithHelper line_with_helper_re = line_to_left;
                    EdgeWithHelper line_with_helper = new EdgeWithHelper(line_to_left.Edge, current_point);
                    helper.Remove(line_with_helper_re);
                    helper.Add(line_with_helper);

                }
                // RegularLeft
                else if (classified_points[i].Type.Equals("RegularLeft"))  
                {
                    Line edge = Find_Line_Start_End(lines, current_point.Point, "End");
                    foreach (EdgeWithHelper line in helper)
                    {
                        if (line.Edge.Start == edge.Start && line.Edge.End == edge.End)
                        {
                            PointWithType h_vertex = line.Helper;
                            if (h_vertex.Type.Equals("Merge")) 
                            {
                                outLines.Add(new Line(h_vertex.Point, current_point.Point));
                            }
                            helper.Remove(line);
                            helper.Add(new EdgeWithHelper(current_line, current_point));
                            break;
                        }
                    }
                }
                // RegularRight
                else if (classified_points[i].Type.Equals("RegularRight")) 
                {
                    //search for first edge on the left of current point in helper 
                    double closest_distance = double.MaxValue;
                    EdgeWithHelper line_to_left = helper[0];
                    for (int s = 0; s < helper.Count; s++)
                    {
                        if (current_point.Point.X >= helper[s].Edge.Start.X && current_point.Point.X >= helper[s].Edge.End.X)
                        {
                            double distance = Math.Min(Math.Abs(current_point.Point.X - helper[s].Edge.Start.X), Math.Abs(current_point.Point.X - helper[s].Edge.End.X));
                            if (distance < closest_distance)
                            {
                                closest_distance = distance;
                                line_to_left = helper[s];
                            }
                        }
                    }

                    PointWithType h_vertex = line_to_left.Helper;
                    if (h_vertex.Type.Equals("Merge"))
                    {
                        outLines.Add(new Line(h_vertex.Point, current_point.Point));
                        EdgeWithHelper line_with_helper_re = line_to_left;
                        EdgeWithHelper line_updated_point = new EdgeWithHelper(line_to_left.Edge, current_point);
                        helper.Remove(line_with_helper_re);
                        helper.Add(line_updated_point);
                    }
                } 
            }
        }
        public List<Line> CounterClockwise(List<Line> lines)
        {
            /// check That polygon is sorted CounterClockwise
            List<Point> line_points = new List<Point>();

            for (int i = 0; i < lines.Count; i++)
            {
                Point p = lines[i].Start;
                if (p.X <= Min_X.X)
                {
                    if (p.X < Min_X.X)
                    {
                        Min_X = p;
                        Index_Min_X = i;
                    }

                    else if (p.Y < Min_X.Y)
                    {
                        Min_X = p;
                        Index_Min_X = i;
                    }
                }
                line_points.Add(p);
            }


            Point previous = line_points[(Index_Min_X - 1 + line_points.Count()) % line_points.Count()];
            Point next = line_points[(Index_Min_X + 1) % line_points.Count()];

            // if points are sorted ClockWise ==> make it sortes CounterClockWise
            if (!(HelperMethods.CheckTurn(new Line(previous, next), Min_X) == Enums.TurnType.Right))
            {
                lines.Reverse();
                line_points.Reverse();
                for (int i = 0; i < lines.Count; i++)
                {
                    Point temp = lines[i].Start;
                    lines[i].Start = lines[i].End;
                    lines[i].End = temp;
                }
            }
            return lines;
        }
        
        List<PointWithType> Extract_Points_and_Classify(List<Line> polygon_lines)
        {
            if (polygon_lines == null || polygon_lines.Count == 0)
            {
                return new List<PointWithType>();
            }

            List<PointWithType> points = new List<PointWithType>();
            for (int i = 0; i < polygon_lines.Count; i++)
            {
                String type;
                if (i == polygon_lines.Count - 1)
                {
                    type = Classify_Point(polygon_lines[i].End, polygon_lines[i].Start, polygon_lines[0].End);
                }
                else
                {
                    type = Classify_Point(polygon_lines[i].End, polygon_lines[i].Start, polygon_lines[i + 1].End);
                }

                PointWithType pointWithType = new PointWithType(polygon_lines[i].End, type);
                if (!points.Contains(pointWithType))
                {
                    points.Add(pointWithType);
                }

            }
            return points;
        }

        public Line Find_Line_Start_End(List<Line> lines, Point point, String point_type)
        {
            foreach (var line in lines)
            {
                if (point_type == "Start" || point_type == "start")
                {
                    if (line.Start.Equals(point))
                    {
                        return line;
                    }
                }
                else if (point_type == "End" || point_type == "end")
                {
                    if (line.End.Equals(point))
                    {
                        return line;
                    }
                }
            }
            return null;
        }

        String Classify_Point(Point current, Point prev, Point next)
        {
            String type = "";

            // Start, Split
            if (current.Y > prev.Y && current.Y > next.Y)
            {
                if (HelperMethods.CheckTurn(new Line(prev, current), next) == Enums.TurnType.Left)
                {
                    type = "Start";
                }
                else
                {
                    type = "Split";
                }
            }
            // End, Merge
            else if (current.Y < prev.Y && current.Y < next.Y)
            {
                if (HelperMethods.CheckTurn(new Line(prev, current), next) == Enums.TurnType.Left)
                {
                    type = "End";
                }
                else
                {
                    type = "Merge";
                }

            }
            // RegularLeft
            else if (current.Y < prev.Y && current.Y > next.Y)
            {
                type = "RegularLeft";
            }
            // RegularRight
            else if (current.Y > prev.Y && current.Y < next.Y)
            {
                type = "RegularRight";
            }

            return type;
        }

        public override string ToString()
        {
            return "Monotone Partitioning";
        }
    }
}