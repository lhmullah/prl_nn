using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class EmployeeDetails
    {
        public EmployeeDetails()
        {

        }
        public virtual Designation prl_designation { get; set; }
        public virtual Department prl_department { get; set; }
        public virtual SubDepartment prl_sub_department { get; set; }
        public virtual SubSubDepartment prl_sub_sub_department { get; set; }
        public virtual Division prl_division { get; set; }
        public virtual JobLevel prl_job_level { get; set; }
        public virtual Grade prl_grade { get; set; }
        public virtual CostCentre prl_cost_centre { get; set; }

        [HiddenInput(DisplayValue = true)]
        public int id { get; set; }

        public int emp_id { get; set; }

        //[Required(ErrorMessage = "Status can't be empty.")]
        [DisplayName("Employee Status")]
        public string emp_status { get; set; }

        //[Required(ErrorMessage = "Category can't be empty.")]
        [DisplayName("Employee Category")]
        public string employee_category { get; set; }

        //[Required(ErrorMessage = "Grade can't be empty.")]
        [DisplayName("Grade")]
        public int? grade_id { get; set; }

        //[Required(ErrorMessage = "Division can't be empty.")]
        [DisplayName("Division")]
        public Nullable<int> division_id { get; set; }

        //[Required(ErrorMessage = "Department can't be empty.")]
        //[Range(1,1000)]
        [DisplayName("Department")]
        public int department_id { get; set; }

        [DisplayName("Sub Dept. Name")]
        public Nullable<int> sub_department_id { get; set; }

        [DisplayName("Sub Sub Dept. Name")]
        public Nullable<int> sub_sub_department_id { get; set; }

        //[Required(ErrorMessage = "Job Level can't be empty.")]
        [DisplayName("Job Level")]
        public int? job_level_id { get; set; }  // Nullable By Yeasin

        //[Required(ErrorMessage = "Cost Centre can't be empty.")]
        [DisplayName("Cost Centre")]
        public int? cost_centre_id { get; set; }

        //[Required(ErrorMessage = "Job Title can not be empty.")]
        [DisplayName("Designation")]
        public int designation_id { get; set; }

        [Required(ErrorMessage = "Salary can't be empty.")]
        [Range(1, int.MaxValue, ErrorMessage = "Salary should be greater than {1}")]
        [DisplayName("Basic Salary")]
        public decimal basic_salary { get; set;}

        //[Required(ErrorMessage = "Marital Status can't be empty.")]
        [DisplayName("Marital Status")]
        public string marital_status { get; set; }

        [DisplayName("Blood Group")]
        public string blood_group { get; set; }

        [DisplayName("Parmanent Address")]
        [MaxLength(320, ErrorMessage="Address should be not more than 320 charcters")]
        public string parmanent_address { get; set; }

        [DisplayName("Present Address")]
        [MaxLength(320, ErrorMessage = "Address should be not more than 320 charcters")]
        public string present_address { get; set; }

        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }

        public string name { get; set; }
    }
}