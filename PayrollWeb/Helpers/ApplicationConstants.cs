using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb
{
    public static partial class AppConstants
    {
        public static class Application
        {
            public const string Name = "Novo Nordisk - Payroll System";
            public const string ShortName = "Novo Nordisk - Payroll System";
            //public const string Logo = "/Images/CompanyLogo/bProperty_logo.png";
            public const string Logo = "/Images/CompanyLogo/novo_nordisk_logo.png";
        }
        public static class AreaNames
        {
            public const string Administration = "Administration";
        }
                
        public static class GridPaging
        {
            public const int Default = 15;
        }

        public static class BaseDocument
        {
            public static class DocumentType
            {
                public const string Logo = "LG";
                public const string Others = "OH";
            }
        }

        public static decimal RoundTwoDigit(this decimal obj)
        {
            return Math.Round(obj, 2);
        }

        public static readonly string StandardErrorMessage = "The application has encountered an unknown error."
                                                           + " It doesn't appear to have affected your data, but our technical staff have been automatically notified and will be looking into this with the utmost urgency."
                                                           + " Also help us improve your experience by sending an error report.";
    }
}