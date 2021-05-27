using System;
using System.Collections.Generic;

namespace com.linde.Model
{
    public partial class  prl_users
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public Nullable<int> emp_id { get; set; }
        public string email { get; set; }
        public string role_name { get; set; }
        public string password { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_by { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        //public virtual ICollection<prl_employee> prl_employee { get; set; }
    }
}
