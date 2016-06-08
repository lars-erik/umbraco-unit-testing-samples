using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umb.Testing.Web.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Umb.Testing.Web.Controllers
{
    public class ContentController : RenderMvcController
    {
        public ContentController()
        {
        }

        public ContentController(UmbracoContext umbracoContext) : base(umbracoContext)
        {
        }

        public ContentController(UmbracoContext umbracoContext, UmbracoHelper umbracoHelper) : base(umbracoContext, umbracoHelper)
        {
        }

        public override ActionResult Index(RenderModel model)
        {
            var replacedContent = new ContentModel(model.Content);
            var replacedModel = new RenderModel<ContentModel>(replacedContent);

            if (User.Identity.IsAuthenticated)
                replacedContent.CtaForm = "AuthenticatedForm";
            else
                replacedContent.CtaForm = "AnonymousForm";

            return base.Index(replacedModel);
        }
    }
}