using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umb.Testing.Web.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Umb.Testing.Web.Controllers
{
    public class CtaFormController : SurfaceController
    {
        private readonly IMailGateway mailGateway;

        #region constructors
        public CtaFormController()
            : this(UmbracoContext.Current)
        {
        }

        public CtaFormController(UmbracoContext umbracoContext) : this(umbracoContext, new UmbracoHelper(umbracoContext), new MailGateway())
        {
        }

        public CtaFormController(UmbracoContext umbracoContext, UmbracoHelper umbracoHelper, IMailGateway mailGateway) : base(umbracoContext, umbracoHelper)
        {
            this.mailGateway = mailGateway;
            
        }

        #endregion

        public PartialViewResult Form(string viewName)
        {
            var model = new FormModel
            {
                PageName = Umbraco.AssignedContentItem.Name
            };

            return PartialView(viewName, model);
        }

        #region step 2

        public PartialViewResult Post(FormModel model)
        {
            mailGateway.Send(
                model.Email + ", hardcoded@email.com",
                "Welcome to our newsletter",
                "Hi, you subscribed with " + model.Email + " via " + model.PageName
                );
            return PartialView(model.ViewName, model);
        }

        #endregion
    }

    #region step 2
    public interface IMailGateway
    {
        void Send(string to, string subject, string message);
    }

    public class MailGateway : IMailGateway
    {
        public void Send(string to, string subject, string message)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}