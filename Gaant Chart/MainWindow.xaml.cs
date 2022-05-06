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
        // Status : Does not work right now

        public static DbConnection myDatabase { get; set; }

        public static EventHandler LoadExistingModelEvent { get; set; }
        private static Dictionary<int, (CheckBox, TextBox)> taskComponents { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            // create database connection
            myDatabase = new DbConnection();

            // set up the canvas
            drawInitialCanvas();
            initTaskBlocks();
            setIdealTaskBlocks();

            taskComponents = new Dictionary<int, (CheckBox, TextBox)>
            {
                {0, (c_Image_Aquisition, txt_Image_Aquisition) },
                {1, (c_Images_Download, txt_Images_Download) },
                {2, (c_SPM8_Autosegmentation, txt_SPM8_Autosegmentation) },
                {3, (c_MD_Volumes_Segmented, txt_MD_Volumes_Segmented)},
                {4, (c_Fully_Segmented_Model, txt_Fully_Segmented_Model) },
                {5, (c_Initial_Peer_Review, txt_Initial_Peer_Review)},
                {6, (c_Segmentation_Corrections, txt_Segmentation_Corrections)},
                {7, (c_2nd_Peer_Review, txt_2nd_Peer_Review)},
                {8, (c_2nd_Segmentation_Corrections, txt_2nd_Segmentation_Corrections) },
                {9, (c_Full_Model_Approved, txt_Full_Model_Approved) },
                {10, (c_Meshed_Model, txt_Meshed_Model) },
                {11, (c_Export_Model_to_Physics, txt_Export_Model_to_Physics) },
                {12, (c_Model_Solved, txt_Model_Solved) },
                {13, (c_Report_Generated, txt_Report_Generated) }
            };
            
        }

        private void displayCurrentModel()
        {
            myCanvas.Visibility = Visibility.Visible;
            setModel(data.currentModel.modelName);
            setDate(data.currentModel.startDate);
            txtDisplayModelName.Text = data.currentModel.modelName;
        }

        private void resetTaskComponents()
        {
            for(int i = 0; i < taskComponents.Count; i++)
            {
                taskComponents[i].Item1.IsChecked = false;
                taskComponents[i].Item2.Text = "";
            }
        }
        private void setTaskBlocksAndComponents()
        {
            //Can only be called after initialized Tasks
            if (data.completedTasks == null) throw new Exception("cannot call setTaskComponents() before initializing completedTasks");

            for (int i = 0; i < data.allTasks.Length; i++)
            {
                String modelName = data.allTasks[i];
                if (!data.completedTasks.ContainsKey(modelName)) break;
                
                // TODO: set checkbox to True
                // TODO: set textbox to the correct person
                // TODO: set textbox to readonly
                // TODO: Resize and move Green Boxes to correct positions  

            }

        }
        
        private void setTaskComponentsReadOnly(Boolean status)
        {
            for(int i = 0; i < taskComponents.Count; i++)
            {
                taskComponents[i].Item2.IsReadOnly = status;
            }
        }


        int dayWidth = 19;
        int blockHeight = 30;
        int topOffset = 120;
        int dateX = 210;

        private void drawInitialCanvas()
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

            Canvas.SetLeft(label_ModelID, 25);
            Canvas.SetTop(label_ModelID, 50);

        }
        private void setIdealTaskBlocks()
        {
            // Probably only needs to be called once at the beginning
            int dayOffset = 0;

            for(int i = 0; i < data.allTasks.Length; i++)
            {
                Rectangle rect = data.plannedTaskBlocks[i];

                rect.Width = dayWidth * data.taskSettingsDuration[i];
                rect.Height = blockHeight;

                Canvas.SetLeft(rect, dateX + dayOffset * dayWidth);
                Canvas.SetTop(rect, topOffset + i * blockHeight);

                dayOffset += data.taskSettingsDuration[i];
            }
        }
        private void initTaskBlocks()
        {
            // This method should only be called once at start of app
            data.plannedTaskBlocks = new List<Rectangle>();
            data.completedTaskBlocks = new List<Rectangle>();
            for(int i=0; i < data.allTasks.Length; i++)
            {
                Rectangle rectP = new Rectangle();
                Rectangle rectC = new Rectangle();

                rectP.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FED49E"));
                rectC.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9EEEA2"));

                data.plannedTaskBlocks.Add(rectP);
                data.completedTaskBlocks.Add(rectC);
                myCanvas.Children.Add(rectP);
                myCanvas.Children.Add(rectC);

            }
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
                    MessageBox.Show(taskName);
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
        }

        private void btnLoadExistingModel_Click(object sender, RoutedEventArgs e)
        {
            List<(String, int)> ModelNames = new List<(string, int)>();
            if(myDatabase != null)
            {
                ModelNames = MainWindow.myDatabase.getModelNames();
            }

            if (ModelNames.Count > 0)
            {
                LoadExistingModel win2 = new LoadExistingModel(ModelNames);
                win2.ShowDialog();
                if (win2.earlyExist) displayCurrentModel();
            }
            else MessageBox.Show("No Models Created");
        }

    }
}
