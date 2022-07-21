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
        public List<oldTaskDisplay> plannedBlocks { get; set; }
        public List<oldTaskDisplay> completedBlocks { get; set; }

        public Model model { get; }

        private CanvasView view { get; set; }

        public ModelDisplay(Model model, CanvasView view)
        {
            this.model = model;
            this.view = view;

            plannedBlocks = new List<oldTaskDisplay>();
            completedBlocks = new List<oldTaskDisplay>();

            initCompletedBlocks();
            initPlannedBlocks();
        }

        private void initCompletedBlocks()
        {
            foreach(Task task in model.tasks)
            {
                if(task.completed)
                {
                    oldTaskDisplay taskDisplay = new CompletedTaskDisplay(task, view);
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
                oldTaskDisplay taskDisplay = new PlannedTaskDisplay(task, view);
                plannedBlocks.Add(taskDisplay);
            }
        }

        public oldTaskDisplay addAndGetCompletedTask(Task task)
        {
            oldTaskDisplay taskDisplay = new CompletedTaskDisplay(task, view);
            completedBlocks.Add(taskDisplay);
            return taskDisplay;
        }

        public void resize(CanvasView view)
        {
            foreach(oldTaskDisplay taskDisplay in plannedBlocks)
            {
                taskDisplay.resize(view);
            }

            foreach(oldTaskDisplay taskDisplay in completedBlocks)
            {
                taskDisplay.resize(view);
            }
        }
    }
}
