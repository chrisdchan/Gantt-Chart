using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart
{
    public class AppSettings
    {
        // tasks dictionary will store key value pairs <task name, planned durration in days>
        public Dictionary<string, int> tasks { get; set; }

        // priority dictionary will store key value pairs <pre-requisite task, task> 
        public Dictionary<string, string> priority { get; set; }
    }
}
