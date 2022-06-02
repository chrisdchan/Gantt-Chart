using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.Models
{
    public class ModelTag
    {
        public String name { get; }
        public int id { get;  }

        public ModelTag(int id, String name)
        {
            this.name = name;
            this.id = id;
        }
    }
}
