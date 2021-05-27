using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class TimeCardRawView
    {
        public List<TimeCard> Cards { get; set; }
        public List<string> ErroList { get; set; }
    }
}