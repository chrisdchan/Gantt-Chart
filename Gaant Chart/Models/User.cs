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

        public Boolean active;
        public String initials;

        public String category { get; set; }

        // FROM EXCEL
        public User(string name, string initials, string category, Boolean active)
        {
            this.name = name;
            this.initials = initials;
            this.active = active;
            this.category = category;

            reqPass = false;
            password = "";
            authorization = data.categoryAuthorization[category];
        }

        // FROM DB
        public User(long rowid,
                    string name,
                    string initials,
                    string password,
                    Boolean reqPass,
                    string category)
        {
            this.rowid = rowid;
            this.name = name;
            this.initials = initials;
            this.password = password;
            this.reqPass = reqPass;
            this.category = category;
        }

        // FROM Admin
        public User(string name, string initials, string password, Boolean reqPass, string category, Boolean[] authorization)
        {
            this.name = name;
            this.initials = initials;
            this.password = password;
            this.reqPass = reqPass;
            this.category = category;
            this.authorization = authorization;

            active = true;
        }

        public void authorize(Boolean[] authorization)
        {
            this.authorization = authorization;
        }

        public Boolean correctPassword(string password)
        {
            return this.password == password;
        }
    }
}
