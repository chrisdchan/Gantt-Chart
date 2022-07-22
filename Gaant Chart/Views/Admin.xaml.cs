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


        public Boolean deletedCurrentModel;
        public Boolean updatedCurrentUser;


        private Boolean isCategoryResetOn = false;
        private Boolean isEditCategoryResetOn = false;

        public Admin()
        {
            InitializeComponent();

            hideAllComponents();

            initAuthorizationCheckBoxes();

            initEditUserCombobox();

            populateDelteModelComoboBox();

            initCategoryComboBox();

            deletedCurrentModel = false;
            updatedCurrentUser = false;
        }

        private void hideAllComponents()
        {
            addTeamMemberGrid.Visibility = Visibility.Hidden;
            removeModelGrid.Visibility = Visibility.Hidden;
            editUserGrid.Visibility = Visibility.Hidden;
        }

        private void createUserBtn_Click(object sender, RoutedEventArgs e)
        {
            String name = nameTxt.Text;
            if(name == String.Empty)
            {
                MessageBox.Show("Name cannot be blank");
                return;
            }
            
            Boolean reqPass = (bool)reqPassCheckBox.IsChecked;

            String password = passwordTxt.Text;
            if(password == String.Empty && reqPass)
            {
                MessageBox.Show("Password cannot be blank if password is required");
                return;
            }

            String initials = initialsTxt.Text;
            String category;
            if (addUserCategoryComboBox.SelectedItem == null)
                category = null;
            else
                category = (addUserCategoryComboBox.SelectedItem as ComboBoxItem).Content as String;

            Boolean[] authorization = setAuthorization();

            User user = new User(name, initials, password, reqPass, category, authorization);

            if(!MainWindow.myDatabase.isUserExist(user))
                MainWindow.myDatabase.insertUser(user);

            nameTxt.Text = "";
            passwordTxt.Text = "";

            foreach(CheckBox checkbox in authorizationSP.Children)
            {
                checkbox.IsChecked = true;
            }


            data.users.Add(user.rowid, user);

        }
        private void initAuthorizationCheckBoxes()
        {
            foreach(String taskname in data.allTasks)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Height = 15;
                checkbox.Margin = new Thickness(0, 5, 0, 5);
                checkbox.Content = taskname;
                checkbox.Checked += new RoutedEventHandler(authorizedCheckBoxChanged);
                checkbox.Unchecked += new RoutedEventHandler(authorizedCheckBoxChanged);
                checkbox.IsChecked = true;
                authorizationSP.Children.Add(checkbox);
            }
        }
        private void initCategoryComboBox()
        {
            foreach(var kvp in data.categoryAuthorization)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = kvp.Key;
                addUserCategoryComboBox.Items.Add(comboBoxItem);

                ComboBoxItem editComboBoxItem = new ComboBoxItem();
                editComboBoxItem.Content = kvp.Key;
                editUserCategoryComboBox.Items.Add(editComboBoxItem);
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

            for(int i = 0; i < editAuthorizationSP.Children.Count; i++)
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
        private void authorizedCheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if(isCategoryResetOn)
                addUserCategoryComboBox.SelectedItem = null;
        }
        private void editAuthorizedCheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if(isEditCategoryResetOn)
                editUserCategoryComboBox.SelectedItem = null;
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
            deletedCurrentModel = true;
        }
        private void initEditUserCombobox()
        {
            foreach(var kvp in data.users)
            {
                User user = kvp.Value;
                ComboBoxItem comboboxitem = new ComboBoxItem();
                comboboxitem.Content = user.name;
                comboboxitem.Tag = user;
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
                checkbox.Checked += new RoutedEventHandler(editAuthorizedCheckBoxChanged);
                checkbox.Unchecked += new RoutedEventHandler(editAuthorizedCheckBoxChanged);
                editAuthorizationSP.Children.Add(checkbox);
            }

            editPasswordTxt.Text = user.password;
            editReqPassCheckbox.IsChecked = user.reqPass;
            editUserActiveComboBox.IsChecked = user.active;
            editUserCategoryComboBox.SelectedItem = getEditUserCategoryComboBoxItem(user.category);
        }

        private ComboBoxItem getEditUserCategoryComboBoxItem(String category)
        {
            foreach(ComboBoxItem comboBoxItem in editUserCategoryComboBox.Items)
            {
                if ((String)comboBoxItem.Content == category) return comboBoxItem;
            }
            return null;
        }
        private void updateUserBtn_Click(object sender, RoutedEventArgs e)
        {
            
            String password = editPasswordTxt.Text;
            Boolean reqPass = (bool)editReqPassCheckbox.IsChecked;
            Boolean active = (bool)editUserActiveComboBox.IsChecked;
            Boolean[] authorization = setEditAuthorization();
            String category;
            if (editUserCategoryComboBox.SelectedItem != null)
                category = (String)(editUserCategoryComboBox.SelectedItem as ComboBoxItem).Content;
            else
                category = null;

            editUser.password = password;
            editUser.authorization = authorization;
            editUser.reqPass = reqPass;
            editUser.category = category;
            editUser.active = active;

            MainWindow.myDatabase.updateUser(editUser);


            editAuthorizationSP.Children.Clear();
            editPasswordTxt.Text = "";
            editReqPassCheckbox.IsChecked = false;
            editUserCombobox.SelectedIndex = -1;

            if(editUser == data.currentUser)
            {
                updatedCurrentUser = true;
            }
        }
        private void categoryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addUserCategoryComboBox.SelectedItem == null) return;
            String category = (addUserCategoryComboBox.SelectedItem as ComboBoxItem).Content as String;
            Boolean[] authorization = data.categoryAuthorization[category];

            isCategoryResetOn = false;

            for(int i = 0; i < authorizationSP.Children.Count; i++)
            {
                CheckBox checkbox = authorizationSP.Children[i] as CheckBox;
                checkbox.IsChecked = authorization[i];
            }

            isCategoryResetOn = true;
        }
        private void editUserCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (editUserCategoryComboBox.SelectedItem == null) return;
            String category = (editUserCategoryComboBox.SelectedItem as ComboBoxItem).Content as String;
            Boolean[] authorization = data.categoryAuthorization[category];

            isEditCategoryResetOn = false;

            for(int i = 0; i < editAuthorizationSP.Children.Count; i++)
            {
                CheckBox checkbox = editAuthorizationSP.Children[i] as CheckBox;
                checkbox.IsChecked = authorization[i];
            }
            isEditCategoryResetOn = true;
        }
    }
}
