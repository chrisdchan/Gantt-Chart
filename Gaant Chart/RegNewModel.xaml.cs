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
        
        public RegNewModel()
        {
            InitializeComponent();
            setSubmitStatus(false);
        }

        private void CreateModel(object sender, RoutedEventArgs e)
        {
            String modelID = tbModelID.Text;
            String date = tbDate.Text;

            if(String.IsNullOrEmpty(date))
            {
                MessageBoxResult res = System.Windows.MessageBox.Show("Would you like to use today's date as the start date?", "Invalid Date", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes)
                {
                    DateTime now = DateTime.Now;
                    date = now.ToString("MM-dd-YY");
                    tbDate.Text = date;
                }
                else
                {
                    System.Windows.MessageBox.Show("Please Enter a valid date");
                    return;
                }
            }

            ModelDb database = new ModelDb();
            database.InsertModel(modelID, date);
           
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
