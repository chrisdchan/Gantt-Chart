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
using System.Diagnostics;
using System.Windows.Forms;

using CheckBox = System.Windows.Controls.CheckBox;
using TextBox = System.Windows.Controls.TextBox;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;

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
        private List <TextBox> taskBarTextBoxes { get; set; }

        private List<DockPanel> dockPanels { get; set; }

        private CanvasView view { get; set; }

        private  ModelDisplay modelDisplay { get; set; }
        private CanvasDisplay canvasDisplay { get; set; }

        private Boolean renderedChecks = true;

        private String ADMIN_PASSWORD = "physics123!";

        public MainWindow()
        {
            InitializeComponent();

            // create database connection
            myDatabase = new DbConnection();
            view = new CanvasView(DateTime.Now, DEFAULT_DAYS_IN_VIEW);


            // Cache user data from database
            data.initUsers();


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
            DateTime now = DateTime.Now;

            String greeting = "";
            if(now.TimeOfDay.TotalHours < 12)
            {
                greeting = "Good Morning";
            }
            else if(now.TimeOfDay.TotalHours < 18)
            {
                greeting = "Good Afternoon";
            }
            else
            {
                greeting = "Good Evening";
            }

            userLabel.Content = "Current User: " +  greeting + " " + data.currentUser.name;
            setTaskBarWithUser();
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
                checkbox.Checked += new RoutedEventHandler(System_Checked);
                checkbox.Unchecked += new RoutedEventHandler(System_Unchecked);

                TextBox textbox = new TextBox();
                textbox.Width = 100;
                textbox.Margin = new Thickness(0, 0, 5, 0);
                textbox.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
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
            updateSlider();
        }

        private void updateSlider()
        {
            double daysOffset = (view.startDate - view.modelStartDate).TotalDays;
            slider.Value = 50 + (daysOffset / 180) * 50;
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
            dockPanels = new List<DockPanel>();

            int i = 0;

            foreach(Object outerObj in taskBarStackPanel.Children)
            {
                if(outerObj.GetType() != typeof(DockPanel)) continue;

                DockPanel dockPanel = outerObj as DockPanel;
                dockPanels.Add(dockPanel);

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
            setTaskBarWithModel();
        }

        private void resetTaskbarWithoutModel()
        {
            SolidColorBrush uncompletedColor = new SolidColorBrush(Color.FromRgb(237, 241, 86));
            renderedChecks = false;

            for (int i = 0; i < data.allTasks.Length; i++)
            {
                DockPanel dockpanel = dockPanels[i];
                CheckBox checkbox = taskBarCheckBoxes[i];
                TextBox textbox = taskBarTextBoxes[i];
                dockpanel.Background = uncompletedColor;
                checkbox.IsChecked = false;
                checkbox.IsHitTestVisible = false;
                textbox.Text = "";
            }
            renderedChecks = true;
            
        }
        
        private void resetTaskBarWithoutUser()
        {
            User user = data.currentUser;
            if (user == null) return;

            SolidColorBrush authorizedColor = new SolidColorBrush(Colors.Black);
            SolidColorBrush unauthorizedColor = new SolidColorBrush(Color.FromRgb(123, 123, 123));

            renderedChecks = false;
            
            for(int i = 0; i < data.allTasks.Length; i++)
            {
                CheckBox checkbox = taskBarCheckBoxes[i];
                checkbox.Foreground = authorizedColor;
            }
            renderedChecks = true;
        }

        private void setTaskBarWithModel()
        {
            SolidColorBrush completedColor = new SolidColorBrush(Color.FromRgb(230, 255, 230));
            SolidColorBrush uncompletedColor = new SolidColorBrush(Color.FromRgb(237, 241, 86));

            renderedChecks = false;

            Model model = data.currentModel;
            if (model == null) return;

            for(int i = 0; i < data.allTasks.Length; i++)
            {
                DockPanel dockpanel = dockPanels[i];
                CheckBox checkbox = taskBarCheckBoxes[i];
                TextBox textbox = taskBarTextBoxes[i];
                Models.Task task = model.tasks[i];

                if (task.completed)
                {
                    dockpanel.Background = completedColor;
                    checkbox.IsHitTestVisible = true;
                    checkbox.IsChecked = true;
                    textbox.Text = data.getUser(task.userCompletedId).name + " | " + task.endDate.ToString("M/d/y");
                }
                else
                {
                    dockpanel.Background = uncompletedColor;
                    checkbox.IsChecked = false;
                    checkbox.IsHitTestVisible = false;
                    textbox.Text = "";
                }
            }

            CheckBox nextCheckbox = taskBarCheckBoxes[model.lastCompletedTaskId + 1];
            nextCheckbox.IsHitTestVisible = true;

            renderedChecks = true;
        }

        public void setTaskBarWithUser()
        {
            User user = data.currentUser;
            if (user == null) return;

            SolidColorBrush authorizedColor = new SolidColorBrush(Colors.Black);
            SolidColorBrush unauthorizedColor = new SolidColorBrush(Color.FromRgb(123, 123, 123));

            renderedChecks = false;
            
            for(int i = 0; i < data.allTasks.Length; i++)
            {
                CheckBox checkbox = taskBarCheckBoxes[i];
                if (user.authorization[i])
                {
                    checkbox.Foreground = authorizedColor;
                }
                else
                {
                    checkbox.Foreground = unauthorizedColor;
                }
            }

            renderedChecks = true;
        }

        private void System_Checked(object sender, RoutedEventArgs e)
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
                System.Windows.MessageBox.Show("No User Logged In");
                checkbox.IsChecked = false;
                return;
            }

            User user = data.currentUser;
            if (!user.authorization[taskTypeId])
            {
                MessageBox.Show("User is not authorized to complete task");
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

        private void System_Unchecked(object sender, RoutedEventArgs e)
        {

            if (!renderedChecks) return;

            CheckBox checkbox = sender as CheckBox; 
            int taskTypeId = (int)checkbox.Tag;
            Model model = data.currentModel;
            Models.Task task = model.tasks[taskTypeId];

            if (task.completed)
            {
                String lastTaskName = (task.typeInd == 0) ? "NONE" : model.tasks[task.typeInd - 1].name;

                System.Windows.MessageBoxResult res = MessageBox.Show("WARNING: The last saved task will be " + lastTaskName, "Undo Task?", MessageBoxButton.YesNo);
                if(res == System.Windows.MessageBoxResult.Yes)
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
            if(data.users.Count == 0)
            {
                System.Windows.MessageBox.Show("No Users Exist: Initialize Users in Admin Settings");
                return;
            }
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
            else System.Windows.MessageBox.Show("No Models Created");
        }

        private void adminBtn_Click(object sender, RoutedEventArgs e)
        {
            String password = adminTxt.Text;
            if(password.ToLower() == ADMIN_PASSWORD)
            {
                adminTxt.Text = "";
                Admin win2 = new Admin();
                win2.ShowDialog();
                if(win2.updatedCurrentUser)
                {
                    resetTaskBarWithoutUser();
                    userLabel.Content = "";
                }

                if(win2.deletedCurrentModel)
                {
                    resetTaskbarWithoutModel();
                    myCanvas.Visibility = Visibility.Hidden;
                    txtDisplayModelName.Text = "";
                }
            }
            else
            {
                adminTxt.Text = "";
                System.Windows.MessageBox.Show("Wrong Password");
            }
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

        private Point referencePoint;
        private DateTime referenceDate;
        Boolean mouseCaptured = false;

        private void myCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            referencePoint = e.GetPosition(myCanvas);
            referenceDate = view.startDate;
            mouseCaptured = true;
        }

        private void myCanvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (referencePoint == null) return;
            if (!mouseCaptured) return;

            Point position = e.GetPosition(sender as IInputElement);

            double dayOffset = (referencePoint.X - position.X) / view.pixelsPerDay;

            view.changeStartDate(referenceDate.AddDays(dayOffset));
            updateCanvas();
         }

        private void myCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseCaptured = false;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("No need to save, model is updated upon every change");
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/chrisdchan/Gantt-Chart/blob/main/README.md",
                UseShellExecute = true
            });
        }

        private void clearModel_Click(object sender, RoutedEventArgs e)
        {
            resetTaskbarWithoutModel();
            myCanvas.Visibility = Visibility.Hidden;
            txtDisplayModelName.Text = "";

        }

        private void importBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV (*.csv)|*.csv, Excel (*.xlsm)|*.xlsm";
            openFileDialog.Title = "Select sheet";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String filename = openFileDialog.FileName;
                ExcelReader excelReader = new ExcelReader(filename);
                
            }
        }
    }
}
