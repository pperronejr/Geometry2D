
//
// Copyright 2017 Paul Perrone.  All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace IDA.Geometry2D
{
    /// <summary>
    /// Polyline is a linked set of line segments.  It is treated as a curve
    /// even if it is closed.  If an area like behavior is necessary then
    /// Polygon is more appropriate.
    /// </summary>
    public class Polyline : IPolyline
    {
        public Polyline(List<Point> points) : 
            this()
        {
            DefinePolyline(points);
        }
        protected Polyline()
        {
            LineSegments = new List<LineSegment> { };
        }

        public void DefinePolyline(List<Point> points)
        {
            LineSegments.Clear();           
            for (int i = 0; i < (points.Count - 1); i++)
            {
                LineSegments.Add(new LineSegment(points[i], points[i + 1]));
            }
            Points = points;
            IsClosed = (points[0] == points.Last());
        }

        [XmlIgnore()]
        public List<Point> Points
        {
            get;
            private set;
        }

        [XmlIgnore()]
        public bool IsClosed
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns an Extent Struct
        /// </summary>
        /// <param name="angle">Transform angle to return extents for an alternate coordinate system</param>
        /// <returns></returns>
        public Extents GetExtents(double angle)
        {
            return new Extents( Points, angle);
        }

        public double GetOutsideDistance(double angle)
        {
            return GetExtents(angle).XDistance;
        }

        protected internal List<LineSegment> LineSegments;

        public bool ThroughPoint(Point point)
        {
            return LineSegments.Exists(lSeg => lSeg.ThroughPoint(point));
        }

        public bool Overlaps(Polyline pLine)
        {
            return LineSegments.Exists(lSeg => pLine.Overlaps(lSeg));
        }
        /// <summary>
        /// Does not account for a coincident line that spans multiple subLineSegments
        /// of this profile where the multiple line segments effectively form a straight line
        /// FUTURE ENHANCEMENT: Consider if a line overlaps a profile by considering subsegments
        /// that are contiguous forming a straight line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool Overlaps(Line line)
        {
            return LineSegments.Exists(lSeg => line.Overlaps(lSeg));
        }

        public List<Point> Intersections(Polyline pLine)
        {
            List<Point> intersections = new List<Point>();
            if (!Overlaps(pLine))
                LineSegments.ForEach(lSeg => intersections.AddRange(pLine.Intersections(lSeg)));

            return intersections;
        }
        public List<Point> Intersections(Line line)
        {
            List<Point> intersections = new List<Point>();
            if (!Overlaps(line))
                LineSegments.ForEach(lSeg => intersections.AddRange(line.Intersections(lSeg)));

            return intersections;
        }
        public List<Point> Intersections(Arc arc)
        {
            List<Point> intersections = new List<Point>();
            LineSegments.ForEach(lSeg => intersections.AddRange(arc.Intersections(lSeg)));

            return intersections;
        }
        public List<Point> Intersections(IGeometry iGeom)
        {
            if (typeof(Line).IsInstanceOfType(iGeom))
                return Intersections((Line)iGeom);
            else if (typeof(Polyline).IsInstanceOfType(iGeom))
                return Intersections((Polyline)iGeom);
            else if (typeof(Arc).IsInstanceOfType(iGeom))
                return Intersections((Arc)iGeom);
            else
                throw new System.ApplicationException("Argument to Polyline.Intersections is not a valid type");
        }

        public virtual bool Interferes(Polyline pLine)
        {
            return LineSegments.Exists(lSeg => pLine.Interferes(lSeg));
        }
        public virtual bool Interferes(Line line)
        {
            return LineSegments.Exists(lSeg => line.Interferes(lSeg));
        }
        public virtual bool Interferes(Arc arc)
        {
            return LineSegments.Exists(lSeg => arc.Interferes(lSeg));
        }
        public bool Interferes(IGeometry iGeom)
        {
            if (typeof(Line).IsInstanceOfType(iGeom))
                return Interferes((Line)iGeom);
            else if (typeof(Polyline).IsInstanceOfType(iGeom))
                return Interferes((Polyline)iGeom);
            else if (typeof(Arc).IsInstanceOfType(iGeom))
                return Interferes((Arc)iGeom);
            else
                throw new System.ApplicationException("Argument to Polyline.Interferes is not a valid type");
        }
    }
}
