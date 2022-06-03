using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gaant_Chart.Models
{
    public abstract class TaskDisplay
    {
        private Task task { get; }
        private CanvasView view { get; set; }

        public Rectangle rectangle { get; set; }
        public SolidColorBrush color { get; }
        public double leftOffset { get; set; }
        public double rightOffset { get; set; }


        protected DateTime start { get; set; }
        protected DateTime end { get; set; }
        private double numDays { get; set; }


        protected SolidColorBrush COMPLETED_COLOR = new SolidColorBrush(Color.FromRgb(149, 219, 139));
        protected SolidColorBrush PLANNED_COLOR = new SolidColorBrush(Color.FromRgb(254, 212, 158));

        public TaskDisplay(Task task, CanvasView view )
        {
            this.task = task;
            this.view = view;
            rectangle = new Rectangle();
        }

        protected void finishInit()
        {
            numDays = (start - end).TotalDays;
            resizeTask(view);
        }

        public void resizeTask(CanvasView view)
        {
            this.view = view;
            rectangle.Width = view.pixelsPerDay * numDays;
            cutOffIfNecessary();
        }

        public void cutOffIfNecessary()
        {
            if(isCutOff())
            {
                double numDaysDisplay = (view.endDate - start).TotalDays;
                rectangle.Width = view.pixelsPerDay * numDaysDisplay;
            }
        }

        private Boolean isCutOff()
        {
            return (start < view.endDate && view.endDate < end);
        }
    }

    public class PlannedTaskDisplay : TaskDisplay 
    { 
        public PlannedTaskDisplay(Task task, CanvasView view) : base(task, view)
        {
            rectangle.Fill = PLANNED_COLOR;
            start = task.plannedStartDate;
            end = task.plannedEndDate;

            finishInit();
        }

    }

    public class CompletedTaskDisplay : TaskDisplay
    {
        public CompletedTaskDisplay(Task task, CanvasView view) : base(task, view)
        {
            rectangle.Fill = COMPLETED_COLOR;
            start = task.startDate;
            end = task.endDate;

            finishInit();
        }
    }







}
