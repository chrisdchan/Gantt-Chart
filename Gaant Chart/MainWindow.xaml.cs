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
        private List <TextBox> taskBarTextBoxes { get; set; }

        private List<DockPanel> dockPanels { get; set; }
        private CanvasGraph canvasGraph { get; set; }


        private Boolean renderedChecks = true;

        private String ADMIN_PASSWORD = "physics123!";

        private PasswordTextBox adminPasswordTxt;
        public MainWindow()
        {
            InitializeComponent();

            // create database connection
            myDatabase = new DbConnection();

            grid = mainGrid;

            // Cache user data from database
            data.initUsers();


            // set up the canvas
            canvasGraph = new CanvasGraph();

            createTaskBar();
            createAdminPasswordBox();
            initTaskBarLists();
            setTaskComponentsReadOnly();

            Loaded += delegate
            {
                canvasGraph.load();
            };
        }

        private void createAdminPasswordBox()
        {
            adminPasswordTxt = new PasswordTextBox();
            TextBox textbox = adminPasswordTxt.textbox;

            mainGrid.Children.Add(textbox);
            Grid.SetRow(textbox, 0);
            Grid.SetColumn(textbox, 6);
            textbox.VerticalAlignment = VerticalAlignment.Top;
            textbox.Margin = new Thickness(0, 35, 150, 0);
            textbox.Width = 100;
            textbox.Height = 20;
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
            Grid grid = mainGrid;
            foreach(String taskname in data.allTasks)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = taskname;
                checkbox.Checked += new RoutedEventHandler(System_Checked);
                checkbox.Unchecked += new RoutedEventHandler(System_Unchecked);

                TextBox textbox = new TextBox();
                textbox.Width = 200;
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
                    checkbox.IsChecked = true;
                    textbox.Text =  task.completedUser.name + " | " + ((DateTime)task.endDate).ToString("M/d/y");
                }
                else
                {
                    dockpanel.Background = uncompletedColor;
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

            if(!isCurrentUser())
            {
                MessageBox.Show("No User Logged In");
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

        private void adminBtn_Click(object sender, RoutedEventArgs e)
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

        }
    }
}
