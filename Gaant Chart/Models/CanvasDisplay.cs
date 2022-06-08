using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gaant_Chart.Models
{
    public class CanvasDisplay
    {
        public List<CanvasElement> canvasTexts { get; set; }
        public List<CanvasElement> dates { get; set; }
        public List<CanvasLine> dynamicLines { get; set; }
        public List<CanvasLine> staticLines { get; set; }

        private CanvasView view { get; set; }

        private SolidColorBrush RED = new SolidColorBrush(Colors.Red);
        private SolidColorBrush DULL_GREEN = new SolidColorBrush(Color.FromRgb(50, 236, 80));
        private SolidColorBrush PISS = new SolidColorBrush(Color.FromRgb(242, 218, 46));
        private SolidColorBrush GRAY = new SolidColorBrush(Color.FromRgb(233, 233, 233));

        public CanvasDisplay(CanvasView view)
        {
            this.view = view;
            canvasTexts = new List<CanvasElement>();

            initDates();
            initDynamicLines();
            initLabels();
            initStaticLines();
        }

        private void initDynamicLines()
        {
            dynamicLines = new List<CanvasLine>();
            updateDynamicLines();
        }

        private void initDates()
        {
            dates = new List<CanvasElement>();
            updateDates();
        }

        private void initStaticLines()
        {
            staticLines = new List<CanvasLine>();  
            
            staticLines.Add(new CanvasLine(
                view.LEFT_OUTER_BORDER, view.BOTTOM_BORDER,
                view.LEFT_INNER_BORDER, view.BOTTOM_BORDER,
                RED ));

            staticLines.Add(new CanvasLine(
                view.LEFT_OUTER_BORDER, view.TOP_BORDER,
                view.LEFT_OUTER_BORDER, view.BOTTOM_BORDER,
                RED ));
            
            staticLines.Add(new CanvasLine(
                view.LEFT_OUTER_BORDER, view.TOP_BORDER,
                view.LEFT_INNER_BORDER, view.TOP_BORDER,
                RED ));

            staticLines.Add(new CanvasLine(
                view.LEFT_INNER_BORDER, view.TOP_BORDER,
                view.LEFT_INNER_BORDER, view.BOTTOM_BORDER,
                RED ));

            
            staticLines.Add(new CanvasLine(
                view.LEFT_INNER_BORDER, view.BOTTOM_BORDER,
                view.RIGHT_OUTER_BORDER, view.BOTTOM_BORDER,
                DULL_GREEN ));

            staticLines.Add(new CanvasLine(
                view.LEFT_INNER_BORDER, view.TOP_BORDER,
                view.RIGHT_OUTER_BORDER, view.BOTTOM_BORDER,
                DULL_GREEN ));

            staticLines.Add(new CanvasLine(
                view.LEFT_INNER_BORDER, view.TOP_BORDER,
                view.RIGHT_OUTER_BORDER, view.TOP_BORDER,
                PISS ));
        }

        private void initLabels()
        {
            canvasTexts = new List<CanvasElement>();

            foreach(String taskName in data.allTasks)
            {
                canvasTexts.Add(new TaskLabel(taskName));
            }

            foreach((String groupName, int index) in data.taskLabelGroups)
            {
                canvasTexts.Insert(index, new TaskGroupLabel(groupName));
            }

            for(int i = 0; i < canvasTexts.Count; i++)
            {
                canvasTexts[i].leftoffset = view.LABEL_LEFT_MARGIN;
                canvasTexts[i].topoffset = view.LABEL_TOP_OFFSET + view.LABEL_VERTICAL_SPACE * i;
            }
        }

        private void updateDynamicLines()
        {
            for(int i = 0; i < view.numDays; i++)
            {
                double x = view.LEFT_INNER_BORDER + i * view.pixelsPerDay;

                if(isWeekSinceStart(i))
                {
                    dynamicLines.Add(new CanvasLine(
                        x, view.TOP_BORDER,
                        x, view.BOTTOM_BORDER,
                        PISS));
                }
                else
                {
                    dynamicLines.Add(new CanvasLine(
                        x, view.TOP_BORDER,
                        x, view.TOP_BORDER + view.DAYLINE_LENGTH,
                        GRAY ));
                }
            }
        }

        private void updateDates()
        {
            double pixelsPerWeek = view.pixelsPerDay * 7;

            for(int i = 0; i < view.numDays; i++)
            {
                if(isWeekSinceStart(i))
                {
                    DateTime date = view.startDate.AddDays(i);
                    CanvasElement dateLabel = new DateLabel(date);
                    dateLabel.leftoffset = view.DATE_LEFT_OFFSET + i * pixelsPerWeek;
                    dateLabel.topoffset = view.DATE_TOP_OFFSET;

                    dates.Add(dateLabel);
                }
            }
        }


        private Boolean isWeekSinceStart(int i)
        {
            DateTime date = view.startDate.AddDays(i);
            int numDays = (int)(date - view.modelStartDate).TotalDays;
            return (numDays % 7 == 0);
        }

        public void resize(CanvasView view)
        {
            this.view = view;
            updateDates();
            updateDynamicLines();
        }

    }
}
