using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;

namespace PayrollWeb.Utility
{
    public class CommonDateClass
    {
        public static string CalculateAge(DateTime p_DateOfBirth, DateTime p_CurrentDate)
        {
            int days = 0, months = 0, years = 0;

            int dobYear = p_DateOfBirth.Year;
            int dobMonth = p_DateOfBirth.Month;
            int dobDay = p_DateOfBirth.Day;

            DateTime curDate = p_CurrentDate.AddDays(1);

            int curYear = curDate.Year;
            int curMonth = curDate.Month;
            int curDay = curDate.Day;


            if (curDay < dobDay)
            {
                curDay = curDay + 30;
                days = curDay - dobDay;
                dobMonth = dobMonth + 1;
            }
            else
                days = curDay - dobDay;


            if (curMonth < dobMonth)
            {
                curMonth = curMonth + 12;
                months = curMonth - dobMonth;
                dobYear = dobYear + 1;
            }
            else
                months = curMonth - dobMonth;


            years = curYear - dobYear;


            return years.ToString() + " Y " + months.ToString() + " M " + days.ToString() + " D";
        }

        /// <summary>
        ///  This Function Returns a number of days for Between Two Months
        /// </summary>
        /// <param name="FromDate">DateTime</param>
        /// <param name="ToDate">DateTime</param>
        /// <returns>long</returns>
        public static long DateDifference(DateTime FromDate, DateTime ToDate)
        {
            int intYear = Convert.ToInt32(ToDate.ToString("yyyy"));
            int intMonth = Convert.ToInt32(ToDate.ToString("MM"));
            int intDay = Convert.ToInt32(ToDate.ToString("dd"));

            DateTime dt1 = new DateTime(intYear, intMonth, intDay);
            intYear = Convert.ToInt32(FromDate.ToString("yyyy"));
            intMonth = Convert.ToInt32(FromDate.ToString("MM"));
            intDay = Convert.ToInt32(FromDate.ToString("dd"));
            DateTime dt2 = new DateTime(intYear, intMonth, intDay);
            TimeSpan s = dt1 - dt2;
            return s.Days + 1;
        }

        /// <summary>
        ///  This Function Returns a number of days Between Two Months
        /// </summary>
        /// <param name="FromDate">DateTime</param>
        /// <param name="ToDate">DateTime</param>
        /// <returns>long</returns>
        public static long DateDifferenceBtn2Months(DateTime FromMonth, DateTime ToMonth)
        {
            string Temp = null;
            ToMonth = ToMonth.AddMonths(1);
            Temp = ToMonth.ToString("MMM-yy");
            Temp = "01-" + Temp;
            ToMonth = Convert.ToDateTime(Temp);

            int intYear = Convert.ToInt32(ToMonth.ToString("yyyy"));
            int intMonth = Convert.ToInt32(ToMonth.ToString("MM"));
            int intDay = Convert.ToInt32(ToMonth.ToString("dd"));

            DateTime dt1 = new DateTime(intYear, intMonth, intDay);
            intYear = Convert.ToInt32(FromMonth.ToString("yyyy"));
            intMonth = Convert.ToInt32(FromMonth.ToString("MM"));
            intDay = 1;
            DateTime dt2 = new DateTime(intYear, intMonth, intDay);
            TimeSpan s = dt1 - dt2;
            return s.Days;
        }

        /// <summary>
        ///  This Function Returns Total minutes Between two Two Time
        /// </summary>
        /// <param name="FromDate">DateTime</param>
        /// <param name="ToDate">DateTime</param>
        /// <returns>long</returns>
        public static long TimeDifference(string FormTime, string ToTime)
        {
            DateTime dt1 = Convert.ToDateTime(FormTime);
            DateTime dt2 = Convert.ToDateTime(ToTime);
            TimeSpan ts = dt2 - dt1;
            return (long)ts.TotalMinutes;
        }

        /// <summary>
        /// This Function Returns Last Date For Current Month
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static DateTime LastDateForCurrentMonth(DateTime Date)
        {

            string Temp = null;
            Date = Date.AddMonths(1);
            Temp = Date.Date.ToString("MMM-yy");
            Temp = "01-" + Temp;
            Date = Convert.ToDateTime(Temp);
            Date = Date.AddDays(-1);
            return Date;
        }

