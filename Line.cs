
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
    public class Line : ILine
    {
        public Line(Point point, double slope) 
            : this()
        {
            DefineLine(point, slope);
        }
        public Line(Point point1, Point point2)
            : this()
        {
            DefineLine(point1, point2);
        }
        public Line(Point point, Vector direction)
            : this()
        {
            DefineLine(point, direction);
        }
        private Line()
        {
            Length = Double.PositiveInfinity;
        }

        /// <summary>
        /// Redefine line definition without having to recreate a new object
        /// </summary>
        /// <param name="point"></param>
        /// <param name="slope"></param>
        public void DefineLine(Point point, double slope)
        {
            Slope = slope;
            IsHorizontal = (slope == 0.0);
            IsVertical = Double.IsInfinity(slope);
            XConstant = IsVertical ? point.X : Double.NaN;
            YIntercept = IsVertical ? Double.NaN : point.Y - slope * point.X;
        }
        /// <summary>
        /// Redefine line definition without having to recreate a new object
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        public void DefineLine(Point point1, Point point2)
        {
            double slope = (point2.Y - point1.Y) / (point2.X - point1.X);
            DefineLine(point1, slope);
        }
        /// <summary>
        /// Redefine line definition without having to recreate a new object
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        public void DefineLine(Point point, Vector direction)
        {
            DefineLine(point, direction.Y / direction.X);
        }

        /// <summary>
        /// Slope of a line.  A value of NegativeInfinity or PositiveInfinity 
        /// represents a vertical line.
        /// </summary>
        public double Slope
        {
            get
            {
                return slope;
            }
            private set
            {
                slope = value;
                if (Double.IsInfinity(slope))
                    Direction = new Vector(0.0, 1.0);
                else
                    Direction = new Vector(1.0, slope);
                Direction.Normalize();
            }
        }
        private double slope;

        /// <summary>
        /// Unitized direction vector for this line.
        /// </summary>
        public Vector Direction
        {
            get;
            private set;
        }
        /// <summary>
        /// Y Intercept of a line.  A value of NaN represents a vertical line.
        /// </summary>
        public double YIntercept
        {
            get;
            private set;
        }

        public double Length
        {
            get;
            protected set;
        }

        public bool IsHorizontal
        { 
            get; 
            private set;
        }
        public bool IsVertical
        { 
            get; 
            private set;
        }

        /// <summary>
        /// For a vertical line (m = NegativeInfinity or PositiveInfinity) then
        /// X is constant.  This private field does not apply for other than a 
        /// vertical line so it defaults to NaN in other cases.
        /// </summary>
        public double XConstant = Double.NaN;

        /// <summary>
        /// If a vertical line then the value is Nan.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Y(double x)
        {
            return (Slope * x + YIntercept);
        }

        public double X(double y)
        {
            if (IsHorizontal)
                return Double.NaN;
            else if (IsVertical)
                return XConstant;
            else 
                return ((y - YIntercept) / Slope);
        }

        public bool IsParallel(Line line)
        {
            return
                (Slope == line.Slope) ||
                // In the case of a vertical line, do special check since slope can
                // be PositiveInfinity or NegativeInfinity;
                (IsVertical && line.IsVertical);
        }

        public bool IsPerpendicular(Line line)
        {
            return
              (Slope * line.Slope == -1.0) ||
              (IsHorizontal && line.IsVertical) ||
              (IsVertical && line.IsHorizontal);
        }

        public virtual bool ThroughPoint(Point point)
        {
            return
                point.Y == Y(point.X) ||
                IsHorizontal && point.Y == YIntercept ||
                IsVertical && point.X == XConstant;
        }

        /// <summary>
        /// This method was written to solve the numerical accuracy issues in the final check
        /// of the Intersections method when using ThroughPoint.
        /// This method should return true if a perpendicular line running through the input 
        /// point perpLinePoint intersects this line.
        /// </summary>
        /// <param name="perpLinePoint"></param>
        /// <returns></returns>
        internal virtual bool IntersectsPerpendicularLine(Point perpLinePoint)
        {
            return true;
        }

        public virtual bool Overlaps(Line line)
        {
            return
                IsParallel(line) &&
                ((YIntercept == line.YIntercept) || (IsVertical && (XConstant == line.XConstant)));
        }

        public virtual double OverlapLength(Line line)
        {
            // Use Math.Min to account for the case of a LineSegment since it has a length
            // less than PositiveInfinity.  Chose this instead of conditionalizing based
            // on line's Type to reduce code.
            return Overlaps(line) ? Math.Min(Length,line.Length) : 0.0;
        }

        /// <summary>
        /// Returns a list containing an intersection point.  If lines are parallel 
        /// then an empty list is returned.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual List<Point> Intersections(Line line)
        {
            List<Point> intersections = new List<Point>();
            if (!IsParallel(line))
            {
                double xCoord;
                double yCoord;
                if (IsVertical)
                {
                    xCoord = XConstant;
                    yCoord = line.Y(xCoord);
                }
                else if (line.IsVertical)
                {
                    xCoord = line.XConstant;
                    yCoord = Y(xCoord);
                }
                else
                {
                    xCoord = (line.YIntercept - YIntercept) / (Slope - line.Slope);
                    yCoord = Y(xCoord);
                }
                intersections.Add(new Point(xCoord, yCoord));
            }
            // In case this or line is a lineSegment or Ray, make sure this and line contains the intersections.
            // If this method is not returning the appropriate intersections it may be due to the 
            // round off in point calculation.  In this case, consider using LineSegment.BetweenEndPoints,
            // Ray.OnRaySide instead of ContainsPoint, and no filter for Line.  This could be conditionalized 
            // based on the Type of Line and This to use the correct filter in this case.  Chose this instead of 
            // conditionalizing based on line's Type to reduce code.

            // return intersections.FindAll(pt => ThroughPoint(pt) && line.ThroughPoint(pt));

            // IntersectsPerpendicularLine implements the approach used to resolve the round off problems
            // when using ThroughPoint as described above.

            return intersections.FindAll(pt => IntersectsPerpendicularLine(pt) && line.IntersectsPerpendicularLine(pt));
        }
        public List<Point> Intersections(Polyline pLine)
        {
            return pLine.Intersections(this);
        }
        public List<Point> Intersections(Arc arc)
        {
            return arc.Intersections(this);
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
                throw new System.ApplicationException("Argument to Line.Intersections is not a valid type");
        }

        public bool IsCoincident(Polyline pLine)
        {
            return pLine.Overlaps(this);
        }

        public virtual bool Interferes(Line line)
        {
            return (Intersections(line).Count > 0) || Overlaps(line);
        }
        public virtual bool Interferes(Polyline pLine)
        {
            return pLine.Interferes(this);
        }
        public virtual bool Interferes(Arc arc)
        {
            return arc.Interferes(this);
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
                throw new System.ApplicationException("Argument to Line.Interferes is not a valid type");
        }

        // --------------------------------

        /// <summary>
        /// This method loosely determines if point lies between point1 and point2.
        /// It does not check to see if they are collinear but just makes sure the 
        /// angles of the vectors from point to point1 and point2 are no greater than
        /// 90 deg with the line formed between point1 and point2.
        /// Points point1 and point2 are considered to be between.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        static public bool BetweenPoints(Point point, Point point1, Point point2)
        {
            // Make sure pt falls between end points point1 and point2 by checking 
            // scalar products of vectors starting at each end point
            return
                Onside(point, point1, point2 - point1) &&
                Onside(point, point2, point1 - point2);
        }
        /// <summary>
        /// This method determines if point falls on the direction side of point0.
        /// Falling on point0 is considered on the same side.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="point0"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        static public bool Onside(Point point, Point point0, Vector direction)
        {
            // Check the scalar product
            return (direction * (point - point0)) >= 0.0;
        }
        static public bool SameDirection(Vector vector1, Vector vector2)
        {
            return Vector.AngleBetween(vector1, vector2) == 0.0;
        }
        static public double Distance(Point point1, Point point2)
        {
            return (point2 - point1).Length;
        }
    }
}
