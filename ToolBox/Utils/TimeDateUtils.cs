using System;
using System.Collections.Generic;

namespace ToolBox
{
    class TimeDateUtils
    {
        public static TimeSpan AverageTimeSpan(List<TimeSpan> TimeSpanList)
        {
            TimeSpan total = new TimeSpan(0L);
            foreach (TimeSpan ts in TimeSpanList)
            {
                total += ts;
            }
            return TimeSpan.FromTicks(total.Ticks / TimeSpanList.Count);
        }           
    }
}