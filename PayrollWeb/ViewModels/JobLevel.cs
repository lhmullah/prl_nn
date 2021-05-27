using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class JobLevel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
    }
}