
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
    public class Ray : Line
    {
        /// <summary>
        /// Creates a Ray starting at point pt in the dir specified. If DefineLine is 
        /// subsequently called then the ray will effectively be the portion of the 
        /// new line on the dir side of a line thru pt1 and perpendicular to dir. It is
        /// recommended to change the Ray definition by changing properties Point and 
        /// Direction rather than using DefineLine.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="dir"></param>
        public Ray(Point pt, Vector dir) :
            base(pt, dir)
        {
            point = pt;
            direction = dir;
        }
        private Point point;
        public Point Point
        {
            get { return point; }
            set
            {
                point = value;
                DefineLine(Point,Direction);
            }
        }
        private Vector direction;
        public new Vector Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                DefineLine(Point, Direction);
            }
        }

        public override bool ThroughPoint(Point point)
        {
            return
                base.ThroughPoint(point) &&
                OnRaySide(point);
        }
        internal override bool IntersectsPerpendicularLine(Point perpLinePoint)
        {
            return OnRaySide(perpLinePoint);
        }

        /// <summary>
        /// For segments and rays, they are considered coincident if they at least share a common 
        /// vertex as well as the same line definition.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public override bool Overlaps(Line line)
        {
            if (typeof(LineSegment).IsInstanceOfType(line))
                return line.Overlaps(this);
            else if (typeof(Ray).IsInstanceOfType(line))
                return overlaps((Ray)line);
            else
                return base.Overlaps(line);
        }
        private bool overlaps(Ray ray)
        {
            return base.Overlaps(ray) &&
                (SameDirection(Direction, ray.Direction) || OnRaySide(ray.Point));
        }

        public override double OverlapLength(Line line)
        {
            if (typeof(LineSegment).IsInstanceOfType(line))
                return line.OverlapLength(this);
            else if (typeof(Ray).IsInstanceOfType(line))
                return overlapLength((Ray)line);
            else
                return base.OverlapLength(line);
        }
        private double overlapLength(Ray ray)
        {
            if (Overlaps(ray) && SameDirection(Direction, ray.Direction))
                return Double.PositiveInfinity;
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
                return line.Intersections(this);
            else if (typeof(Ray).IsInstanceOfType(line))
                return intersections((Ray)line);
            else
                return base.Intersections(line);
        }
        private List<Point> intersections(Ray ray)
        {
            List<Point> intersections = new List<Point>();

            if (!Overlaps(ray))
                intersections = base.Intersections(ray);
            else if (OverlapLength(ray) == 0.0)
                // Add shared vertices as intersection points
                intersections.Add(Point);

            return intersections;
        }

        // ------------------------------------------

        internal protected bool OnRaySide(Point point)
        {
            return Onside(point, Point, Direction);
        }
    }
}
