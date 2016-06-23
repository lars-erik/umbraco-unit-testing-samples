using System;
using System.Web.Mvc;
using Umb.Testing.Web.Models;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Umb.Testing.Web.Controllers
{
    public class BylineController : SurfaceController
    {
        public BylineController(UmbracoContext umbracoContext, UmbracoHelper umbracoHelper)
            : base(umbracoContext, umbracoHelper)
        {
        }

        public PartialViewResult Byline()
        {
            var content = Umbraco.AssignedContentItem;
            return PartialView("Byline", new BylineModel(
                content.GetPropertyValue<string>("author").IfNullOrWhiteSpace(content.WriterName), 
                content.GetPropertyValue<DateTime?>("date") ?? content.CreateDate
            ));
        }
    }
}