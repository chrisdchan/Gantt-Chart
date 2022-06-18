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
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {

        private SolidColorBrush notToggleColor = new SolidColorBrush(Color.FromRgb(131, 131, 131));
        private SolidColorBrush toggleColor = new SolidColorBrush(Colors.Black);

        private User editUser;

        public Admin()
        {
            InitializeComponent();

            navAddTeamMemberBtn.Foreground = toggleColor;
            navRemoveModelBtn.Foreground = notToggleColor;

            initAuthorizationCheckBoxes();

            initEditUserCombobox();

            populateDelteModelComoboBox();
        }

        private void createUserBtn_Click(object sender, RoutedEventArgs e)
        {
            String name = nameTxt.Text;
            if(name == String.Empty)
            {
                MessageBox.Show("Name cannot be blank");
                return;
            }
            
            String password = passwordTxt.Text;
            if(password == String.Empty)
            {
                MessageBox.Show("Password cannot be blank");
                return;
            }

            Boolean reqPass = (bool)reqPassCheckBox.IsChecked;
            Boolean[] authorization = setAuthorization();

            User user = new User(name, password, reqPass, authorization);

            MainWindow.myDatabase.insertUser(user);
            nameTxt.Text = "";
            passwordTxt.Text = "";


            data.users.Add(user.rowid, user);

        }

        private void initAuthorizationCheckBoxes()
        {
            foreach(String taskname  in data.allTasks)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Height = 15;
                checkbox.Margin = new Thickness(0, 5, 0, 5);
                checkbox.Content = taskname;
                checkbox.IsChecked = true;
                authorizationSP.Children.Add(checkbox);
            }
        }

        private Boolean[] setAuthorization()
        {
            Boolean[] authorization = new bool[data.allTasks.Length];

            for(int i = 0; i < authorizationSP.Children.Count; i++)
            {
                CheckBox checkbox = authorizationSP.Children[i] as CheckBox;
                authorization[i] = (Boolean) checkbox.IsChecked;
            }

            return authorization;
        }

        private Boolean[] setEditAuthorization()
        {
            Boolean[] authorization = new bool[data.allTasks.Length];

            for(int i = 0; i < authorizationSP.Children.Count; i++)
            {
                CheckBox checkbox = editAuthorizationSP.Children[i] as CheckBox;
                authorization[i] = (Boolean) checkbox.IsChecked;
            }

            return authorization;

        }

        private void navAddTeamMemberBtn_Click(object sender, RoutedEventArgs e)
        {
            resetToNothing();
            navAddTeamMemberBtn.Foreground = toggleColor;
            addTeamMemberGrid.Visibility = Visibility.Visible;
        }

        private void navRemoveModelBtn_Click(object sender, RoutedEventArgs e)
        {
            resetToNothing();
            navRemoveModelBtn.Foreground = toggleColor;
            removeModelGrid.Visibility = Visibility.Visible;
        }
        private void navEditUserBtn_Click(object sender, RoutedEventArgs e)
        {
            resetToNothing();
            navEditUserBtn.Foreground = toggleColor;
            editUserGrid.Visibility = Visibility.Visible;

        }

        private void resetToNothing()
        {
            navAddTeamMemberBtn.Foreground = notToggleColor;
            navRemoveModelBtn.Foreground = notToggleColor;
            navEditUserBtn.Foreground = notToggleColor;
            removeModelGrid.Visibility = Visibility.Hidden;
            addTeamMemberGrid.Visibility = Visibility.Hidden;
            editUserGrid.Visibility = Visibility.Hidden;

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(deleteModelComboBox.SelectedIndex > 0)
            {
                deleteModelComboBox.Foreground = toggleColor;
                (deleteModelComboBox.Items[0] as ComboBoxItem).Foreground = notToggleColor;
            }
            else
            {
                deleteModelComboBox.Foreground = notToggleColor;
            }
        }


        private void populateDelteModelComoboBox()
        {
            List<ModelTag> modelTags = MainWindow.myDatabase.getModelTags();
            foreach(ModelTag modelTag in modelTags)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = modelTag.name;
                comboBoxItem.Tag = modelTag.id;
                comboBoxItem.Foreground = toggleColor;
                deleteModelComboBox.Items.Add(comboBoxItem);
            }
        }
        private void unpopulateDeleteModelComboBox()
        {
            ComboBoxItem descriptionComboBoxItem = deleteModelComboBox.Items[0] as ComboBoxItem;
            deleteModelComboBox.Items.Clear();
            deleteModelComboBox.Items.Add(descriptionComboBoxItem);
        }

        private void deleteModelBtn_Click(object sender, RoutedEventArgs e)
        {
            int modelId = (int)(deleteModelComboBox.SelectedItem as ComboBoxItem).Tag;
            MainWindow.myDatabase.deleteModel(modelId);
            unpopulateDeleteModelComboBox();
            populateDelteModelComoboBox();
        }

        private void initEditUserCombobox()
        {
            foreach(var item in data.users)
            {
                ComboBoxItem comboboxitem = new ComboBoxItem();
                comboboxitem.Content = item.Value.name;
                comboboxitem.Tag = item.Value;
                editUserCombobox.Items.Add(comboboxitem);
            }
        }

        private void editUserCombobox_Selected(object sender, RoutedEventArgs e)
        {
            ComboBoxItem comboBoxItem = editUserCombobox.SelectedItem as ComboBoxItem;
            if (comboBoxItem == null) return;
            User user = comboBoxItem.Tag as User;
            if (user == null) return;

            initEditUserInfo(user);
            editUser = user;
        }

        private void initEditUserInfo(User user)
        {
            editAuthorizationSP.Children.Clear();
            for(int i = 0; i < data.allTasks.Length; i++)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = data.allTasks[i];
                checkbox.IsChecked = user.authorization[i];
                checkbox.Height = 15;
                checkbox.Margin = new Thickness(0, 5, 0, 5);
                editAuthorizationSP.Children.Add(checkbox);
            }

            editPasswordTxt.Text = user.password;
            editReqPassCheckbox.IsChecked = user.reqPass;

        }

        private void editCreateUserBtn_Click(object sender, RoutedEventArgs e)
        {
            
            String password = editPasswordTxt.Text;
            if(password == String.Empty)
            {
                MessageBox.Show("Password cannot be blank");
                return;
            }

            Boolean reqPass = (bool)reqPassCheckBox.IsChecked;
            Boolean[] authorization = setEditAuthorization();

            editUser.password = password;
            editUser.authorization = authorization;
            editUser.reqPass = reqPass;

            data.users[editUser.rowid] = editUser;

            MainWindow.myDatabase.updateUser(editUser);


            editAuthorizationSP.Children.Clear();
            editPasswordTxt.Text = "";
            editReqPassCheckbox.IsChecked = false;
            editUserCombobox.SelectedIndex = -1;



        }
    }
}
