using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FifaResults.Entities.Comparer
{
    public class SortByOverallValueDesc : IComparer<Club>
    {
        public int Compare([AllowNull] Club clubLeft, [AllowNull] Club clubRight)
        {
            if (clubLeft == null && clubRight == null)
                return 0;
            else if (clubLeft == null)
                return 1;
            else if (clubRight == null)
                return -1;

            return clubRight.OverallValue.CompareTo(clubLeft.OverallValue);
        }
    }
}
