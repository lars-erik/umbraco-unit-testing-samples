using System;
using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Umbraco.UnitTesting.Adapter.Support
{
    public class FakeModelFactoryResolver : PublishedContentModelFactoryResolver
    {
        public FakeModelFactoryResolver(IPublishedContentModelFactory factory) : base(factory)
        {
        }
    }

    public class FakeContentModelFactory : IPublishedContentModelFactory
    {
        protected Dictionary<string, Func<IPublishedContent, IPublishedContent>> Factories { get; } = new Dictionary<string, Func<IPublishedContent, IPublishedContent>>();

        public void Register(string alias, Func<IPublishedContent, IPublishedContent> factory)
        {
            if (Factories.ContainsKey(alias))
                Factories[alias] = factory;
            else
                Factories.Add(alias, factory);
        }

        public IPublishedContent CreateModel(IPublishedContent content)
        {
            if (Factories.ContainsKey(content.DocumentTypeAlias))
                content = Factories[content.DocumentTypeAlias](content);

            return content;
        }
    }
}
