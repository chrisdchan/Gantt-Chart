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
        public MainWindow()
        {
            InitializeComponent();

            initSettingsData();

            myDatabase = new DbConnection();
        }

        public void setExistingModel(int modelId, String modelName, DateTime date)
        {
            // update static variables
            data.ModelId = modelId;
            data.modelName = modelName;
            data.startDate = date;

            // Initialize completeTasks dictionary
            data.initcompletedTasks(modelId);

            // TODO: Reset completed Task Blocks

            // TODO: Redraw blocks

            // TODO: Log out of User
            
        }

        public void setNewModel(String modelName, DateTime date)
        {
            //update static variables
            data.modelName = modelName;
            data.startDate = date;

            // Initialize Canvas
            myCanvas.Visibility = Visibility.Visible;

            //TODO: uncheck all TaskCheckBoxes and TaskUserLabels
            
            // Draw lines with correct modelName and date
            drawInitialCanvas(modelName, date);

            // Draw planned blocks
            drawIdealTaskBlocks();

            //TODO: Log out of User

        }

        public static void newModel(String modelName, DateTime date)
        {
            // Insert new model to Database
            data.ModelId = myDatabase.InsertModel(modelName, date);
             
        }

        private void initSettingsData()
        {
            //Eventually this data should be mutable by admin, for now this will be hard coded


            // taskStartDelayPlanned specifies the ideal start date of every task
            data.initTaskStartDelayPlanned();

            // store (Not display) data for ideal schedule blocks
            initIdealTaskBlocks();

        }

        int dayWidth = 19;
        int blockHeight = 30;
        int topOffset = 120;
        int dateX = 210;

        private void initIdealTaskBlocks()
        {
            // Must precede initTaskStartDelayPlanned

            data.plannedTaskBlocks = new List<Rectangle>();

            int i = 0;
            foreach(KeyValuePair<String, int > task in data.taskStartDelayPlanned)
            {
                Rectangle rect = new Rectangle();
                rect.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FED49E"));
                rect.Width = dayWidth * data.taskSettings[task.Key].Item1;
                rect.Height = blockHeight;
                Canvas.SetLeft(rect, dateX + task.Value * (dayWidth));
                Canvas.SetTop(rect, topOffset - 2 + i * (blockHeight + 2));
                data.plannedTaskBlocks.Add(rect);

                i += 1;
            }
        }

        private void btnRegNewModel_clicked(object sender, RoutedEventArgs e)
        {
    
            RegNewModel win2 = new RegNewModel();
            win2.Show();
            
        }


        private void drawInitialCanvas(String modelName, DateTime date)
        {
            int leftOffset = 20;
            int labelSpacing = 32;

            // Set the position of each task label
            Canvas.SetLeft(l1, leftOffset);
            Canvas.SetLeft(l2, leftOffset);
            Canvas.SetLeft(l3, leftOffset);
            Canvas.SetLeft(l4, leftOffset);
            Canvas.SetLeft(l5, leftOffset);
            Canvas.SetLeft(l6, leftOffset);
            Canvas.SetLeft(l7, leftOffset);
            Canvas.SetLeft(l8, leftOffset);
            Canvas.SetLeft(l9, leftOffset);
            Canvas.SetLeft(l10, leftOffset);
            Canvas.SetLeft(l11, leftOffset);
            Canvas.SetLeft(l12, leftOffset);
            Canvas.SetLeft(l13, leftOffset);
            Canvas.SetLeft(l14, leftOffset);

            Canvas.SetTop(l1, topOffset + labelSpacing * 0);
            Canvas.SetTop(l2, topOffset + labelSpacing * 1);
            Canvas.SetTop(l3, topOffset + labelSpacing * 2);
            Canvas.SetTop(l4, topOffset + labelSpacing * 3);
            Canvas.SetTop(l5, topOffset + labelSpacing * 4);
            Canvas.SetTop(l6, topOffset + labelSpacing * 5);
            Canvas.SetTop(l7, topOffset + labelSpacing * 6);
            Canvas.SetTop(l8, topOffset + labelSpacing * 7);
            Canvas.SetTop(l9, topOffset + labelSpacing * 8);
            Canvas.SetTop(l10, topOffset + labelSpacing * 9);
            Canvas.SetTop(l11, topOffset + labelSpacing * 10);
            Canvas.SetTop(l12, topOffset + labelSpacing * 11);
            Canvas.SetTop(l13, topOffset + labelSpacing * 12);
            Canvas.SetTop(l14, topOffset + labelSpacing * 13);

            Canvas.SetLeft(L1, leftOffset + 15);
            Canvas.SetLeft(L2, leftOffset + 10);
            Canvas.SetLeft(L3, leftOffset + 10);
            Canvas.SetLeft(L4, leftOffset);

            Canvas.SetTop(L1, topOffset + labelSpacing * 1.5);
            Canvas.SetTop(L2, topOffset + labelSpacing * 6);
            Canvas.SetTop(L3, topOffset + labelSpacing * 10.15);
            Canvas.SetTop(L4, topOffset + labelSpacing * 12.05);

            int minX = -80;
            int labelX = 125;
            int minY = 15;
            int maxY = 465;
            int maxX = 713;
            
            // Draw a boarder around the data
            newLine(minX, labelX, 180, 180, "#FF0000");
            newLine(minX, labelX, 340, 340, "#FF0000");
            newLine(minX, labelX, 400, 400, "#FF0000");
            newLine(minX, labelX, maxY, maxY, "#FF0000");
            newLine(minX, minX, minY, maxY, "#FF0000");
            newLine(minX, labelX, minY, minY, "#FF0000");
            newLine(labelX, labelX, minY, maxY, "#FF0000");

            newLine(labelX, maxX, maxY, maxY, "#32EC00");
            newLine(maxX, maxX, minY, maxY, "#32EC00");

            L1.LayoutTransform = new RotateTransform(270);
            L2.LayoutTransform = new RotateTransform(270);
            L3.LayoutTransform = new RotateTransform(270);
            L4.LayoutTransform = new RotateTransform(270);


            newLine(labelX, maxX, minY, minY, "#F2DA2E");

            for (int i = labelX; i < maxX; i += 147)
            {
                if (i != labelX) newLine(i, i, minY, maxY, "#DFDFDF");

                for (int j = i; j < i + 147; j += 21)
                {
                    if (j != i) newLine(j, j, minY, minY + 5, "#F2DA2E");
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

            setDate(date);

            Canvas.SetLeft(label_ModelID, 25);
            Canvas.SetTop(label_ModelID, 50);

            setModel(modelName);

            drawIdealTaskBlocks();

        }
        private void drawIdealTaskBlocks()
        {
            // Must preced initIdealTaskBlocks

            foreach (Rectangle rect in data.plannedTaskBlocks)
            {
                myCanvas.Children.Add(rect);
            }
        }
        
        private void drawAndLoadCompletedTasks()
        {
            //must be called once a new model is loaded



        }

        private void setTaskCheckBoxs()
        {
            foreach(DockPanel myDockPanel in taskBarStackPanel.Children)
            {
                if (myDockPanel.GetType() != typeof(DockPanel)) continue;
                String taskName;

                foreach(CheckBox checkbox in myDockPanel.Children)
                {
                    taskName = (String)checkbox.Content;
                    MessageBox.Show(taskName)
                }
            }
        }


        private void setDate(DateTime date)
        {
            date1.Content = date.ToString("MM-dd-yy");
            date2.Content = date.AddDays(7).ToString("MM-dd-yy");
            date3.Content = date.AddDays(14).ToString("MM-dd-yy");
            date4.Content = date.AddDays(21).ToString("MM-dd-yy");
            date5.Content = date.AddDays(28).ToString("MM-dd-yy");
        }

        private void setModel(String model)
        {
            label_ModelID.Content = model;
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
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((CheckBox)sender).Name);
        }

    }
}
