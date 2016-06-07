using System.Web.Mvc;
using Umb.Testing.Web.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Umb.Testing.Web.Controllers
{
    public class CtaFormController : SurfaceController
    {
        public CtaFormController(UmbracoContext umbracoContext) : base(umbracoContext)
        {
        }

        public CtaFormController(UmbracoContext umbracoContext, UmbracoHelper umbracoHelper) : base(umbracoContext, umbracoHelper)
        {
        }

        public CtaFormController()
        {
        }

        public PartialViewResult Form(string viewName)
        {
            var model = new FormModel
            {
                PageName = Umbraco.AssignedContentItem.Name
            };

            return PartialView(viewName, model);
        }
    }
}