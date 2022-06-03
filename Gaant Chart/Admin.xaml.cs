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

        public Admin()
        {
            InitializeComponent();

            navAddTeamMemberBtn.Foreground = notToggleColor;
            navRemoveModelBtn.Foreground = notToggleColor;

            populateDelteModelComoboBox();
        }

        private void createUserBtn_Click(object sender, RoutedEventArgs e)
        {

            

        }

        private void navAddTeamMemberBtn_Click(object sender, RoutedEventArgs e)
        {
            navAddTeamMemberBtn.Foreground = toggleColor;
            navRemoveModelBtn.Foreground = notToggleColor;
            addTeamMemberGrid.Visibility = Visibility.Visible;
            removeModelGrid.Visibility = Visibility.Hidden;
        }

        private void navRemoveModelBtn_Click(object sender, RoutedEventArgs e)
        {
            navAddTeamMemberBtn.Foreground = notToggleColor;
            navRemoveModelBtn.Foreground = toggleColor;
            removeModelGrid.Visibility = Visibility.Visible;
            addTeamMemberGrid.Visibility = Visibility.Hidden;
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
    }
}
