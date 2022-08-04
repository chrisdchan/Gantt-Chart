using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.DataStructures
{
    public class Conversions
    {
        public static DateTime toDate(DateTime date, int hours, int minutes, int dayPeriod)
        {
            return date.AddDays(hours + 12 * dayPeriod^1).AddMinutes(minutes);
        }

        public static (DateTime, int, int, int) toDateUI(DateTime date)
        {
            DateTime flooredDate = CanvasGraph.floorDate(date);
            int hours = (int)Math.Floor(date.TimeOfDay.TotalHours);
            int minutes = (int)((date.TimeOfDay.TotalHours - hours) * 60);

            int dayPeriod = 0;
            if(hours > 12)
            {
                dayPeriod = 1;
                hours -= 12;
            }

            return (flooredDate, hours, minutes, dayPeriod);
        }

    }
}
