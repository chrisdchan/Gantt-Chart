using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gaant_Chart.Models
{
    public class TaskDisplay
    {
        private double pixelsPerDay { get; set; }
        Task task { get; }

        public Rectangle rectangle { get; set; }
        public SolidColorBrush color { get; }
        private double numDays { get; set; }

        private DateTime start { get; set; }
        private DateTime end { get; set; }

        const double DEFAULT_WIDTH = 23;
        const double DEFAULT_HEIGHT = 30;

        private SolidColorBrush COMPLETED_COLOR = new SolidColorBrush(Color.FromRgb(149, 219, 139));
        private SolidColorBrush PLANNED_COLOR = new SolidColorBrush(Color.FromRgb(254, 212, 158));

        public TaskDisplay(Task task, double pixelsPerDay)
        {
            this.task = task;
            this.pixelsPerDay = pixelsPerDay;
            rectangle = new Rectangle();

            if (task.completed)
            {
                initCompletedTask();
            }
            else
            {
                initPlannedTask();
            }

            numDays = (start - end).TotalDays;

            resizeTask(pixelsPerDay);
        }

        private void initCompletedTask()
        {
            rectangle.Fill = COMPLETED_COLOR;
            start = task.startDate;
            end = task.endDate;
        }

        private void initPlannedTask()
        {
            rectangle.Fill = PLANNED_COLOR;
            start = task.plannedStartDate;
            end = task.plannedEndDate;
        }

        public void resizeTask(double pixelsPerDay)
        {
            rectangle.Width = pixelsPerDay * numDays;
        }

        public void cutOffTask(DateTime lastDateDisplayed)
        {
            if(isCutOff(lastDateDisplayed))
            {
                double numDaysDisplay = (lastDateDisplayed - start).TotalDays;
                rectangle.Width = pixelsPerDay * numDaysDisplay;
            }

        }

        private Boolean isCutOff(DateTime lastDateDisplayed)
        {
            return (start < lastDateDisplayed && lastDateDisplayed < end);
        }



    }


}
