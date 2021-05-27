using System.Collections.Generic;
using com.linde.Model;

namespace PayrollWeb.Models
{
    public interface ISalaryProcess
    {
        IProcessResult Process();
        List<prl_employee_details> GetEmployeeList();
    }
}