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

        public Boolean inView { get; set; }

        protected DateTime startDate { get; set; }
        protected DateTime endDate { get; set; }
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
            sizeTask();
            setInView();
            setOffsets();
            cutOffIfNecessary();
            numDays = (startDate - endDate).TotalDays;
        }
        private void sizeTask()
        {
            rectangle.Width = view.pixelsPerDay * numDays;
            cutOffIfNecessary();
        }

        public void resize(CanvasView view)
        {
            this.view = view;
            sizeTask();
            setInView();
            setOffsets();
            cutOffIfNecessary();
        }

        private void setOffsets()
        {
            double dayOffset = (view.startDate - startDate).TotalDays;
            leftOffset = view.LEFT_START_OFF + dayOffset * view.pixelsPerDay;

            rightOffset = view.TOP_START_OFF + task.typeInd * view.TASK_HEIGHT;
        }

        private void setInView()
        {
            inView = (startDate <= view.endDate || endDate >= view.startDate);
            if(inView)
            {
                rectangle.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                rectangle.Visibility = System.Windows.Visibility.Hidden;
            }
        }


        private void cutOffIfNecessary()
        {
            if (!inView) return;

            double numDaysInView;
            if(startDate < view.startDate)
            {
                numDaysInView = (endDate - view.startDate).TotalDays;
                leftOffset = view.LEFT_START_OFF;
                rectangle.Width = view.pixelsPerDay * numDaysInView;
            }
            
            if(endDate > view.endDate)
            {
                numDaysInView = (view.endDate - startDate).TotalDays;
                rectangle.Width = view.pixelsPerDay * numDaysInView; 
            }
        }

    }

    public class PlannedTaskDisplay : TaskDisplay 
    { 
        public PlannedTaskDisplay(Task task, CanvasView view) : base(task, view)
        {
            rectangle.Fill = PLANNED_COLOR;
            startDate = task.plannedStartDate;
            endDate = task.plannedEndDate;

            finishInit();
        }

    }

    public class CompletedTaskDisplay : TaskDisplay
    {
        public CompletedTaskDisplay(Task task, CanvasView view) : base(task, view)
        {
            rectangle.Fill = COMPLETED_COLOR;
            startDate = task.startDate;
            endDate = task.endDate;

            finishInit();
        }
    }







}
