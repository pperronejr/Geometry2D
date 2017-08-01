
//
// Copyright 2017 Paul Perrone.  All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace IDA.Geometry2D
{
    public class LineSegment : Line
    {
        /// <summary>
        /// Creates a line segment starting at pt1 to pt2.  If DefineLine is 
        /// subsequently called then the segment will effectively be whatever 
        /// portion of the new line that projects onto the segment.  It is 
        /// recommended to change the LineSegment definition by changing 
        /// properties Point1 and Point2 rather than using DefineLine.
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        public LineSegment(Point pt1, Point pt2) :
            base(pt1, pt2)
        {
            point1 = pt1;
            point2 = pt2;
            Length = Distance(pt1, pt2);
        }
        private Point point1;
        public Point Point1
        {
            get { return point1; }
            set
            {
                point1 = value;
                DefineLine(Point1,Point2);
                Length = Distance(Point1, Point2);
            }
        }
        private Point point2;
        public Point Point2
        {
            get { return point2; }
            set
            {
                point2 = value;
                DefineLine(Point1,Point2);
                Length = Distance(Point1, Point2);
            }
        }

        public override bool ThroughPoint(Point point)
        {
            return
                base.ThroughPoint(point) &&
                BetweenEndPoints(point);
        }
        internal override bool IntersectsPerpendicularLine(Point perpLinePoint)
        {
            return BetweenEndPoints(perpLinePoint);
        }

        /// <summary>
        /// For segments and rays, they are considered coincident if they at least share a common 
        /// vertex as well as the same line definition.  Only a portion of either needs to be 
        /// overlapping.  This is not the correct definition of Coincident so this method should
        /// probably change name.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public override bool Overlaps(Line line)
        {
            if (typeof(LineSegment).IsInstanceOfType(line))
                return overlaps((LineSegment)line);
            else if (typeof(Ray).IsInstanceOfType(line))
                return overlaps((Ray)line);
            else
                return base.Overlaps(line);
        }
        private bool overlaps(LineSegment lSeg)
        {
            return base.Overlaps(lSeg) &&
                (BetweenEndPoints(lSeg.Point1) || BetweenEndPoints(lSeg.Point2));
        }
        private bool overlaps(Ray ray)
        {
            return base.Overlaps(ray) &&
                (ray.OnRaySide(Point1) || ray.OnRaySide(Point2));
        }

        public override double OverlapLength(Line line)
        {
            if (typeof(LineSegment).IsInstanceOfType(line))
                return overlapLength((LineSegment)line);
            else if (typeof(Ray).IsInstanceOfType(line))
                return overlapLength((Ray)line);
            else
                return base.OverlapLength(line);
        }
        private double overlapLength(LineSegment lSeg)
        {
            if (Overlaps(lSeg))
            {
                List<double> dists = new List<double>() 
                {
                    Distance(Point1, lSeg.Point1),
                    Distance(Point1, lSeg.Point2),
                    Distance(Point2, lSeg.Point1),
                    Distance(Point2, lSeg.Point2),
                    Length,
                    lSeg.Length
                };
                double cumDist = dists.Max();
                return Length + lSeg.Length - cumDist;
            }
            else
                return 0.0;
        }
        private double overlapLength(Ray ray)
        {
            if (Overlaps(ray))
                if (ray.OnRaySide(Point1))
                    return Distance(ray.Point, Point1);
                else
                    return Distance(ray.Point, Point2);
            else
                return 0.0;
        }

        // Intersections for segments and rays also include vertices of each if they lie
        // on the other curve or are coincident with one of the other curve's vertices.
        // If adding the vertices was not necessary for segments and rays then the 
        // base method does not need to be overridden.

        public override List<Point> Intersections(Line line)
        {
            if (typeof(LineSegment).IsInstanceOfType(line))
                return intersections((LineSegment)line);
            else if (typeof(Ray).IsInstanceOfType(line))
                return intersections((Ray)line);
            else
                return base.Intersections(line);
        }
        public List<Point> Intersections(Line line, List<Point> excludePoints)
        {
            List<Point> intersections = Intersections(line);
            excludePoints.ForEach(exPt => intersections.Remove(exPt));
            return intersections;
        }
        private List<Point> intersections(LineSegment lSeg)
        {
            List<Point> intersections = new List<Point>();

            if (!Overlaps(lSeg))
                intersections = base.Intersections(lSeg);
            else if (OverlapLength(lSeg) == 0.0)
                // Add shared vertices as intersection points
                if (Distance(Point1, lSeg.Point1) < Distance(Point2, lSeg.Point1))
                    intersections.Add(Point1);
                else
                    intersections.Add(Point2);

            return intersections;
        }
        private List<Point> intersections(Ray ray)
        {
            List<Point> intersections = new List<Point>();

            if (!Overlaps(ray))
                intersections = base.Intersections(ray);
            else if (OverlapLength(ray) == 0.0)
                // Add shared vertices as intersection points
                intersections.Add(ray.Point);

            return intersections;
        }

        // ---------------------------------------

        internal protected bool BetweenEndPoints(Point point)
        {
            return BetweenPoints(point, Point1, Point2);
        }
    }
}
