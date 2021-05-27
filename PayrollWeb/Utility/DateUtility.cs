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
            var start = 2019;
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

        public static int MonthNo(string monthName)
        {
            switch (monthName)
            {
                case "January":
                    return 1;
                case "February":
                    return 2;
                case "March":
                    return 3;
                case "April":
                    return 4;
                case "May":
                    return 5;
                case "June":
                    return 6;
                case "July":
                    return 7;
                case "August":
                    return 8;
                case "September":
                    return 9;
                case "October":
                    return 10;
                case "November":
                    return 11;
                default:
                    return 12;
            }
        }

        public static int getTaxTotalMonthForSeparatedEmp(int monthId)
        {
            switch (monthId)
            {
                case 1:
                    return 7;
                case 2:
                    return 8;
                case 3:
                    return 9;
                case 4:
                    return 10;
                case 5:
                    return 11;
                case 6:
                    return 12;
                case 7:
                    return 1;
                case 8:
                    return 2;
                case 9:
                    return 3;
                case 10:
                    return 4;
                case 11:
                    return 5;
                default:
                    return 6;
            }
        }

        public static int getTaxTotalMonthForNewEmp(int monthId)
        {
            switch (monthId)
            {
                case 1:
                    return 6;
                case 2:
                    return 5;
                case 3:
                    return 4;
                case 4:
                    return 3;
                case 5:
                    return 2;
                case 6:
                    return 1;
                case 7:
                    return 12;
                case 8:
                    return 11;
                case 9:
                    return 10;
                case 10:
                    return 9;
                case 11:
                    return 8;
                default:
                    return 7;
            }
        }
    }
}