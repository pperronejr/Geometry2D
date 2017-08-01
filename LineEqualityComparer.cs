
//
// Copyright 2017 Paul Perrone.  All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDA.Geometry2D
{
    public class LineEqualityComparer : IEqualityComparer<Line>
    {
        public bool Equals(Line l1, Line l2)
        {
            return l1.Overlaps(l2);
        }

        public int GetHashCode(Line l)
        {
            string code;
            if (l.IsVertical)
                code = Double.PositiveInfinity.ToString() + l.XConstant.ToString();
            else
                code = l.Slope.ToString() + l.YIntercept.ToString();

            return code.GetHashCode();
        }
    }
}
