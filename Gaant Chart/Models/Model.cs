using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.Models
{
    public class Model
    {
        public int modelId { get; set; }
        public String modelName { get; set; }
        public DateTime startDate { get; }

        public List<Task> tasks = new List<Task>();
        public int lastCompletedTaskId { get; set; }

        public Model(int modelId, String modelName, DateTime startDate)
        {
            this.modelId = modelId; 
            this.modelName = modelName;
            this.startDate = startDate;
            this.lastCompletedTaskId = -1;

            DateTime plannedStart = startDate;
            DateTime plannedEnd = startDate;
            for(int i = 0; i < data.allTasks.Length; i++)
            {
                plannedEnd = plannedStart.AddDays(data.taskSettingsDuration[i]);
                Task newTask = new Task(data.allTasks[i], plannedStart, plannedEnd);
                tasks.Add(newTask);
                plannedStart = plannedEnd.AddDays(1);
            }
        }

        public void completeTask(int taskId, DateTime endDate, User user)
        {
            if (taskId <= lastCompletedTaskId) throw new Exception("Attempting to complete an already completed task");

            lastCompletedTaskId = taskId;

            DateTime taskStartDate = startDate;

            if (taskId != 0) taskStartDate = tasks[taskId - 1].endDate;

            tasks[taskId].complete(taskStartDate, endDate, user);
        }

        public void removeTask(int taskId)
        {
            for(int i = lastCompletedTaskId; i >= taskId; i--)
            {
                tasks[i].remove();
            }
            lastCompletedTaskId = taskId - 1;
        }
    }
}
