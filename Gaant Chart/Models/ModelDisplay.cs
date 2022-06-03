using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gaant_Chart.Models
{
    public class ModelDisplay
    {
        public List<TaskDisplay> plannedBlocks { get; set; }
        public List<TaskDisplay> completedBlocks { get; set; }

        public Model model { get; }

        private int numDaysInView { get; set; }

        private double pixelsPerDay { get; set; }

        private DateTime startDisplayDate { get; set; }
        private DateTime endDisplayDate { get; set; }

        public ModelDisplay(Model model, int numDaysInView, double pixelsPerDay)
        {
            this.model = model;
            this.numDaysInView = numDaysInView;
            this.pixelsPerDay = pixelsPerDay;

            startDisplayDate = model.startDate;
            endDisplayDate = startDisplayDate.AddDays(numDaysInView);


            initCompletedBlocks();
            initPlannedBlocks();
        }
        
        public ModelDisplay(Model model, CanvasView canvasView)
        {
            this.model = model;
            numDaysInView = (int)(canvasView.startDate - canvasView.endDate).TotalDays;
            pixelsPerDay = canvasView.pixelsPerDay;
        }

        private void initCompletedBlocks()
        {
            foreach(Task task in model.tasks)
            {
                if(task.completed)
                {
                    TaskDisplay taskDisplay = new CompletedTaskDisplay(task, pixelsPerDay);
                    taskDisplay.cutOffIfNecessary(endDisplayDate);
                    completedBlocks.Add(taskDisplay);
                }
                else
                {
                    break;
                }
            }
        }

        private void initPlannedBlocks()
        {
            foreach(Task task in model.tasks)
            {
                TaskDisplay taskDisplay = new PlannedTaskDisplay(task, pixelsPerDay);
                taskDisplay.cutOffIfNecessary(endDisplayDate);
                plannedBlocks.Add(taskDisplay);
            }
        }

    }
}
