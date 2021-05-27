using System;
using System.Linq;
using com.gpit.DataContext;

namespace PayrollWeb.CustomSecurity
{
    public class UserActionChecker
    {
        public static bool IsAuthorizedForAction(string userName,string controller, string action)
        {
            using (var dataContext= new payroll_systemContext())
            {
                string[] roles = System.Web.Security.Roles.GetRolesForUser(userName);
                if (roles.Any())
                {
                    var rolName = roles[0];

                    var lstPrivilege = dataContext.prl_role_privilege.AsEnumerable().Where(x => x.role == rolName)
                        .Join(dataContext.prl_sub_menu.AsEnumerable(), ok => ok.sub_menu_id, ik => ik.id, (ok, ik) => ik)
                        .Any(y =>String.Equals(y.controller_name, controller, StringComparison.CurrentCultureIgnoreCase));
                    return lstPrivilege;
                }
                return false;
            }
        }
    }
}