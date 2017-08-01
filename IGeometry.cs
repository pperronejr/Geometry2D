
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
    /// This is the common interface for all geometric entities.  All fields, properties, and methods
    /// required by these entities to interact and are common amongst them should be included here.
    /// </summary>
    public interface IGeometry
    {
        // These method definitions are the same in Line and Polyline. These field definitions
        // should be extracted from these 2 classes if there is a way to do it.  Could not
        // use an abstract class since Polyline already inherits from a class.  When including 
        // abstract definitions for overloaded versions of these, they cannot be overriden in 
        // beyond the class that inherits from the abstract class which is a limitation of 
        // abstract classes.  
        // Since I could not find a clean way to do this, the method definitions are defined
        // in each class Line and Polyline.

        bool Interferes(IGeometry iGeom);
        List<Point> Intersections(IGeometry iGeom);
    }
}
