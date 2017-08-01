
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
    public class Rectangle : Polygon
    {
        public Rectangle(Point center, double width, double height)
        {
            DefineRectangle(center, width, height);
        }
        public Rectangle(Point center, double width, double height, double angle)
        {
            DefineRectangle(center, width, height, angle);
        }

        protected Rectangle()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="angle">Angle about center which defines the rectangles orientation</param>
       public void DefineRectangle(Point center, double width, double height, double angle)
        {
            Point bottomLeft = center + GeometricFunctions.RotateVector( new Vector(-width / 2.0, -height / 2.0), angle);
            Point bottomRight = center + GeometricFunctions.RotateVector( new Vector(width / 2.0, -height / 2.0), angle);
            Point topRight = center + GeometricFunctions.RotateVector( new Vector(width / 2.0, height / 2.0), angle);
            Point topLeft = center + GeometricFunctions.RotateVector(new Vector(-width / 2.0, height / 2.0), angle);

            Center = center;
            Width = width;
            Height = height;
            Angle = angle;

            DefinePolygon
                (new List<Point>() { bottomLeft, bottomRight, topRight, topLeft, bottomLeft });
        }

        public void DefineRectangle(Point center, double width, double height)
        {
            DefineRectangle(center, width, height, 0.0); 
        }

        [XmlIgnore()]
        public Point Center;
        [XmlIgnore()]
        public double Angle;
        public double Width;
        public double Height;
    }
}