        /// <summary>
        /// This Function Returns First Date For Current Month
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static DateTime FirstDateForCurrentMonth(DateTime Date)
        {
            string Temp = null;
            Temp = Date.Date.ToString("MMM-yy");
            Temp = "01-" + Temp;
            Date = Convert.ToDateTime(Temp);
            return Date;
        }

        /// <summary>
        /// This Function Return 1 if Firsdate is greater then SecondDate
        /// return -1 if Firsdate is less then SecondDate
        /// return 0 if Firsdate is equal to SecondDate
        /// String Date Format "dd/mm/yyyy"
        /// </summary>
        /// <param name="FirstDate"></param>
        /// <param name="SecondDate"></param>
        /// <returns></returns>
        public static int DateCompeare(string FirstDate, string SecondDate)
        {
            string[] str = null;
            int result;
            str = FirstDate.Split(new Char[] { '/' });
            DateTime dt1 = new DateTime(Convert.ToInt32(str[2]), Convert.ToInt32(str[1]), Convert.ToInt32(str[0]));
            str = SecondDate.Split(new Char[] { '/' });
            DateTime dt2 = new DateTime(Convert.ToInt32(str[2]), Convert.ToInt32(str[1]), Convert.ToInt32(str[0]));
            if (dt1 >= dt2)
                result = 1;
            else if (dt1 <= dt2)
                result = -1;
            else
                result = 0;

            return result;
        }

        public static DateTime GetDateFromString(string p_Date)
        {
            DateTime date;
            string[] str = p_Date.Split(new Char[] { ' ' });
            if (str.Length > 1)
            {
                date = Convert.ToDateTime(p_Date);
            }
            else
            {
                string[] stringArray = p_Date.Split(new Char[] { '/' });
                date = new DateTime(Convert.ToInt32(stringArray[2]), Convert.ToInt32(stringArray[1]), Convert.ToInt32(stringArray[0]));
            }
            return date;
        }

        public static DateTime GetDateFromString(string p_Date, char ch)
        {
            string[] stringArray = p_Date.Split(new Char[] { ch });
            DateTime date = new DateTime(Convert.ToInt32(stringArray[2]), Convert.ToInt32(stringArray[1]), Convert.ToInt32(stringArray[0]));
            return date;
        }

        public static DateTime ConvertToDateTime(string p_Date, string p_Time, char ch)
        {

            string[] stringArray = p_Date.Split(new Char[] { ch });

            DateTime dt = Convert.ToDateTime(p_Time);

            DateTime date = new DateTime(Convert.ToInt32(stringArray[2]), Convert.ToInt32(stringArray[1]), Convert.ToInt32(stringArray[0]), dt.Hour, dt.Minute, dt.Second);

            return date;
        }

        /// <summary>
        ///  This Function Return true 
        ///  If CurrentDate is between FromDate and ToDate Otherwise False
        /// </summary>
        /// <param name="FromDate">DateTime</param>
        /// <param name="ToDate">DateTime</param>
        /// <returns>long</returns>
        public static bool BetweenTwoDate(DateTime CurrentDate, DateTime FromDate, DateTime ToDate)
        {
            int intYear = Convert.ToInt32(ToDate.ToString("yyyy"));
            int intMonth = Convert.ToInt32(ToDate.ToString("MM"));
            int intDay = Convert.ToInt32(ToDate.ToString("dd"));
            DateTime dtTo = new DateTime(intYear, intMonth, intDay);

            intYear = Convert.ToInt32(FromDate.ToString("yyyy"));
            intMonth = Convert.ToInt32(FromDate.ToString("MM"));
            intDay = Convert.ToInt32(FromDate.ToString("dd"));
            DateTime dtForm = new DateTime(intYear, intMonth, intDay);

            intYear = Convert.ToInt32(CurrentDate.ToString("yyyy"));
            intMonth = Convert.ToInt32(CurrentDate.ToString("MM"));
            intDay = Convert.ToInt32(CurrentDate.ToString("dd"));
            DateTime dtCurrent = new DateTime(intYear, intMonth, intDay);

            bool blnResult = false;

            if (dtForm <= dtCurrent && dtCurrent <= dtTo)
                blnResult = true;

            return blnResult;
        }

