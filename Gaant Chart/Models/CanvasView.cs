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
        public double numDays { get; set; }

        public double LEFT_START_OFF = 225;
        public double TOP_START_OFF = 120;
        public double BlOCK_HEIGHT = 30;
        public double TASK_HEIGHT = 31.9;

        public double RIGHT_OUTER_BORDER = 713;
        public double LEFT_INNER_BORDER = 125;

        public CanvasView(DateTime startDate, int numDays)
        {
            this.numDays = numDays;
            this.startDate = startDate;
            endDate = startDate.AddDays(numDays);
            pixelsPerDay = (RIGHT_OUTER_BORDER - LEFT_INNER_BORDER) / numDays;
        }
        public CanvasView(Model model, int numDays) : this(model.startDate, numDays) { }

        public void changeStartDate(DateTime startDate)
        {
            this.startDate = startDate;
            endDate = startDate.AddDays(numDays);
        }

        public void changeNumDays(int numDays)
        {
            this.numDays = numDays;
            endDate = startDate.AddDays(numDays);
            pixelsPerDay = (RIGHT_OUTER_BORDER - LEFT_INNER_BORDER) / numDays;
        }

    }
}
