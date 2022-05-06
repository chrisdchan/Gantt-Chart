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
       
        public LoadExistingModel(List<(String, int)> ModelNames)
        {
            InitializeComponent();
            earlyExist = true;
            
            if (ModelNames == null) return;
            foreach((String, int) modelName in ModelNames)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = modelName.Item1;
                comboBoxItem.Tag = modelName.Item2;
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
                int modelId = (int)((ComboBoxItem)myComboBox.SelectedItem).Tag;
                data.currentModel = MainWindow.myDatabase.GetModel(modelId);
                earlyExist = false;
                this.Close();
            }
        }
    }
}
