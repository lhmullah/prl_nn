using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayrollWeb.Utility
{
    public static class DateUtility
    {

        private static readonly Dictionary<string, int> listMonths = new Dictionary<string, int>()
                                                               {
                                                                   {"January", 1},{"February", 2},{"March", 3},{"April", 4},
                                                                   {"May", 5},{"June", 6},{"July", 7},{"August", 8},
                                                                   {"September", 9},{"October", 10},{"November", 11},{"December", 12}
                                                               };

        private static List<int> listYears;


        public static Dictionary<string, int> GetMonths()
        {
            return listMonths;
        }

        public static List<int> GetYears()
        {
            var lst = new List<int>();
            var limit = DateTime.Now.Year+12;
            var start = DateTime.Now.Year - 10;
            for (int i = start; i < limit; i++)
            {
                lst.Add(i);
            }
            return lst;
        }

        public static string MonthName(int monthId)
        {
            switch (monthId)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                default :
                    return "December";
            }
        }
    }
}