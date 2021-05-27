using System;
using System.Collections.Generic;

namespace com.linde.Model
{
    public partial class prl_company_bank
    {
        public int id { get; set; }
        public string bank_name { get; set; }
        public string account_no { get; set; }
        public string routing_no { get; set; }
        public string account_type { get; set; }
        public string account_category { get; set; }
    }
}
