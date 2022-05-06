using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.Models
{
    public class Task
    {
        private readonly String name;
        private User user { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime plannedStartDate { get; set; }
        public DateTime plannedEndDate { get; set; }

        public Task(string name, DateTime plannedStartDate, DateTime plannedEndDate)
        {
            this.name = name;
            this.plannedStartDate = plannedStartDate;
            this.plannedEndDate = plannedEndDate;

            this.startDate = DateTime.MinValue;
            this.endDate = DateTime.MinValue;
            this.user = null;
        }

        public void complete(DateTime startDate, DateTime endDate, User user)
        {
            this.startDate = startDate;
            this.endDate = endDate;
            this.user = user;
        }

        public void remove()
        {
            this.user = null;
            this.startDate = DateTime.MinValue;
            this.endDate = DateTime.MinValue;
        }

     }
}
