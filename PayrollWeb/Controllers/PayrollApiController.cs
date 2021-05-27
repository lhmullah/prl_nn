using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Ninject.Selection.Heuristics;
using PayrollWeb.ViewModels;

namespace PayrollWeb.Controllers
{
    public class PayrollApiController : ApiController
    {
        // GET api/payrollapi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/payrollapi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/payrollapi
        public void Post([FromBody]string value)
        {
        }

        // PUT api/payrollapi/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE api/payrollapi/5
        public void Delete(int id)
        {
        }
        
        
    }
}
