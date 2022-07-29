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

namespace Gaant_Chart.Views
{
    /// <summary>
    /// Interaction logic for EditTask.xaml
    /// </summary>
    public partial class EditTask : Window
    {
        private Models.Task task { get; set; }
        private Model model { get; set; }

        public Boolean exitEarlyFlag = true;
        public EditTask(Models.Task task)
        {
            InitializeComponent();
            addUsersToComboBox();

            this.task = task;
            model = data.currentModel;
            header.Text = "Edit " + task.name;
        }
        private void addUsersToComboBox()
        {
            foreach(KeyValuePair<long, User> kvp in data.users)
            {
                User user = kvp.Value;
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Height = 30;
                comboBoxItem.FontSize = 16;
                comboBoxItem.Content = user.name;
                comboBoxItem.Tag = user;
                selectUserComboBox.Items.Add(comboBoxItem);
            }
        }
        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            User user = (selectUserComboBox.SelectedItem as ComboBoxItem).Tag as User;

            if(datePicker.SelectedDate == null)
            {
                MessageBox.Show("ERROR: No date entered");
                return;
            }

            DateTime endDate = (DateTime)datePicker.SelectedDate;

            String hourString = hoursTxt.Text;
            String minuteString = minutesTxt.Text;

            double hours = 0;
            double minutes = 0;

            if (String.IsNullOrEmpty(hourString) || String.IsNullOrEmpty(minuteString))
            {
                MessageBox.Show("No Time Specified");
                hoursTxt.Text = "";
                minutesTxt.Text = "";
                return;
            }
            else if(!Double.TryParse(hourString, out hours))
            {
                MessageBox.Show("Invalid Time, please use numbers");
                hoursTxt.Text = "";
                minutesTxt.Text = "";
                return;
            }
            else if(!Double.TryParse(minuteString, out minutes))
            {
                MessageBox.Show("Invalid Time, please use numbers");
                hoursTxt.Text = "";
                minutesTxt.Text = "";
                return;
            }

            if(timeComboBox.SelectedIndex == 1)
            {
                hours += 12;
            }

            endDate = CanvasGraph.floorDate(endDate);
            endDate = endDate.AddHours(hours).AddMinutes(minutes);

            if(task.typeInd == 0 && endDate < model.startDate)
            {
                MessageBox.Show("INVALID DATE: Cannot complete a task before the model start date (" + model.startDate.ToString() + ")");
                datePicker.SelectedDate = null;
                return;
            }

            if(task.typeInd != 0 && endDate < model.tasks[task.typeInd - 1].endDate)
            {
                DateTime lastCompleted = (DateTime)model.tasks[task.typeInd - 1].endDate;
                MessageBox.Show("INVALID DATE: Cannot complete a task before a prerequisite task was completed (" + lastCompleted.ToString() + ")");
                datePicker.SelectedDate = null;
                return;
            }

            if(task.typeInd != data.NTASKS - 1 && model.tasks[task.typeInd + 1].completed)
            {
                DateTime nextCompleted = (DateTime)model.tasks[task.typeInd + 1].endDate;
                if(endDate > nextCompleted)
                {
                    MessageBox.Show("INVALID DATE: Cannot complete a task after the next task is completed (" +
                        nextCompleted.ToString() + ")");
                }
            }

            updateTaskLocally(endDate, user);
            updateDatabase();
            exitEarlyFlag = false;

            this.Close();
        }
        private DateTime computeStartDate(DateTime endDate)
        {
            DateTime startDate;
            if(model.lastCompletedTaskId == -1 || model.lastCompletedTaskId == 0)
            { 
                startDate = model.startDate;
            }
            else
            {
                startDate = (DateTime)model.tasks[model.lastCompletedTaskId].endDate.Value;
                if((endDate - startDate).TotalDays < 0.5)
                {
                    startDate = ((DateTime)model.tasks[model.lastCompletedTaskId].endDate).AddDays(-1);
                }
            }
            return startDate;
        }
        private void updateTaskLocally(DateTime endDate, User user)
        {
            DateTime startDate = computeStartDate(endDate);
            task.complete(user, startDate, endDate);
        }
        private void updateDatabase()
        {
            MainWindow.myDatabase.updateModel(model);
        }
        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
