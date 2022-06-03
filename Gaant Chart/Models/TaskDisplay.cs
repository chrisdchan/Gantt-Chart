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
        DateTime day { get; set; }
        SolidColorBrush color { get; }

        Rectangle rectangle { get; set; }

        const double DEFAULT_WIDTH = 23;
        const double DEFAULT_HEIGHT = 30;

        public TaskDisplay(DateTime day, SolidColorBrush color)
        {
            this.day = day;
            this.color = color;

            rectangle = new Rectangle();
            rectangle.Fill = color;

            rectangle.Width = DEFAULT_WIDTH;
            rectangle.Height = DEFAULT_HEIGHT;
        }



    }
}
