using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Transactions;
using com.gpit.DataContext;
using com.gpit.Model;

namespace PayrollWeb.Service
{
    public class SettlementService
    {
        public void SettleEmployee(int employeeId, int allowanceCalculationDays, int dueCalculationDays, decimal earnedBasic, decimal dueBasic, List<prl_employee_settlement_allowance> lstAllowances, List<prl_employee_settlement_deduction> lstDeductions, List<prl_employee_settlement_detail> lstDetails, List<prl_employee_settlement_over_time> lstEmployeeSettlementOverTimes, decimal pf, decimal cpf)
        {
            var userIdentity = Thread.CurrentPrincipal.Identity;

            using (var k = new payroll_systemContext())
            {
                var objectContext = ((IObjectContextAdapter) k).ObjectContext;
                objectContext.Connection.Open();


                var empSettlement = new prl_employee_settlement();
                empSettlement.emp_id = employeeId;
                empSettlement.fractional_salary_earning = earnedBasic;
                empSettlement.fractional_salary_deduction = dueBasic;
                empSettlement.created_by = userIdentity.Name;
                empSettlement.created_date = DateTime.Now;
               
               
                empSettlement.pf_own_amount = pf;
                empSettlement.pf_company_amount = cpf;

                empSettlement.ot_amount = lstEmployeeSettlementOverTimes != null ? lstEmployeeSettlementOverTimes.Sum(x => x.amount) : 0;

                decimal earnedAlw = lstAllowances != null ? lstAllowances.Sum(x => x.amount) : 0;
                decimal dueAlw = lstAllowances != null ? lstAllowances.Sum(x => x.due_amount) : 0;
                empSettlement.other_allowances = earnedAlw - dueAlw;

                empSettlement.other_deductions = lstDeductions != null ? lstDeductions.Sum(x => x.amount) : 0;

                decimal bonusEarning =  lstDetails != null ? lstDetails.Sum(x => x.bonus_earning_amount) : 0;
                decimal bonusDeduction = lstDetails != null ? lstDetails.Sum(x => x.bonus_deduction_amount) : 0;
                empSettlement.bonus_earning_amount = bonusEarning - bonusDeduction;

                empSettlement.total_employee_earnings = empSettlement.fractional_salary_earning +
                                                        empSettlement.other_allowances +
                                                        empSettlement.pf_company_amount +
                                                        empSettlement.pf_own_amount +
                                                        empSettlement.ot_amount +
                                                        empSettlement.gf_amount +
                                                        empSettlement.bonus_earning_amount -
                                                        empSettlement.bonus_deduction -
                                                        empSettlement.other_deductions -
                                                        empSettlement.fractional_salary_deduction;
                                                        

                using (var ts = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue))
                {
                    k.prl_employee_settlement.Add(empSettlement);
                    k.SaveChanges();

                    if (lstEmployeeSettlementOverTimes != null)
                        foreach (var ot in lstEmployeeSettlementOverTimes)
                        {
                            ot.settlement_id = empSettlement.id;
                            k.prl_employee_settlement_over_time.Add(ot);
                        }

                    if (lstAllowances != null)
                        foreach (var alw in lstAllowances)
                        {
                            alw.settlement_id = empSettlement.id;
                            k.prl_employee_settlement_allowance.Add(alw);
                        }
                    if (lstDeductions != null)
                        foreach (var ded in lstDeductions)
                        {
                            ded.settlement_id = empSettlement.id;
                            k.prl_employee_settlement_deduction.Add(ded);
                        }
                    if (lstDetails != null)
                        foreach (var dt in lstDetails)
                        {
                            dt.settlement_id = empSettlement.id;
                            k.prl_employee_settlement_detail.Add(dt);
                        }
                    k.SaveChanges();
                    ts.Complete();
                }
            }
        }
    }
}