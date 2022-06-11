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
            initTaskBarLists();
            setTaskComponentsReadOnly();

        }

        private void displayCurrentModel()
        {

            Model model = data.currentModel;
            view = new CanvasView(data.currentModel, 28);

            myCanvas.Visibility = Visibility.Visible;


            label_ModelID.Content = model.modelName;
            txtDisplayModelName.Text = data.currentModel.modelName;

            initTaskBlocks();
            initTaskBarWithModel();

            updateCanvas();
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
                checkbox.Unchecked += new RoutedEventHandler(CheckBox_Unchecked);

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
            initInivisibleRectangle();
        }

        private void initInivisibleRectangle()
        {
            Rectangle background = new Rectangle();
            background.Width = view.RIGHT_OUTER_BORDER - view.LEFT_OUTER_BORDER;
            background.Height = view.BOTTOM_BORDER - view.TOP_BORDER;
            background.Opacity = 0.001;
            background.Fill = new SolidColorBrush(Colors.White);
            myCanvas.Children.Add(background);

            Canvas.SetTop(background, view.LABEL_TOP_OFFSET);
            Canvas.SetLeft(background, view.LABEL_LEFT_MARGIN);
        }

        private void updateTaskBlocks()
        {
            if(isCurrentModel())
            {
                foreach(TaskDisplay taskDisplay in Enumerable.Concat(modelDisplay.plannedBlocks, modelDisplay.completedBlocks))
                {
                    UIElement element = taskDisplay.rectangle;
                    myCanvas.Children.Remove(element);
                }

                modelDisplay.resize(view);

                foreach(TaskDisplay taskDisplay in Enumerable.Concat(modelDisplay.plannedBlocks, modelDisplay.completedBlocks))
                {
                    addRectToCanvas(taskDisplay);
                }
            }
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

            updateTaskBlocks();
        }

        private void removeDynamicElements()
        {
            foreach(CanvasLine canvasLine in canvasDisplay.dynamicLines)
            {
                UIElement element = canvasLine.line;
                myCanvas.Children.Remove(element);
            }

            foreach(CanvasElement canvasElement in canvasDisplay.dates)
            {
                UIElement element = canvasElement.element;
                myCanvas.Children.Remove(element);
            }

        }

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

            renderedChecks = false;

            for(int i = 0; i < data.allTasks.Length; i++)
            {
                CheckBox checkbox = taskBarCheckBoxes[i];
                TextBox textbox = taskBarTextBoxes[i];

                checkbox.Foreground = unavalibleColor;
                checkbox.IsChecked = false;
                checkbox.IsHitTestVisible = false;

                textbox.Text = "";
            }

            renderedChecks = true;
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

                    textbox.Text = task.user.name + " | " + task.endDate.ToString("M/d/y");
                }
            }
            renderedChecks = true;
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

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

            if (!renderedChecks) return;

            CheckBox checkbox = sender as CheckBox;
            int taskTypeId = (int)checkbox.Tag;
            Model model = data.currentModel;
            Models.Task task = model.tasks[taskTypeId];

            if (task.completed)
            {
                String lastTaskName = (task.typeInd == 0) ? "NONE" : model.tasks[task.typeInd - 1].name;

                MessageBoxResult res = MessageBox.Show("WARNING: The last saved task will be " + lastTaskName, "Undo Task?", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes)
                {
                    model.uncompleteTask(taskTypeId);
                    myDatabase.updateModel(model);
                }
            }

            initTaskBarWithModel();
            initTaskBlocks();

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

        private void myCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                view.addOneDay();
                updateCanvas();
            }
            else
            {
                view.removeOneDay();
                updateCanvas();
            }
        }

        private void rightBtn_Click(object sender, RoutedEventArgs e)
        {
            view.addOneDay();
            updateCanvas();

        }

        private void leftBtn_Click(object sender, RoutedEventArgs e)
        {
            view.removeOneDay();
            updateCanvas();
        }
    }
}
