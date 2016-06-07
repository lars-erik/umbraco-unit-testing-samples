using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Umb.Testing.Web.Controllers
{
    public class CtaFormController : SurfaceController
    {
        public PartialViewResult Form(string viewName)
        {
            return PartialView(viewName);
        }
    }
}