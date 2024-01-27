using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CGUtilities;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    class SweepLine : Algorithm
    {
        private enum EventType
        {
            Start,
            End,
            Intersection
        }

        private struct EventQ
        {
            public Point Point { get; }
            public Line Segment { get; }
            public EventType Type { get; }

            public EventQ(Point point, Line segment, EventType type)
            {
                Point = point;
                Segment = segment;
                Type = type;
            }
        }


        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            List<EventQ> events = new List<EventQ>();
            //TODO: Extract events from lines
            for (int i = 0; i < lines.Count - 1; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    //TODO: Add Event of Line 1
                    Line line1 = lines[i];
                    EventQ event1_Line1 = new EventQ(line1.Start, line1, EventType.Start);
                    EventQ event2_Line1 = new EventQ(line1.End, line1, EventType.End);
                    events.Add(event1_Line1);
                    events.Add(event2_Line1);
                    //TODO: Add Event of Line 2
                    Line line2 = lines[j];
                    EventQ event1_Line2 = new EventQ(line2.Start, line2, EventType.Start);
                    EventQ event2_Line2 = new EventQ(line2.End, line2, EventType.End);
                    events.Add(event1_Line2);
                    events.Add(event2_Line2);
                }
            }
            //TODO: Apply SweepLine Algorithms
            outPoints = sweepLineFind(events, outLines);
        }

        private List<Point> sweepLineFind(List<EventQ> events, List<Line> outLines)
        {
            HashSet<Line> checkedlst = new HashSet<Line>();
            List<Point> intersectionslst = new List<Point>();
            List<Line> lQueue = new List<Line>();

            //TODO: Sort the events by their x-coordinate
            events = events.OrderBy(e => e.Point.X).ThenBy(e => e.Point.Y).ToList();
            foreach (EventQ ev in events)
            {
                if (ev.Type == EventType.Start)
                {
                    lQueue.Add(ev.Segment);
                    findOverlapping(ev.Segment, checkedlst, lQueue, ev.Point, intersectionslst, outLines);
                }
                else if (ev.Type == EventType.End)
                {
                    findOverlapping(ev.Segment, checkedlst, lQueue, ev.Point, intersectionslst, outLines);
                    lQueue.Remove(ev.Segment);
                }
                else if (ev.Type == EventType.Intersection)
                {
                    handleIntersection(ev, lQueue, ref intersectionslst, ref outLines);
                }

                // Add intersection events between the current segment and other active segments
                foreach (Line otherSegment in lQueue)
                {
                    if (ev.Segment != otherSegment) // Avoid self-intersection events
                    {
                        List<Point> segmentIntersections = FindIntersectionPoints(ev.Segment, otherSegment);
                        intersectionslst.AddRange(segmentIntersections);
                        outLines.AddRange(segmentIntersections.Select(point => ev.Segment));
                        outLines.AddRange(segmentIntersections.Select(point => otherSegment));
                    }
                }
            }
            return intersectionslst;
        }
        //TODO: check segment with currSegments
        private void findOverlapping(Line currentSegment, HashSet<Line> checkedSegments, List<Line> currSegments, Point currentPoint, List<Point> intersections, List<Line> outLines)
        {
            foreach (var segment in currSegments.ToList())  //create a copy for safe removal
            {
                bool isOverlap = (currentSegment == null || segment == null) ? false : (currentSegment.Start.Y <= segment.Start.Y && segment.Start.Y <= currentSegment.End.Y) || (segment.Start.Y <= currentSegment.Start.Y && currentSegment.Start.Y <= segment.End.Y);
                if (segment != null && !checkedSegments.Contains(segment) && isOverlap)
                {
                    currSegments.Remove(segment);
                    handleOverlapping(currentSegment, segment, intersections, outLines);
                    checkedSegments.Add(segment);
                }
            }
        }
        //TODO: check line with before & after lines
        private void handleOverlapping(Line seg1, Line seg2, List<Point> intersections, List<Line> outLines)
        {
            // Continue with the intersection logic if the segments are not parallel
            List<Point> intersectionPoints = FindIntersectionPoints(seg1, seg2);
            intersections.AddRange(intersectionPoints);

            // Add all segments involved in the intersection to outLines
            outLines.AddRange(intersectionPoints.Select(point => seg1));
            outLines.AddRange(intersectionPoints.Select(point => seg2));
        }
        //TODO:  Calculate Intersection Points
        private List<Point> FindIntersectionPoints(Line l1, Line l2)
        {
            List<Point> lst = new List<Point>();

            double fact = ((l2.End.X - l2.Start.X) * (l1.Start.Y - l2.Start.Y) - (l2.End.Y - l2.Start.Y) * (l1.Start.X - l2.Start.X))
                / ((l2.End.Y - l2.Start.Y) * (l1.End.X - l1.Start.X) - (l2.End.X - l2.Start.X) * (l1.End.Y - l1.Start.Y));

            double x = l1.Start.X + fact * (l1.End.X - l1.Start.X);
            double y = l1.Start.Y + fact * (l1.End.Y - l1.Start.Y);

            // Check if the intersection point lies on both line segments
            if (isOnSegment(new Point(x, y), l1) && isOnSegment(new Point(x, y), l2))
            {
                lst.Add(new Point(x, y));
            }
            return lst;
        }
        //TODO: sweep lines then check line with before & after lines
        private void handleIntersection(EventQ ev, List<Line> currSegments, ref List<Point> intersections, ref List<Line> outLines)
        {
            Line aboveSegm = null, belowSegm = null;
            foreach (var segment in currSegments)
            {
                if (segment.Start.Y <= ev.Point.Y && segment.End.Y <= ev.Point.Y)
                {
                    belowSegm = segment;
                }
                else if (segment.Start.Y >= ev.Point.Y && segment.End.Y >= ev.Point.Y)
                {
                    aboveSegm = segment;
                }
                else
                {
                    // Check if the current segment intersects with the event segment
                    if ((segment == null || ev.Segment == null) ? false : (segment.Start.Y <= ev.Segment.Start.Y && ev.Segment.Start.Y <= segment.End.Y) || (ev.Segment.Start.Y <= segment.Start.Y && segment.Start.Y <= ev.Segment.End.Y))
                    {
                        List<CGUtilities.Point> segmentIntersections = FindIntersectionPoints(segment, ev.Segment);
                        intersections.AddRange(segmentIntersections);

                        // Add all segments involved in the intersection to outLines
                        outLines.AddRange(segmentIntersections.Select(point => segment));
                        outLines.AddRange(segmentIntersections.Select(point => ev.Segment));
                    }
                }
            }

            if (aboveSegm != null && belowSegm != null && aboveSegm.Start.Y > belowSegm.Start.Y)
            {
                var temp = aboveSegm;
                aboveSegm = belowSegm;
                belowSegm = temp;
            }

            if (aboveSegm != null)
            {
                outLines.Add(aboveSegm);
                outLines.Add(ev.Segment);
            }

            if (belowSegm != null)
            {
                outLines.Add(belowSegm);
                outLines.Add(ev.Segment);
            }

            currSegments.Remove(ev.Segment);
        }
        //TODO: check if point is subpoint of segment
        private bool isOnSegment(Point point, Line l)
        {
            bool res = (point.X >= Math.Min(l.Start.X, l.End.X))
                && (point.X <= Math.Max(l.Start.X, l.End.X))
                && (point.Y >= Math.Min(l.Start.Y, l.End.Y))
                && (point.Y <= Math.Max(l.Start.Y, l.End.Y));
            return res;
        }

        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}