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
        private SolidColorBrush PINK = new SolidColorBrush(Color.FromRgb(252, 180, 180));
        private SolidColorBrush DULL_GREEN = new SolidColorBrush(Color.FromRgb(50, 236, 80));
        private SolidColorBrush PISS = new SolidColorBrush(Color.FromRgb(242, 218, 46));
        private SolidColorBrush GRAY = new SolidColorBrush(Color.FromRgb(233, 233, 233));

        public CanvasDisplay(CanvasView view)
        {
            this.view = view;
            canvasTexts = new List<CanvasElement>();

            initDates();
            initDynamicLines();
            initTaskLabels();
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
            staticLines = new List<CanvasLine>
            {
                new CanvasLine(
                    view.LEFT_OUTER_BORDER, view.BOTTOM_BORDER,
                    view.LEFT_INNER_BORDER, view.BOTTOM_BORDER,
                    RED ),
                new CanvasLine(
                    view.LEFT_OUTER_BORDER, view.TOP_BORDER,
                    view.LEFT_OUTER_BORDER, view.BOTTOM_BORDER,
                    RED ),
                new CanvasLine(
                    view.LEFT_OUTER_BORDER, view.TOP_BORDER,
                    view.LEFT_INNER_BORDER, view.TOP_BORDER,
                    RED ),
                new CanvasLine(
                    view.LEFT_OUTER_BORDER, view.TOP_BORDER,
                    view.LEFT_INNER_BORDER, view.TOP_BORDER,
                    RED ),
                new CanvasLine(
                    view.LEFT_INNER_BORDER, view.TOP_BORDER,
                    view.LEFT_INNER_BORDER, view.BOTTOM_BORDER,
                    RED ),

                new CanvasLine(
                        view.LEFT_OUTER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 5,
                        view.LEFT_INNER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 5,
                        RED ),
                new CanvasLine(
                        view.LEFT_OUTER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 10,
                        view.LEFT_INNER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 10,
                        RED ),
                new CanvasLine(
                        view.LEFT_OUTER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 12,
                        view.LEFT_INNER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 12,
                        RED ),
                /*
                new CanvasLine(
                        view.LEFT_INNER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 5,
                        view.RIGHT_OUTER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 5,
                        PINK ),
                new CanvasLine(
                        view.LEFT_INNER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 10,
                        view.RIGHT_OUTER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 10,
                        PINK ),
                new CanvasLine(
                        view.LEFT_INNER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 12,
                        view.RIGHT_OUTER_BORDER, view.TOP_BORDER + view.TASK_HEIGHT * 12,
                        PINK ),
                */


                new CanvasLine(
                    view.LEFT_INNER_BORDER, view.BOTTOM_BORDER,
                    view.RIGHT_OUTER_BORDER, view.BOTTOM_BORDER,
                    DULL_GREEN ),
                new CanvasLine(
                    view.RIGHT_OUTER_BORDER, view.TOP_BORDER,
                    view.RIGHT_OUTER_BORDER, view.BOTTOM_BORDER,
                    DULL_GREEN ),

                new CanvasLine(
                    view.LEFT_INNER_BORDER, view.TOP_BORDER,
                    view.RIGHT_OUTER_BORDER, view.TOP_BORDER,
                    PISS )

            };
        }

        private void initTaskLabels()
        {
            canvasTexts = new List<CanvasElement>();

            foreach(String taskName in data.allTasks)
            {
                canvasTexts.Add(new TaskLabel(taskName));
            }

            for(int i = 0; i < canvasTexts.Count; i++)
            {
                canvasTexts[i].leftoffset = view.LABEL_LEFT_MARGIN;
                canvasTexts[i].topoffset = view.LABEL_TOP_OFFSET + view.LABEL_VERTICAL_SPACE * i;
            }

            initGroupLabels();
        }

        private void initGroupLabels()
        {
            // must be done manually :(
            canvasTexts.Add(new TaskGroupLabel("Segmentation", 100, 35, 168));
            canvasTexts.Add(new TaskGroupLabel("Segmentation Review and Approval", 100,  30, 312));
            canvasTexts.Add(new TaskGroupLabel("Mesh Prep and Export", 50, 30, 444.8));
            canvasTexts.Add(new TaskGroupLabel("Physics Modeling and Report", 50, 20, 505.6));
        }

        private void updateDynamicLines()
        {
            dynamicLines.Clear();

            for(int i = 1; i < view.numDays; i++)
            {
                double x = view.LEFT_INNER_BORDER + i * view.pixelsPerDay;

                if(isWeekSinceStart(i))
                {
                    CanvasLine line = new CanvasLine(
                        x, view.TOP_BORDER,
                        x, view.BOTTOM_BORDER,
                        GRAY);

                    line.line.Tag = true;
                    dynamicLines.Add(line);
                }
                else
                {
                     CanvasLine line = new CanvasLine(
                        x, view.TOP_BORDER,
                        x, view.TOP_BORDER + view.DAYLINE_LENGTH,
                        PISS);

                    line.line.Tag = true;
                    dynamicLines.Add(line);

                }
            }
        }

        private void updateDates()
        {
            dates.Clear();

            double pixelsPerWeek = view.pixelsPerDay * 7;
            int numWeeks = (int) view.numDays / 7;

            for(int i = 0; i <= numWeeks; i++)
            {
                DateTime date = view.startDate.AddDays(i * 7);
                CanvasElement dateLabel = new DateLabel(date);
                dateLabel.leftoffset = view.DATE_LEFT_OFFSET + i * pixelsPerWeek;
                dateLabel.topoffset = view.DATE_TOP_OFFSET;

                dates.Add(dateLabel);
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
