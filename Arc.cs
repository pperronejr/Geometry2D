
//
// Copyright 2017 Paul Perrone.  All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using IDA.Numeric;

namespace IDA.Geometry2D
{
    public class Arc : IArc
    {
        public Arc(Point center, double radius, double startAngle, double endAngle)
        {
            DefineArc(center, radius, startAngle, endAngle);
        }

        /// <summary>
        /// Redefine Arc definition without having to recreate a new object
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        public void DefineArc(Point center, double radius, double startAngle, double endAngle)
        {
            Center = center;
            Radius = radius;
            EndAngle = endAngle;
            StartAngle = startAngle;
            StartPoint = GetPointAtAngle(startAngle);
            EndPoint = GetPointAtAngle(endAngle);
        }

        public Point Center
        {
            get;
            private set;
        }

        public double Radius
        {
            get;
            private set;
        }

        /// <summary>
        /// 0.0 LE StartAngle LT 360.0
        /// </summary>
        public double StartAngle
        {
            get
            {
                return startAngle;
            }
            private set
            {
                startAngle = getNormalizedAngle(value, true);
                Angle = (startAngle <= EndAngle) ? (EndAngle - startAngle) : (EndAngle - startAngle + 360.0);
            }
        }
        private double startAngle;

        /// <summary>
        /// 0.0 LT EndAngle LE 360.0
        /// </summary>
        public double EndAngle
        {
            get
            {
                return endAngle;
            }
            private set
            {
                endAngle = getNormalizedAngle(value, false);
            }
        }
        private double endAngle;

        /// <summary>
        /// 0.0 LE StartAngle LT 360.0
        /// </summary>
        public double MidAngle
        {
            get
            {
                return getNormalizedAngle( (StartAngle + Angle/2.0), true);
            }
        }

        /// <summary>
        /// Total angle of this arc
        /// </summary>
        public double Angle
        {
            get;
            private set;
        }

        /// <summary>
        /// Point on arc at StartAngle
        /// </summary>
        public Point StartPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Point on arc at EndAngle
        /// </summary>
        public Point EndPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Point on arc at MidAngle
        /// </summary>
        public Point MidPoint
        {
            get
            {
                return GetPointAtAngle(MidAngle);
            }
        }

        /// <summary>
        /// Returns X coordinates given a Y coordinate for the underlying circle independent of 
        /// start and end angles.
        /// </summary>
        /// <param name="yCoord"></param>
        /// <returns></returns>
        public List<double> GetXCoordinates(double yCoord)
        {
            double a = 1.0;
            double b = -2.0 * Center.X;
            double c = Math.Pow(Center.X, 2.0) + Math.Pow(yCoord - Center.Y, 2.0) - Math.Pow(Radius, 2.0);

            return NumericalMethods.GetQuadraticRoots(a, b, c);
        }

        /// <summary>
        /// Returns Y coordinates given a X coordinate for the underlying circle independent of 
        /// start and end angles.
        /// </summary>
        /// <param name="xCoord"></param>
        /// <returns></returns>
        public List<double> GetYCoordinates(double xCoord)
        {
            double a = 1.0;
            double b = -2.0 * Center.Y;
            double c = Math.Pow(Center.Y, 2.0) + Math.Pow(xCoord - Center.X, 2.0) - Math.Pow(Radius, 2.0);

            return NumericalMethods.GetQuadraticRoots(a, b, c);
        }

        public virtual bool Interferes(Arc arc)
        {
            return (Intersections(arc).Count > 0) || Overlaps(arc);
        }
        public virtual bool Interferes(Line line)
        {
            return (Intersections(line).Count > 0);
        }
        public virtual bool Interferes(Polyline pLine)
        {
            return pLine.Interferes(this);
        }
        public bool Interferes(IGeometry iGeom)
        {
            if (typeof(Arc).IsInstanceOfType(iGeom))
                return Interferes((Arc)iGeom);
            else if (typeof(Line).IsInstanceOfType(iGeom))
                return Interferes((Line)iGeom);
            else if (typeof(Polyline).IsInstanceOfType(iGeom))
                return Interferes((Polyline)iGeom);
            else 
                throw new System.ApplicationException("Argument to Arc.Interferes is not a valid type");
        }

