using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.Models
{
    public class User
    {
        public int userId { get; set; } 
        public String name;
        private String password;

        public User(int userId, string name, string password)
        {
            this.userId = userId;
            this.name = name;
            this.password = password;
        }

        public Boolean correctPassword(string _password)
        {
            return password == _password;
        }
    }
}
