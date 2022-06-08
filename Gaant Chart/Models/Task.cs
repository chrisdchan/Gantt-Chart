using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.Models
{
    public class Task
    {
        public long rowid { get; set; }
        public readonly String name;
        public int typeInd { get; set; }
        public User user { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime plannedStartDate { get; set; }
        public DateTime plannedEndDate { get; set; }

        public Boolean completed = false;

        public Task(int rowid, int typeInd, DateTime plannedStartDate, DateTime plannedEndDate) : this(typeInd, plannedStartDate, plannedEndDate)
        {
            this.rowid = rowid;
        }

        public Task(int typeInd, DateTime plannedStartDate, DateTime plannedEndDate)
        {
            this.typeInd = typeInd;
            this.name = data.allTasks[typeInd];
            this.plannedStartDate = plannedStartDate;
            this.plannedEndDate = plannedEndDate;

            this.startDate = DateTime.MinValue;
            this.endDate = DateTime.MinValue;
            this.user = null;

            rowid = -1;
        }

        public void complete(TaskCompletor taskCompletor)
        {
            this.user = taskCompletor.user;
            this.startDate = taskCompletor.startDate;
            this.endDate = taskCompletor.endDate;

            completed = true;
        }

        public void uncomplete()
        {
            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;
            completed = false;
            user = null;
        }

        public void remove()
        {
            this.user = null;
            this.startDate = DateTime.MinValue;
            this.endDate = DateTime.MinValue;
        }
     }

}
