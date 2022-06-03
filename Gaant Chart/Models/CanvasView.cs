using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.Models
{
    public class CanvasView
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public double pixelsPerDay { get; set; }

        public const double LEFT_START_OFF = 225;
        public const double TOP_START_OFF = 120;
       
        public CanvasView(DateTime startDate, DateTime endDate, double pixelsPerDay)
        {
            this.startDate = startDate;
            this.endDate = endDate;
            this.pixelsPerDay = pixelsPerDay;
        }
    }
}
