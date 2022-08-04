using Gaant_Chart.DataStructures;
using Gaant_Chart.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

        public Boolean earlyExit;

        public Boolean changedUserFlag = false;

        public CompleteTask(Task task)
        {
            InitializeComponent();

            model = data.currentModel;
            this.task = task;

            header.Text = task.name;

            if (task.assignedUser != null)
                assignedTxtBlk.Text = "Assigned to " + task.assignedUser.name;
            else
                assignedTxtBlk.Text = "Unassigned Task";

            earlyExit = true;

            initRecommendedTime();
            addUsersToComboBox();

        }
        private void addUsersToComboBox()
        {
            ComboBoxItem selectedUser = null;
            foreach(KeyValuePair<long, User> kvp in data.users)
            {
                User user = kvp.Value;
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Height = 30;
                comboBoxItem.FontSize = 16;
                comboBoxItem.Content = user.name;
                comboBoxItem.Tag = user;
                selectUserComboBox.Items.Add(comboBoxItem);

                if(data.currentUser == user)
                {
                    selectedUser = comboBoxItem;
                }
            }
            selectUserComboBox.SelectedItem = selectedUser;
        }

        private void initRecommendedTime()
        {
            (DateTime date, int hour, int minute, int dayPeriod) = Conversions.toDateUI(computeRecommendedEndDate());
            datePicker.SelectedDate = date;
            hoursTxt.Text = hour.ToString();
            minutesTxt.Text = minute.ToString();
            timeComboBox.SelectedIndex = dayPeriod;

        }
        private DateTime computeRecommendedEndDate()
        {
            DateTime recommendation;
            int duration = data.taskSettingsDuration[task.typeInd];

            if(task.typeInd == 0)
            {
                recommendation = model.startDate.AddDays(duration);
            }
            else
            {
                recommendation = model.tasks[model.lastCompletedTaskId].endDate.Value.AddDays(duration);
            }
            return recommendation;
        }
        private DateTime computeStartDate()
        {
            DateTime startDate;
            if(model.lastCompletedTaskId == -1)
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
                datePicker.SelectedDate = model.startDate;
                return;
            }

            if(task.typeInd != 0 && endDate < model.tasks[task.typeInd - 1].endDate)
            {
                DateTime lastCompleted = (DateTime)model.tasks[task.typeInd - 1].endDate;
                MessageBox.Show("INVALID DATE: Cannot complete a task before a prerequisite task was completed (" + lastCompleted.ToString() + ")");
                datePicker.SelectedDate = lastCompleted;
                return;
            }

            if(selectUserComboBox.SelectedItem == null)
            {
                MessageBox.Show("No User selected");
                return;
            }

            User user = (selectUserComboBox.SelectedItem as ComboBoxItem).Tag as User;

            completeTaskLocally(user);
            updateDatabase();

            earlyExit = false;

            this.Close();
        }
        private void completeTaskLocally(User user)
        {
            startDate = computeStartDate();
            model.completeTask(user, task.typeInd, startDate, endDate);
        }
        private void updateDatabase()
        {
            MainWindow.myDatabase.completeTask(task);
            if(task.typeInd == data.allTasks.Length - 1)
            {
                MainWindow.myDatabase.completeModel(model);
            }
        }
        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();   
        }

        Boolean overRideSelect = false;
        private void selectUserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (overRideSelect) return;
            ComboBoxItem comboBoxItem = selectUserComboBox.SelectedItem as ComboBoxItem;
            User user = comboBoxItem.Tag as User;

            if(user != data.currentUser && user.reqPass)
            {
                Login loginWin = new Login(user);
                loginWin.ShowDialog();

                changedUserFlag = !loginWin.earlyExit;

                if (!loginWin.earlyExit)
                    setUserSelected(user);
                else
                    setUserToCurrent();
            }

            if(!user.authorization[task.typeInd])
            {
                MessageBox.Show("User is not authorized to complete this task");
                overRideSelect = true;
                selectUserComboBox.SelectedItem = null;
                overRideSelect = false;
                return;
            }
        }

        private void setUserSelected(User user)
        {
            overRideSelect = true;
            foreach(ComboBoxItem item in selectUserComboBox.Items)
                if(data.currentUser == item.Tag) selectUserComboBox.SelectedItem = item;
            overRideSelect = false;
        }

        private void setUserToCurrent()
        {
            overRideSelect = true;
            if(data.currentUser != null)
                setUserSelected(data.currentUser);
            else
                selectUserComboBox.SelectedItem = null;
            overRideSelect = false;
        }
    }
}
