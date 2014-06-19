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

        [ObjectInfo(Title = "Written Content", Description = "Import Brafton Written Content?")]
        [ConfigurationProperty("BraftonWritten", DefaultValue = true)]
        public bool BraftonWritten
        {
            get
            {
                return (bool)this["BraftonWritten"];
            }
            set
            {
                this["BraftonWritten"] = value;
            }
        }

        [ObjectInfo(Title = "Content API URL Base", Description = "Brafton Written Content API URL")]
        [ConfigurationProperty("ContentURL", DefaultValue = "http://api.brafton.com/")]
        public string ContentURL
        {
            get
            {
                return (string)this["ContentURL"];
            }
            set
            {
                this["ContentURL"] = value;
            }
        }

        [ObjectInfo(Title = "Content API Key", Description = "Brafton Written Content API Key<br/>Example:dada3480-9d3b-4989-876a-663fdbe48be8")]
        [ConfigurationProperty("ContentKey", DefaultValue = "xxxx")]
        public string ContentKey
        {
            get
            {
                return (string)this["ContentKey"];
            }
            set
            {
                this["ContentKey"] = value;
            }
        }

        [ObjectInfo(Title = "Video Content", Description = "Import Brafton Video Content?")]
        [ConfigurationProperty("BraftonVideo", DefaultValue = true)]
        public bool BraftonVideo
        {
            get
            {
                return (bool)this["BraftonVideo"];
            }
            set
            {
                this["BraftonVideo"] = value;
            }
        }


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


        [ObjectInfo(Title = "Archive Content", Description = "Import Brafton Written Content from Archive?")]
        [ConfigurationProperty("BraftonArchive", DefaultValue = false)]
        public bool BraftonArchive
        {
            get
            {
                return (bool)this["BraftonArchive"];
            }
            set
            {
                this["BraftonArchive"] = value;
            }
        }

        [ObjectInfo(Title = "Archive File URL", Description = "Brafton Written Content Archive")]
        [ConfigurationProperty("ArchiveURL", DefaultValue = "")]
        public string ArchiveURL
        {
            get
            {
                return (string)this["ArchiveURL"];
            }
            set
            {
                this["ArchiveURL"] = value;
            }
        }

        [ObjectInfo(Title = "Import As Blogs Instead of News", Description = "By default importer will import into News. Check this to import into Blogs.")]
        [ConfigurationProperty("ImportBlogs", DefaultValue = false)]
        public bool ImportBlogs
        {
            get
            {
                return (bool)this["ImportBlogs"];
            }
            set
            {
                this["ImportBlogs"] = value;
            }
        }


    }
}