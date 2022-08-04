using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Gaant_Chart.Models;
using System.Diagnostics;
using System.Windows.Forms;

using CheckBox = System.Windows.Controls.CheckBox;
using TextBox = System.Windows.Controls.TextBox;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;
using Gaant_Chart.Components;
using System.Windows.Input;
using Gaant_Chart.Views;

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
        public static Grid grid { get; set; }
        private List<CheckBox> taskBarCheckBoxes { get; set; }
        private List<TextBox> taskBarTextBoxes { get; set; }
        private List<DockPanel> taskDockPanels { get; set; }
        private CanvasGraph canvasGraph { get; set; }

        private Boolean renderedChecks = true;

        private String ADMIN_PASSWORD = "physics123!";

        private SolidColorBrush taskTextBoxColor = new SolidColorBrush(Colors.LightGray);
        private SolidColorBrush taskCheckBoxUncompletedColor = new SolidColorBrush(Color.FromRgb(237, 241, 86));
        private SolidColorBrush taskCheckBoxCompletedColor = new SolidColorBrush(Color.FromRgb(230, 255, 230));

        public int fontSize { get; set; }

        public double topRightButtonFont { get; set; }
        public double topLeftButtonFont { get; set; }
        public (double assemblies, double groupTask, double progress, double userHeader) headerFonts { get; set; }

        private List<System.Windows.Controls.Control> elements { get; set; }

        private PasswordTextBox adminPasswordTxt;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // create database connection
            myDatabase = new DbConnection();

            grid = mainGrid;

            // Cache user data from database
            data.initUsers();

            // set up the canvas
            canvasGraph = new CanvasGraph();

            createTaskBar();
            createAdminPasswordBox();
            setTaskComponentsReadOnly();
            initButtons();

            Loaded += delegate
            {
                canvasGraph.load();
                setBindings();
            };
        }

        private void initButtons()
        {
            elements = new List<System.Windows.Controls.Control> {
                btnRegModel,
                btnEditCurrentModel,
                btnLoadExistingModel,
                btnImportModel,
                adminBtn,
                btnHelp,
                labelProgTracker,
                btnZoomIn,
                btnZoomOut,
                btnResetPos,
                userLabel,
                labelProgTracker,
                txtDisplayModelName,
                labelAssemblies,
                label1,
                label2,
                label3,
                label4
            };
        }
        private void setBindings()
        { 
            foreach(System.Windows.Controls.Control element in elements)
            {
                element.FontSize = Math.Min(element.ActualWidth * 0.1, element.ActualHeight * 0.4);
            }
        }
        private void createAdminPasswordBox()
        {
            adminPasswordTxt = new PasswordTextBox();
            TextBox textbox = adminPasswordTxt.textbox;

            adminPanelGrid.Children.Add(textbox);
            Grid.SetRow(textbox, 0);
            Grid.SetColumn(textbox, 2);
            textbox.KeyDown += adminPasswordBoxKeyDown;
        }
        private void displayCurrentModel()
        {
            Model model = data.currentModel;
            canvasGraph.loadModel(model);
            txtDisplayModelName.Text = data.currentModel.modelName;
            initTaskBarWithModel();
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
            foreach(TextBox textBox in taskBarTextBoxes)
            {
                textBox.IsReadOnly = true;
            }
        }
        private void createTaskBar()
        {
            taskBarTextBoxes = new List<TextBox>();
            taskBarCheckBoxes = new List<CheckBox>();
            taskDockPanels = new List<DockPanel>();

            for(int i = 0; i < data.allTasks.Length; i++)
            {
                createTaskCheckBox(i);
                createTaskTextBox(i);
            }
        }
        private void createTaskCheckBox(int i)
        {
            CheckBox checkbox = new CheckBox();
            checkbox.Content = data.allTasks[i];
            checkbox.Tag = i;
            checkbox.Checked += new RoutedEventHandler(System_Checked);
            checkbox.Unchecked += new RoutedEventHandler(System_Unchecked);

            DockPanel dockPanel = new DockPanel();
            dockPanel.Background = taskCheckBoxUncompletedColor;
            dockPanel.Children.Add(checkbox);
            taskDockPanels.Add(dockPanel);

            taskBarGrid.Children.Add(dockPanel);
            taskBarCheckBoxes.Add(checkbox);
            Grid.SetColumn(dockPanel, 0);
            Grid.SetRow(dockPanel, data.taskGridRow[i]);
        }
        private void createTaskTextBox(int i)
        {
            TextBox textbox = new TextBox();
            textbox.Background = taskTextBoxColor;
            textbox.Tag = i;
            textbox.PreviewMouseDown += textBoxMouseDown;
            textbox.PreviewMouseUp += textBoxMouseUp;

            taskBarGrid.Children.Add(textbox);
            taskBarTextBoxes.Add(textbox);
            Grid.SetColumn(textbox, 1);
            Grid.SetRow(textbox, data.taskGridRow[i]);
        }

        private TextBox textBoxMouseDowned;
        private Boolean mouseDownFlag = false;
        private void textBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            textBoxMouseDowned = sender as TextBox;
            mouseDownFlag = true;
        }
        private void textBoxMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBox textBoxMouseUped = sender as TextBox;

            int taskTypeId = (int)textBoxMouseUped.Tag;

            if(taskTypeId > data.currentModel.lastCompletedTaskId)
            {
                MessageBox.Show("You cannot edit an uncompleted task");
                return;
            }

            if(mouseDownFlag && textBoxMouseUped == textBoxMouseDowned)
            {
                Task task = data.currentModel.tasks[taskTypeId];
                EditTask win2 = new EditTask(task);
                win2.ShowDialog();
                if(!win2.exitEarlyFlag)
                {
                    setTaskBarWithModel();
                    canvasGraph.updateCompletedTask(task);
                }
                if (win2.changedUserFlag)
                    displayCurrentUser();
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
                DockPanel dockpanel = taskDockPanels[i];
                CheckBox checkbox = taskBarCheckBoxes[i];
                TextBox textbox = taskBarTextBoxes[i];
                dockpanel.Background = uncompletedColor;
                checkbox.IsChecked = false;
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
            renderedChecks = false;

            Model model = data.currentModel;
            if (model == null) return;

            for(int i = 0; i < data.allTasks.Length; i++)
            {
                DockPanel dockpanel = taskDockPanels[i];
                CheckBox checkbox = taskBarCheckBoxes[i];
                TextBox textbox = taskBarTextBoxes[i];
                Task task = model.tasks[i];

                if (task.completed)
                {
                    dockpanel.Background = taskCheckBoxCompletedColor;
                    checkbox.IsChecked = true;
                    textbox.Text =  task.completedUser.name + " | " + ((DateTime)task.endDate).ToString("M/d/y");
                }
                else
                {
                    dockpanel.Background = taskCheckBoxUncompletedColor;
                    checkbox.IsChecked = false;
                    textbox.Text = "";
                }
            }

            if(model.lastCompletedTaskId != model.tasks.Length - 1)
            {
                CheckBox nextCheckbox = taskBarCheckBoxes[model.lastCompletedTaskId + 1];
            }

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

            if (isCurrentUser() && !data.currentUser.authorization[taskTypeId])
            {
                MessageBox.Show("User is not authorized to complete task");
                checkbox.IsChecked = false;
                return;
            }

            if(taskTypeId != 0 && !model.tasks[taskTypeId - 1].completed)
            {
                MessageBox.Show("Previous task is not completed");
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
                canvasGraph.addCompletedTask(task);
            }

            if (win2.changedUserFlag)
                displayCurrentUser();

            setTaskBarWithModel();
        }
        private void System_Unchecked(object sender, RoutedEventArgs e)
        {

            if (!renderedChecks) return;
            if (data.currentModel == null) return;

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
            canvasGraph.reloadModel(model);

        }
        private Boolean isCurrentModel()
        {
            return data.currentModel != null;
        }
        private Boolean isCurrentUser()
        {
            return data.currentUser != null;
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
                MessageBox.Show("No Users Exist: Initialize Users in Admin Settings");
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
            else MessageBox.Show("No Models Created");
        }
        private void handleAdminRequest()
        {
            String password = adminPasswordTxt.password;
            if(password.ToLower() == ADMIN_PASSWORD)
            {
                Admin win2 = new Admin();
                win2.ShowDialog();
                if(win2.updatedCurrentUser)
                {
                    resetTaskBarWithoutUser();
                    userLabel.Content = "";
                }

                if(win2.deletedCurrentModel)
                {
                    canvasGraph.clearModel();
                    resetTaskbarWithoutModel();
                    txtDisplayModelName.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Wrong Password");
            }
            adminPasswordTxt.textbox.Text = "";

        }
        private void adminBtn_Click(object sender, RoutedEventArgs e)
        {
            handleAdminRequest();
        }
        private void adminPasswordBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                handleAdminRequest();
            }
        }
        private void zoomInBtn_Click(object sender, RoutedEventArgs e)
        {
            canvasGraph.addDays(-1);
        }
        private void zoomOutBtn_Click(object sender, RoutedEventArgs e)
        {
            canvasGraph.addDays(1);
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
            data.currentModel = null;
            txtDisplayModelName.Text = "";
            resetTaskbarWithoutModel();
            canvasGraph.clearModel();

        }
        private void importBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel (*.xlsm)|*.xlsm";
            openFileDialog.Title = "Select sheet";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String filename = openFileDialog.FileName;
                ExcelReader excelReader = new ExcelReader(filename);

                Model model = excelReader.model;

                long modelId = MainWindow.myDatabase.findModelId(model.modelName);

                if(modelId == - 1)
                {
                    MainWindow.myDatabase.insertModel(model);
                    data.currentModel = model;
                }
                else
                {
                    Model oldModel = MainWindow.myDatabase.getModel(modelId);

                    String msg = "Replace Existing Model? \n" +
                        "Click YES to replace model\n" +
                        "Click NO to load existing model (last updated: " + model.lastUpdated.ToString("MM-dd-yy") + " )";

                    MessageBoxResult res = MessageBox.Show(msg, "Model Already Exists", MessageBoxButton.YesNo);

                    if (res == MessageBoxResult.Yes)
                    {
                        MainWindow.myDatabase.deleteModel(oldModel.rowid);
                        MainWindow.myDatabase.insertModel(model);

                        data.currentModel = model;
                    }
                    else if(res == MessageBoxResult.No)
                    {
                        data.currentModel = oldModel;
                    }
                }

                displayCurrentModel();
            }
        }
        private void resetPosition_Click(object sender, RoutedEventArgs e)
        {
            canvasGraph.resetPosition();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            setBindings();

        }
    }
}
