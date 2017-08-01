
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
    public class TestGeometry2D
    {
        public static void Main(string[] args)
        {
            // Perpendicular lines
            Line line1 = new Line(new Point(0.0, 4.0), 2.0);
            Line line2 = new Line(new Point(4.0, 4.0), -0.5);
            LineSegment ls2 = new LineSegment(new Point(4.0, 4.0), new Point(5.0, 3.5));
            Ray r2 = new Ray(ls2.Point2, new Vector(1, -.5));

            // Colinear Vertical lines
            Line line3 = new Line(new Point(4.0, 4.0), Double.NegativeInfinity);
            Line line4 = new Line(new Point(4.0, 4.0), new Point(4.0, 8.0));
            LineSegment ls4 = new LineSegment(new Point(4.0, 0.0), new Point(4.0, 1.5));
            LineSegment ls5 = new LineSegment(new Point(4.0, 1.5), new Point(4.0, 3.0));
            Ray r4 = new Ray(ls4.Point2, new Vector(0.0,1.0));
            // Colinear Horizontal lines
            Line line5 = new Line(new Point(1.0, 1.0), 0.0);
            Line line6 = new Line(new Point(1.0, 1.0), new Point(3.0, 1.0));
            Ray r6 = new Ray(new Point(1.0, 1.0), new Vector(0.0, 1.0));
            // Horizontal line
            Line line7 = new Line(new Point(88.0, 3.0), 0.0);
            // From vector
            Line line8 = new Line(new Point(0.0, 4.0), new Vector(-4.0, 2.0));
            Line line9 = new Line(new Point(4.0, 0.0), new Vector(0.0,3.0));
            Line line10 = new Line(new Point(3.4, 3.0), new Vector(3.4, 0.0));

            // Vertical Rays
            Ray r11 = new Ray(new Point(1.0,1.0),new Vector(0.0,1.0));
            Ray r12 = new Ray(new Point(1.0, 1.0),new Vector(0.0, -1.0));

            //Horizontal Rays
            Ray r13 = new Ray(new Point(1.0, 1.0), new Vector(1.0, 0.0));

            Point p1 = new Point(0.0, 0.0);
            Point p2 = new Point(1.0, 0.0);
            Point p3 = new Point(1.0, 1.0);
            Point p4 = new Point(0.0, 1.0);
            Point p5 = new Point(-0.5, 0.5);
            Point p6 = new Point(2.33, 0.5);

            Point outsidePt = new Point(-1.0, -1.0);
            Point insidePt = new Point(0.5, 0.5);

            List<Point> openPts = new List<Point>() { p1, p2, p3, p4 };
            List<Point> closedPts = new List<Point>() { p1, p2, p3, p4, p1};
            List<Point> straightPts = new List<Point>() { p5, p6 };

            Polyline straightPL = new Polyline(straightPts);

            Polyline openPL = new Polyline(openPts);
            Polyline closedPL = new Polyline(closedPts);

            Polygon openPG = new Polygon(openPts);
            Polygon closedPG = new Polygon(closedPts);

            Arc circle1 = new Arc(new Point(1.0, 1.0), 1.0, 0.0, 360.0);
            Arc circle2 = new Arc(new Point(0.0, 0.0), 1.5, -90.0, 90.0);
            Arc circle3 = new Arc(new Point(0.0, 0.0), 1.0, 90.0, 0.0);

            // intersects circle1
            Line line91 = new Line(new Point(2.0, 2.0), 1.0);
            // Tangent to circle 1
            Line line92 = new Line(new Point(2.0, 1.0), Double.PositiveInfinity);
            // does not intersect circle 1
            Line line93 = new Line(new Point(3.0, 1.0), Double.PositiveInfinity);

            // intersect circle1 twice
            LineSegment ls91 = new LineSegment(new Point(2.0, 2.0), new Point(-2.0, -2.0));
            // intersect circle1 once
            LineSegment ls92 = new LineSegment(new Point(2.0, 2.0), new Point(0.7, 0.7));
            // does not intersect circle1
            LineSegment ls93 = new LineSegment(new Point(2.0, 2.0), new Point(4.0, 4.0));
            // intersects circle 1 twice
            Ray r91 = new Ray(new Point(2.0,2.0),new Vector(-1.0,-1.0));
            // does not intersect circle 1
            Ray r92 = new Ray(new Point(2.0, 2.0), new Vector(1.0, 1.0));
            // intersects circle 1 once
            Ray r93 = new Ray(new Point(1.5, 1.5), new Vector(1.0, 1.0));
            


            // circle4 totally separate from circle1
            Arc circle4 = new Arc(new Point(99.0, 99.0), 1.0, 0.0, 270.0);
            // circle5 contained by circle1
            Arc circle5 = new Arc(new Point(1.0,1.0), 0.5, 0.0, 270.0);
            // circle6 inside of circle1 and tangent at (2.0, 1.0)
            Arc circle6 = new Arc(new Point(1.5, 1.0), 0.5, 0.0, 270.0);
            // circle7 intersects circle1
            Arc circle7 = new Arc(new Point(1.75, 1.0), 0.5, 0.0, 360.0);
            // circle8 outside of circle1 and tangent at (2.0, 1.0)
            Arc circle8 = new Arc(new Point(2.5, 1.0), 0.5, 0.0, 270.0);
            // arc9 does not overlap circle8 arc
            Arc arc9 = new Arc(new Point(2.5, 1.0), 0.5, 280.0, 290.0);
            // arc10 does overlaps circle8 arc
            Arc arc10 = new Arc(new Point(2.5, 1.0), 0.5, 280.0, 10.0);
        }
    }
}