        /// <summary>
        /// This Function Return a string (Time Format)
        /// </summary>
        /// <param name="Min"></param>
        /// <returns></returns>
        public static string ConvertMinutetoHour(long Min)
        {
            long Hour = 0;
            long Minutes = 0;
            string st = null;
            Hour = Min / 60;
            Minutes = Math.Abs(Min % 60);
            st = Hour.ToString() + " H " + Minutes.ToString("00") + " M";
            return st;
        }

        /// <summary>
        /// Finds The Number of Days of the selected month and year.
        /// </summary>
        /// <param name="YearID"></param>
        /// <param name="MonthID"></param>
        /// <returns></returns>
        public static int GetNumberofDays(int YearID, int MonthID)
        {
            //Added By Kumaresh 
            int NumberOfDays = DateTimeFormatInfo.CurrentInfo.Calendar.GetDaysInMonth(YearID, MonthID);
            return NumberOfDays;
        }

        public static string GetNameofMonth(int monthId)
        {
            string returnValue = "";
            string monthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(monthId);
            if (monthName == "January")
            {
                returnValue = "JAN";
            }
            else if (monthName == "February")
            {
                returnValue = "FEB";
            }
            else if (monthName == "March")
            {
                returnValue = "MAR";
            }
            else if (monthName == "April")
            {
                returnValue = "APR";
            }
            else if (monthName == "May")
            {
                returnValue = "MAY";
            }
            else if (monthName == "June")
            {
                returnValue = "JUN";
            }
            else if (monthName == "July")
            {
                returnValue = "JUL";
            }
            else if (monthName == "August")
            {
                returnValue = "AUG";
            }
            else if (monthName == "September")
            {
                returnValue = "SEP";
            }
            else if (monthName == "October")
            {
                returnValue = "OCT";
            }
            else if (monthName == "November")
            {
                returnValue = "NOV";
            }
            else if (monthName == "December")
            {
                returnValue = "DEC";
            }
            return returnValue;
        }

        public static DateTime ConvertToDateTime(string DateTimeString) //Added by Ishtiaque...
        {
            string DateString = DateTimeString;
            int Index = DateString.IndexOf("/");
            string DayString;
            if (Index == 1)
                DayString = DateString.Substring(0, 1);
            else
                DayString = DateString.Substring(0, 2);
            DateString = DateString.Substring(Index + 1, DateString.Length - (Index + 1));
            string MonthString;
            Index = DateString.IndexOf("/");
            if (Index == 1)
                MonthString = DateString.Substring(0, 1);
            else
                MonthString = DateString.Substring(0, 2);
            DateString = DateString.Substring(Index + 1, DateString.Length - (Index + 1));
            string YearString = DateString;
            DateTime DT = new DateTime(Convert.ToInt32(YearString), Convert.ToInt32(MonthString), Convert.ToInt32(DayString));
            return DT;
        }

        public static bool MonthYearIsInRange(DateTime toBechecked, DateTime from, DateTime upto)
        {
            var tb = new DateTime(toBechecked.Year, toBechecked.Month, 1);
            var f = new DateTime(from.Year, from.Month, 1);
            var u = new DateTime(upto.Year, upto.Month, 1);
            if (f.Subtract(tb).Days == 0)
                return true;
            if (tb.Subtract(f).Days > 0 && u.Subtract(tb).Days >= 0)
                return true;
            return false;
        }

        public static bool MonthYearIsSame(DateTime toBechecked, DateTime dt)
        {
            var tb = new DateTime(toBechecked.Year, toBechecked.Month, 1);
            var d = new DateTime(dt.Year, dt.Month, 1);

            if (d.Subtract(tb).Days == 0)
                return true;
            return false;
        }

        public static bool DayMonthYearIsInRange(DateTime toBechecked, DateTime from, DateTime upto)
        {
            var tb = new DateTime(toBechecked.Year, toBechecked.Month, toBechecked.Day);
            var f = new DateTime(from.Year, from.Month, from.Day);
            var u = new DateTime(upto.Year, upto.Month, upto.Day);
            if (f.Subtract(tb).Days == 0)
                return true;
            if (tb.Subtract(f).Days > 0 && u.Subtract(tb).Days >= 0)
                return true;
            return false;
        }
    }
}