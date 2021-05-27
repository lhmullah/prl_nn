using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace PayrollWeb.Service
{
    public class ProvidentFundService
    {
        private string connectionString;

        public ProvidentFundService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["conPfFundDb"].ConnectionString;
        }

        public decimal TotalFundAmount(string empId, DateTime monthYearUpto)
        {
            decimal amnt = 0;
            try
            {
                using (var con = new MySqlConnection(connectionString))
                {
                    using (var cmd= new MySqlCommand("",con))
                    {
                        cmd.Parameters.AddWithValue("?empId", "");
                        con.Open();

                    }
                }
            }
            catch (Exception excp)
            {
                
                throw;
            }
            


            return amnt;
        }
    }
}