
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
    class Triangle : Polygon
    {
        public Triangle(Point point1, Point point2, Point point3)
        {
            DefineTriangle(point1, point2, point3);
        }

        protected Triangle()
        { }

        public void DefineTriangle(Point point1, Point point2, Point point3)
        {
            DefinePolygon
                (new List<Point>() { point1, point2, point3, point1 });
        }
    }
}
