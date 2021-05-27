using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayrollWeb.ViewModels
{
    public class CertificateUpload
    {
        public int ID { get; set; }

        public string Emp_ID { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int? No_of_Cars { get; set; }

        public decimal Amount { get; set; }

        public string Is_Appropved { get; set; }

        public DateTime Submission_Date { get; set; }

    }
}