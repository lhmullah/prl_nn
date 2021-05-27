using System;


namespace com.linde.Model
{
    public partial class vw_empwisemonthlyallowances
    {
        public int id { get; set; }
        public string emp_no { get; set; }
        public string name { get; set; }
        public string designation { get; set; }
        public string emp_category { get; set; }
        public string emp_grade { get; set; }
        public decimal office_staff_ot { get; set; }
        public decimal factory_staff_ot { get; set; }
        public decimal shift_allowance { get; set; }
        public decimal officiating { get; set; }
        public decimal advance { get; set; }
        public decimal lda { get; set; }
        public decimal ramadan_allowance { get; set; }
        public DateTime salary_month { get; set; }
        public string month_name { get; set; }
    }
}
