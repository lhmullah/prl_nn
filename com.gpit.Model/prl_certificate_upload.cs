using com.linde.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.linde.Model
{
    public partial class prl_certificate_upload
    {
        public int id { get; set; }

        public int emp_id { get; set; }

        public string income_year { get; set; }

        public string certificat_type { get; set; }

        public decimal amount { get; set; }

        public string file_path { get; set; }

        public int? number_of_car { get; set; }

        public bool is_approved { get; set; }

        public string created_by { get; set; }

        public DateTime created_date { get; set; }

        public string updated_by { get; set; }

        public Nullable<DateTime> updated_date { get; set; }

        public virtual prl_employee prl_employee { get; set; }

    }
}
