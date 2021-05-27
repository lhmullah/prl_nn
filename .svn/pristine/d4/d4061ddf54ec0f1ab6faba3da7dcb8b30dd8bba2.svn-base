using System.Web;
using System.Web.Mvc;
using PayrollWeb.ViewModels;

namespace PayrollWeb.CustomSecurity
{
    public class PayrollAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized)
            {
                return false;
            }
            string authenticatedUser = httpContext.User.Identity.Name;

            var ok =  UserActionChecker.IsAuthorizedForAction(authenticatedUser,httpContext.Request.RequestContext.RouteData.Values["controller"].ToString(), httpContext.Request.RequestContext.RouteData.Values["action"].ToString());
            if (!ok)
            {
                httpContext.Items.Add("_payrollAuthorizeAttribute",false);
            }

           return ok;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Items.Contains("_payrollAuthorizeAttribute"))
            {
                var or = new OperationResult();
                or.IsSuccessful = false;
                or.Message = "You are not authorised to access the desired page.";
                filterContext.Controller.TempData["msg"] = or;
                filterContext.Result = new RedirectResult("~/Home/Index");
            }
        }
    }
}