using System;
using System.Linq;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Data;

namespace SitefinityWebApp.Publishing
{
    [ObjectInfo(typeof(PublishingModuleExtensionsResources), Title = "PublishingResourcesTitle", Description = "PublishingResourcesDescription")]
    public class PublishingModuleExtensionsResources : Resource
    {
        public PublishingModuleExtensionsResources()
        {
        }

        public PublishingModuleExtensionsResources(ResourceDataProvider dataProvider)
            : base(dataProvider)
        {
        }

        [ResourceEntry("XmlInboundPipeName",
                        Value = "Xml Inbound Pipe Name",
                        Description = "Xml Inbound Pipe Name",
                        LastModified = "2011/08/10")]
        public string XmlInboundPipeName
        {
            get { return this["XmlInboundPipeName"]; }
        }
    }
}