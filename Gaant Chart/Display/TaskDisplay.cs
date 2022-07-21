using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Gaant_Chart.Display
{
    public class TaskDisplay
    { 
        //Store rectangle position on graph

        public DateTime startDate;
        public DateTime endDate;

        public int taskTypeId;
            
        public Rectangle rectangle;

        public TaskDisplay(DateTime startDate, DateTime endDate, int taskTypeId)
        {
            rectangle = new Rectangle();
            this.startDate = startDate;
            this.endDate = endDate;
            this.taskTypeId = taskTypeId;
        }
    }

    public class DateDisplay
    {
        public DateTime date;
        public Label label;

        public DateDisplay(DateTime date)
        {
            label = new Label();
            this.date = date;
        }
    }

    public class LineDisplay
    {
        public DateTime date;
        public Line line;
        public LineDisplay(DateTime date)
        {
            line = new Line();
            this.date = date;
        }

    }
}
