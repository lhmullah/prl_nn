using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcSiteMapProvider;


namespace PayrollWeb.Models
{
    public class NVisibility : ISiteMapNodeVisibilityProvider
    {
        public bool IsVisible(ISiteMapNode node, IDictionary<string, object> sourceMetadata)
        {
            throw new NotImplementedException();
        }

        public bool AppliesTo(string providerName)
        {
            throw new NotImplementedException();
        }
    }
}