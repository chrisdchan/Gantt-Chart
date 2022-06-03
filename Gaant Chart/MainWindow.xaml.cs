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
        // Status : Does not work right now

        public static DbConnection myDatabase { get; set; }

        public static EventHandler LoadExistingModelEvent { get; set; }
        private static Dictionary<int, (CheckBox, TextBox)> taskComponents { get; set; }

        private static int numDaysInView { get; set; }
        private static double heightPerTask { get; set; }

        double WIDTH = 850;
        double HEIGHT = 592;

        int blockHeight = 30;
        int topOffset = 120;

        int dateX = 210;
        int verticalLabelSpace = 32;

        int labelLeftMargin = 20;

        int leftOuterBorder = -80;
        int leftInnerBorder = 125;
        int rightOuterBorder = 713;
        int topBorder = 15;
        int bottomBorder = 465;

        public MainWindow()
        {
            InitializeComponent();

            // create database connection
            myDatabase = new DbConnection();

            // set up the canvas

            numDaysInView = 7 * 4;
            heightPerTask = (bottomBorder + 5 - topBorder) / 14.0;

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

            myCanvas.Visibility = Visibility.Visible;
            
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

        // NOTE: The top left corner for lines is (-100, -100), Components start at (0, 0)

        private void drawInitialCanvas()
        {

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
            double meshBottomBorder = reviewBottomBorder + heightPerTask * 2;

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
        private void setIdealTaskBlocks()
        {
            double pixelsPerDay = (rightOuterBorder - leftInnerBorder) / numDaysInView;
            // Probably only needs to be called once at the beginning
            int dayOffset = 0;

            for(int i = 0; i < data.allTasks.Length; i++)
            {
                Rectangle rect = data.plannedTaskBlocks[i];

                rect.Width = pixelsPerDay * data.taskSettingsDuration[i] - 2;
                rect.Height = blockHeight;

                Canvas.SetLeft(rect, leftInnerBorder + 100 + dayOffset * pixelsPerDay);
                Canvas.SetTop(rect, topBorder + 105 + i * 32);

                dayOffset += data.taskSettingsDuration[i];
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
