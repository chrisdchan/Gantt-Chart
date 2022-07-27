using Gaant_Chart.Models;
using Gaant_Chart.Structures;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Gaant_Chart
{
    public partial class LoadExistingModel : Window
    {
        public Boolean earlyExist { get; set; }
        private Boolean firstFocus = true;
        private Label clickedLabel { get; set; }
        private Label selectedLabel { get; set; }

        private SolidColorBrush labelBackgroundColor = new SolidColorBrush(Color.FromRgb(215, 215, 215));
        private SolidColorBrush labelHoverColor = new SolidColorBrush(Color.FromRgb(235, 235, 235));
 
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

            for(int i = 0; i < suggestions.Count; i++)
            {
                (String name, long id) = suggestions[i];

                Label label = new Label();
                label.Content = name;
                label.Tag = id;
                label.FontSize = 18;
                label.Width = 225;
                label.Height = 45;
                label.Background = labelBackgroundColor;
                label.MouseEnter += new MouseEventHandler(onMouseEnter);
                label.MouseLeave += new MouseEventHandler(onMouseLeave);
                label.PreviewMouseDown += new MouseButtonEventHandler(onMouseDown);
                label.PreviewMouseUp += new MouseButtonEventHandler(onMouseUp);

                modelNamesSP.Children.Add(label);
            }
        }
        private void onMouseEnter(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            label.Background = labelHoverColor;
        }
        private void onMouseLeave(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            label.Background = labelBackgroundColor;
        }
        private Boolean clickdown = false;
        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            clickdown = true;
            clickedLabel = sender as Label;
        }
        private void onMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(clickdown)
            {
                Label thisLabel = sender as Label;
                if(thisLabel == clickedLabel)
                {
                    long modelId = (long)(thisLabel.Tag);
                    data.currentModel = MainWindow.myDatabase.getModel(modelId);
                    earlyExist = false;
                    Close();  
                }
            }
        }
    }
}
