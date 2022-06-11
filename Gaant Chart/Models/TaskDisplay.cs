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
        public double topOffset { get; set; }
        public double width;
        public double height;

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
            numDays = (endDate - startDate).TotalDays;
            setInView();
            sizeTask();
            setOffsets();
        }
        private void sizeTask()
        {
            if(numDays < 0)
            {
                rectangle.Width = view.pixelsPerDay;

            }
            else
            {
               rectangle.Width = view.pixelsPerDay * numDays;
            }
            cutOffIfNecessary();
        }

        public void resize(CanvasView view)
        {
            this.view = view;
            setInView();
            sizeTask();
            setOffsets();
        }

        private void setOffsets()
        {
            double dayOffset = (startDate - view.startDate).TotalDays;
            leftOffset = view.LEFT_START_OFF + dayOffset * view.pixelsPerDay;

            topOffset = view.TOP_START_OFF + task.typeInd * view.TASK_HEIGHT;
        }

        private void setInView()
        {
            inView = (startDate <= view.endDate && endDate >= view.startDate);
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
            rectangle.Height = view.BLOCK_HEIGHT;
            rectangle.Opacity = 0.8;
            startDate = task.plannedStartDate;
            endDate = task.plannedEndDate;

            finishInit();
        }

    }

    public class CompletedTaskDisplay : TaskDisplay
    {
        public double BlOCK_HEIGHT = 22;
        public CompletedTaskDisplay(Task task, CanvasView view) : base(task, view)
        {
            rectangle.Fill = COMPLETED_COLOR;
            rectangle.Height= BlOCK_HEIGHT;
            startDate = task.startDate;
            endDate = task.endDate;

            finishInit();

            topOffset += (view.BLOCK_HEIGHT - BlOCK_HEIGHT) / 2;
        }
    }







}
