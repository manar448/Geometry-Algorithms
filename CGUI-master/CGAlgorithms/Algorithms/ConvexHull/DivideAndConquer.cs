using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {
                outPoints = new List<Point>(points);
            }

            // Sort the points by their x-coordinate
            points = points.OrderBy(point => point.X).ThenBy(point => point.Y).ToList();
            outPoints = Divide(points);
        }
        private List<Point> Divide(List<Point> points)
        {
            if (points.Count < 2)
            {
                return points;
            }

            int mid = (points.Count / 2);

            List<Point> left = Divide(points.GetRange(0, mid));
            List<Point> right = Divide(points.GetRange(mid, points.Count - mid));

            return MergeHULL(left, right);
        }

        private List<Point> MergeHULL(List<Point> left, List<Point> right)
        {
            // returns the point with the highest X coordinate ==> (right most point in the left hull ) and,
            // if there are multiple points with the same X coordinate, the one with the highest Y coordinate.
            Point pointOnLeft = left.OrderByDescending(point => point.X).ThenByDescending(point => point.Y).FirstOrDefault();

            //returns the point with the lowest x-axis ==> (left most point in the right hull) and,
            //if there are multiple points with the same X coordinate, the one with the lowest Y coordinate.
            Point pointOnRight = right.OrderBy(point => point.X).ThenBy(point => point.Y).FirstOrDefault();

            // Up Supporting line (upper tangent line)
            Point upOnLeft = pointOnLeft;
            Point upOnRight = pointOnRight;
            bool rightChange, leftChange;

            do
            {
                rightChange = leftChange = false;
                // points[(points.IndexOf(value) + 1) % points.Count]
                //check turn between upper line and next point(left[(left.IndexOf(upOnLeft) + 1) % left.Count])
                while (CGUtilities.HelperMethods.CheckTurn(new Line(upOnRight, upOnLeft), left[(left.IndexOf(upOnLeft) + 1) % left.Count]) == Enums.TurnType.Right)
                {
                    upOnLeft = left[(left.IndexOf(upOnLeft) + 1) % left.Count];
                    leftChange = true;
                }

                if (!leftChange && CGUtilities.HelperMethods.CheckTurn(new Line(upOnRight, upOnLeft), left[(left.IndexOf(upOnLeft) + 1) % left.Count]) == Enums.TurnType.Colinear)
                    upOnLeft = left[(left.IndexOf(upOnLeft) + 1) % left.Count];

                //check turn between upper line and previous point   
                Point previouspoint;
                if(right.IndexOf(upOnRight) == 0)
                {
                    previouspoint = right[right.Count - 1];
                }
                else
                {
                    previouspoint = right[right.IndexOf(upOnRight) - 1];
                }
                while (CGUtilities.HelperMethods.CheckTurn(new Line(upOnLeft, upOnRight), previouspoint) == Enums.TurnType.Left)
                {
                    upOnRight = previouspoint;
                    rightChange = true;
                }

                if (!rightChange && CGUtilities.HelperMethods.CheckTurn(new Line(upOnLeft, upOnRight), previouspoint) == Enums.TurnType.Colinear)
                    upOnRight = previouspoint;

            }
            while (rightChange || leftChange);

            // Down Supporting Line (lower tangent line)
            Point downOnLeft = pointOnLeft;
            Point downOnRight = pointOnRight;
            do
            {
                rightChange = leftChange = false;
                //check turn between lower line and previous point   
                Point previouspoint;
                
                if (left.IndexOf(downOnLeft) == 0)
                {
                    previouspoint = left[left.Count - 1];
                }
                else
                {
                    previouspoint = left[left.IndexOf(downOnLeft) - 1];
                }
                while (CGUtilities.HelperMethods.CheckTurn(new Line(downOnRight, downOnLeft), previouspoint) == Enums.TurnType.Left)
                {
                    downOnLeft = previouspoint;
                    leftChange = true;
                }

                if (!leftChange && CGUtilities.HelperMethods.CheckTurn(new Line(downOnRight, downOnLeft), previouspoint) == Enums.TurnType.Colinear)
                    downOnLeft = previouspoint;

                //check turn between lower line and next point (right[(right.IndexOf(downOnRight) + 1) % right.Count])
                while (CGUtilities.HelperMethods.CheckTurn(new Line(downOnLeft, downOnRight), right[(right.IndexOf(downOnRight) + 1) % right.Count]) == Enums.TurnType.Right)
                {
                    downOnRight = right[(right.IndexOf(downOnRight) + 1) % right.Count];
                    rightChange = true;
                }

                if (!rightChange && CGUtilities.HelperMethods.CheckTurn(new Line(downOnLeft, downOnRight), right[(right.IndexOf(downOnRight) + 1) % right.Count]) == Enums.TurnType.Colinear) 
                    downOnRight = right[(right.IndexOf(downOnRight) + 1) % right.Count];
            }
            while (leftChange || rightChange);


            List<Point> ret = new List<Point>();

            // add points from left hull
            Point up_left = upOnLeft;
            ret.Add(up_left);
            while (!up_left.Equals(downOnLeft))
            {
                // up_left = next point (left[(left.IndexOf(up_left) + 1) % left.Count])
                up_left = left[(left.IndexOf(up_left) + 1) % left.Count];
                ret.Add(up_left);
            }

            // add points from right hull
            Point down_right = downOnRight;
            ret.Add(down_right);
            while (!down_right.Equals(upOnRight))
            {
                // down_right = next point (right[(right.IndexOf(down_right) + 1) % right.Count])
                down_right = right[(right.IndexOf(down_right) + 1) % right.Count];
                ret.Add(down_right);
            }

            return ret;
        }        

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}