        public List<Point> Intersections(IGeometry iGeom)
        {
            if (typeof(Arc).IsInstanceOfType(iGeom))
                return Intersections((Arc)iGeom);
            else if (typeof(Line).IsInstanceOfType(iGeom))
                return Intersections((Line)iGeom);
            else if (typeof(Polyline).IsInstanceOfType(iGeom))
                return Intersections((Polyline)iGeom);
            else
                throw new System.ApplicationException("Argument to Arc.Intersections is not a valid type");
        }

        /// <summary>
        /// Returns intersection points for 2 arcs.  
        /// Reference: http://paulbourke.net/geometry/2circle/
        /// </summary>
        /// <param name="arc"></param>
        /// <returns></returns>
        public List<Point> Intersections(Arc arc)
        {
            List<Point> intersections = new List<Point>();

            Point p0 = Center;
            Point p1 = arc.Center;
            Vector centersVector = (p1 - p0);
            double d = centersVector.Length;

            if (!IsSeparateCircles(arc) && !IsContainedCircles(arc) && !IsEqualCircle(arc))
            {
                double r0 = Radius;
                double r1 = arc.Radius;
                double a = (Math.Pow(r0, 2.0) - Math.Pow(r1, 2.0) + Math.Pow(d, 2.0)) / (2.0 * d);
                double h = Math.Sqrt(Math.Pow(r0, 2.0) - Math.Pow(a, 2.0));
                Point p2 = p0 + (a/d)*centersVector;
               
                if (h == 0.0)
                    // Circle are tangent so just 1 intersection point
                    intersections.Add(p2);
                else
                {
                    double xDelta = (h / d) * (p1.Y - p0.Y);
                    double yDelta = -(h / d) * (p1.X - p0.X);
                    intersections.Add(new Point(p2.X + xDelta, p2.Y + yDelta));
                    intersections.Add(new Point(p2.X - xDelta, p2.Y - yDelta));
                }
            }
            return intersections.FindAll(pt => IncludesAngleAtPoint(pt) && arc.IncludesAngleAtPoint(pt));
        }

        public List<Point> Intersections(Line line)
        {
            List<Point> intersections = new List<Point>();

            if (Double.IsInfinity(line.Slope))
            {
                List<double> yCoords = GetYCoordinates(line.XConstant);
                yCoords.ForEach(y => intersections.Add(new Point(line.XConstant, y)));
            }
            else
            {
                double a = Math.Pow(line.Slope, 2.0) + 1.0;
                double b = 2.0 * ((line.Slope * line.YIntercept) - (line.Slope * Center.Y) - Center.X);
                double c = (Math.Pow(line.YIntercept - Center.Y, 2.0) + Math.Pow(Center.X, 2.0) - Math.Pow(Radius, 2.0));
                List<double> xCoords = NumericalMethods.GetQuadraticRoots(a, b, c);
                xCoords.ForEach(x => intersections.Add(new Point(x, line.Y(x))));
            }

            return intersections.FindAll(pt => IncludesAngleAtPoint(pt) && line.IntersectsPerpendicularLine(pt));
        }

        public List<Point> Intersections(Polyline pLine)
        {
            return pLine.Intersections(this);
        }

        public bool ThroughPoint(Point point)
        {
            if (IsPointOnCircle( point))
            {
                double angleAtPoint = GetAngleAtPoint( point);
                return IncludesAngle(angleAtPoint);
            }
            else
                return false;
        }

        public bool Overlaps(Arc arc)
        {
            return
                IsEqualCircle(arc) &&
                (
                // This covers if the input arc partially overlaps this arc.
                IncludesAngle(arc.StartAngle) || IncludesAngle(arc.EndAngle) ||
                // This covers if this entire arc overlaps the input arc which has a larger total angle
                arc.IncludesAngle(StartAngle) || arc.IncludesAngle(EndAngle));
        }

