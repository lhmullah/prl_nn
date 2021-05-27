using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using com.gpit.DataContext;
using com.gpit.Model;

namespace PayrollWeb.Service
{
    public class DeductionService
    {
        private payroll_systemContext dataContext;
        private IIdentity userIdentity;

        public DeductionService(payroll_systemContext context)
        {
            this.dataContext = context;
            userIdentity = Thread.CurrentPrincipal.Identity;
        }

        ~DeductionService()
        {
            Dispose(false);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dataContext != null)
                {
                    dataContext.Dispose();
                    dataContext = null;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public bool CreateConfiguration(prl_deduction_configuration deductionConfiguration)
        {
                //dataContext.Entry(deductionConfiguration).State = EntityState.Detached;
            try
            {
               
                if (deductionConfiguration.id == 0)
                {
                    deductionConfiguration.created_by = userIdentity.Name;
                    deductionConfiguration.created_date = DateTime.Now;
                    dataContext.prl_deduction_configuration.Add(deductionConfiguration);
                   
                }
                else
                {
                    var extOb = dataContext.prl_deduction_configuration.Include("prl_deduction_name").SingleOrDefault(x => x.id == deductionConfiguration.id);
                    extOb.prl_deduction_name.prl_grade = deductionConfiguration.prl_deduction_name.prl_grade;
                    extOb.updated_by = userIdentity.Name;
                    extOb.updated_date = DateTime.Now;
                    var entry = dataContext.Entry(extOb);
                    entry.Property(x => x.id).IsModified = false;
                    entry.CurrentValues.SetValues(deductionConfiguration);
                    entry.State = EntityState.Modified;
                    //dataContext.Entry(entry).State = EntityState.Modified;
                    
                }

                //dataContext.Entry(entry).Reload();
                //dataContext.Refresh(RefreshMode.ClientWins, dataContext.prl_deduction_configuration);
                //dataContext.SaveChanges();

                dataContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                
                throw;
            }
            
            
            
           
            
            
        }

    }
}