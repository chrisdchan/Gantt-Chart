using System;

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
