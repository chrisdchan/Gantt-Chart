using Gaant_Chart.Models;
using Gaant_Chart.Structures;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gaant_Chart
{
    public partial class LoadExistingModel : Window
    {
        public Boolean earlyExist { get; set; }
        private Boolean firstFocus = true;

        private SolidColorBrush GRAY = new SolidColorBrush(Colors.Gray);
        private ModelNameTrie trie { get; set; }
       
        public LoadExistingModel(List<ModelTag> modelTags)
        {
            InitializeComponent();
            earlyExist = true;

            if (modelTags == null) return;

            trie = new ModelNameTrie();

            foreach(ModelTag modelTag in modelTags)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = modelTag.name;
                comboBoxItem.Tag = modelTag.id;
                comboBoxItem.FontSize = 15;
                //myComboBox.Items.Add(comboBoxItem);

                trie.insert(modelTag.name, modelTag.id);
            }

            populateModelList("");
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            earlyExist = true;
            Close();
        }

        private void btn_Open_Click(object sender, RoutedEventArgs e)
        {
            //if(myComboBox.SelectedItem == null)
            //{
            //    MessageBox.Show("Please select a model");
            //}
            //else 
            //{
            //    int modelId = (int)(myComboBox.SelectedItem as ComboBoxItem).Tag;
            //    data.currentModel = MainWindow.myDatabase.getModel(modelId);
            //    earlyExist = false;
            //    Close();
            //}
        }

        private Boolean dontSearch = false;
        private void searchTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dontSearch) return;
            if (trie == null) return;
            if (modelNamesSP == null) return;


            modelNamesSP.Children.Clear();

            String text = searchTxt.Text;
            populateModelList(text);

        }

        private void populateModelList(String text)
        {
            List<(String, long)> suggestions = trie.suggest(text);

            int n = Math.Min(suggestions.Count, 5);

            for(int i = 0; i < n; i++)
            {
                (String name, long id) = suggestions[i];

                Label label = new Label();
                label.Content = name;
                label.Tag = id;
                label.FontSize = 15;
                label.Width = 250;
                label.Height = 30;
                label.Background = new SolidColorBrush(Colors.Beige);
                modelNamesSP.Children.Add(label);
            }
        }
    }
}
