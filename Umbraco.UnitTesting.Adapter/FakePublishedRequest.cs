using System;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Umbraco.UnitTesting.Adapter
{
    public class FakePublishedRequest : PublishedRequest
    {
        public FakePublishedRequest(IPublishedRouter publishedRouter, UmbracoContext umbracoContext, Uri uri)
            : base(publishedRouter, umbracoContext, uri)
        {
            
        }
    }
}
