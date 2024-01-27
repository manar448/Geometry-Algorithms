using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation : Algorithm
    {
        public override void Run(System.Collections.Generic.List<CGUtilities.Point> points, System.Collections.Generic.List<CGUtilities.Line> lines, System.Collections.Generic.List<CGUtilities.Polygon> polygons, ref System.Collections.Generic.List<CGUtilities.Point> outPoints, ref System.Collections.Generic.List<CGUtilities.Line> outLines, ref System.Collections.Generic.List<CGUtilities.Polygon> outPolygons)
        {
            Polygon polygon = new Polygon(lines);

            // make orintaion of polygon is counter clock wise
            polygon = CounterClockwise(polygon);

            // check type of monotone
            bool check = CheckMonotone(polygon);

            // get points from polygon edges
            for (int i = 0; i < polygon.lines.Count; i++)
            {
                if (!points.Contains(polygon.lines[i].Start))
                {
                    points.Add(polygon.lines[i].Start);
                }

            }

            Line lastLine = polygon.lines[polygon.lines.Count - 1];
            if (!points.Contains(lastLine.End))
            {
                points.Add(lastLine.End);
            }
            ////////////////////////////////////////////////////////////////////////////

            //sort the points on max Y and max X
            points.Sort(sort_point);

            if (check == true)
            {
                Stack<CGUtilities.Point> stack_point = new Stack<CGUtilities.Point>();

                stack_point.Push(points[0]);
                stack_point.Push(points[1]);

                // loop about remaining points 
                for (int p = 2; p < points.Count; p++)
                {
                    Point current = points[p];

                    bool on_edge = false;
                    int count_stack = stack_point.Count;

                    Point previous_top = stack_point.Pop();

                    //case1 ==> the two vertex are on opposite sides
                    if (!CheckSide(current, previous_top, polygon))
                    {
                        Point try_p, top;
                        stack_point.Push(previous_top);
                        while (true)
                        {
                            try_p = stack_point.Pop();
                            if(stack_point.Count != 0)
                            {
                                outLines.Add(new Line(try_p, current));
                            }
                            else
                            {
                                top = previous_top;
                                break;
                            }
                        }
                        stack_point.Push(top);
                        stack_point.Push(current);
                    }
                    //case2 ==> the two vertex are on same sides
                    else
                    {
                        Point last_p = current;   
                        while (true)
                        {
                            Point try_point = stack_point.Pop();   // point check if put in stack or not
                            bool final_case = false;
                            on_edge = PointOnEdge(try_point, current, polygon);   //check if line between try and current points is inside the polygon  

                            if (stack_point.Count == 0)
                            {
                                final_case = true;
                            }
                            stack_point.Push(try_point);
                            if(on_edge == false && (count_stack == 2))
                            {
                                last_p = previous_top;
                            }
                            else
                            {
                                last_p = stack_point.Pop();
                                outLines.Add(new Line(last_p, current));
                            }
                            if (final_case) { break; }
                        }
                        stack_point.Push(last_p);
                        stack_point.Push(current);
                    }
                }
            }
            else
            {
                //that is not monotone
            }
        }

        //Check the orientation of the polygon
        public Polygon CounterClockwise(Polygon polygon)
        {
            double area = 0;
            for (int i = 0; i < polygon.lines.Count; i++)
            {
                double diff_x = polygon.lines[i].End.X - polygon.lines[i].Start.X;
                double diff_y = polygon.lines[i].End.Y + polygon.lines[i].Start.Y;
                area += diff_x * diff_y;
            }
 
            if ((area / 2) > 0) // clockwise
            {
                //convert from ClockWise to CounterClockWisw
                polygon.lines.Reverse();
                for (int i = 0; i < polygon.lines.Count; i++)
                {
                    Point replace = polygon.lines[i].Start;
                    polygon.lines[i].Start = polygon.lines[i].End;
                    polygon.lines[i].End = replace;
                }
            }
            return polygon;
        }

        //Check Monotone: return true if no cusp points
        public bool CheckMonotone(Polygon pol)
        {
            int count = 0;   // number of cusp points

            for (int i = 0; i < pol.lines.Count; i++)
            {

                int previous = ((i - 1) + pol.lines.Count) % pol.lines.Count;
                int next = (i + 1) % pol.lines.Count;

                Point p = pol.lines[i].Start;
                Point prev_p = pol.lines[previous].Start;
                Point next_p = pol.lines[next].Start;

                //the two edges in the same side and the angle > 180
                if (next_p.Y < p.Y && prev_p.Y < p.Y && !IsConvex(pol, i))
                    count++;
                else if (next_p.Y > p.Y && prev_p.Y > p.Y && !IsConvex(pol, i))
                    count++;
            }

            if (count == 0)
                return true;

            return false;
        }

        //Check Convex point: return true if convex
        public bool IsConvex(Polygon polygon, int Current)
        {
            if (polygon.lines == null || polygon.lines.Count < 3)
            {
                return false;
            }

            int previous = (Current - 1 + polygon.lines.Count) % polygon.lines.Count;
            int next = (Current + 1) % polygon.lines.Count;

            // indices are out of range
            if (previous < 0 || previous >= polygon.lines.Count || next < 0 || next >= polygon.lines.Count || Current < 0 || Current >= polygon.lines.Count)
            {
                return false;
            }

            Point s_previous = polygon.lines[previous].Start;
            Point s_current = polygon.lines[Current].Start;
            Point s_next = polygon.lines[next].Start;
            Line l = new Line(s_previous, s_current);

            if (HelperMethods.CheckTurn(l, s_next) == Enums.TurnType.Left)
            {
                return true;
            }

            return false;
        }

        //sort the points on max Y and max X
        public static int sort_point(Point a, Point b)
        {
            // sort descending about x-axis
            if (a.Y == b.Y)
                return -a.X.CompareTo(b.X);
            // sort descending about y-axis
            else
                return -a.Y.CompareTo(b.Y);
        }

        // CheckSide: return true if the two points in the same side
        public bool CheckSide(Point p1, Point p2, Polygon polygon)
        {
            // Iterate through each line in the polygon
            for (int i = 0; i < polygon.lines.Count; i++)
            {
                if ((polygon.lines[i].Start.Equals(p1) && polygon.lines[i].End.Equals(p2)) ||
                    (polygon.lines[i].Start.Equals(p2) && polygon.lines[i].End.Equals(p1)))
                {
                    return true;
                }
            }

            return false;
            /*for (int i = 0; i < polygon.lines.Count; i++)
            {
                if (p1.X < polygon.lines[i].Start.X && p2.X < polygon.lines[i].Start.X)
                    return true;
                else if (p1.X > polygon.lines[i].Start.X && p2.X > polygon.lines[i].Start.X)
                    return true;
                else return false;
            }
            return false;*/
        }

        // return true if point on edge of polygon
        bool PointOnEdge(Point p1, Point p2, Polygon polygon)
        {
            Line segment = new Line(p1, p2);
            bool intersect = false;
            for (int i = 0; i < polygon.lines.Count; i++)
            {
                // get Cross Product 
                double Cross_1 = (segment.End.X - segment.Start.X) * (polygon.lines[i].Start.Y - segment.End.Y) - (segment.End.Y - segment.Start.Y) * (polygon.lines[i].Start.X - segment.End.X);
                double Cross_2 = (segment.End.X - segment.Start.X) * (polygon.lines[i].End.Y - segment.End.Y) - (segment.End.Y - segment.Start.Y) * (polygon.lines[i].End.X - segment.End.X);
                double Cross_3 = (polygon.lines[i].End.X - polygon.lines[i].Start.X) * (segment.Start.Y - polygon.lines[i].End.Y) - (polygon.lines[i].End.Y - polygon.lines[i].Start.Y) * (segment.Start.X - polygon.lines[i].End.X);
                double Cross_4 = (polygon.lines[i].End.X - polygon.lines[i].Start.X) * (segment.End.Y - polygon.lines[i].End.Y) - (polygon.lines[i].End.Y - polygon.lines[i].Start.Y) * (segment.End.X - polygon.lines[i].End.X);

                if ((Cross_1 * Cross_2) < 0f && (Cross_3 * Cross_4) < 0f)
                    intersect = true;

                // collinearity
                if ((Cross_1 == 0f) && (HelperMethods.PointOnSegment(segment.Start, segment.End, polygon.lines[i].Start))) intersect = true;
                if ((Cross_2 == 0f) && (HelperMethods.PointOnSegment(segment.Start, segment.End, polygon.lines[i].End))) intersect = true;
                if ((Cross_3 == 0f) && (HelperMethods.PointOnSegment(polygon.lines[i].Start, polygon.lines[i].End, segment.Start))) intersect = true;
                if ((Cross_4 == 0f) && (HelperMethods.PointOnSegment(polygon.lines[i].Start, polygon.lines[i].End, segment.End))) intersect = true;

                // Skip the segment if it's an edge of the polygon
                if (((polygon.lines[i].Start).Equals(p1) && (polygon.lines[i].End).Equals(p2)) ||
                    ((polygon.lines[i].Start).Equals(p2) && (polygon.lines[i].End).Equals(p1)))
                {
                    continue;
                }
                if (intersect == true) { return false; }

            }

            return true;
        }

        public override string ToString()
        {
            return "Monotone Triangulation";
        }
    }
}