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

        private int taskId;

        private User user;

        public CompleteTask(int taskId)
        {
            InitializeComponent();

            model = data.currentModel;
            user = data.currentUser;

            this.taskId = taskId;


        }

        private DateTime computeStartDate()
        {
            if(taskId == 0)
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

        }

        private void completeTaskLocally()
        {
            startDate = computeStartDate();
            TaskCompletor taskCompletor = new TaskCompletor(user, startDate, endDate);
            model.tasks[taskId].complete(taskCompletor);
        }

        private void updateDatabase()
        {

        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
