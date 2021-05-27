using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.gpit.DataContext
{
    public class BaseContext<TContext>: DbContext where TContext : DbContext
    {
        static BaseContext()
        {
            Database.SetInitializer<TContext>(null);
        }

        protected BaseContext(): base("Name=payroll_systemContext")
        {
                
        }
    }
}
