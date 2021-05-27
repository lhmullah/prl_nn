using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class ReportIncomeTax
    {
        public int empId { get; set; }

        public string empNo { get; set; }
        public string empName { get; set; }
        public string designation { get; set; }
        public string grade { get; set; }
        public string emp_category { get; set; }
        public string joining_date { get; set; }
        
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

        public decimal lfa_allowance { get; set; }
        public decimal exemption_lfa { get; set; }
        public decimal taxable_lfa { get; set; }

        public decimal lda { get; set; }
        public decimal children_edu { get; set; }
        public decimal overtime { get; set; }
        public decimal performance_bonus { get; set; }
        public decimal festibal_bonus { get; set; }
        public decimal other_bonus { get; set; }
        public decimal shift_allowance { get; set; }

        public decimal officiating { get; set; }
        public decimal mid_month_advance { get; set; }
        public decimal leave_encashment { get; set; }
        public decimal long_service_award { get; set; }

        public decimal loan_refund { get; set; }
        public decimal sip { get; set; }
        public decimal keb { get; set; }

 
        public decimal wppf { get; set; }
        public decimal exemption_wppf { get; set; }
        public decimal taxable_wppf { get; set; }

        public decimal ramadan_allowance { get; set; }
        public decimal company_provided_car { get; set; }
        
        public decimal ltp { get; set; }
        public decimal stip { get; set; }

        public decimal other_allowance { get; set; }
        public decimal personal_allowance { get; set; }
        public decimal pf_cc_amount { get; set; }


        public string fiscal_year { get; set; }
        public string assesment_year { get; set; }
        public string tin { get; set; }


        public decimal totalAnnualIncomeTillDate { get; set; }
        public decimal totalAnnualIncomeCurrentMonth { get; set; }
        public decimal totalAnnualIncomeProjected { get; set; }
        public decimal totalLessExempted { get; set; }
        public decimal totalTaxableIncome { get; set; }

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

        public decimal incomeTaxableAmountTotal { get; set; }
        public decimal individualTaxLiabilityAmountTotal { get; set; }

        public decimal pf_Contribution_Both_Parts { get; set; }
        public decimal other_Investment_except_PF { get; set; }

        public decimal actualInvestementTotal { get; set; }
        public decimal eligibleInvestementAmount { get; set; }

        public decimal rebatable_amount_15 { get; set; }
        public decimal firstRebateSlabAmount_15 { get; set; }

        public decimal rebatable_amount_12 { get; set; }
        public decimal secondRebateSlabAmount_12 { get; set; }

        public decimal rebatable_amount_10 { get; set; }
        public decimal thirdRebateSlabAmount_10 { get; set; }
        public decimal netRebateAmount { get; set; }

        public decimal taxRefund { get; set; }
        public decimal netTaxPayable { get; set; }

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
        public DateTime monthYear { get; set; }
        public int processId { get; set; }
        public int taxProcessId { get; set; }

        public string  tillDateRange { get; set; }
        public string  currentMonth { get; set; }
        public string  projectedDateRange { get; set; }

    }
}
