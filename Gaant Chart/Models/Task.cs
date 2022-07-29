using System;

namespace Gaant_Chart.Models
{
    public class Task
    {
        public long rowid { get; set; }
        public readonly String name;
        public int typeInd { get; set; }
        public User completedUser { get; set; }
        public User assignedUser { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
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

            startDate = null;
            endDate = null;
            completedUser = null;
            assignedUser = null;
            rowid = -1;
        }

        public Task(long rowid, int typeInd, 
            DateTime plannedStartDate, DateTime plannedEndDate,
            DateTime? startDate, DateTime? endDate,
            User assignedUser, User completedUser)
        {
            this.rowid = rowid;
            this.typeInd = typeInd;
            this.plannedStartDate = plannedStartDate;
            this.plannedEndDate = plannedEndDate;
            this.startDate = startDate;
            this.endDate = endDate;
            this.assignedUser = assignedUser;
            this.completedUser = completedUser;

            name = data.allTasks[typeInd];
            if (completedUser != null) completed = true;
            
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
            startDate = null;
            endDate = null;
            completed = false;
            completedUser = null;
        }

        public void assign(User user)
        {
            assignedUser = user;
        }
     }

}