        public double GetOverlapAngle(Arc arc)
        {
            if (IsEqualCircle(arc))
            {
                if (IncludesAngle(arc.StartAngle) && IncludesAngle(arc.EndAngle))
                    return arc.Angle;
                else if (arc.IncludesAngle(StartAngle) && arc.IncludesAngle(EndAngle))
                    return Angle;
                else if (IncludesAngle(arc.StartAngle))
                    return GetAngleDifference(arc.StartAngle, EndAngle);
                else if (IncludesAngle(arc.EndAngle))
                    return GetAngleDifference(StartAngle, arc.EndAngle);
                else
                    return 0.0;
            }
            else
                return 0.0;
        }

        /// <summary>
        /// Returns true if the underlying circle or the input arc's circle is contained by the other.  That is
        /// one is contained completely by the other and they do not intersect according to the full circle definitions
        /// independent of start and end angles.
        /// </summary>
        /// <param name="arc"></param>
        /// <returns></returns>
        public bool IsContainedCircles(Arc arc)
        {
            double centersDist = (arc.Center - Center).Length;

            return (centersDist < Math.Abs(Radius - arc.Radius));
        }

        /// <summary>
        /// Returns true if the underlying circle is completely separate from the input arc's circle.  That is
        /// one does not contain the other and they do not intersect according to the full circle definitions
        /// independent of start and end angles.
        /// </summary>
        /// <param name="arc"></param>
        /// <returns></returns>
        public bool IsSeparateCircles(Arc arc)
        {
            double centersDist = (arc.Center - Center).Length;

            return (centersDist > (Radius + arc.Radius));
        }

        /// <summary>
        /// Return true if the underlying circle definition is equal to the input arc's circle definition.
        /// This is checking that the arcs are equal independent of start and end angles
        /// </summary>
        /// <param name="arc"></param>
        /// <returns></returns>
        public bool IsEqualCircle(Arc arc)
        {
            return (arc.Center == Center) && (arc.Radius == Radius);
        }

        /// <summary>
        /// Return point at a specific angle on the circle defined by this arc's Center and Radius.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Point GetPointAtAngle(double angleDegrees)
        {
            return GetPointAtAngle(angleDegrees, Center, Radius);
        }

        /// <summary>
        /// Return point at a specific angle on the circle defined by Center and Radius.
        /// </summary>
        /// <param name="angleDegrees"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Point GetPointAtAngle(double angleDegrees, Point center, double radius)
        {
            double angleRadians = Math.PI * angleDegrees / 180.0;
            double xCoord = center.X + radius * Math.Cos(angleRadians);
            double yCoord = center.Y + radius * Math.Sin(angleRadians);

            return new Point(xCoord, yCoord);
        }

        /// <summary>
        /// Find angle or point from X axis using this arc's Center as the origin.
        /// The argument point does not need to be on the circle.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double GetAngleAtPoint(Point point)
        {
            return GetAngleAtPoint(point, Center);
        }

        /// <summary>
        /// Return angle at point given circle center.  Assumes positive x axis is the zero angle and 
        /// angle is measured counter clockwise from the x axis.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static double GetAngleAtPoint(Point point, Point center)
        {
            double deltaX = point.X - center.X;
            double deltaY = point.Y - center.Y;
            double angleRadians = Math.Atan2(deltaY, deltaX);
            angleRadians = (angleRadians < 0.0) ? (2.0 * Math.PI + angleRadians) : angleRadians;

            return (360.0 * (angleRadians / (2.0 * Math.PI)));
        }

        /// <summary>
        /// Determines if a point is on the circle independent of the Start and End angles of this arc.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointOnCircle(Point point)
        {
            // Note: May need to introduce an equal tolerance for this
            return ((point - Center).Length == Radius);
        }

        /// <summary>
        /// Returns true if angle is between this arc's start angle and end angle.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public bool IncludesAngle(double angle)
        {
            return IncludesAngle(angle, StartAngle, EndAngle);
        }

