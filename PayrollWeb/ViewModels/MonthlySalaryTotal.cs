using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class MonthlySalaryTotal
    {
        public int Year { get; set; }
        public int month_no { get; set; }
        public string month_name { get; set; }
        public string this_month_name { get; set; }

        public int frm_month_no { get; set; }
        public int to_month_no { get; set; }

        //For RDLC

        public int cost_centre_id { get; set; }
        public string cost_centre_name { get; set; }
        public Nullable<decimal> basic_salary { get; set; }

        public Nullable<decimal> this_month_basic { get; set; }
        public Nullable<decimal> basic { get; set; }
        public Nullable<decimal> houseR { get; set; }
        public Nullable<decimal> conveyance { get; set; }
        public Nullable<decimal> car_mc_allowance { get; set; }
        public Nullable<decimal> bta { get; set; }
        public Nullable<decimal> incentive_q1 { get; set; }
        public Nullable<decimal> incentive_q2 { get; set; }
        public Nullable<decimal> incentive_q3 { get; set; }
        public Nullable<decimal> incentive_q4 { get; set; }
        public Nullable<decimal> special_allowance { get; set; }
        public Nullable<decimal> sti { get; set; }
        public Nullable<decimal> tax_paid_by_company { get; set; }
        public Nullable<decimal> bonus { get; set; }
        public Nullable<decimal> festival_bonus { get; set; }
        public Nullable<decimal> leave_encashment { get; set; }
        public Nullable<decimal> long_service_award { get; set; }

        public Nullable<decimal> one_time { get; set; }
        public Nullable<decimal> training_allowance { get; set; }
        public Nullable<decimal> gift { get; set; }

        public Nullable<decimal> total_arrear_allowance { get; set; }
        public Nullable<decimal> totalA { get; set; }

        public Nullable<decimal> ipad_or_mobile_bill { get; set; }
        public Nullable<decimal> modem_bill { get; set; }
        public Nullable<decimal> mobile_bill { get; set; }
        public Nullable<decimal> ipad_bill { get; set; }

        //Arrear
        public Nullable<decimal> arrear_basic { get; set; }
        public Nullable<decimal> arrear_house { get; set; }
        public Nullable<decimal> arrear_conveyance { get; set; }
        public Nullable<decimal> arrear_car_mc_allowance { get; set; }
        public Nullable<decimal> arrear_leave_encashment { get; set; }


        public Nullable<decimal> lunch_support { get; set; }
        public Nullable<decimal> others_deduction { get; set; }

        public Nullable<decimal> tax_return_non_submission { get; set; }
        public Nullable<decimal> income_tax { get; set; }

        public string bank_name { get; set; }
        public Nullable<decimal> pf_cc_amount { get; set; }
        public Nullable<decimal> pf_arrear { get; set; }
        public Nullable<decimal> pf_co_amount { get; set; }

        public Nullable<decimal> monthly_tax { get; set; }

        public Nullable<decimal> totalD { get; set; }

        public Nullable<decimal> netPay { get; set; }
    }
}
