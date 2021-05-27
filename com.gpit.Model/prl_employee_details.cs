using System;

namespace com.linde.Model
{
    public partial class prl_employee_details
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public string emp_status { get; set; }
        public string employee_category { get; set; }
        public Nullable<int> job_level_id { get; set; }
        public int? grade_id { get; set; }    
        public Nullable<int> division_id { get; set; }

        public int department_id { get; set; }
        public Nullable<int> sub_department_id { get; set; }
        public Nullable<int> sub_sub_department_id { get; set; }

        public int designation_id { get; set; }
        public Nullable<int> cost_centre_id { get; set; }

        public decimal basic_salary { get; set; }

        public string marital_status { get; set; }
        public string blood_group { get; set; }
        public string parmanent_address { get; set; }
        public string present_address { get; set;}

        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }

        public virtual prl_cost_centre prl_cost_centre { get; set; }
        public virtual prl_job_level prl_job_level { get; set; }
        public virtual prl_department prl_department { get; set; }
        public virtual prl_sub_department prl_sub_department { get; set; }
        public virtual prl_sub_sub_department prl_sub_sub_department { get; set; }
        public virtual prl_designation prl_designation { get; set; }
        public virtual prl_division prl_division { get; set; }
        public virtual prl_employee prl_employee { get; set; }
        public virtual prl_grade prl_grade { get; set; }
    }
}
