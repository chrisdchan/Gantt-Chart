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

        public LoadExistingModel()
        {
            InitializeComponent();

            ModelNames = MainWindow.myDatabase.getModelNames();
            loadModelNames();
        }

        private void loadModelNames()
        {
            foreach((String, int) modelName in ModelNames)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = modelName.Item1;
                comboBoxItem.Tag = modelName;
                myComboBox.Items.Add(comboBoxItem);
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Open_Click(object sender, RoutedEventArgs e)
        {
            String ModelName = ((ComboBoxItem)myComboBox.SelectedItem).Tag.ToString();
            MessageBox.Show(ModelName);
        }
    }
}
