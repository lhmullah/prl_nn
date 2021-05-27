using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using com.linde.DataContext;
using com.linde.Model;
using PayrollWeb.CustomSecurity;
using PayrollWeb.Utility;
using PayrollWeb.ViewModels;
using System.Net;
using System.Net.Mail;

namespace PayrollWeb.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly payroll_systemContext dataContext;

        public AuthenticationController(payroll_systemContext cont)
        {
            this.dataContext = cont;
        }

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                
                    if (Membership.ValidateUser(model.UserName, model.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        //if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        //    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        //{
                        //    string s = User.Identity.Name;
                        //    return Redirect(returnUrl);
                        //}
                        //else
                        //{
                        return RedirectToAction("Index", "Home");
                        //}
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    } 
            }

            return View(model);
        }


        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            System.Web.HttpContext.Current.Session.Abandon();
            
            //Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
            //Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            //HttpCookie cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            //if (cookie != null)
            //{
            //   cookie.Expires = DateTime.Now.AddDays(-1);
            //   Response.Cookies.Add(cookie);
            //}

            return RedirectToAction("LogOn", "Authentication");
        }

        public ActionResult Index()
        {
            
            //string s = Authentication();
            try
            {
                string[] roles = Roles.GetRolesForUser(User.Identity.Name);
                string rolename = roles[0];
                var _submenuGenerator = new SubMenuGenerator(dataContext);
                List<SubMenu> _submenuList = _submenuGenerator.GenerateSubMenuByMenuName("User Management", rolename);

                //List<SubMenu> _submenuList = _submenuGenerator.GenerateSubMenu(("User Management", rolename);
                //List<SubMenu> _submenuList = new List<SubMenu>();

                if (_submenuList.Count > 0)
                {
                    ViewData["SubMenu"] = _submenuList;
                }
                else
                {
                    var res = new OperationResult();
                    res.IsSuccessful = false;
                    res.Message = "You are not authorised to access the desired page";
                    TempData.Add("msg", res);
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
            }

            var users = Membership.GetAllUsers().Cast<MembershipUser>().Select(x => new RegisterModel
                        {
                            UserName = x.UserName,
                            Email = x.Email,
                            Role = Roles.GetRolesForUser(x.UserName).Count()>0?Roles.GetRolesForUser(x.UserName).First():""
                        }).ToList();
            
            return View(users);
        }

        //[PayrollAuthorize]
        public ActionResult Register()
        {

            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, model.PasswordQuestion, model.PasswordAnswer, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    //FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Authentication");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }


            return View(model);
        }

        [PayrollAuthorize]
        public ActionResult EditUser(string usr)
        {
            MembershipUser mu = Membership.GetUser(usr);
            RegisterModel _user = new RegisterModel();
            _user.UserName = mu.UserName;
            _user.Password = "";
            _user.PasswordQuestion = mu.PasswordQuestion;
            return View(_user);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult EditUser(RegisterModel model)
        {
            MembershipUser mu = Membership.GetUser("");
            return View();
        }

        [PayrollAuthorize]
        public ActionResult DeleteUser(string usr)
        {
            var res = new OperationResult();
            if (usr == User.Identity.Name)
            {
                res.IsSuccessful = false;
                res.Message = "You can't delete yourself";
            }
            else
            {
                try
                {
                    Membership.DeleteUser(usr);

                    res.IsSuccessful = true;
                    res.Message = usr + " deleted successfully.";
                }
                catch (Exception ex)
                {
                    res.IsSuccessful = false;
                    res.Message = ex.Message;
                }
            }

            TempData.Add("msg", res);
            return RedirectToAction("Index");
        }

        [PayrollAuthorize]
        public ActionResult Role()
        {

            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult Role(RoleModel role)
        {

            var res = new OperationResult();
            if (ModelState.IsValid)
            {
                try
                {
                    Roles.CreateRole(role.RoleName);
                    res.IsSuccessful = true;
                    res.Message = role.RoleName + " role created successfully. ";
                    TempData.Add("msg", res);
                    return RedirectToAction("Role");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
            }
            return View(role);
        }

        [PayrollAuthorize]
        public ActionResult DeleteRole(string rol)
        {
            var res = new OperationResult();
            try
            {
                Roles.DeleteRole(rol);

                res.IsSuccessful = true;
                res.Message = rol + " deleted successfully.";
                TempData.Add("msg", res);
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = ex.Message;
                TempData.Add("msg", res);
            }
            return RedirectToAction("Role");
        }

        [PayrollAuthorize]
        [HttpGet]
        public ActionResult UserRole(string _user)
        {

            var usrRole = new UserRoleModel();
            string _role = "";
            if (!string.IsNullOrEmpty(_user))
            {
                string[] roles = Roles.GetRolesForUser(_user);

                usrRole.User = _user;
                if (roles.Count() > 0)
                {
                    usrRole.Role = roles[0];
                    _role = roles[0];
                }
                else
                {
                    usrRole.Role = "";
                    _role = "";
                }
            }
            ViewBag.UserInR = _role;
            ViewBag.Users = Membership.GetAllUsers();
            ViewBag.Roles = Roles.GetAllRoles();
            return View(usrRole);
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult UserRole(UserRoleModel rm)
        {

            string[] role = Roles.GetRolesForUser(rm.User);
            if (role.Count() > 0)
            {
                Roles.RemoveUserFromRole(rm.User, role[0]);
            }
            Roles.AddUserToRole(rm.User, rm.Role);

            ViewBag.Users = Membership.GetAllUsers();
            ViewBag.Roles = Roles.GetAllRoles();
            ViewBag.UserInR = rm.Role;

            return View(rm);
        }

        [PayrollAuthorize]
        public ActionResult RoleMenu(string roleAndMenuId)
        {
            var _RP = new RolePrivilege();
            List<int> SavedSubMenuIds = new List<int>() ;

            ViewBag.Menus = dataContext.prl_menu.ToList(); ;
            ViewBag.Roles = Roles.GetAllRoles();

            if (roleAndMenuId != null)
            {
                string[] rm = roleAndMenuId.Split(',');

                string  _role = rm[0];
                int _menuId = rm[1] == "" ? 0 : Convert.ToInt32(rm[1]);

                _RP.role = _role;
                _RP.menu_id = _menuId;

                ViewBag.sbMenus = dataContext.prl_sub_menu.Where(x => x.menu_id == _menuId).ToList();

                var SavedSubMenus = dataContext.prl_role_privilege.Where(x => x.role == _role).ToList();
                if (SavedSubMenus.Count > 0)
                {
                    foreach (var smid in SavedSubMenus)
                    {
                        SavedSubMenuIds.Add(Convert.ToInt32(smid.sub_menu_id));
                    }
                }
                ViewBag.sBIds = SavedSubMenuIds;
                return View(_RP);
            }
            
            ViewBag.sbMenus = dataContext.prl_sub_menu.Where(x => x.id == 0);
            ViewBag.sBIds = SavedSubMenuIds;
            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult RoleMenu(RolePrivilege rp)
        {

            var res = new OperationResult();
            List<int> SavedSubMenuIds = new List<int>();

            var items = dataContext.prl_role_privilege.Where(x => x.role == rp.role && x.menu_id==rp.menu_id).ToList();
            if(items.Count>0)
            {
                foreach(var item in items)
                {
                    dataContext.prl_role_privilege.Remove(item);
                    dataContext.SaveChanges();
                }
            }

            if (rp.SubMenus!=null)
            {
                var rolP = new prl_role_privilege();
                foreach (var submId in rp.SubMenus)
                {
                    rolP.menu_id = rp.menu_id;
                    rolP.sub_menu_id = Convert.ToInt16(submId);
                    rolP.role = rp.role;

                    dataContext.prl_role_privilege.Add(rolP);
                    dataContext.SaveChanges();

                    SavedSubMenuIds.Add(Convert.ToInt32(submId));
                }
                res.IsSuccessful = true;
                res.Message = "Sub-menus successfully assigned to the sellected role.";
                TempData.Add("msg", res);
            }

            

            ViewBag.Menus = dataContext.prl_menu.ToList(); ;
            ViewBag.Roles = Roles.GetAllRoles();
            ViewBag.sbMenus = dataContext.prl_sub_menu.Where(x => x.menu_id == rp.menu_id).ToList();
            ViewBag.sBIds = SavedSubMenuIds;
            return View(rp);
        }

        [PayrollAuthorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [PayrollAuthorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            var res = new OperationResult();
            if (ModelState.IsValid)
            {
                
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                    res.IsSuccessful = true;
                    res.Message = "Your password has been changed successfully.";
                    TempData.Add("msg", res);

                    var cp = new ChangePasswordModel();
                    return View(cp);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            return View(model);
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string newPassword, string emailFor )
        {

            //var verifyUrl = "/User/" + emailFor + "/" + activationCode;
            //var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("mlhm1187@gmail.com", "Lukman Hakim");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "01911779229lh"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                //subject = "Your account is successfully created!";
                //body = "<br/><br/>We are excited to tell you that your account is" +
                //    " successfully created. Please click on the below link to verify your account" +
                //    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                //body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                //    "<br/><br/><a href=" + link + ">Reset Password link</a>";
                body = "Hi,<br/><br/>We got a request to reset your HR payroll account password." +
                " Your New Password is: " + newPassword +"";
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel FP)
        {
            var res = new OperationResult();

            if (ModelState.IsValid)
            {

                if (Membership.FindUsersByEmail(FP.Email).Count > 0)
                {
                    string userName = Membership.GetUserNameByEmail(FP.Email);
                    MembershipUser currentUser = Membership.GetUser(userName, true /* userIsOnline */);

                    //string password = Membership.GeneratePassword(10, 2);
                    //string password = Guid.NewGuid().ToString();

                    string newPassword;

                    try
                    {
                         newPassword = currentUser.ResetPassword(FP.PasswordAnswer);

                         //Send email for reset password

                         SendVerificationLinkEmail(FP.Email, newPassword, "ResetPassword");

                         res.IsSuccessful = true;
                         res.Message = "Your password has been sent to your email id. Please check and ";
                         TempData.Add("msg", res);
                    }
                    catch (MembershipPasswordException e)
                    {
                        res.Message = "Invalid password answer. Please re-enter and try again.";
                        TempData.Add("msg", res);

                        var fp = new ForgotPasswordModel();
                        return View(fp);
                    }
                    catch (Exception e)
                    {
                        res.Message = e.Message;
                        var fp = new ForgotPasswordModel();
                        return View(fp);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No user found by that email.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Wrong Attempt..!!");
            }
            return View(FP);
        }


        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