        /// <summary>
        /// Returns true if angle is between this arc's start angle and end angle.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <returns></returns>
        public static bool IncludesAngle(double angle, double startAngle, double endAngle)
        {
            double normalizedAngle = GetNormalizedAngle(angle);

            // Note: May need to introduce an equal tolerance for this

            return
                // StartAngle is less than the EndAngle
                ((startAngle <= endAngle) && (normalizedAngle >= startAngle) && (normalizedAngle <= endAngle)) ||
                // StartAngle is greater than than the EndAngle so the Arc includes the 0.0 angle.
                ((startAngle > endAngle) && ((normalizedAngle >= startAngle) || (normalizedAngle <= endAngle))) ||
                // EndAngle is at 360.0 and StartAngle is greater than 0.0
                ((endAngle == 360.0) && (normalizedAngle == 0.0));
        }

        public bool IncludesAngleAtPoint(Point point)
        {
            return IncludesAngleAtPoint(point, Center, StartAngle, EndAngle);
        }

        public static bool IncludesAngleAtPoint(Point point, Point center, double startAngle, double endAngle)
        {
            return IncludesAngle(GetAngleAtPoint(point, center), startAngle, endAngle);
        }

        public Vector GetRadialUnitVector(Point point)
        {
            return GetRadialUnitVector(point, Center);
        }

        public Vector GetRadialVector(Point point)
        {
            return GetRadialVector(point, Center);
        }

        public double GetRadialDistance(Point point)
        {
            return GetRadialVector(point).Length;
        }

        /// <summary>
        /// Returns a positive angle in degrees between 0.0 and 359.9999...  If the input
        /// angle is non zero or an even multiple of 360.0 then 0.0 is returned.
        /// </summary>
        /// <param name="angleDegrees"></param>
        /// <returns></returns>
        public static double GetNormalizedAngle(double angleDegrees)
        {
            return getNormalizedAngle( angleDegrees, true);
        }
        /// <summary>
        /// Returns a positive angle in degrees between 0.0 and 360.0.  An angle of 0.0 or an even
        /// multiple of 360.0 will be normalized to 0.0 or 360.0 depending on the input useZeroAngle.
        /// </summary>
        /// <param name="angleDegrees"></param>
        /// <returns></returns>
        private static double getNormalizedAngle(double angleDegrees, bool useZeroAngle)
        {
            double normalizedAngle = (angleDegrees % 360.0);

            if (normalizedAngle < 0.0)
                normalizedAngle = 360.0 + normalizedAngle;
            else if (normalizedAngle == 0.0)
                // If useZeroAngle is true then angles that are even multiples of 360 will be normalized 
                // to 0.0.  If false then 360.0 is used.
                normalizedAngle = useZeroAngle ? 0.0 : 360.0;

            return normalizedAngle;
        }

        public static double GetArcLength(double chord, double radius)
        {
            double chordAngle = 2.0*Math.Asin(0.5 * chord / radius);
            double circumference = 2.0 * Math.PI * radius;

            return (chordAngle / (2.0 * Math.PI)) * circumference;
        }

        public static double GetChordAngle(double chord, double radius)
        {
            if (chord <= 2.0 * radius)
            {            
                double chordAngleRadians = 2.0 * Math.Asin(0.5 * chord / radius);
                double angleDegrees = chordAngleRadians * 180.0/Math.PI;

                return angleDegrees;
            }
            else
                throw new System.ApplicationException("When calling method GetChordAngle, input chord is greater than twice the radius.");
        }

        public static Vector GetRadialVector(Point point, Point center)
        {
            return (point - center);
        }

        public static Vector GetRadialUnitVector(Point point, Point center)
        {
            Vector vec = GetRadialVector(point, center);
            vec.Normalize();

            return vec;
        }

        public static double GetAngleDifference(double startAngle, double endAngle)
        {
            startAngle = GetNormalizedAngle(startAngle);
            endAngle = GetNormalizedAngle(endAngle);
            if (endAngle < startAngle)
                endAngle = endAngle + 360.0;

            return GetNormalizedAngle(endAngle - startAngle);
        }
    }
}
