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

            earlyExit = true;

        }

        private DateTime computeStartDate()
        {
            if(task.typeInd == 0)
            {
                return model.startDate;
            }
            else
            {
                return model.tasks[model.lastCompletedTaskId].endDate;
            }
        }

        private void completeTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            String dateString = dateTbx.Text;

            if(String.IsNullOrEmpty(dateString))
            {
                MessageBoxResult res = System.Windows.MessageBox.Show("Would you like to use today's date as the start date?", "Invalid Date", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes)
                {
                    endDate = DateTime.Now;
                }
                else
                {
                    System.Windows.MessageBox.Show("Please Enter a valid date");
                    return;
                }
            }
            else if(!DateTime.TryParse(dateString, out endDate))
            {
                MessageBox.Show("Invalid Date Form, please use MM-dd-yyyy");
                dateTbx.Text = "";
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
            TaskCompletor taskCompletor = new TaskCompletor(user, startDate, endDate);
            task.complete(taskCompletor);
        }

        private void updateDatabase()
        {
            MainWindow.myDatabase.completeTask(task.rowid, endDate);
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();   
        }
    }
}
