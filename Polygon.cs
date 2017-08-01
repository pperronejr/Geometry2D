
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
    /// <summary>
    /// Polygon has characteristics of an area enclosed by a polyline.
    /// </summary>
    public class Polygon : Polyline
    {
        public Polygon(List<Point> points)
        {
            DefinePolygon( points);
        }

        protected Polygon()
        { }

        public void DefinePolygon(List<Point> points)
        {
            List<Point> closedPoints;
            if (points.Last() == points.First())
                closedPoints = points;
            else
            {
                closedPoints = points.ToList();
                closedPoints.Add(points.First());
            }
            DefinePolyline(closedPoints);
        }

        public override bool Interferes(Line line)
        {
            return
                base.Interferes(line) ||
                ((typeof(LineSegment).IsInstanceOfType(line)) && Encloses((LineSegment)line));
        }
        public override bool Interferes(Polyline pLine)
        {
            return base.Interferes(pLine) || Encloses(pLine);
        }
        public override bool Interferes(Arc arc)
        {
            return base.Interferes(arc) || Encloses(arc);
        }

        /// <summary>
        /// Point must fall within polygon to return true.
        /// If point falls on the surrounding polyline then 
        /// false is returned
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Encloses(Point point)
        {
            Ray ray = new Ray(point, new Vector(1.0, 0.0));
            int numIntersections = Intersections(ray).Count;
            return (numIntersections % 2) != 0;
        }
        /// <summary>
        /// LineSegment must be completely within the polygon to return true.
        /// If the LineSegment is coincident with the surroundign polyline then 
        /// false is returned.
        /// </summary>
        /// <param name="lSeg"></param>
        /// <returns></returns>
        public bool Encloses(LineSegment lSeg)
        {
            List<Point> intersections = Intersections(lSeg);
            intersections.Remove(lSeg.Point1);
            intersections.Remove(lSeg.Point2);
            return (intersections.Count == 0) && Encloses(lSeg.Point1) && Encloses(lSeg.Point2);
        }
        /// <summary>
        /// Polyline can be enclosed as long as all of its line segments are coincident or enclosed.
        /// FUTURE ENHANCEMENT: Have Encloses require that at least some portion of the pLine
        /// is enclosed by this Polygon.  Right now Enclose will return true if the input polyLine is "this".
        /// </summary>
        /// <param name="pLine"></param>
        /// <returns></returns>
        public bool Encloses(Polyline pLine)
        {
            return !pLine.LineSegments.Exists(lSeg => !Encloses(lSeg) && !Overlaps(lSeg));
        }

        /// <summary>
        /// Arc must be completely within the polygon to return true.
        /// </summary>
        /// <param name="arc"></param>
        /// <returns></returns>
        public bool Encloses(Arc arc)
        {
            List<Point> intersections = Intersections(arc);
            intersections.Remove(arc.StartPoint);
            intersections.Remove(arc.EndPoint);
            return (intersections.Count == 0) && Encloses(arc.StartPoint) && Encloses(arc.EndPoint);
        }
    }
}
