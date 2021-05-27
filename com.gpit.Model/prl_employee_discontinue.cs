using System;

namespace com.linde.Model
{
    public partial class prl_employee_discontinue
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public string is_active { get; set; }
        public System.DateTime discontinue_date { get; set; }
        public Nullable<System.DateTime> continution_date { get; set; }
        public string discontination_type { get; set; }
        public string with_salary { get; set; }
        public string without_salary { get; set; }
        public string remarks { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public virtual prl_employee prl_employee { get; set; }
    }
}
