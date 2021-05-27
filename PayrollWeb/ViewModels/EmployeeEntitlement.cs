using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.linde.Model;

namespace PayrollWeb.ViewModels
{

    public class EmployeeEntitlement
    {
        public List<SettlementAllowance> EntitledAllowances { get; set; }
        public List<SettlementAllowance> DueAllowances { get; set; }
        public List<SettlementDeduction> EntitledDeductions { get; set; }
        public List<BonusSettlement> EntitledBonus { get; set; }
        public List<BonusSettlement> BonusDue { get; set; }
       
        public int EmployeeId { get; set; }
        public DateTime SettlementDate { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal DueSalary { get; set; }

        public int EntitledCalculationDays { get; set; }
        public int DueCalculationDays { get; set; }

        public decimal TotalEntitledAmount
        {
            get
            {
                decimal total = 0;
                if (EntitledAllowances != null)
                {
                    total = EntitledAllowances.Sum(x => x.Amount);
                }

                if (EntitledBonus!=null)
                {
                    total += EntitledBonus.Sum(x => x.Amount);
                }
                return total+this.BasicSalary;
            }
        }

        public decimal TotalEmployeeDue
        {
            get
            {
                decimal total = 0;
                if (DueAllowances != null)
                {
                    total = DueAllowances.Sum(x => x.Amount);
                }
                if (BonusDue != null)
                {
                    total += BonusDue.Sum(x => x.Amount);
                }
                if (EntitledDeductions != null)
                {
                    total += EntitledDeductions.Sum(x => x.Amount);
                }
                return total+this.DueSalary;
            }
        }
    }

    public class SettlementDeduction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class SettlementAllowance
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class OTSettlement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class BonusSettlement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }


}