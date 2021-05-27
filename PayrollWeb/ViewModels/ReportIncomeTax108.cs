﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class ReportIncomeTax108
    {
        public int empId { get; set; }

        public string empNo { get; set; }
        public string empName { get; set; }
        public string designation { get; set; }
        public string department { get; set; }
        public string emp_category { get; set; }
        public string joining_date { get; set; }
        public string job_level { get; set; }
        
        public string gender { get; set; }
        public decimal basic { get; set; }

        public decimal hr_allowance { get; set; }
        public decimal exemption_hr { get; set; }
        public decimal taxable_hr { get; set; }

        public decimal medical_allowance { get; set; }
        public decimal exemption_medical { get; set; }
        public decimal taxable_medical { get; set; }

        public decimal conveyance_allowance { get; set; }
        public decimal exemption_conv { get; set; }
        public decimal taxable_conv { get; set; }

        public decimal bonus { get; set; }
        public decimal other_allowance { get; set; }
        
        public decimal pf_cc_amount { get; set; }

        public string fiscal_year { get; set; }
        public string fiscal_yr_start { get; set; }
        public string fiscal_yr_end { get; set; }

        public string assesment_year { get; set; }
        public string tin { get; set; }

        public decimal incomeTaxableAmount_0 { get; set; }
        public decimal individualTaxLiabilityAmount_0 { get; set; }
        public decimal incomeTaxableAmount_10 { get; set; }
        public decimal individualTaxLiabilityAmount_10 { get; set; }
        public decimal incomeTaxableAmount_15 { get; set; }
        public decimal individualTaxLiabilityAmount_15 { get; set; }
        public decimal incomeTaxableAmount_20 { get; set; }
        public decimal individualTaxLiabilityAmount_20 { get; set; }
        public decimal incomeTaxableAmount_25 { get; set; }
        public decimal individualTaxLiabilityAmount_25 { get; set; }
        public decimal incomeTaxableAmount_30 { get; set; }
        public decimal individualTaxLiabilityAmount_30 { get; set; }

        public decimal totalAnnualIncomeTillDate { get; set; }
        public decimal totalAnnualIncomeCurrentMonth { get; set; }
        public decimal totalAnnualIncomeProjected { get; set; }
        public decimal totalLessExempted { get; set; }
        public decimal totalTaxableIncome { get; set; }

        public decimal incomeTaxableAmountTotal { get; set; }
        public decimal individualTaxLiabilityAmountTotal { get; set; }

        public decimal pf_Contribution_Both_Parts { get; set; }
        public decimal other_Investment_except_PF { get; set; }

        public decimal netRebateAmount { get; set; }

        public decimal taxRefund { get; set; }
        public decimal netTaxPayable { get; set; }
        public decimal paid_total { get; set; }

        public decimal totalTaxTillDate { get; set; }
        public decimal taxDeductedThisMonth { get; set; }


        public decimal TaxToBeAdjusted { get; set; }

        public string paymentMode { get; set; }
        public string bank { get; set; }
        public string accNo { get; set; }
        public string routing_no { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string month_name { get; set; }

        public string  tillDateRange { get; set; }
        public string  currentMonth { get; set; }
        public string  projectedDateRange { get; set; }

    }
}
