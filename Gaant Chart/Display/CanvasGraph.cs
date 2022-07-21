using Gaant_Chart.Display;
using Gaant_Chart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gaant_Chart
{
    public class CanvasGraph
    {
        public Canvas canvas;
        public DateTime viewStartDate { get; set; }
        public DateTime viewEndDate { get; set; }
        public DateTime modelStartDate { get;set; }

        private double LEFT_START_OFF = 225;
        private double TOP_START_OFF = 120;
        private double TASK_HEIGHT = 31.9;

        private double BLOCK_HEIGHT = 30;

        private double RIGHT_OUTER_BORDER = 713;
        private double LEFT_INNER_BORDER = 125;
        private double LEFT_OUTER_BORDER = -80;
        private double GRAPH_WIDTH = 588;

        private double BOTTOM_BORDER = 465;
        private double TOP_BORDER = 15;

        private double DAYLINE_LENGTH = 5;
        private double LABEL_LEFT_MARGIN = 20;
        private double LABEL_TOP_OFFSET = 120;
        private double LABEL_VERTICAL_SPACE = 32;

        private double DATE_TOP_OFFSET = 40;
        private double DATE_LEFT_OFFSET = 210;

        private double DATE_ROTATION = 285; // 285
        private double GROUPLABEL_ROTATION = 270;

        private double DEFAULT_DAYS_IN_VIEW = 28;


        private SolidColorBrush RED = new SolidColorBrush(Colors.Red);
        private SolidColorBrush PINK = new SolidColorBrush(Color.FromRgb(252, 180, 180));
        private SolidColorBrush DULL_GREEN = new SolidColorBrush(Color.FromRgb(50, 236, 80));
        private SolidColorBrush PISS = new SolidColorBrush(Color.FromRgb(242, 218, 46));
        private SolidColorBrush GRAY = new SolidColorBrush(Color.FromRgb(233, 233, 233));
        protected SolidColorBrush WHITE = new SolidColorBrush(Colors.White);
        protected SolidColorBrush GREEN = new SolidColorBrush(Color.FromRgb(50, 236, 0));
        protected SolidColorBrush COMPLETED_COLOR = new SolidColorBrush(Color.FromRgb(149, 219, 139));
        protected SolidColorBrush PLANNED_COLOR = new SolidColorBrush(Color.FromRgb(254, 212, 158));


        private List<TaskDisplay> taskDisplays = new List<TaskDisplay>();
        private List<DateDisplay> dateDisplays = new List<DateDisplay>();
        private List<LineDisplay> lineDisplays = new List<LineDisplay>();
        private Label modelNameLabel;

        private Func<int, double> taskBlockTypeToPixel;
        private Func<int, double> taskLabelToPixel;
        private Func<DateTime, DateTime> floorDate;
        private Func<DateTime, Boolean> isWeekFromModelStart;
        private Func<double, double> degToRad;
        public CanvasGraph()
        {
            canvas = new Canvas();
            addCanvasToApp();
            initMapFunctions();
            initModelLabel();
            drawStaticLines();
            addTaskLabels();
            addGroupLabels();
        }
        private void addCanvasToApp()
        {
            MainWindow.grid.Children.Add(canvas);

            Grid.SetColumn(canvas, 5);
            Grid.SetRow(canvas, 1);
            Grid.SetRowSpan(canvas, 6);
            canvas.Margin = new Thickness(10, 65, 10, 25);
        }
        private void initMapFunctions()
        {
            taskBlockTypeToPixel = typeId => TOP_START_OFF + typeId * TASK_HEIGHT;
            taskLabelToPixel = typeId => LABEL_TOP_OFFSET + LABEL_VERTICAL_SPACE * typeId;
            floorDate = date => date.AddMinutes(-date.TimeOfDay.TotalMinutes);
            isWeekFromModelStart = date => (date - modelStartDate).TotalDays % 7 == 0;
            degToRad = deg => deg * Math.PI / 180;
        }
        private double dateToPixel(DateTime date)
        {
            if (date < viewStartDate || date > viewEndDate) throw new Exception("date out of range");
            double pixelsPerDay = GRAPH_WIDTH / (viewEndDate - viewStartDate).TotalDays;
            return LEFT_INNER_BORDER + (date - viewStartDate).TotalDays * pixelsPerDay + 100;
        }
        private DateTime cielDate(DateTime date)
        {
            if(date.TimeOfDay.TotalMinutes == 0)
            {
                return date;
            }
            else
            {
                return date.AddMinutes(1440 - date.TimeOfDay.TotalMinutes);
            }
        }
        private void initModelLabel()
        {
            modelNameLabel = new Label();
            modelNameLabel.FontSize = 25;
            modelNameLabel.Foreground = GREEN;
            modelNameLabel.Width = 100;
            modelNameLabel.Height = 50;
            canvas.Children.Add(modelNameLabel);

            Canvas.SetLeft(modelNameLabel, 30);
            Canvas.SetTop(modelNameLabel, 30);
        }
        private void addTaskLabels()
        {
            for(int i = 0; i < data.allTasks.Length; i++) 
                addTaskLabel(data.allTasks[i], i);
        }
        private void addTaskLabel(String taskName, int taskTypeId)
        {
            Label label = new Label();
            label.Content = taskName;

            label.FontSize = 10;
            label.Foreground = WHITE;
            label.Width = 200;
            label.Height = 30;
            label.HorizontalContentAlignment = HorizontalAlignment.Right;

            canvas.Children.Add(label);

            Canvas.SetTop(label, taskLabelToPixel(taskTypeId));
            Canvas.SetLeft(label, LABEL_LEFT_MARGIN);
        }
        private void addGroupLabels()
        {
            addGroupLabel("Segmentation", 100, 35, 150);
            addGroupLabel("Segmentation Review and Approval", 100,  30, 305);
            addGroupLabel("Mesh Prep and Export", 50, 30, 440);
            addGroupLabel("Physics Modeling and Report", 50, 20, 505.6);
        }
        private void addGroupLabel(String name, double width, double leftOffset, double topOffset)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = name;
            textBlock.Width = width;
            textBlock.FontSize = 10;
            textBlock.Foreground = WHITE;
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Bottom;
            textBlock.LayoutTransform = new RotateTransform(GROUPLABEL_ROTATION);

            canvas.Children.Add(textBlock);

            Canvas.SetLeft(textBlock, leftOffset);
            Canvas.SetTop(textBlock, topOffset);
        }
        public void load()
        {
            makeCanvasHittable();
            addCanvasEventHandlers();
        }
        private void addCanvasEventHandlers()
        {
            canvas.PreviewMouseUp += new MouseButtonEventHandler(canvasPreviewMouseUp);
            canvas.PreviewMouseMove += new MouseEventHandler(canvasPreviewMouseMove);
            canvas.PreviewMouseDown += new MouseButtonEventHandler(canvasPreviewMouseDown);
            canvas.PreviewMouseWheel += new MouseWheelEventHandler(canvasMouseWheel);
            canvas.MouseLeave += new MouseEventHandler(canvasMouseLeave);
        }
        private void makeCanvasHittable()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = canvas.ActualWidth;
            rectangle.Height = canvas.ActualHeight;
            rectangle.Fill = WHITE;
            rectangle.Opacity = 0.1;

            canvas.Children.Add(rectangle);

            Canvas.SetLeft(rectangle, 0);
            Canvas.SetTop(rectangle, 0);
        }
        private void drawStaticLines()
        {
            addStaticLine(
                LEFT_OUTER_BORDER, BOTTOM_BORDER,
                LEFT_INNER_BORDER, BOTTOM_BORDER,
                RED);
            addStaticLine(
                LEFT_OUTER_BORDER, TOP_BORDER,
                LEFT_OUTER_BORDER, BOTTOM_BORDER,
                RED);

            addStaticLine(
                LEFT_OUTER_BORDER, TOP_BORDER,
                LEFT_INNER_BORDER, TOP_BORDER,
                RED );

            addStaticLine(
                LEFT_OUTER_BORDER, TOP_BORDER,
                LEFT_INNER_BORDER, TOP_BORDER,
                RED );

            addStaticLine(
                LEFT_INNER_BORDER, TOP_BORDER,
                LEFT_INNER_BORDER, BOTTOM_BORDER,
                RED );
            addStaticLine(
                    LEFT_OUTER_BORDER, TOP_BORDER + TASK_HEIGHT * 5,
                    LEFT_INNER_BORDER, TOP_BORDER + TASK_HEIGHT * 5,
                    RED );

            addStaticLine(
                    LEFT_OUTER_BORDER, TOP_BORDER + TASK_HEIGHT * 10,
                    LEFT_INNER_BORDER, TOP_BORDER + TASK_HEIGHT * 10,
                    RED );
            
            addStaticLine(
                    LEFT_OUTER_BORDER, TOP_BORDER + TASK_HEIGHT * 12,
                    LEFT_INNER_BORDER, TOP_BORDER + TASK_HEIGHT * 12,
                    RED );

            addStaticLine(
                LEFT_INNER_BORDER, BOTTOM_BORDER,
                RIGHT_OUTER_BORDER, BOTTOM_BORDER,
                DULL_GREEN );

            addStaticLine(
                RIGHT_OUTER_BORDER, TOP_BORDER,
                RIGHT_OUTER_BORDER, BOTTOM_BORDER,
                DULL_GREEN );

            addStaticLine(
                LEFT_INNER_BORDER, TOP_BORDER,
                RIGHT_OUTER_BORDER, TOP_BORDER,
                PISS);

        }
        private Line addStaticLine(double x1, double y1, double x2, double y2, SolidColorBrush color)
        {
            Line line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            line.Stroke = color;
            line.Margin = new Thickness(100);

            canvas.Children.Add(line);

            return line;
        }
        private void updateTaskPositions()
        {
            foreach(TaskDisplay taskDisplay in taskDisplays)
            {
                updateTaskPosition(taskDisplay);
            }
        }
        private void updateTaskPosition(TaskDisplay taskDisplay)
        {
            Rectangle rect = taskDisplay.rectangle;
            DateTime start = taskDisplay.startDate;
            DateTime end = taskDisplay.endDate;

            if(start <= viewEndDate && end >= viewStartDate)
            {
                if (start < viewStartDate) start = viewStartDate;
                if (end > viewEndDate) end = viewEndDate;

                rect.Width = dateToPixel(end) - dateToPixel(start);
                Canvas.SetLeft(rect, dateToPixel(start));
            }
            else
            {
                rect.Width = 0;
            }


        }
        private void initCompletedTask(TaskDisplay taskDisplay)
        {
            Rectangle rect = taskDisplay.rectangle;
            rect.Fill = COMPLETED_COLOR;
            rect.Opacity = 0.8;
            rect.Height = BLOCK_HEIGHT - 8;

            initTask(taskDisplay);

            Canvas.SetTop(rect, taskBlockTypeToPixel(taskDisplay.taskTypeId) + 4);
        }
        private void initPlannedTask(TaskDisplay taskDisplay)
        {
            Rectangle rect = taskDisplay.rectangle;
            rect.Fill = PLANNED_COLOR;
            rect.Height = BLOCK_HEIGHT;

            initTask(taskDisplay);

            Canvas.SetTop(rect, taskBlockTypeToPixel(taskDisplay.taskTypeId));
        }
        private void initTask(TaskDisplay taskDisplay)
        {
            Rectangle rect = taskDisplay.rectangle;
            DateTime start = taskDisplay.startDate;
            DateTime end = taskDisplay.endDate;

            double leftOffset = 0;

            if(start <= viewEndDate && end >= viewStartDate)
            {
                rect.Visibility = Visibility.Visible;

                if (start < viewStartDate) start = viewStartDate;
                if (end > viewEndDate) end = viewEndDate;

                rect.Width = dateToPixel(end) - dateToPixel(start);
                leftOffset = dateToPixel(start);
            }
            else
            {
                rect.Width = 0;
            }

            canvas.Children.Add(rect);
            taskDisplays.Add(taskDisplay);
            Canvas.SetLeft(rect, leftOffset);
        }
        public void loadModel(Model model)
        {
            canvas.Visibility = Visibility.Visible;
            modelStartDate = model.startDate;
            viewStartDate = model.startDate;
            viewEndDate = model.startDate.AddDays(DEFAULT_DAYS_IN_VIEW);

            modelNameLabel.Content = model.modelName;

            clearTaskBlocks();

            foreach(Task task in model.tasks)
            {
                addTaskBlocks(task);
            }

            drawDynamicLinesAndDates();
        }
        public void reloadModel(Model model)
        {
            clearTaskBlocks();
            foreach (Task task in model.tasks)
                addTaskBlocks(task);
        }
        public void addCompletedTask(Task task)
        {
            if(task.completed)
            {
                TaskDisplay taskDisplay = new TaskDisplay(task.startDate.Value, task.endDate.Value, task.typeInd);
                initCompletedTask(taskDisplay);
            }
        }
        private void clearTaskBlocks()
        {
            foreach(TaskDisplay taskDisplay in taskDisplays)
            {
                canvas.Children.Remove(taskDisplay.rectangle);
            }
        }
        private void addTaskBlocks(Task task)
        {
            TaskDisplay plannedTaskDisplay = new TaskDisplay(task.plannedStartDate, task.plannedEndDate, task.typeInd);
            initPlannedTask(plannedTaskDisplay);

            if(task.completed)
            {
                TaskDisplay completedTaskDisplay = new TaskDisplay(task.startDate.Value, task.endDate.Value, task.typeInd);
                initCompletedTask(completedTaskDisplay);
            }
        }
        private void drawDynamicLinesAndDates()
        {
            clearDynamicLinesAndDates();
            drawDyanimcLines();
            drawDates();
        }
        private void drawDates()
        {
            for(DateTime date = cielDate(viewStartDate); date <= viewEndDate; date = date.AddDays(1))
            {
                if(isWeekFromModelStart(date))
                {
                    DateDisplay dateDisplay = new DateDisplay(date);
                    dateDisplay.label.Content = date.ToString("MM/dd/y");
                    dateDisplay.label.Foreground = GREEN;
                    dateDisplay.label.FontSize = 14;
                    dateDisplay.label.Height = 25;
                    dateDisplay.label.Width = 70;
                    dateDisplay.label.LayoutTransform = new RotateTransform(DATE_ROTATION);

                    dateDisplays.Add(dateDisplay);
                    canvas.Children.Add(dateDisplay.label);

                    double width_i = dateDisplay.label.Height * Math.Sin(degToRad(-DATE_ROTATION));
                    double leftOffset = dateToPixel(date) - width_i;

                    Canvas.SetLeft(dateDisplay.label, leftOffset);
                    Canvas.SetTop(dateDisplay.label, DATE_TOP_OFFSET);
                }
            }

        }
        private void drawDyanimcLines()
        {
            for(DateTime date = cielDate(viewStartDate); date < viewEndDate; date = date.AddDays(1))
            {
                if (date == viewStartDate) continue;

                double x = dateToPixel(date);
                LineDisplay lineDisplay = new LineDisplay(date);
                lineDisplay.line.X1 = x;
                lineDisplay.line.Y1 = TOP_BORDER + 100;
                lineDisplay.line.X2 = x;

                if(isWeekFromModelStart(date))
                {
                    lineDisplay.line.Y2 = BOTTOM_BORDER + 100;
                    lineDisplay.line.Stroke = GRAY;
                }
                else
                {
                    lineDisplay.line.Y2 = TOP_BORDER + DAYLINE_LENGTH + 100;
                    lineDisplay.line.Stroke = PISS;
                }

                lineDisplays.Add(lineDisplay);
                canvas.Children.Add(lineDisplay.line);
            }

        }
        private void clearDynamicLinesAndDates()
        {
            foreach(LineDisplay lineDisplay in lineDisplays)
            {
                canvas.Children.Remove(lineDisplay.line);
            }

            foreach(DateDisplay dateDisplay in dateDisplays)
            {
                canvas.Children.Remove(dateDisplay.label);
            }
        }
        private void rerenderCanvas()
        {
            if(data.currentModel != null)
            {
                drawDynamicLinesAndDates();
                updateTaskPositions();
            }
        }
        public void clearCanvas()
        {
            clearTaskBlocks();
            clearDynamicLinesAndDates();
            modelNameLabel.Content = "";
            canvas.Visibility = Visibility.Hidden;
        }
        public void resetPosition()
        {
            viewStartDate = modelStartDate;
            viewEndDate = viewStartDate.AddDays(DEFAULT_DAYS_IN_VIEW);
            rerenderCanvas();
        }
        public void addDays(double n)
        {
            double numDays = (viewEndDate - viewStartDate).TotalDays;
            if (numDays + n <= 365 && numDays + n >= 1)
            {
                viewEndDate = viewEndDate.AddDays(n);
                rerenderCanvas();
            }
        }

        private Point referencePoint;
        private DateTime referenceStartDate;
        private DateTime referenceEndDate;
        Boolean mouseCaptured = false;
        private void canvasPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            referencePoint = e.GetPosition(canvas);
            referenceStartDate = viewStartDate;
            referenceEndDate = viewEndDate;
            mouseCaptured = true;
        }
        private void canvasPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(mouseCaptured && data.currentModel != null)
            {
                Point position = e.GetPosition(sender as IInputElement);
                double daysPerPixel = (viewEndDate - viewStartDate).TotalDays / GRAPH_WIDTH;
                double dayOffset = (referencePoint.X - position.X) * daysPerPixel;
                viewStartDate = referenceStartDate.AddDays(dayOffset);
                viewEndDate = referenceEndDate.AddDays(dayOffset);


                rerenderCanvas();
            }    
        }
        private void canvasPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseCaptured = false;
        }
        private void canvasMouseLeave(object sender, MouseEventArgs e)
        {
            mouseCaptured = false;
        }
        private void canvasMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                addDays(1);
            }
            else
            {
                addDays(-1);
            }
        }
    }
}
