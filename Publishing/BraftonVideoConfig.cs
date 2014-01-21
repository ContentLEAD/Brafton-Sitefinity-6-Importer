using System;
using System.Configuration;
using System.Linq;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Localization;

namespace SitefinityWebApp.Publishing
{
    /// <summary>
    /// Sitefinity configuration section.
    /// </summary>
    /// <remarks>
    /// If this is a Sitefinity module's configuration,
    /// you need to add this to the module's Initialize method:
    /// App.WorkWith()
    ///     .Module(ModuleName)
    ///     .Initialize()
    ///         .Configuration<BraftonVideoConfig1>();
    /// 
    /// You also need to add this to the module:
    /// protected override ConfigSection GetModuleConfig()
    /// {
    ///     return Config.Get<BraftonVideoConfig1>();
    /// }
    /// </remarks>
    /// <see cref="http://www.sitefinity.com/documentation/documentationarticles/developers-guide/deep-dive/configuration/creating-configuration-classes"/>
    [ObjectInfo(Title = "BraftonVideoConfig1 Title", Description = "BraftonVideoConfig1 Description")]
    public class BraftonVideoConfig : ConfigSection
    {
        [ObjectInfo(Title = "Public Key", Description = "Brafton Video Public Key")]
        [ConfigurationProperty("PublicKey", DefaultValue = "Public")]
        public string BraftonVideoPublic
        {
            get
            {
                return (string)this["PublicKey"];
            }
            set
            {
                this["PublicKey"] = value;
            }
        }

        [ObjectInfo(Title = "Private Key", Description = "Brafton Video Private Key")]
        [ConfigurationProperty("PrivateKey", DefaultValue = "Private")]
        public string BraftonVideoPrivate
        {
            get
            {
                return (string)this["PrivateKey"];
            }
            set
            {
                this["PrivateKey"] = value;
            }
        }

        [ObjectInfo(Title = "Feed Number", Description = "Brafton Video Feed Number (Usually 0)")]
        [ConfigurationProperty("FeedNumber", DefaultValue = "0")]
        public int BraftonVideoFeedNumber
        {
            get
            {
                return (int)this["FeedNumber"];
            }
            set
            {
                this["FeedNumber"] = value;
            }
        }
    }
}