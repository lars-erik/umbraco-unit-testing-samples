using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Umb.Testing.Web.Models
{
    public class ContentModel : PublishedContentModel
    {
        public ContentModel(IPublishedContent content) : base(content)
        {
        }

        public string MessageFromController { get; set; }
        public string CtaForm { get; set; }
    }
}