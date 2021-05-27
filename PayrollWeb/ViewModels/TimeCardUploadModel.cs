using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class TimeCardUploadModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public List<int> YearList { get; set; }
        public List<string> ErrorList { get; set; }
    }
}