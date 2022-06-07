using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.Models
{
    public class Model
    {
        public int rowid { get; set; }
        public String modelName { get; set; }
        public DateTime startDate { get; }

        public List<Task> tasks = new List<Task>();
        public int lastCompletedTaskId { get; set; }

        public Model(int rowid, String modelName, DateTime startDate)
        {
            this.rowid = rowid; 
            this.modelName = modelName;
            this.startDate = startDate;
            this.lastCompletedTaskId = -1;

            DateTime plannedStart = startDate;
            DateTime plannedEnd = startDate;
            for(int i = 0; i < data.allTasks.Length; i++)
            {
                plannedEnd = plannedStart.AddDays(data.taskSettingsDuration[i]);
                Task newTask = new Task(i, plannedStart, plannedEnd);
                tasks.Add(newTask);
                plannedStart = plannedEnd;
            }
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
