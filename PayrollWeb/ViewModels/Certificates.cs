using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class Certificates
    {
        [DisplayName("Income Year")]
        public int fiscalYr_id { get; set; }
        public string SelectedEmployeesOnly { get; set; }

    }
}