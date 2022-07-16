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
        public Boolean earlyExit { get; set; }

        public Login()
        {
            InitializeComponent();

            displayUsers();
            earlyExit = true;

        }

        private void displayUsers()
        {
            foreach(var item in data.users)
            {
                User user = item.Value;
                if (!user.active) continue;
                ComboBoxItem newComboBoxItem = new ComboBoxItem();
                newComboBoxItem.Content = user.name;
                newComboBoxItem.Tag = user;
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
                MessageBox.Show("Please select a user");
                return;
            }

            User currentUser = (comboBox.SelectedItem as ComboBoxItem).Tag as User;

            string passwordAttempt = passwordTxt.Text;

            if(currentUser == null)
            {
                MessageBox.Show("User does not exist");
                return;
            }

            if(currentUser.reqPass)
            {
                if(!currentUser.correctPassword(passwordAttempt))
                {
                    MessageBox.Show("Incorrect Password");
                    passwordTxt.Text = "";
                    return;
                }
            }

            data.currentUser = currentUser;
            earlyExit = false;
            this.Close();
            

        }
    }
}
