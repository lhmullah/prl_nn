using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class Users
    {
        [HiddenInput(DisplayValue = true)]
        public int User_Id { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int Emp_Id { get; set; }

        [DisplayName("User Name")]
        [Required(ErrorMessage = "User Name cannot be empty.")]
        public string User_Name { get; set; }

        [DisplayName("Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; }

        [DisplayName("Role Name")]
        [Required(ErrorMessage = "Role Name cannot be empty.")]
        public string Role_Name { get; set; }

        //[DataType(DataType.Password)]
        [DisplayName("Passoword")]
        [Required(ErrorMessage = "Password cannot be empty.")]
        public string Password { get; set; }

        [DisplayName("Password Question")]
        [Required(ErrorMessage = "Password Question cannot be empty.")]
        public string PasswordQuestion { get; set; }

        [DisplayName("Password Answer")]
        [Required(ErrorMessage = "Password Answer cannot be empty.")]
        public string PasswordAnswer { get; set; }

        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
    }
}