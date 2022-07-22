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

        public CompleteTask(Models.Task task)
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
            dateTbx.Focus();
        }
        private DateTime computeStartDate()
        {
            if(task.typeInd == 0)
            {
                return model.startDate;
            }

            DateTime startDate = (DateTime)model.tasks[model.lastCompletedTaskId].endDate;

            if((endDate - startDate).TotalDays < 0.5)
            {
                startDate = ((DateTime)model.tasks[model.lastCompletedTaskId].endDate).AddDays(-1);
            }

            return startDate;
        }

        private void completeTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            String dateString = dateTbx.Text;

            if(String.IsNullOrEmpty(dateString))
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
            else if(!DateTime.TryParse(dateString, out endDate))
            {
                MessageBox.Show("Invalid Date Form, please use MM-dd-yy hh:mm");
                dateTbx.Text = "";
                return;
            }

            if(task.typeInd == 0 && endDate <= model.startDate)
            {
                MessageBox.Show("INVALID DATE: Cannot complete a task before the model start date (" + model.startDate.ToString() + ")");
                dateTbx.Text = "";
                return;
            }

            if(task.typeInd != 0 && endDate < model.tasks[task.typeInd - 1].endDate)
            {
                DateTime lastCompleted = (DateTime)model.tasks[task.typeInd - 1].endDate;
                MessageBox.Show("INVALID DATE: Cannot complete a task before a prerequisite task was completed (" + lastCompleted.ToString() + ")");
                dateTbx.Text = "";
                return;
            }

            if(endDate.TimeOfDay.TotalSeconds == 0)
            {
                endDate.AddMinutes(1439);
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
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();   
        }
    }
}
