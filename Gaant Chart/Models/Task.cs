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
        public long userCompletedId { get; set; }
        public long userAssignedId { get; set; }

        public User completedUser { get; set; }
        public User assignedUser { get; set; }

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

            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;
            completedUser = null;
            assignedUser = null;
            rowid = -1;
        }

        public void complete(User user, DateTime start, DateTime end)
        {
            completedUser = user;
            startDate = start;
            endDate = end;

            completed = true;
        }

        public void uncomplete()
        {
            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;
            completed = false;
            userCompletedId = -1;
        }

        public void assign(User user)
        {
            assignedUser = user;
        }
     }

}
