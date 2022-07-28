using Gaant_Chart.Display;
using Gaant_Chart.Models;
using System;
using System.Collections.Generic;
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

        private double DATE_ROTATION = 285; // 285
        private double GROUPLABEL_ROTATION = 270;

        private double DEFAULT_DAYS_IN_VIEW = 28;
        private double N_TASK_TYPES = data.allTasks.Length;

        private SolidColorBrush RED = new SolidColorBrush(Colors.Red);
        private SolidColorBrush PINK = new SolidColorBrush(Color.FromRgb(252, 180, 180));
        private SolidColorBrush DULL_GREEN = new SolidColorBrush(Color.FromRgb(50, 236, 80));
        private SolidColorBrush PISS = new SolidColorBrush(Color.FromRgb(242, 218, 46));
        private SolidColorBrush GRAY = new SolidColorBrush(Color.FromRgb(233, 233, 233));
        protected SolidColorBrush WHITE = new SolidColorBrush(Colors.White);
        protected SolidColorBrush GREEN = new SolidColorBrush(Color.FromRgb(50, 236, 0));
        protected SolidColorBrush COMPLETED_COLOR = new SolidColorBrush(Color.FromRgb(149, 219, 139));
        protected SolidColorBrush PLANNED_COLOR = new SolidColorBrush(Color.FromRgb(254, 212, 158));

        private List<TaskDisplay> completedTaskDisplays = new List<TaskDisplay>();
        private List<TaskDisplay> plannedTaskDisplays = new List<TaskDisplay>();

        private List<DateDisplay> dateDisplays = new List<DateDisplay>();
        private List<LineDisplay> lineDisplays = new List<LineDisplay>();
        private List<Line> staticLines = new List<Line>();
        private List<Label> taskLabels = new List<Label>();
        private List<TextBlock> taskGroupLabels = new List<TextBlock>();

        private Label modelNameLabel;
        private Rectangle hittableRect;

        public static Func<DateTime, DateTime> floorDate;
        private Func<DateTime, Boolean> isWeekFromModelStart;
        private Func<double, double> degToRad;
        private Func<double, double> toRawLineDim;
        private (
            Func<double> outerLeft, Func<double> innerLeft, 
            Func<double> right, Func<double> top, 
            Func<double> tick, Func<double> firstRed,
            Func<double> secondRed, Func<double> thirdRed,
            Func<double> bottom) borders { get; set; }

        private (Func<double> width, Func<double> height, Func<int, double> top, Func<double> font) taskLabelSize;
        private (Func<double, double> width, Func<double> height, Func<int, double, double> top,  Func<double> font) taskGroupLabelSize;
        private (Func<double> height, Func<int, double> top) completedTaskSize;
        private (Func<double> height, Func<int, double> top) plannedTaskSize;
        private (Func<double> width, Func<double> height, Func<double> top, Func<double> font) dateSize;
        public CanvasGraph()
        {
            canvas = new Canvas();
            canvas.Visibility = Visibility.Hidden;
            addCanvasToApp();
            addCanvasEventHandlers();
            initMapFunctions();
            initModelLabel();
        }
        private void addCanvasToApp()
        {
            MainWindow.grid.Children.Add(canvas);

            Grid.SetColumn(canvas, 5);
            Grid.SetRow(canvas, 1);
            Grid.SetRowSpan(canvas, 15);
            canvas.Margin = new Thickness(10, 65, 10, 25);
        }
        private void initMapFunctions()
        {
            borders = (
                outerLeft: () => canvas.ActualWidth * 0.025,
                innerLeft: () => canvas.ActualWidth * 0.3,
                right: () => canvas.ActualWidth * 0.96,
                top: () => canvas.ActualHeight * 0.2,
                tick: () => canvas.ActualHeight * 0.21,
                firstRed: () => borders.tick() + 5 * (borders.bottom() - borders.tick()) / N_TASK_TYPES,
                secondRed: () => borders.tick() + 10 * (borders.bottom() - borders.tick()) / N_TASK_TYPES,
                thirdRed: () => borders.tick() + 12 * (borders.bottom() - borders.tick()) / N_TASK_TYPES,
                bottom: () => canvas.ActualHeight * 0.98
            );
            taskLabelSize = (
                width: () => borders.innerLeft() - borders.outerLeft(),
                height: () => (borders.bottom() - borders.tick()) / N_TASK_TYPES,
                top: typeId => borders.tick() + typeId * (borders.bottom() - borders.top()) / N_TASK_TYPES,
                font: () => Math.Min(taskLabelSize.height() * 0.375, taskLabelSize.width() * 0.06)
                );
            taskGroupLabelSize = (
                width: length => Math.Min(length, 3.5) * taskLabelSize.height(),
                height: () => taskLabelSize.width() * 0.5,
                top: (index, length) => borders.tick() + index * taskLabelSize.height() + (length - Math.Min(length, 3.5)) * taskLabelSize.height() * 0.5,
                font: () => taskLabelSize.font() * 0.9
                );
            plannedTaskSize = (
                height: () => (borders.bottom() - borders.tick()) / N_TASK_TYPES,
                top: typeId => borders.tick() + typeId * plannedTaskSize.height()
                );
            completedTaskSize = (
                height: () => plannedTaskSize.height() * 0.85,
                top: typeId => plannedTaskSize.top(typeId) + plannedTaskSize.height() * 0.075
                );
            dateSize = (
                width: () => canvas.ActualHeight * 0.175,
                height: () => canvas.ActualWidth * 0.045,
                top: () => borders.top() - dateSize.height() * Math.Cos(degToRad(-DATE_ROTATION)) - dateSize.width() * Math.Sin(degToRad(-DATE_ROTATION)),
                font: () => taskLabelSize.font() * 1.2
                );

            floorDate = date => date.AddMinutes(-date.TimeOfDay.TotalMinutes);
            isWeekFromModelStart = date => (date - cielDate(modelStartDate)).TotalDays % 7 == 0;
            degToRad = deg => deg * Math.PI / 180;
            toRawLineDim = val => val - 100;
        }
        private double dateToPixel(DateTime date)
        {
            if (date < viewStartDate || date > viewEndDate) throw new Exception("date out of range");
            double pixelsPerDay = (borders.right() - borders.innerLeft()) / (viewEndDate - viewStartDate).TotalDays;
            return borders.innerLeft() + (date - viewStartDate).TotalDays * pixelsPerDay;
        }
        public void load()
        {
            drawStaticLines();
        }
        private DateTime cielDate(DateTime date)
        {
            if(date.TimeOfDay.TotalMinutes == 0)
            {
                return date;
            }
            else
            {
                double timeOfDay = date.TimeOfDay.TotalMinutes;
                return date.AddMinutes(1440 - timeOfDay);
            }
        }
        private void initModelLabel()
        {
            modelNameLabel = new Label();
            modelNameLabel.FontSize = 25;
            modelNameLabel.Foreground = GREEN;
            modelNameLabel.Width = 200;
            modelNameLabel.Height = 50;
            canvas.Children.Add(modelNameLabel);

            Canvas.SetLeft(modelNameLabel, 30);
            Canvas.SetTop(modelNameLabel, 30);
        }
        private void initTaskLabels()
        {
            for(int i = 0; i < data.allTasks.Length; i++) 
                initTaskLabel(data.allTasks[i], i);
        }
        private void initTaskLabel(String taskName, int taskTypeId)
        {
            Label label = new Label();
            label.Content = taskName;

            label.FontSize = taskLabelSize.font();
            label.Foreground = WHITE;
            label.Width = taskLabelSize.width();
            label.Height = taskLabelSize.height();
            label.HorizontalContentAlignment = HorizontalAlignment.Right;

            canvas.Children.Add(label);
            taskLabels.Add(label);

            Canvas.SetTop(label, taskLabelSize.top(taskTypeId));
            Canvas.SetLeft(label, borders.outerLeft());
        }
        private void updateTaskLabels()
        {
            if(taskLabels.Count == 0)
            {
                initTaskLabels();
            }

            int i = 0;
            foreach(Label label in taskLabels)
            {
                updateTaskLabel(label, i);
                i++;
            }
        }
        private void updateTaskLabel(Label label, int taskTypeId)
        {
            label.FontSize = taskLabelSize.font();
            label.Width = taskLabelSize.width();
            label.Height = taskLabelSize.height();

            Canvas.SetLeft(label, borders.outerLeft());
            Canvas.SetTop(label, taskLabelSize.top(taskTypeId));
        }
        private void initGroupLabels()
        {
            for(int i = 0; i < 4; i++)
            {
                (String name, int index, int length) = data.taskLabelGroupPosition[i];
                TextBlock textBlock = new TextBlock();
                textBlock.Text = name;
                textBlock.Foreground = WHITE;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                textBlock.LayoutTransform = new RotateTransform(GROUPLABEL_ROTATION);

                textBlock.Width = taskGroupLabelSize.width(length);
                textBlock.Height = taskGroupLabelSize.height();
                textBlock.FontSize = taskGroupLabelSize.font();

                canvas.Children.Add(textBlock);
                taskGroupLabels.Add(textBlock);

                Canvas.SetLeft(textBlock, borders.outerLeft());
                Canvas.SetTop(textBlock, taskGroupLabelSize.top(index, length));
            }
        }
        private void updateGroupLabels()
        {
            if(taskGroupLabels.Count <= 0)
            {
                initGroupLabels();
                return;
            }

            for(int i = 0; i < 4; i++)
            {
                TextBlock textBlock = taskGroupLabels[i];
                (String name, int index, int length) = data.taskLabelGroupPosition[i];

                textBlock.Width = taskGroupLabelSize.width(length);
                textBlock.Height = taskGroupLabelSize.height();
                textBlock.FontSize = taskGroupLabelSize.font();

                Canvas.SetLeft(textBlock, borders.outerLeft());
                Canvas.SetTop(textBlock, taskGroupLabelSize.top(index, length));
            }
        }
        private void addCanvasEventHandlers()
        {
            canvas.PreviewMouseUp += new MouseButtonEventHandler(canvasPreviewMouseUp);
            canvas.PreviewMouseMove += new MouseEventHandler(canvasPreviewMouseMove);
            canvas.PreviewMouseDown += new MouseButtonEventHandler(canvasPreviewMouseDown);
            canvas.PreviewMouseWheel += new MouseWheelEventHandler(canvasMouseWheel);
            canvas.MouseLeave += new MouseEventHandler(canvasMouseLeave);
            canvas.SizeChanged += new SizeChangedEventHandler(canvasSizeChanged);
        }
        private void initHittableRect()
        {
            hittableRect = new Rectangle();
            hittableRect.Fill = new SolidColorBrush(Colors.Black);
            hittableRect.Opacity = 0.05;
            hittableRect.Width = canvas.ActualWidth;
            hittableRect.Height = canvas.ActualHeight;

            canvas.Children.Add(hittableRect);

            Canvas.SetLeft(hittableRect, 0);
            Canvas.SetTop(hittableRect, 0);
        }
        private void updateHittableRect()
        {
            if (hittableRect == null)
                initHittableRect();

            hittableRect.Width = canvas.ActualWidth;
            hittableRect.Height = canvas.ActualHeight;
        }
        private void drawStaticLines()
        {
            clearStaticLines();

            addStaticLine(
                toRawLineDim(borders.outerLeft()), toRawLineDim(borders.bottom()),
                toRawLineDim(borders.innerLeft()), toRawLineDim(borders.bottom()),
                RED);
            addStaticLine(
                toRawLineDim(borders.outerLeft()), toRawLineDim(borders.top()),
                toRawLineDim(borders.outerLeft()), toRawLineDim(borders.bottom()),
                RED);

            addStaticLine(
                toRawLineDim(borders.outerLeft()), toRawLineDim(borders.top()),
                toRawLineDim(borders.innerLeft()), toRawLineDim(borders.top()),
                RED );

            addStaticLine(
                toRawLineDim(borders.outerLeft()), toRawLineDim(borders.top()),
                toRawLineDim(borders.innerLeft()), toRawLineDim(borders.top()),
                RED );

            addStaticLine(
                toRawLineDim(borders.innerLeft()), toRawLineDim(borders.top()),
                toRawLineDim(borders.innerLeft()), toRawLineDim(borders.bottom()),
                RED );
            addStaticLine(
                    toRawLineDim(borders.outerLeft()), toRawLineDim(borders.firstRed()),
                    toRawLineDim(borders.innerLeft()), toRawLineDim(borders.firstRed()),
                    RED );

            addStaticLine(
                    toRawLineDim(borders.outerLeft()), toRawLineDim(borders.secondRed()),
                    toRawLineDim(borders.innerLeft()), toRawLineDim(borders.secondRed()),
                    RED );
            
            addStaticLine(
                    toRawLineDim(borders.outerLeft()), toRawLineDim(borders.thirdRed()),
                    toRawLineDim(borders.innerLeft()), toRawLineDim(borders.thirdRed()),
                    RED );

            addStaticLine(
                toRawLineDim(borders.innerLeft()), toRawLineDim(borders.bottom()),
                toRawLineDim(borders.right()), toRawLineDim(borders.bottom()),
                DULL_GREEN );

            addStaticLine(
                toRawLineDim(borders.right()), toRawLineDim(borders.top()),
                toRawLineDim(borders.right()), toRawLineDim(borders.bottom()),
                DULL_GREEN );

            addStaticLine(
                toRawLineDim(borders.innerLeft()), toRawLineDim(borders.top()),
                toRawLineDim(borders.right()), toRawLineDim(borders.top()),
                PISS);
        }
        private void clearStaticLines()
        {
            foreach(Line line in staticLines)
            {
                canvas.Children.Remove(line);
            }
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
            staticLines.Add(line);

            return line;
        }
        private void updateTaskPositions()
        {
            foreach(TaskDisplay taskDisplay in completedTaskDisplays)
            {
                updateCompletedTask(taskDisplay);
            }

            foreach(TaskDisplay taskDisplay in plannedTaskDisplays)
            {
                updatePlannedTask(taskDisplay);
            }
        }
        private void updatePlannedTask(TaskDisplay taskDisplay)
        {
            Rectangle rect = taskDisplay.rectangle;
            rect.Height = plannedTaskSize.height();
            Canvas.SetTop(rect, plannedTaskSize.top(taskDisplay.taskTypeId));
            updateTask(taskDisplay);
        }
        private void updateCompletedTask(TaskDisplay taskDisplay)
        {
            Rectangle rect = taskDisplay.rectangle;
            rect.Height = completedTaskSize.height();
            Canvas.SetTop(rect, completedTaskSize.top(taskDisplay.taskTypeId));
            updateTask(taskDisplay);
        }
        private void updateTask(TaskDisplay taskDisplay)
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
            rect.Height = completedTaskSize.height();

            canvas.Children.Add(rect);
            completedTaskDisplays.Add(taskDisplay);

            initTask(taskDisplay);

            Canvas.SetTop(rect, completedTaskSize.top(taskDisplay.taskTypeId));
        }
        private void initPlannedTask(TaskDisplay taskDisplay)
        {
            Rectangle rect = taskDisplay.rectangle;
            rect.Fill = PLANNED_COLOR;
            rect.Height = plannedTaskSize.height();

            canvas.Children.Add(rect);
            plannedTaskDisplays.Add(taskDisplay);

            initTask(taskDisplay);

            Canvas.SetTop(rect, plannedTaskSize.top(taskDisplay.taskTypeId));
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
            foreach(TaskDisplay taskDisplay in completedTaskDisplays)
            {
                canvas.Children.Remove(taskDisplay.rectangle);
            }
            foreach(TaskDisplay taskDisplay in plannedTaskDisplays)
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
                    dateDisplay.label.FontSize = dateSize.font();
                    dateDisplay.label.Height = dateSize.height();
                    dateDisplay.label.Width = dateSize.width();
                    dateDisplay.label.LayoutTransform = new RotateTransform(DATE_ROTATION);
                    dateDisplay.label.VerticalContentAlignment = VerticalAlignment.Center;

                    dateDisplays.Add(dateDisplay);
                    canvas.Children.Add(dateDisplay.label);

                    double width_i = dateDisplay.label.Height * Math.Sin(degToRad(-DATE_ROTATION)) * 0.5;
                    double leftOffset = dateToPixel(date) - width_i;

                    Canvas.SetLeft(dateDisplay.label, leftOffset);
                    Canvas.SetTop(dateDisplay.label, dateSize.top());
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
                lineDisplay.line.Y1 = borders.top();
                lineDisplay.line.X2 = x;

                if(isWeekFromModelStart(date))
                {
                    lineDisplay.line.Y2 = borders.bottom();
                    lineDisplay.line.Stroke = GRAY;
                }
                else
                {
                    lineDisplay.line.Y2 = borders.tick();
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
        public void clearModel()
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
                double daysPerPixel = (viewEndDate - viewStartDate).TotalDays / (borders.right() - borders.innerLeft());
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
        private void canvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            updateHittableRect();
            updateTaskLabels();
            updateGroupLabels();
            drawStaticLines();
            drawDynamicLinesAndDates();
            updateTaskPositions();
        }
    }
}
