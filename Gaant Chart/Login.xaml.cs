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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gaant_Chart
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        List<User> users { get; set; }

        public Boolean earlyExit { get; set; }

        public Login()
        {
            InitializeComponent();

            users = MainWindow.myDatabase.getUsers();
            earlyExit = true;

        }

        private void displayUsers()
        {
            foreach(User user in users)
            {
                ComboBoxItem newComboBoxItem = new ComboBoxItem();
                newComboBoxItem.Content = user.name;
                comboBox.Items.Add(newComboBoxItem);
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            earlyExit = true;
            this.Close();
        }

        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            if(comboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a model");
            }
            else
            {
                int userIndex = (int)((ComboBox)comboBox.SelectedItem).SelectedIndex;
                data.currentUser = users[userIndex];
                earlyExit = false;
                this.Close();
            }
        }
    }
}
