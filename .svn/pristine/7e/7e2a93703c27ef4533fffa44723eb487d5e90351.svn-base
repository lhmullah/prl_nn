using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class Company
    {
        [HiddenInput(DisplayValue=true)]
        public int id { get; set; }

        [Required(ErrorMessage = "Company name can not be empty.")]
        [DisplayName("Company Name ")]
        public string name { get; set; }
        [DisplayName("Description ")]
        public string description { get; set; }
        [DisplayName("Address ")]
        public string address { get; set; }

        [DisplayName("Primary Phone ")]
        public string primary_phone { get; set; }
        [DisplayName("Secondary Phone ")]
        public string secondary_phone { get; set; }

        [DisplayName("Email Address ")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",ErrorMessage="Please enter a valid email address.")]
        public string email { get; set; }

        [DisplayName("Web Address ")]
        public string web { get; set; }

        [DisplayName("Company Logo ")]
        public string logo { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
    }
}