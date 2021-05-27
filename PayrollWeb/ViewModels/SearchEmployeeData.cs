using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    [Serializable]
    public class SearchEmployeeData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string empno { get; set; }
    }
}