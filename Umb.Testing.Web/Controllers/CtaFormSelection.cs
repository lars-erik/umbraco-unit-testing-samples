using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umb.Testing.Web.Models;

namespace Umb.Testing.Web.Controllers
{
    public class CtaFormSelection
    {
        private readonly HttpContextBase httpContext;

        public CtaFormSelection(HttpContextBase httpContext)
        {
            this.httpContext = httpContext;
        }

        public void SelectForm(ContentModel contentModel)
        {
            if (httpContext.User.Identity.IsAuthenticated)
                contentModel.CtaForm = "AuthenticatedForm";
            else
                contentModel.CtaForm = "AnonymousForm";
        }
    }
}