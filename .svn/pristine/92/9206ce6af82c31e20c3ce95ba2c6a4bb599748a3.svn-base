using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.Models
{
    public class EnumToSelectItemList
    {
        public static IEnumerable<SelectListItem> GetEnumSelectList<T>()
        {
            return (Enum.GetValues(typeof(T)).Cast<T>().Select(
                enu => new SelectListItem() { Text = enu.ToString(), Value = Convert.ToInt32(enu).ToString() })).ToList();
        }
    }
}