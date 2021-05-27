using System.Collections.Generic;

namespace com.gpit.Model
{
    public partial class prl_menu
    {
        public prl_menu()
        {
            this.prl_sub_menu = new List<prl_sub_menu>();
        }

        public int id { get; set; }
        public string menue_name { get; set; }
        public string remarks { get; set; }
        public string module { get; set; }
        public virtual ICollection<prl_sub_menu> prl_sub_menu { get; set; }
    }
}
