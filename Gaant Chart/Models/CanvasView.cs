using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gaant_Chart.Models
{
    public class CanvasView
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public DateTime modelStartDate { get;set; }
        public double pixelsPerDay { get; set; }
        public double numDays { get; set; }

        public double LEFT_START_OFF = 225;
        public double TOP_START_OFF = 120;
        public double TASK_HEIGHT = 31.9;

        public double BLOCK_HEIGHT = 30;

        public double RIGHT_OUTER_BORDER = 713;
        public double LEFT_INNER_BORDER = 125;
        public double LEFT_OUTER_BORDER = -80;

        public double BOTTOM_BORDER = 465;
        public double TOP_BORDER = 15;

        public double DAYLINE_LENGTH = 5;
        public double LABEL_LEFT_MARGIN = 20;
        public double LABEL_TOP_OFFSET = 120;
        public double LABEL_VERTICAL_SPACE = 32;

        public double DATE_TOP_OFFSET = 40;
        public double DATE_LEFT_OFFSET = 210;

        public CanvasView(DateTime startDate, int numDays)
        {
            this.numDays = numDays;
            this.startDate = startDate;
            modelStartDate = startDate;
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
