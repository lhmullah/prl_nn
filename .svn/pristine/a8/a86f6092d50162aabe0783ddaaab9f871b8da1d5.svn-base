using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.Utility
{
    public static class BatchNumberGenerator
    {
        public static string generateBonusBatchNumber(string prefix, DateTime festivalDate, int bonusID, string isFestival, string IsPaiWithSalary)
        {
            string is_festiva="";
            string is_pay = "";

            if (isFestival == "Y")
                is_festiva = "01";
            else
                is_festiva = "00";

            if (IsPaiWithSalary == "Y")
                is_pay = "01";
            else
                is_pay = "00";

            return prefix + "0" + festivalDate.Month.ToString() + festivalDate.Year.ToString() + "-" + bonusID.ToString() + is_festiva + is_pay;
        }

        public static string generateSalaryBatchNumber(string prefix, DateTime salaryDate)
        {
            return prefix + "-0" + salaryDate.Month.ToString() + salaryDate.Year.ToString();
        }
    }
}