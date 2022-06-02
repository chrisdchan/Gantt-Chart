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
    /// Interaction logic for LoadExistingModel.xaml
    /// </summary>

    public partial class LoadExistingModel : Window
    {
        private List<(String, int)> ModelNames;

        private int modelId;

        public Boolean earlyExist { get; set; }

        public int getModelId()
        {
            return modelId;
        }
       
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
            this.Close();
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
                data.currentModel = MainWindow.myDatabase.GetModel(modelId);
                earlyExist = false;
                this.Close();
            }
        }
    }
}
