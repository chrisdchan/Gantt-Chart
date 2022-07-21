using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.Models
{
    public class Model
    {
        public long rowid { get; set; }
        public String modelName { get; set; }
        public DateTime startDate { get; }
        public DateTime? endDate { get; set; }

        public DateTime lastUpdated { get; set; }
        public Task[] tasks { get; set; }
        public int lastCompletedTaskId { get; set; }

        // FROM RegModel
        public Model(string modelName, DateTime startDate)
        {
            this.modelName = modelName;
            this.startDate = startDate;
            this.endDate = null;
            this.lastCompletedTaskId = -1;
            lastUpdated = DateTime.Now;

            tasks = initDefaultTasks();
        }

        // FROM DATABASE
        public Model(long rowid, String modelName, DateTime startDate, DateTime? endDate,
            DateTime lastUpdated, Task[] tasks, int lastCompletedTaskId)
        {
            this.rowid = rowid; 
            this.modelName = modelName;
            this.startDate = startDate;
            this.endDate = endDate;
            this.lastUpdated = lastUpdated;
            this.tasks = tasks;
            this.lastCompletedTaskId = lastCompletedTaskId;
        }

        //FROM EXCEL
        public Model(string modelName, DateTime startDate, Task[] tasks, int lastCompletedTaskId)
        {
            this.modelName = modelName;
            this.startDate = startDate;
            this.tasks = tasks;
            this.lastCompletedTaskId = lastCompletedTaskId;
            endDate = null;
            lastUpdated = DateTime.Now;

            for(int i = 0; i < tasks.Length; i++)
            {
                if(!tasks[i].completed)
                {
                    lastCompletedTaskId = i;
                    break;
                }
            }

            if(lastCompletedTaskId == tasks.Length - 1)
            {
                endDate = tasks[lastCompletedTaskId].endDate;
            }
        }

        public void addTasks(Task[] tasks)
        {
            this.tasks = tasks;
            lastUpdated = DateTime.Now;
        }

        private Task[] initDefaultTasks()
        {
            Task[] tasks = new Task[data.NTASKS];

            DateTime plannedStart = startDate;
            DateTime plannedEnd = startDate;

            tasks = new Task[data.NTASKS];
            for(int i = 0; i < data.NTASKS; i++)
            {
                plannedEnd = plannedStart.AddDays(data.taskSettingsDuration[i]);
                Task newTask = new Task(i, plannedStart, plannedEnd);
                tasks[i] = newTask;
                plannedStart = plannedEnd;
            }

            return tasks;
        }

        public void completeTask(User user, int taskTypeId, DateTime startDate, DateTime endDate)
        {
            tasks[taskTypeId].complete(user, startDate, endDate);
            lastCompletedTaskId = taskTypeId;
            lastUpdated = DateTime.Now;
        }

        public void uncompleteTask(int taskTypeId)
        {

            if(taskTypeId > lastCompletedTaskId)
            {
                tasks[taskTypeId].uncomplete();
            }
            else
            {
                for(int i = lastCompletedTaskId; i >= taskTypeId; i--)
                {
                    tasks[i].uncomplete();
                }
            }

            if(taskTypeId == 0)
            {
                lastCompletedTaskId = -1;
            }
            else
            {
                lastCompletedTaskId = taskTypeId - 1;
            }
            lastUpdated = DateTime.Now;
        }

    }
}
