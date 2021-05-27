using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class Menu
    {
        public int id { get; set; }
        public string menue_name { get; set; }
        public string remarks { get; set; }
        public string module { get; set; }
    }
}