using Gaant_Chart.Components;
using Gaant_Chart.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Gaant_Chart
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Boolean earlyExit { get; set; }
        private PasswordTextBox passwordTextBox;
        public Login()
        {
            InitializeComponent();

            displayUsers();
            createPasswordTextBox();
            earlyExit = true;

        }
        private void createPasswordTextBox()
        {
            passwordTextBox = new PasswordTextBox();
            TextBox textbox = passwordTextBox.textbox;
            mainGrid.Children.Add(textbox);

            Grid.SetRow(textbox, 2);

            textbox.Margin = new Thickness(0, 50, 0, 0);
            textbox.Width = 300;
            textbox.Height = 50;
            textbox.FontSize = 18;
        }
        private void displayUsers()
        {
            foreach(var item in data.users)
            {
                User user = item.Value;
                if (!user.active) continue;
                ComboBoxItem newComboBoxItem = new ComboBoxItem();
                newComboBoxItem.Content = user.name;
                newComboBoxItem.FontSize = 18;
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

            if(currentUser == null)
            {
                MessageBox.Show("User does not exist");
                return;
            }

            if(currentUser.reqPass)
            {
                if(!currentUser.correctPassword(passwordTextBox.password))
                {
                    MessageBox.Show("Incorrect Password");
                    passwordTextBox.textbox.Text = "";
                    return;
                }
            }

            data.currentUser = currentUser;
            earlyExit = false;
            Close();
        }
    }
}
