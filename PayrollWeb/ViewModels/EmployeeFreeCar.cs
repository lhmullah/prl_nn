using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayrollWeb.Utility;

namespace PayrollWeb.ViewModels
{
    public class EmployeeFreeCar
    {
        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        [DisplayName("Employee Name")]
        public int emp_id { get; set; }

        [DisplayName("Active Status")]
        public string is_active { get; set; }
        
        [DisplayName("Is Projected")]
        public string is_projected { get; set; }
        
        [DisplayName("Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime start_date { get; set; }
        
        [DisplayName("End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> end_date { get; set; }
        
        public Employee prl_employee { get; set; }
    }
}