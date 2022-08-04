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
        public Boolean earlyExist { get; set;}
        private Label clickedLabel { get; set; }

        private SolidColorBrush labelBackgroundColor = new SolidColorBrush(Color.FromRgb(235, 235, 235));
        private SolidColorBrush labelHoverColor = new SolidColorBrush(Color.FromRgb(245, 245, 245));
        private SolidColorBrush BLACK = new SolidColorBrush(Colors.Black);
        private SolidColorBrush GRAY = new SolidColorBrush(Colors.Gray);
 
        private Trie trie { get; set; }
        public LoadExistingModel(List<ModelTag> modelTags)
        {
            InitializeComponent();
            earlyExist = true;

            if (modelTags == null) return;

            trie = new Trie();

            foreach(ModelTag modelTag in modelTags)
            {
                trie.insert(modelTag.name, modelTag.id);
            }
            populateModelList("");

            dontSearch = true;
            searchTxt.Text = "Search Database";
            dontSearch = false;
            searchTxt.Foreground = GRAY;
            searchTxt.Background = labelBackgroundColor;
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
            searchTxt.Foreground = BLACK;

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
                label.FontSize = 17;
                label.Width = 250;
                label.Height = 45;
                label.Padding = new Thickness(7, 0, 0, 0);
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalContentAlignment = VerticalAlignment.Center;
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
        private Boolean hasText = false;
        private void searchTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            dontSearch = true;
            searchTxt.Text = "";
            dontSearch = false;
        }
        private void searchTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            if(!hasText)
            {
                dontSearch = true;
                searchTxt.Foreground = GRAY;
                searchTxt.Text = "Search Database";
                populateModelList("");
                dontSearch = false;
            }
        }
    }
}
