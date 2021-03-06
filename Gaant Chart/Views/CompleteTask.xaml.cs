using Gaant_Chart.Models;
using System;
using System.Windows;

namespace Gaant_Chart
{
    /// <summary>
    /// Interaction logic for CompleteTask.xaml
    /// </summary>
    public partial class CompleteTask : Window
    {
        private DateTime endDate;
        private DateTime startDate;
        private Model model;

        private Models.Task task;

        private User user;

        public Boolean earlyExit;

        public CompleteTask(Task task)
        {
            InitializeComponent();

            model = data.currentModel;
            user = data.currentUser;
            this.task = task;

            if (task.assignedUser != null)
                assignedTxtBlk.Text = "Assigned to " + task.assignedUser.name;
            else
                assignedTxtBlk.Text = "Unassigned Task";

            earlyExit = true;
        }
        private DateTime computeStartDate()
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
        private void completeTaskBtn_Click(object sender, RoutedEventArgs e)
        {

            if(datePicker.SelectedDate == null)
            {
                MessageBoxResult res = MessageBox.Show("Would you like to use today's date as the start date?", "Invalid Date", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes)
                {
                    endDate = DateTime.Now;
                }
                else
                {
                    MessageBox.Show("Please Enter a valid date");
                    return;
                }
            }
            else
            {
                endDate = (DateTime)datePicker.SelectedDate;
            }

            String hourString = hoursTxt.Text;
            String minuteString = minutesTxt.Text;

            double hours = 0;
            double minutes = 0;

            Boolean preSetTimeFlag = false;

            if (String.IsNullOrEmpty(hourString) || String.IsNullOrEmpty(minuteString))
            {
                MessageBoxResult res = MessageBox.Show("Would you like to use 11:59PM ?", "No Time Entered", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes)
                {

                    hours = 23;
                    minutes = 59;
                    preSetTimeFlag = true;
                }
                else
                {
                    hoursTxt.Text = "";
                    minutesTxt.Text = "";
                    return;
                }
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

            if(!preSetTimeFlag && timeComboBox.SelectedIndex == 1)
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


            completeTaskLocally();
            updateDatabase();

            earlyExit = false;

            this.Close();
        }
        private void completeTaskLocally()
        {
            startDate = computeStartDate();
            model.completeTask(user, task.typeInd, startDate, endDate);
        }
        private void updateDatabase()
        {
            MainWindow.myDatabase.completeTask(task.rowid, startDate, endDate);
            if(task.typeInd == data.allTasks.Length - 1)
            {
                MainWindow.myDatabase.completeModel(model);
            }
        }
        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();   
        }
    }
}
