
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
    public struct Extents
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points">List of points to determine extents from</param>
        /// <param name="angle">Transform angle to return extents for an alternate coordinate system</param>
        public Extents(List<Point> points, double angle)
        {
            Angle = angle;

            List<Point> transformedPts = new List<Point>() { };

            points.ForEach(pt => transformedPts.Add((Point)(GeometricFunctions.RotateVector((Vector)pt, -angle))));

            XMin = transformedPts.Min(pt => pt.X);
            XMax = transformedPts.Max(pt => pt.X);
            YMin = transformedPts.Min(pt => pt.Y);
            YMax = transformedPts.Max(pt => pt.Y);
            XDistance = XMax - XMin;
            YDistance = YMax - YMin;
        }
        public double XMin, XMax, YMin, YMax, Angle, XDistance, YDistance;
    }

    public static class GeometricFunctions
    {
        /// <summary>
        /// Defines the precision of underlying calculations in all static methods of this class
        /// </summary>
        static public int DecimalPlaces = 15;
        /// <summary>
        /// Return the input num rounded to a certain number of decimal places defined by DecimalPlaces;
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        static public double GetRoundNum(double num)
        {
            return Math.Round(num, DecimalPlaces);
        }
        /// <summary>
        /// Return a new vector, which is the result of rotating vector v counterclockwise about the z axis by angle
        /// </summary>
        /// <param name="v"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        static public Vector RotateVector(Vector v, double angle)
        {
            if (angle != 0.0)
            {
                double cosAngle = GetRoundNum(Math.Cos(Math.PI * angle / 180.0));
                double sinAngle = GetRoundNum(Math.Sin(Math.PI * angle / 180.0));
                double x = cosAngle * v.X - sinAngle * v.Y;
                double y = sinAngle * v.X + cosAngle * v.Y;
                return new Vector(x, y);
            }
            else
                return v;
        }

        /// <summary>
        /// Returns true if points are the same within a tolerance defined through DecimalPlaces
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        static public bool EqualPoints(Point pt1, Point pt2)
        {
            return (0.0 == GetRoundNum((pt1 - pt2).Length));
        }
    }
}
