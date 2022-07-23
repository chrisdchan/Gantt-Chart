using Gaant_Chart.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Gaant_Chart
{
    /// <summary>
    /// Interaction logic for LoadExistingModel.xaml
    /// </summary>

    public partial class LoadExistingModel : Window
    {
        public Boolean earlyExist { get; set; }
       
        public LoadExistingModel(List<ModelTag> modelTags)
        {
            InitializeComponent();
            earlyExist = true;

            if (modelTags == null) return;

            foreach(ModelTag modelTag in modelTags)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = modelTag.name;
                comboBoxItem.Tag = modelTag.id;
                myComboBox.Items.Add(comboBoxItem);
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            earlyExist = true;
            Close();
        }

        private void btn_Open_Click(object sender, RoutedEventArgs e)
        {
            if(myComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a model");
            }
            else 
            {
                int modelId = (int)(myComboBox.SelectedItem as ComboBoxItem).Tag;
                data.currentModel = MainWindow.myDatabase.getModel(modelId);
                earlyExist = false;
                Close();
            }
        }
    }
}
