using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using Gaant_Chart.Models;

namespace Gaant_Chart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    // All DB calls will go through MainWindow

    public partial class MainWindow : Window
    {

        public static DbConnection myDatabase { get; set; }

        public static EventHandler LoadExistingModelEvent { get; set; }
        private static Dictionary<int, (CheckBox, TextBox)> taskComponents { get; set; }

        private static double heightPerTask { get; set; }

        private const int DEFAULT_DAYS_IN_VIEW = 28;

        private List<CheckBox> taskBarCheckBoxes { get; set; }
        private List<TextBox> taskBarTextBoxes { get; set; }

        private CanvasView view { get; set; }

        private  ModelDisplay modelDisplay { get; set; }
        private CanvasDisplay canvasDisplay { get; set; }

        private Boolean renderedChecks = true;

        // double WIDTH = 850;
        // double HEIGHT = 592;

        /*
        int topOffset = 120;

        int dateX = 210;
        int verticalLabelSpace = 32;

        int labelLeftMargin = 20;

        int leftOuterBorder = -80;
        int leftInnerBorder = 125;
        int rightOuterBorder = 713;
        int topBorder = 15;
        int bottomBorder = 465;
        */

        public MainWindow()
        {
            InitializeComponent();

            // create database connection
            myDatabase = new DbConnection();
            view = new CanvasView(DateTime.Now, DEFAULT_DAYS_IN_VIEW);

            // set up the canvas

            createTaskBar();

            initCanvas();


            
            myCanvas.Visibility = Visibility.Visible;

            initTaskBarLists();

            setTaskComponentsReadOnly();

        }

        private void displayCurrentModel()
        {

            Model model = data.currentModel;
            view = new CanvasView(data.currentModel, 28);

            myCanvas.Visibility = Visibility.Visible;

            updateCanvas();

            label_ModelID.Content = model.modelName;
            txtDisplayModelName.Text = data.currentModel.modelName;

            initTaskBlocks();
            initTaskBarWithModel();

        }

        private void displayCurrentUser()
        {
            userLabel.Content = "Hello " + data.currentUser.name;

        }

        private void setTaskComponentsReadOnly()
        {
            foreach(CheckBox checkBox in taskBarCheckBoxes)
            {
                checkBox.IsHitTestVisible = false;
            }

            foreach(TextBox textBox in taskBarTextBoxes)
            {
                textBox.IsReadOnly = true;
            }
        }


        private void createTaskBar()
        {
            foreach(String taskname in data.allTasks)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = taskname;
                checkbox.Checked += new RoutedEventHandler(CheckBox_Checked);

                TextBox textbox = new TextBox();
                textbox.Width = 100;
                textbox.Margin = new Thickness(0, 0, 5, 0);
                textbox.HorizontalAlignment = HorizontalAlignment.Right;
                textbox.Background = new SolidColorBrush(Colors.LightGray);

                DockPanel dockpanel = new DockPanel();
                dockpanel.Background = new SolidColorBrush(Color.FromRgb(237, 241, 86));
                dockpanel.Margin = new Thickness(5, 2.5, 5, 2.5);

                dockpanel.Children.Add(checkbox);
                dockpanel.Children.Add(textbox);

                taskBarStackPanel.Children.Add(dockpanel);
            }

            foreach((String groupname, int index) in data.taskLabelGroups)
            {
                Label label = new Label();
                label.Content = groupname;

                if(groupname == "Assemblies")
                {
                    label.FontWeight = FontWeights.Bold;
                }

                taskBarStackPanel.Children.Insert(index, label);
            }
            
        }



        // NOTE: The top left corner for lines is (-100, -100), Components start at (0, 0)

        private void initCanvas()
        {
            Canvas.SetLeft(label_ModelID, 25);
            Canvas.SetTop(label_ModelID, 50);

            canvasDisplay = new CanvasDisplay(view);

            foreach(CanvasElement canvasElement in canvasDisplay.canvasTexts)
            {
                addFrameworkElementToCanvas(canvasElement);
            }

            foreach(CanvasLine canvasLine in canvasDisplay.staticLines)
            {
                myCanvas.Children.Add(canvasLine.line);
            }

            renderDynamicElements();
        }

        private void renderDynamicElements()
        {
            foreach(CanvasElement canvasElement in canvasDisplay.dates)
            {
                addFrameworkElementToCanvas(canvasElement);
            }

            foreach(CanvasLine dynamicLine in canvasDisplay.dynamicLines)
            {
                myCanvas.Children.Add(dynamicLine.line);
            }
        }

        private void addFrameworkElementToCanvas(CanvasElement canvasElement)
        {
            FrameworkElement element = canvasElement.element;
            myCanvas.Children.Add(element);
            Canvas.SetLeft(element, canvasElement.leftoffset);
            Canvas.SetTop(element, canvasElement.topoffset);
        }

        private void updateCanvas()
        {
            removeDynamicElements();
            canvasDisplay.resize(view);
            renderDynamicElements();
        }

        private void removeDynamicElements()
        {
            foreach(CanvasLine canvasLine in canvasDisplay.dynamicLines)
            {
                myCanvas.Children.Remove(canvasLine.line);
            }

            foreach(CanvasElement canvasElement in canvasDisplay.dates)
            {
                myCanvas.Children.Remove(canvasElement.element);
            }
        }

        /*
        private void drawInitialCanvas1()
        {
            heightPerTask = (bottomBorder + 5 - topBorder) / 14.0;

            Canvas.SetLeft(l1, labelLeftMargin);
            Canvas.SetLeft(l2, labelLeftMargin);
            Canvas.SetLeft(l3, labelLeftMargin);
            Canvas.SetLeft(l4, labelLeftMargin);
            Canvas.SetLeft(l5, labelLeftMargin);
            Canvas.SetLeft(l6, labelLeftMargin);
            Canvas.SetLeft(l7, labelLeftMargin);
            Canvas.SetLeft(l8, labelLeftMargin);
            Canvas.SetLeft(l9, labelLeftMargin);
            Canvas.SetLeft(l10, labelLeftMargin);
            Canvas.SetLeft(l11, labelLeftMargin);
            Canvas.SetLeft(l12, labelLeftMargin);
            Canvas.SetLeft(l13, labelLeftMargin);
            Canvas.SetLeft(l14, labelLeftMargin);

            Canvas.SetTop(l1, topOffset + verticalLabelSpace * 0);
            Canvas.SetTop(l2, topOffset + verticalLabelSpace * 1);
            Canvas.SetTop(l3, topOffset + verticalLabelSpace * 2);
            Canvas.SetTop(l4, topOffset + verticalLabelSpace * 3);
            Canvas.SetTop(l5, topOffset + verticalLabelSpace * 4);
            Canvas.SetTop(l6, topOffset + verticalLabelSpace * 5);
            Canvas.SetTop(l7, topOffset + verticalLabelSpace * 6);
            Canvas.SetTop(l8, topOffset + verticalLabelSpace * 7);
            Canvas.SetTop(l9, topOffset + verticalLabelSpace * 8);
            Canvas.SetTop(l10, topOffset + verticalLabelSpace * 9);
            Canvas.SetTop(l11, topOffset + verticalLabelSpace * 10);
            Canvas.SetTop(l12, topOffset + verticalLabelSpace * 11);
            Canvas.SetTop(l13, topOffset + verticalLabelSpace * 12);
            Canvas.SetTop(l14, topOffset + verticalLabelSpace * 13);

            Canvas.SetLeft(L1, labelLeftMargin + 15);
            Canvas.SetLeft(L2, labelLeftMargin + 10);
            Canvas.SetLeft(L3, labelLeftMargin + 10);
            Canvas.SetLeft(L4, labelLeftMargin);

            Canvas.SetTop(L1, topOffset + verticalLabelSpace * 1.5);
            Canvas.SetTop(L2, topOffset + verticalLabelSpace * 6);
            Canvas.SetTop(L3, topOffset + verticalLabelSpace * 10.15);
            Canvas.SetTop(L4, topOffset + verticalLabelSpace * 12.05);


            // Draw a boarder around the data
            double segmentationBottomBorder = topBorder + heightPerTask * 5;
            double reviewBottomBorder = segmentationBottomBorder + heightPerTask * 5;
            double meshBottomBorder = reviewBottomBorder + heightPerTask * 2 - 5;

            drawLine(new Point(leftOuterBorder, segmentationBottomBorder),
                     new Point(leftInnerBorder, segmentationBottomBorder),
                     Color.FromRgb(255, 0, 0));

            drawLine(new Point(leftInnerBorder, segmentationBottomBorder),
                     new Point(rightOuterBorder, segmentationBottomBorder),
                     Color.FromRgb(252, 180, 180));

            drawLine(new Point(leftOuterBorder, reviewBottomBorder),
                     new Point(leftInnerBorder, reviewBottomBorder),
                     Color.FromRgb(255, 0, 0));
            drawLine(new Point(leftInnerBorder, reviewBottomBorder),
                     new Point(rightOuterBorder, reviewBottomBorder),
                     Color.FromRgb(252, 180, 180));

            drawLine(new Point(leftOuterBorder, meshBottomBorder),
                     new Point(leftInnerBorder, meshBottomBorder),
                     Color.FromRgb(255, 0, 0));
            drawLine(new Point(leftInnerBorder, meshBottomBorder),
                     new Point(rightOuterBorder, meshBottomBorder),
                     Color.FromRgb(252, 180, 180));


            newLine(leftOuterBorder, leftInnerBorder, bottomBorder, bottomBorder, "#FF0000");
            newLine(leftOuterBorder, leftOuterBorder, topBorder, bottomBorder, "#FF0000");
            newLine(leftOuterBorder, leftInnerBorder, topBorder, topBorder, "#FF0000");
            newLine(leftInnerBorder, leftInnerBorder, topBorder, bottomBorder, "#FF0000");

            newLine(leftInnerBorder, rightOuterBorder, bottomBorder, bottomBorder, "#32EC00");
            newLine(rightOuterBorder, rightOuterBorder, topBorder, bottomBorder, "#32EC00");

            L1.LayoutTransform = new RotateTransform(270);
            L2.LayoutTransform = new RotateTransform(270);
            L3.LayoutTransform = new RotateTransform(270);
            L4.LayoutTransform = new RotateTransform(270);


            newLine(leftInnerBorder, rightOuterBorder, topBorder, topBorder, "#F2DA2E");

            for (int i = leftInnerBorder; i < rightOuterBorder; i += 147)
            {
                if (i != leftInnerBorder) newLine(i, i, topBorder, bottomBorder, "#DFDFDF");

                for (int j = i; j < i + 147; j += 21)
                {
                    if (j != i) newLine(j, j, topBorder, topBorder + 5, "#F2DA2E");
                }
            }

            int dateSpacing = 147;
            int dateHeight = 40;
            int dateRotation = 285;

            Canvas.SetLeft(date1, dateX + dateSpacing * 0);
            Canvas.SetLeft(date2, dateX + dateSpacing * 1);
            Canvas.SetLeft(date3, dateX + dateSpacing * 2);
            Canvas.SetLeft(date4, dateX + dateSpacing * 3);
            Canvas.SetLeft(date5, dateX + dateSpacing * 4);

            Canvas.SetTop(date1, dateHeight);
            Canvas.SetTop(date2, dateHeight);
            Canvas.SetTop(date3, dateHeight);
            Canvas.SetTop(date4, dateHeight);
            Canvas.SetTop(date5, dateHeight);

            date1.LayoutTransform = new RotateTransform(dateRotation);
            date2.LayoutTransform = new RotateTransform(dateRotation);
            date3.LayoutTransform = new RotateTransform(dateRotation);
            date4.LayoutTransform = new RotateTransform(dateRotation);
            date5.LayoutTransform = new RotateTransform(dateRotation);

            Canvas.SetLeft(label_ModelID, 25);
            Canvas.SetTop(label_ModelID, 50);

        }
        */

        private void initTaskBlocks()
        {
            Model model = data.currentModel;

            if (model == null) return;

            if(modelDisplay != null)
            {
                removeAllTaskBlocks();
            }

            modelDisplay = new ModelDisplay(model, view);
            
            view.changeStartDate(model.startDate);

            foreach(TaskDisplay taskDisplay in modelDisplay.plannedBlocks)
            {
                addRectToCanvas(taskDisplay);
            }

            foreach(TaskDisplay taskDisplay in modelDisplay.completedBlocks)
            {
                addRectToCanvas(taskDisplay);
            }
        }

        private void removeAllTaskBlocks()
        {
            foreach(TaskDisplay taskDisplay in modelDisplay.plannedBlocks)
            {
                removeRectFromCanvas(taskDisplay);
            }

            foreach(TaskDisplay taskDisplay in modelDisplay.completedBlocks)
            {
                removeRectFromCanvas(taskDisplay);
            }

        }

        private void removeRectFromCanvas(TaskDisplay taskDisplay)
        {
            Rectangle rect = taskDisplay.rectangle;
            myCanvas.Children.Remove(rect);
        }

        private void addRectToCanvas(TaskDisplay taskDisplay)
        {
            Rectangle rect = taskDisplay.rectangle;
            myCanvas.Children.Add(rect);
            Canvas.SetLeft(rect, taskDisplay.leftOffset);
            Canvas.SetTop(rect, taskDisplay.topOffset);
        }

        private void initTaskBarLists()
        {
            taskBarTextBoxes = new List<TextBox>();
            taskBarCheckBoxes = new List<CheckBox>();

            int i = 0;

            foreach(Object outerObj in taskBarStackPanel.Children)
            {
                if(outerObj.GetType() != typeof(DockPanel)) continue;

                DockPanel dockPanel = outerObj as DockPanel;

                foreach(Object innerObj in dockPanel.Children)
                {
                    if(innerObj.GetType() == typeof(CheckBox))
                    {
                        CheckBox checkBox = innerObj as CheckBox;
                        checkBox.Tag = i; 
                        taskBarCheckBoxes.Add(checkBox);
                    }
                    else if(innerObj.GetType() == typeof(TextBox))
                    {
                        TextBox textBox = innerObj as TextBox;
                        taskBarTextBoxes.Add(textBox);
                    }
                }
                i++;
            }
        }
        
        private void initTaskBarWithModel()
        {
            resetTaskBar();
            setTaskBar();
            setNextCheckbox();
        }

        private void setNextCheckbox()
        {
            SolidColorBrush completedColor = new SolidColorBrush(Colors.Black);
            Model model = data.currentModel;
            CheckBox nextCheckbox = taskBarCheckBoxes[model.lastCompletedTaskId + 1];
            nextCheckbox.Foreground = completedColor;
            nextCheckbox.IsHitTestVisible = true;
        }

        private void resetTaskBar()
        {
            SolidColorBrush unavalibleColor = new SolidColorBrush(Color.FromRgb(194, 194, 194));

            for(int i = 0; i < data.allTasks.Length; i++)
            {
                CheckBox checkbox = taskBarCheckBoxes[i];
                TextBox textbox = taskBarTextBoxes[i];

                checkbox.Foreground = unavalibleColor;
                checkbox.IsChecked = false;
                checkbox.IsHitTestVisible = false;

                textbox.Text = "";
            }
        }

        private void setTaskBar()
        {
            SolidColorBrush completedColor = new SolidColorBrush(Colors.Black);
            Model model = data.currentModel;

            renderedChecks = false;

            for(int i = 0; i < data.allTasks.Length; i++)
            {
                CheckBox checkbox = taskBarCheckBoxes[i];
                TextBox textbox = taskBarTextBoxes[i];
                Models.Task task = model.tasks[i];

                if(task.completed)
                {
                    checkbox.IsChecked = true;
                    checkbox.IsHitTestVisible = true;
                    checkbox.Foreground = completedColor;

                    textbox.Text = task.user.name + " | " + task.endDate.ToString("d/M/y");
                }
            }

            renderedChecks = true;


        }



        /*
        private void setDate(DateTime date)
        {
            date1.Content = date.ToString("MM-dd-yy");
            date2.Content = date.AddDays(7).ToString("MM-dd-yy");
            date3.Content = date.AddDays(14).ToString("MM-dd-yy");
            date4.Content = date.AddDays(21).ToString("MM-dd-yy");
            date5.Content = date.AddDays(28).ToString("MM-dd-yy");
        }

        */

        private void adjustLabels(Model model)
        {
            label_ModelID.Content = model.modelName;
        }


        private void newLine(int X1, int X2, int Y1, int Y2, String color)
        {
            Line newLine = new Line();
            Thickness thickness = new Thickness(100);
            newLine.Margin = thickness;
            SolidColorBrush brush = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
            newLine.Stroke = brush;
            newLine.X1 = X1;
            newLine.X2 = X2;
            newLine.Y1 = Y1;
            newLine.Y2 = Y2;

            myCanvas.Children.Add(newLine);
        }

        private void drawLine(Point p1, Point p2, Color color)
        {
            Line newLine = new Line();
            Thickness thickness = new Thickness(100);
            newLine.Margin = thickness;
            SolidColorBrush brush = new SolidColorBrush(color);
            newLine.Stroke = brush;
            newLine.X1 = p1.X;
            newLine.Y1 = p1.Y;
            newLine.X2 = p2.X;
            newLine.Y2 = p2.Y;

            myCanvas.Children.Add(newLine);

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            int taskTypeId = (int)checkbox.Tag;
            Model model = data.currentModel;


            if (!isCurrentModel())
            {
                MessageBox.Show("No Model is selected");
                checkbox.IsChecked = false;
                return;
            }

            if (!renderedChecks) return;

            if (checkboxTaskCompleted(taskTypeId))
            {
                String lastTaskName = model.tasks[model.lastCompletedTaskId].name;
                MessageBoxResult res = MessageBox.Show("WARNING: The last saved task will be " + lastTaskName, "Undo Task?", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes)
                {
                    model.uncompleteTask(taskTypeId);
                }
            }

            if(!isCurrentUser())
            {
                MessageBox.Show("No User Logged In");
                checkbox.IsChecked = false;
                return;
            }

            Models.Task task = model.tasks[taskTypeId];

            CompleteTask win2 = new CompleteTask(task);
            win2.ShowDialog();

            if(win2.earlyExit)
            {
                checkbox.IsChecked = false;
            }
            else
            {
                addCompletedTaskBlock(task);
            }
        }

        private Boolean checkboxTaskCompleted(int taskTypeId)
        {
            Model model = data.currentModel;
            return (model.tasks[taskTypeId].completed);
        }

        private Boolean isCurrentModel()
        {
            return data.currentModel != null;
        }

        private Boolean isCurrentUser()
        {
            return data.currentUser != null;
        }

        private void addCompletedTaskBlock(Models.Task task)
        {
            TaskDisplay taskDisplay = modelDisplay.addAndGetCompletedTask(task);
            addRectToCanvas(taskDisplay);
            initTaskBarWithModel();
        }

        private void btnRegNewModel_clicked(object sender, RoutedEventArgs e)
        {
            RegNewModel win2 = new RegNewModel();
            win2.ShowDialog();
            if(!win2.earlyExit) displayCurrentModel();
        }

        private void btnEditCurrentModel_Click(object sender, RoutedEventArgs e)
        {
            Login win2 = new Login();
            win2.ShowDialog();
            if (!win2.earlyExit) displayCurrentUser();
        }

        private void btnLoadExistingModel_Click(object sender, RoutedEventArgs e)
        {
            List<ModelTag> modelTags = MainWindow.myDatabase.getModelTags();

            if (modelTags.Count > 0)
            {
                LoadExistingModel win2 = new LoadExistingModel(modelTags);
                win2.ShowDialog();
                if (!win2.earlyExist) displayCurrentModel();
            }
            else MessageBox.Show("No Models Created");
        }

        private void adminBtn_Click(object sender, RoutedEventArgs e)
        {
            Admin win2 = new Admin();
            win2.ShowDialog();

        }
    }
}
