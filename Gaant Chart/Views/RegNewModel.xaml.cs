using Gaant_Chart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gaant_Chart
{
    /// <summary>
    /// Interaction logic for RegNewModel.xaml
    /// </summary>
    public partial class RegNewModel : Window
    {
        private Boolean submitStatus;
        public Boolean earlyExit { get; set; }
        public RegNewModel()
        {
            InitializeComponent();
            earlyExit = true;
            setSubmitStatus(false);
            tbModelID.Focus();
        }

        private void btnCreateModel_Click(object sender, RoutedEventArgs e)
        {
            // Only proceed if Model has a Name
            if (!submitStatus) return;

            String modelName = tbModelID.Text;
            String dateString = tbDate.Text;
            DateTime date;

            if(String.IsNullOrEmpty(dateString))
            {
                // If there is no date, allow user to choose to use today's date
                MessageBoxResult res = System.Windows.MessageBox.Show("Would you like to use today's date as the start date?", "Invalid Date", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes)
                {
                    date = DateTime.Now;
                }
                else
                {
                    System.Windows.MessageBox.Show("Please Enter a valid date");
                    return;
                }
            }
            else if(!DateTime.TryParse(dateString, out date))
            {
                // If date is invalid, return and clear the text field
                System.Windows.MessageBox.Show("Invalid Date Form, please use MM-dd-yyyy");
                tbDate.Text = "";
                return;
            }

            int existingModelId = MainWindow.myDatabase.getModelId(modelName);
            if(existingModelId != -1)
            {
                MessageBoxResult res = System.Windows.MessageBox.Show("This Model Already Exists, would you like to load it instead?", "Existing Model", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    data.currentModel = MainWindow.myDatabase.getModel(existingModelId);
                    earlyExit = false;
                    this.Close();
                }
                else return;
            }
            Model model = new Model(modelName, date);
            MainWindow.myDatabase.insertModel(model);
            data.currentModel = model;
            earlyExit = false;
            this.Close();

        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            //close the window
            this.Close();
        }

        private void tbModelID_TextChanged(object sender, TextChangedEventArgs e)
        {
            String ModelID = tbModelID.Text;

            if (String.IsNullOrEmpty(ModelID)) setSubmitStatus(false);
            else setSubmitStatus(true);
        }

        private void setSubmitStatus(Boolean status)
        {
            submitStatus = status;

            if (status) btnCreateModel.Foreground = new SolidColorBrush(Colors.Black);
            else btnCreateModel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BEBEBE"));

        }
    }
}
