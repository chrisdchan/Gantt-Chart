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

        public List<Task> tasks = new List<Task>();
        public int lastCompletedTaskId { get; set; }

        public Model(long rowid, String modelName, DateTime startDate)
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

        public void completeTask(TaskCompletor taskCompletor)
        {
            tasks[taskCompletor.taskTypeId].complete(taskCompletor);
            lastCompletedTaskId = taskCompletor.taskTypeId;
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
        }

    }

    public class TaskCompletor
    {
        public User user { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public int taskTypeId { get; set; }

        public TaskCompletor(int taskTypeId, User user, DateTime startDate, DateTime endDate)
        {
            this.user = user;
            this.startDate = startDate;
            this.endDate = endDate;
            this.taskTypeId = taskTypeId;
        }

       public TaskCompletor(Task task, User user, DateTime startDate, DateTime endDate) : this(task.typeInd, user, startDate, endDate) { }
    }
}
