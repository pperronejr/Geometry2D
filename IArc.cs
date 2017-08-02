
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
    public interface IArc : IGeometry
    {
        bool ThroughPoint(Point point);
        bool Overlaps(Arc arc);
    }
}
