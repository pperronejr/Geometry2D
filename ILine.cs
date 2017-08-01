
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
    public interface ILine : IGeometry
    {
        bool ThroughPoint(Point point);
        bool Overlaps(Line line);

        // Overlap returns the distance of the overlap between 2 line objects if there is one.
        //
        // FUTURE ENHANCEMENT: Consider havign Overlap return a Line, LineSegment, or Ray 
        // representing the actual overlap else null if there is no overlap
        double OverlapLength(Line line);

        bool IsCoincident(Polyline pLine);
    }
}
