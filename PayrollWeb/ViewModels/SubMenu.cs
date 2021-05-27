using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class SubMenu
    {
        public int id { get; set; }
        public int menu_id { get; set; }
        public string sub_menu { get; set; }
        public string view_name { get; set; }
        public string controller_name { get; set; }
        public int parent_id { get; set; }
        public Nullable<int> level { get; set; }
        public string module { get; set; }

        public virtual Menu prl_menu { get; set; }
    }
}