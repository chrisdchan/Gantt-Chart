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
        public String password;
        public Boolean reqPass;
        public User(string name, string password, bool reqPass)
        {
            this.name = name;
            this.password = password;
            this.reqPass = reqPass;
        }
        public User(int userId, string name, string password, bool reqPass)
        {
            this.userId = userId;
            this.name = name;
            this.password = password;
            this.reqPass = reqPass;
        }

        public Boolean correctPassword(string _password)
        {
            return password == _password;
        }
    }
}
