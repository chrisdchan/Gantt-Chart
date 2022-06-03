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
            displayUsers();
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
                MessageBox.Show("Please select a user");
                return;
            }

            int userIndex = (int)((ComboBox)comboBox.SelectedItem).SelectedIndex;
            User currentUser = users[userIndex];
            string passwordAttempt = passwordTxt.Text;
            if(currentUser == null)
            {
                MessageBox.Show("User does not exist");
                return;
            }
            else if(!currentUser.correctPassword(passwordAttempt))
            {
                MessageBox.Show("Incorrect Password");
                passwordTxt.Text = "";
            }
            else
            {
                data.currentUser = users[userIndex];
                earlyExit = false;
                this.Close();
            }
            

        }
    }
}
