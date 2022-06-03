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

        public int numDaysInView { get; set; }

        private SolidColorBrush COMPLETED_COLOR = new SolidColorBrush(Color.FromRgb(149, 219, 139));
        private SolidColorBrush PLANNED_COLOR = new SolidColorBrush(Color.FromRgb(254, 212, 158));

        public ModelDisplay(Model model, int numDaysInView)
        {


        }

        private void initCompletedBlocks()
        {
            foreach(Task task in model.tasks)
            {
                if(task.completed)
                {
                    TaskDisplay taskDisplay = new TaskDisplay(model.startDate, COMPLETED_COLOR);
                    plannedBlocks.Add(taskDisplay);

                }
            }
        }
    }
}
