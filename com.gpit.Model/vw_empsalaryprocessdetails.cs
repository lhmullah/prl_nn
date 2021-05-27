using System;


namespace com.linde.Model
{
    public partial class vw_empsalaryprocessdetails
    {
        public int id { get; set; }
        public string emp_no { get; set; }
        public string empName { get; set; }
        public DateTime joining_date { get; set; }
        public string designation { get; set; }

        public int cost_centre_id { get; set; }
        public string cost_centre_name { get; set; }

        public string routing_no { get; set; }
        public string account_no { get; set; }

        public int salary_process_id { get; set; }
        public DateTime salary_month { get; set; }
        public DateTime payment_date { get; set; }

        public DateTime last_working_date { get; set; }
        public string discontinued_reason { get; set; }
        public string bank_name { get; set; }


        public decimal no_of_days_lwp { get; set; }
        public DateTime loan_start_date { get; set; }
        public DateTime loan_end_date { get; set; }
        public decimal principal_amount { get; set; }
        public int no_of_installment { get; set; }
        public string is_discontinued { get; set; }

        public int month_no { get; set; }
        public string month_name { get; set; }
        public string calendar_days { get; set; }
        public string working_days { get; set; }
        public decimal basic_salary { get; set; }

        public decimal this_month_basic { get; set; }

        public decimal house { get; set; }
        public decimal conveyance { get; set; }
        public decimal car_mc_allowance { get; set; }
        public decimal bta { get; set; }
        public decimal incentive_q1 { get; set; }
        public decimal incentive_q2 { get; set; }
        public decimal incentive_q3 { get; set; }
        public decimal incentive_q4 { get; set; }
        public decimal special_allowance { get; set; }
        public decimal sti { get; set; }
        public decimal tax_paid_by_company { get; set; }
        public decimal bonus { get; set; }

        public decimal festival_bonus { get; set; }
        public decimal leave_encashment { get; set; }
        public decimal long_service_award { get; set; }

        public decimal one_time { get; set; }
        public decimal training_allowance { get; set; }
        public decimal gift { get; set; }
        public decimal pf_refund { get; set; }
        public decimal basic_arrear { get; set; }

        public decimal total_arrear_allowance { get; set; }
        public decimal total_allowance { get; set; }


        public decimal ipad_or_mobile_bill { get; set; }
        public decimal modem_bill { get; set; }
        public decimal mobile_bill { get; set; }
        public decimal ipad_bill { get; set; }

        //Arrear
        public decimal arrear_basic { get; set; }
        public decimal arrear_house { get; set; }
        public decimal arrear_conveyance { get; set; }
        public decimal arrear_car_mc_allowance { get; set; }
        public decimal arrear_leave_encashment { get; set; }

        public decimal lunch_support { get; set; }
        public decimal others_deduction { get; set; }

        public decimal tax_return_non_submission { get; set; }
        public decimal tax_Paid { get; set; }
        public decimal income_tax { get; set; }

        public decimal monthly_tax { get; set; }
        public decimal pf_arrear { get; set; }
        public decimal pf_co_amount { get; set; }
        public decimal pf_cc_amount { get; set; }

        public decimal total_deduction { get; set; }
        public decimal net_pay { get; set; }

        public string remarks_Sa { get; set; }
        public string remarks_Sd { get; set; }
        public string remarks { get; set; }
    }
}
