using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace Umb.Testing.Web.Controllers
{
    public class SimpleAuthorizedController : UmbracoAuthorizedApiController
    {
        public SimpleAuthorizedController()
        {
        }

        public SimpleAuthorizedController(UmbracoContext umbracoContext) : base(umbracoContext)
        {
        }

        public SimpleAuthorizedController(UmbracoContext umbracoContext, UmbracoHelper umbracoHelper) : base(umbracoContext, umbracoHelper)
        {
        }

        public string[] List()
        {
            return new[]
            {
                "A", "B"
            };
        }

        public string GetUserInfo()
        {
            return Security.CurrentUser.Name;
        }
    }
}