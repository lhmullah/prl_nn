using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayrollWeb.ViewModels.Utility
{
    public class ImportFileViewModel
    {
        [DisplayName("Upload File")]
        [Required(ErrorMessage = "File is required.")]
        public HttpPostedFileBase ImportFile { get; set; }
    }
}