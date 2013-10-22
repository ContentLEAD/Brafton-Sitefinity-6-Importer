using System;
using System.Linq;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Personalization;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Personalization.Impl.Evaluators;
using Telerik.Sitefinity.Personalization.Impl;
using Telerik.Sitefinity.Publishing;
using SitefinityWebApp.Publishing;
using Telerik.Sitefinity.Publishing.Model;

namespace SitefinityWebApp
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Telerik.Sitefinity.Abstractions.Bootstrapper.Initialized += Bootstrapper_Initialized;
        }

        protected void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs args)
        {
			//Register Inbound Pipe
            PublishingSystemFactory.RegisterPipe(XmlInboundPipe.PipeName, typeof(XmlInboundPipe));

            //Register Mappings
            var mappingsList = XmlInboundPipe.GetDefaultMapping();
            PublishingSystemFactory.RegisterPipeMappings(XmlInboundPipe.PipeName, true, mappingsList);

            //Register Pipe Settings
            RssPipeSettings pipeSettings = new RssPipeSettings();
            pipeSettings.IsInbound = true;
            pipeSettings.IsActive = true;
            pipeSettings.MaxItems = 25;
            pipeSettings.InvocationMode = PipeInvokationMode.Push;
            pipeSettings.UIName = "Brafton Feed";
            pipeSettings.PipeName = XmlInboundPipe.PipeName;
            pipeSettings.ResourceClassId = typeof(PublishingModuleExtensionsResources).Name;

            PublishingSystemFactory.RegisterPipeSettings(XmlInboundPipe.PipeName, pipeSettings);

            //Register Pipe Definitions
            var definitions = XmlInboundPipe.CreateDefaultPipeDefinitions();
            PublishingSystemFactory.RegisterPipeDefinitions(XmlInboundPipe.PipeName, definitions);

            //Register Outbound Pipe
            PublishingSystemFactory.RegisterPipe(CustomContentOutboundPipe.PipeName, typeof(CustomContentOutboundPipe));
        }
        
    }
}