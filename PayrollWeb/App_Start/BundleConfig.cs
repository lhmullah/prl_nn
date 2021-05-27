using System.Web;
using System.Web.Optimization;

namespace PayrollWeb
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/payrolljs").Include(
            //            "~/Scripts/jquery-2.0.3.min.js",
            //            "~/Scripts/jquery.validate.min.js",
            //            "~/Scripts/jquery.validate.unobtrusive.min.js", 
            //            "~/Scripts/jquery.unobtrusive-ajax.min.js",
            //            "~/Scripts/bootstrap.min.js",
            //            "~/Scripts/bootstrap-datepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/payrolljs").Include(
                        "~/Scripts/jquery-2.0.3.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/jquery-ui-1.10.3.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/payroll_themejs").Include(
                        "~/themes/js/bootstrap-tooltip.js",
                        "~/themes/js/bootstrap-popover.js",
                        "~/themes/js/business_ltd_1.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/fileuploadjs").Include(
                "~/Content/fu/jquery.ui.widget.js",
                "~/Content/fu/jquery.iframe-transport.js",
                "~/Content/fu/jquery.fileupload.js"));

            //css bundling for main theme of this app WARNING: Please maintain the order
            bundles.Add(new StyleBundle("~/fileuploadcss")
                    .Include(
                    "~/Content/jquery.fileupload-ui.css"));

            bundles.Add(new StyleBundle("~/payrollcss")
                    .Include("~/themes/css/bootstrap.min.css",
                    "~/themes/css/bootstrap-responsive.min.css",
                    "~/Content/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/payroll_themecss")
                    .Include("~/themes/css/base.css",
                    "~/themes/css/font-awesome.css",
                    "~/themes/css/base-custom.css"));

            //BundleTable.EnableOptimizations = true;
        }
    }
}