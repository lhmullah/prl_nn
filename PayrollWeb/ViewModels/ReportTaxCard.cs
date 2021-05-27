using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels
{
    public class ReportTaxCard
    {
        public int eId { get; set; }

        public string empNo { get; set; }
        public string empName { get; set; }
        public string designation { get; set; }
        public string job_level { get; set; }
        public string category { get; set; }
        public DateTime joining_date { get; set; }
        public string department { get; set; }
        public string gender { get; set; }
        public decimal basicSalary { get; set; }

        public string fiscal_year { get; set; }
        public string assesment_year { get; set; }
        public string tin { get; set; }

        public decimal totalEarnings { get; set; }
        public decimal totalDeduction { get; set; }
        public Nullable<decimal> netPay { get; set; }
        public Nullable<decimal> tax { get; set; }
        public string paymentMode { get; set; }
        public string bank { get; set; }
        public string accNo { get; set; }
        public string routing_no { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public DateTime monthYear { get; set; }
        public int processId { get; set; }

        public int taxProcessId { get; set; }

        public string tillDateRange { get; set; }
        public string currentMonth { get; set; }
        public string projectedDateRange { get; set; }

        public string currentRate { get; set; }
        public string parameterName { get; set; }
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
        public decimal totalAnnualIncomeTillDate { get; set; }
        public decimal totalAnnualIncomeCurrentMonth { get; set; }
        public decimal totalAnnualIncomeProjected { get; set; }
        public decimal totalLessExempted { get; set; }
        public decimal totalTaxableIncome { get; set; }

        public decimal pf_Contribution_Both_Parts { get; set; }
        public decimal other_Investment_except_PF { get; set; }
        public decimal actualInvestementTotal { get; set; }
        public decimal netRebateAmount { get; set; }
        public decimal netTaxPayable { get; set; }
        public decimal taxPaidTotal { get; set; }

        public decimal totalTaxTillDate { get; set; }
        public decimal taxDeductedThisMonth { get; set; }
        public decimal TaxToBeAdjusted { get; set; }
        
        public List<ReportTaxCardIncomeHead> ReportTaxCardIncomeHeadList { get; set; }
    }

    public class ReportTaxCardIncomeHead
    {
        public int eId { get; set; }
        public string empNo { get; set; }
        public string empName { get; set; }
        public string incomeAnnualGrossName { get; set; }
        public decimal incomeTillDateAmount { get; set; }
        public decimal incomeCurrentMonthAmount { get; set; }
        public decimal incomeProjectedDateAmount { get; set; }
        public decimal incomeAnnualGrossAmount { get; set; }
        public decimal exemptedLessAmount { get; set; }
        public decimal incomeTotalTaxableAmount { get; set; }
        public string tillDateRange { get; set; }
        public string projectedDateRange { get; set; }
    }
}