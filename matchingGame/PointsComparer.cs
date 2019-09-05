using System;
using System.Collections.Generic;

namespace matchingGame
{
    public class PointsComparer : IComparer<Score>
    {

        int IComparer<Score>.Compare(Score x, Score y)
        {
            return x.MyPoints.CompareTo(y.MyPoints);

            throw new NotImplementedException();
        }
    }

}
