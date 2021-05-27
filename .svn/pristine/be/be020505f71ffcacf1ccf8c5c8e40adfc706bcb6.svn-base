using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using com.gpit.DataContext;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Html.Models;
using PayrollWeb.Utility;
using PayrollWeb.ViewModels;

namespace PayrollWeb.Models
{
    public class PayrollMenuProvider : DynamicNodeProviderBase 
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            using (var db = new payroll_systemContext())
            {
                string authMsg;
                List<SubMenu> _submenuList;
                var _submenuGenerator = new SubMenuGenerator(db);
                
                _submenuGenerator.GenerateSubMenu(out _submenuList, out authMsg, node.Title, HttpContext.Current.User.Identity.Name);
                foreach (var subMenu in _submenuList)
                {
                    DynamicNode dynamicNode = new DynamicNode();
                    dynamicNode.Title = subMenu.sub_menu;
                    dynamicNode.Controller = subMenu.controller_name;
                    dynamicNode.Action = subMenu.view_name;
                    yield return dynamicNode;
                }
            }
        }
    }
}