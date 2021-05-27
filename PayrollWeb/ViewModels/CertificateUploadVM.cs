using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class CertificateUploadVM
    {
        public int id { get; set; }

        public string emp_id { get; set; }
        public string income_year { get; set; }
        public string certificat_type { get; set; }
        public decimal amount { get; set; }
        public string file_path { get; set; }
        public int? number_of_car { get; set; }
        public bool is_approved { get; set; }

        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
    }
}