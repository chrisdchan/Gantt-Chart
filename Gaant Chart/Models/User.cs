using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaant_Chart.Models
{
    public class User
    {
        public long rowid { get; set; } 
        public String name;
        public String password;
        public Boolean reqPass;

        public Boolean[] authorization;

        public User(string name, string password, bool reqPass)
        {
            this.name = name;
            this.password = password;
            this.reqPass = reqPass;
        }
        public User(long rowid, string name, string password, bool reqPass) : this(name, password, reqPass)
        {
            this.rowid = rowid;
        }

        public User(string name, string password, bool reqPass, Boolean[] authorization) : this(name, password, reqPass)
        {
            this.authorization = authorization;
        }

        public Boolean correctPassword(string _password)
        {
            return password == _password;
        }
    }
}
