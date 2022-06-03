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

        private CanvasView view { get; set; }

        
        public ModelDisplay(Model model, CanvasView view)
        {
            this.model = model;
            this.view = view;

            initCompletedBlocks();
            initPlannedBlocks();
        }

        private void initCompletedBlocks()
        {
            foreach(Task task in model.tasks)
            {
                if(task.completed)
                {
                    TaskDisplay taskDisplay = new CompletedTaskDisplay(task, view);
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
                TaskDisplay taskDisplay = new PlannedTaskDisplay(task, view);
                plannedBlocks.Add(taskDisplay);
            }
        }

        public void resize(CanvasView view)
        {
        }

    }
}
