using System;
using System.Reflection;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Web;
using AdferoVideoDotNet.AdferoArticles;
using AdferoVideoDotNet.AdferoArticlesVideoExtensions;
using AdferoVideoDotNet.AdferoPhotos;

using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Publishing.Configuration;
using Telerik.Sitefinity.Publishing.Model;
using Telerik.Sitefinity.Publishing.Pipes;
using Telerik.Sitefinity.Publishing.Translators;
using Telerik.Sitefinity;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Workflow;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Scheduling;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.News.Model;


namespace SitefinityWebApp.Publishing
{
    [PipeDesigner(typeof(XmlPipeImportDesignerView), null)]
    public class XmlInboundPipe : IPipe, IPushPipe, IPullPipe, IInboundPipe
    {
        public virtual PipeSettings PipeSettings
        {
            get;
            set;

        }

        public virtual RssPipeSettings RssPipeSettings
        {
            get
            {
                return (RssPipeSettings)this.PipeSettings;
            }
            set
            {
                this.PipeSettings = value;
            }
        }


        public virtual string PublishingProviderName
        {
            get
            {
                if (string.IsNullOrEmpty(this.publishingProviderName))
                {
                    this.publishingProviderName = Config.Get<PublishingConfig>().DefaultProvider;
                }
                return this.publishingProviderName;
            }
            set
            {
                this.publishingProviderName = value;
            }
        }

        public virtual string Name { get { return XmlInboundPipe.PipeName; } }

        public virtual IDefinitionField[] Definition
        {
            get
            {
                if (this.definition == null)
                {
                    this.definition = PublishingSystemFactory.GetPipeDefinitions(this.Name);
                }
                return this.definition;
            }
        }

        public virtual Type PipeSettingsType
        {
            get
            {
                return typeof(RssPipeSettings);
            }
        }

        public const string PipeName = "Brafton Feed";

        private IPublishingPointBusinessObject publishingPoint;
        private string publishingProviderName;
        private IDefinitionField[] definition = null;


        protected virtual IList<XmlDocument> DeserializeXml()
        {

            var result = new List<XmlDocument>();
            //string tr = ;
            BraftonVideoConfig config = Config.Get<BraftonVideoConfig>();

            bool braftonVideo = config.BraftonVideo;
            bool braftonWritten = config.BraftonWritten;

            string contentURL = config.ContentURL;
            string contentKey = config.ContentKey;
            string publicKey = config.BraftonVideoPublic;
            string secretKey = config.BraftonVideoPrivate;
            int feedNumber = config.BraftonVideoFeedNumber;


            //if (!ValidateVideoPublicKey(publicKey))
            //{
            //    //Log("Invalid video public key. Stopping.", LogLevel.Error);
            //    return;
            //}

            //if (!ValidateGuid(secretKey))
            //{
            //    //Log("Invalid video secret key. Stopping.", LogLevel.Error);
            //    return;
            //}

            //Log("Starting video import.", LogLevel.Debug);

            if (braftonVideo)
            {
                string baseUrl = "http://api.video.brafton.com/v2/";
                string basePhotoUrl = "http://pictures.video.brafton.com/v2/";
                AdferoVideoClient videoClient = new AdferoVideoClient(baseUrl, publicKey, secretKey);
                AdferoClient client = new AdferoClient(baseUrl, publicKey, secretKey);
                AdferoPhotoClient photoClient = new AdferoPhotoClient(basePhotoUrl);

                AdferoVideoDotNet.AdferoArticles.ArticlePhotos.AdferoArticlePhotosClient photos = client.ArticlePhotos();
                string scaleAxis = AdferoVideoDotNet.AdferoPhotos.Photos.AdferoScaleAxis.X;

                AdferoVideoDotNet.AdferoArticles.Feeds.AdferoFeedsClient feeds = client.Feeds();
                AdferoVideoDotNet.AdferoArticles.Feeds.AdferoFeedList feedList = feeds.ListFeeds(0, 10);

                AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticlesClient articles = client.Articles();
                AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticleList articleList = articles.ListForFeed(feedList.Items[feedNumber].Id, "live", 0, 100);

                int articleCount = articleList.Items.Count;
                AdferoVideoDotNet.AdferoArticles.Categories.AdferoCategoriesClient categories = client.Categories();


                foreach (AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticleListItem item in articleList.Items)
                {
                    int brafId = item.Id;
                    AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticle article = articles.Get(brafId);
                    var guid = convertIdToGuid(brafId.ToString());

                    var title = article.Fields["title"].Trim();

                    string embedCode = videoClient.VideoPlayers().GetWithFallback(article.Id, AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoPlayers.AdferoPlayers.RedBean, new AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoPlayers.AdferoVersion(1, 0, 0), AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoPlayers.AdferoPlayers.RcFlashPlayer, new AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoPlayers.AdferoVersion(1, 0, 0)).EmbedCode;

                    string content = string.Format("<div class=\"videoContainer\">{0}</div> {1}", embedCode, article.Fields["content"]);

                    PhotoInstance? fullSizePhoto = GetPhotoInstance(article, photos, photoClient, scaleAxis, 500);

                    var imageurl = "http://fc02.deviantart.net/fs20/f/2007/271/6/e/TF2_Sniper_Thanks_by_The_Loiterer.jpg";
                    var album = "Brafton";

                    if (fullSizePhoto != null)
                    {
                        var imageid = fullSizePhoto.Value.Id.ToString();
                        var remoteimageurl = fullSizePhoto.Value.Url;
                        var imagename = fullSizePhoto.Value.DestinationFileName;
                        var imageguid = convertIdToGuid(imageid);
                        imageurl = DownloadRemoteImageFile(imageguid, album, imagename, remoteimageurl, ".jpg");
                    }

                    //article.Fields["lastModifiedDate"];

                    var pubdate = article.Fields["date"];

                    //category code

                    AdferoVideoDotNet.AdferoArticles.Categories.AdferoCategoryList categoryList = categories.ListForArticle(article.Id, 0, 100);

                    StringBuilder itemCategories = new StringBuilder();

                    for (int i = 0; i < categoryList.TotalCount; i++)
                    {
                        AdferoVideoDotNet.AdferoArticles.Categories.AdferoCategory category = categories.Get(categoryList.Items[i].Id);
                        if (i > 0)
                        {
                            itemCategories.Append(",");
                        }
                        itemCategories.Append(GetCleanCategoryName(category.Name));
                        
                    }


                    //save post

                    result.Add(new XmlDocument(guid, title, content, pubdate, imageurl, itemCategories.ToString()));
                }
            }

            //regular content here
            
            if (braftonWritten)
            {
                ApiContext api = new ApiContext(contentKey, contentURL);
                foreach (newsItem ni in api.News)
                {
                    Guid brafId = convertIdToGuid(ni.id.ToString());

                    StringBuilder itemCategories = new StringBuilder();
                    int count = 0;
                    foreach (category c in ni.categories)
                    {
                        if (count > 0)
                        {
                            itemCategories.Append(",");
                        }
                        count++;
                     
                        itemCategories.Append(GetCleanCategoryName(c.name));
                        
                    }
                    
                    string Content = ni.text;

                    string DateCreated = ni.publishDate.ToString();
                    var DateModified = ni.lastModifiedDate;
                    string Description = ni.extract;

                    string Slug = Slugify(ni.headline);

                    string Title = ni.headline.Trim();

                    PhotoInstance? fullSizePhoto = GetPhotoInstance(ni, enumeratedTypes.enumPhotoInstanceType.Large);

                    var imageurl = "http://fc02.deviantart.net/fs20/f/2007/271/6/e/TF2_Sniper_Thanks_by_The_Loiterer.jpg";
                    var album = "Brafton";

                    if (fullSizePhoto != null)
                    {
                        var imageid = fullSizePhoto.Value.Id.ToString();
                        var remoteimageurl = fullSizePhoto.Value.Url;
                        var imagename = fullSizePhoto.Value.DestinationFileName;
                        var imageguid = convertIdToGuid(imageid);
                        imageurl = DownloadRemoteImageFile(imageguid, album, imagename, remoteimageurl, ".jpg");
                    }

                    result.Add(new XmlDocument(brafId, Title, Content, DateCreated, imageurl, itemCategories.ToString()));

                 }
             }
      
            return result;
        }

        private PhotoInstance? GetPhotoInstance(newsItem ni, enumeratedTypes.enumPhotoInstanceType photoType)
        {
            return GetPhotoInstance(ni, new enumeratedTypes.enumPhotoInstanceType[] { photoType });
        }

        private PhotoInstance? GetPhotoInstance(newsItem ni, enumeratedTypes.enumPhotoInstanceType[] photoTypes)
        {
            IEnumerator<photo> phEn = ni.photos.GetEnumerator();
            if (!phEn.MoveNext())
                return null;

            PhotoInstance phIns = new PhotoInstance();
            bool found = false;
            photo ph = phEn.Current;
            IEnumerator<photo.Instance> phIEn = ph.Instances.GetEnumerator();
            while (phIEn.MoveNext())
            {
                foreach (enumeratedTypes.enumPhotoInstanceType phType in photoTypes)
                {
                    if (phIEn.Current.type == phType)
                    {
                        phIns.Width = phIEn.Current.width;
                        phIns.Height = phIEn.Current.height;
                        phIns.Url = phIEn.Current.url;
                        phIns.Type = phIEn.Current.type;
                        phIns.Type = phType;

                        string cleanedUrl = phIns.Url;
                        if (cleanedUrl.IndexOf('?') >= 0)
                            cleanedUrl = cleanedUrl.Substring(0, cleanedUrl.IndexOf('?'));

                        string phTypeSlug = Slugify(phType.ToString());
                        phIns.DestinationFileName = Slugify(ni.headline) + (phTypeSlug == "" ? "" : "-" + phTypeSlug) + Path.GetExtension(cleanedUrl);

                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    phIns.AltText = phEn.Current.htmlAlt;
                    phIns.Caption = phEn.Current.caption;
                    phIns.Id = phEn.Current.id;
                    phIns.Orientation = phEn.Current.orientation;

                    break;
                }
            }

            if (!found)
                return null;
            return phIns;
        }


        private string readCategories(string url)
        {
            StringBuilder itemCategories = new StringBuilder();
            var doc = XDocument.Load(url);
            var xRoot = doc.Root;
            var categories = xRoot.Elements("category");
            int count = 0;
            foreach (var item in categories)
            {
                if (count > 0)
                {
                    itemCategories.Append(",");
                }
                count++;
                itemCategories.Append(item.Element("name").Value);
            }

            return itemCategories.ToString();
        }

        protected string GetCleanCategoryName(string catName)
        {
            // blogengine treats categories with dashes as child categories.
            // HACK: this is an en dash.
            // not quite the same, but avoids ophaning this category.
            return catName.Replace("-", "–").Trim();
        }

        private Guid convertIdToGuid(string id) {
            var guid = "00000000000000000000000000000000";
            guid = id + guid.Substring(id.Length);
            guid = guid.Insert(8, "-");
            guid = guid.Insert(13, "-");
            guid = guid.Insert(18, "-");
            guid = guid.Insert(23, "-");
            return new Guid(guid);
        }

        private string readContent(string url) {
            return XDocument.Load(url).Element("newsItem").Element("text").Value;
        }

        private string readImages(string url)
        {
            return XDocument.Load(url).Element("photos").Element("photo").Element("instances").Element("instance").Element("url").Value;
        }

        private string readImageID(string url)
        {
            return XDocument.Load(url).Element("photos").Element("photo").Element("id").Value;
        }

        //Remove spaces and special characters from image file names.

        private static string MakeValidFileName(string name)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in name)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        //Download images to Sitefinity project directory. To do: allow user to assign a directory through Sitefinity admin interface.

        private static string DownloadRemoteImageFile(Guid masterImageId, string albumTitle, string imageTitle, string uri, string imageExtension)
        {
                // if the remote file was found, download it
                WebRequest req = HttpWebRequest.Create(uri);
                using (Stream inputStream = req.GetResponse().GetResponseStream())
                {
                  return CreateImage(masterImageId, albumTitle, imageTitle, inputStream, imageExtension);
                }
        }
        
        private static string CreateImage(Guid masterImageId, string albumTitle, string imageTitle, Stream imageStream, string imageExtension)
        {
            LibrariesManager librariesManager = LibrariesManager.GetManager();
            Image image = librariesManager.GetImages().Where(i => i.Id == masterImageId).FirstOrDefault();

            if (image == null)
            {
                //The album post is created as master. The masterImageId is assigned to the master version.
                image = librariesManager.CreateImage(masterImageId);

                //Set the parent album.
                Album album = librariesManager.GetAlbums().Where(i => i.Title == albumTitle).SingleOrDefault();
                image.Parent = album;

                //Set the properties of the album post.
                image.Title = imageTitle;
                image.DateCreated = DateTime.UtcNow;
                image.PublicationDate = DateTime.UtcNow;
                image.LastModified = DateTime.UtcNow;
                image.UrlName = Regex.Replace(imageTitle.ToLower(), @"[^\w\-\!\$\'\(\)\=\@\d_]+", "-");

                //Upload the image file.
                librariesManager.Upload(image, imageStream, imageExtension);

                //Save the changes.
                librariesManager.SaveChanges();

                //Publish the Albums item. The live version acquires new ID.
                var bag = new Dictionary<string, string>();
                bag.Add("ContentType", typeof(Image).FullName);
                WorkflowManager.MessageWorkflow(masterImageId, typeof(Image), null, "Publish", false, bag);
            }
            return (librariesManager.GetImage(masterImageId)).Url;
        }

        public virtual WrapperObject ConvertToWraperObject(XmlDocument item)
        {
            WrapperObject obj = new WrapperObject(null);
            obj.MappingSettings = this.PipeSettings.Mappings;
            obj.Language = this.PipeSettings.LanguageIds.FirstOrDefault();

            obj.AddProperty(PublishingConstants.FieldTitle, item.Title);
            obj.AddProperty(PublishingConstants.FieldContent, item.Content);
            obj.AddProperty(PublishingConstants.FieldItemHash, GenerateItemHash(item));
            obj.AddProperty(PublishingConstants.FieldLink, item.Image);
            obj.AddProperty(PublishingConstants.FieldPublicationDate, item.Date);
            obj.AddProperty(PublishingConstants.FieldIdentifier, item.Id);
            obj.AddProperty(PublishingConstants.FieldCategories, item.Categories); //list of categories (Category.Name) separated by comma
            //obj.AddProperty(PublishingConstants.FieldCategories, "News");
            return obj;
        }

        //Logic for differentiating between archive file and api url. Checks for XML extension.
        //If no xml extension, "/news" is appended to UrlName.

        protected virtual IList<WrapperObject> LoadWrapperObjectItems()
        {
            string feedinput = this.RssPipeSettings.UrlName;
            string ext = Path.GetExtension(feedinput).Replace(".", "");
            var url = feedinput + "/news";

            string braftonPublic = "";

            string braftonPrivate = "";

            if (ext == "xml")
            { url = feedinput; }
            var wrapperObjects = new List<WrapperObject>();

            IEnumerable<XmlDocument> result = this.DeserializeXml();
            foreach (var item in result)
            {
                var wrapperObject = this.ConvertToWraperObject(item);
                wrapperObjects.Add(wrapperObject);
            }

            return wrapperObjects;
        }

        //Default mapping for fields. The image url is stored in the link field. 

        internal protected static List<Mapping> GetDefaultMapping()
        {
            var mappingsList = new List<Mapping>();

            mappingsList.Add(PublishingSystemFactory.CreateMapping(PublishingConstants.FieldTitle, ConcatenationTranslator.TranslatorName, true, PublishingConstants.FieldTitle));
            mappingsList.Add(PublishingSystemFactory.CreateMapping(PublishingConstants.FieldContent, ConcatenationTranslator.TranslatorName, true, PublishingConstants.FieldDescription));
            mappingsList.Add(PublishingSystemFactory.CreateMapping(PublishingConstants.FieldItemHash, TransparentTranslator.TranslatorName, false, PublishingConstants.FieldItemHash));
            mappingsList.Add(PublishingSystemFactory.CreateMapping(PublishingConstants.FieldPipeId, TransparentTranslator.TranslatorName, false, PublishingConstants.FieldPipeId));
            mappingsList.Add(PublishingSystemFactory.CreateMapping(PublishingConstants.FieldLink, TransparentTranslator.TranslatorName, false, PublishingConstants.FieldLink));
            mappingsList.Add(PublishingSystemFactory.CreateMapping(PublishingConstants.FieldPublicationDate, TransparentTranslator.TranslatorName, false, PublishingConstants.FieldPublicationDate));
            mappingsList.Add(PublishingSystemFactory.CreateMapping(PublishingConstants.FieldCategories, TransparentTranslator.TranslatorName, false, PublishingConstants.FieldCategories));

            return mappingsList;
        }

        internal protected static IList<IDefinitionField> CreateDefaultPipeDefinitions()
        {
            return new IDefinitionField[]
                {
                    new SimpleDefinitionField(PublishingConstants.FieldTitle, Res.Get<PublishingMessages>().ContentTitle),
                    new SimpleDefinitionField(PublishingConstants.FieldContent, Res.Get<PublishingMessages>().ContentContent),
                    new SimpleDefinitionField(PublishingConstants.FieldItemHash, Res.Get<PublishingMessages>().ItemHash),
                    new SimpleDefinitionField(PublishingConstants.FieldPipeId, Res.Get<PublishingMessages>().PipeId),
                    new SimpleDefinitionField(PublishingConstants.FieldPublicationDate, Res.Get<PublishingMessages>().Date),
                    new SimpleDefinitionField(PublishingConstants.FieldLink, Res.Get<PublishingMessages>().ContentLink),
                    new SimpleDefinitionField(PublishingConstants.FieldOriginalContentId, Res.Get<PublishingMessages>().OriginalItemId),
                    new SimpleDefinitionField(PublishingConstants.FieldCategories, Res.Get<PublishingMessages>().Categories),
                };
        }

        protected virtual string GenerateItemHash(XmlDocument item)
        {
            var builder = new StringBuilder(1000);

            builder.Append(item.Id);
            builder.Append("|");
            builder.Append(item.Title);

            SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();

            byte[] originalBytes = Encoding.UTF8.GetBytes(builder.ToString());
            byte[] encodedBytes = hasher.ComputeHash(originalBytes);
            return Convert.ToBase64String(encodedBytes);
        }

        public virtual IEnumerable<WrapperObject> GetConvertedItemsForMapping(params object[] items)
        {
            throw new NotImplementedException();
        }

        public void Initialize(PipeSettings pipeSettings)
        {
            this.PipeSettings = pipeSettings;
            this.publishingPoint = PublishingSystemFactory.GetPublishingPoint(this.PipeSettings.PublishingPoint);

        }

        public virtual bool CanProcessItem(object item)
        {
            return false;
        }

        public virtual PipeSettings GetDefaultSettings()
        {
            return PublishingSystemFactory.CreatePipeSettings(this.Name, PublishingManager.GetManager(this.PublishingProviderName));
        }

        public virtual string GetPipeSettingsShortDescription(PipeSettings initSettings)
        {
            return "Brafton News Pipe";
        }

        public virtual void CleanUp(string transactionName)
        {
            throw new NotImplementedException();
        }

        public virtual void PushData(IList<PublishingSystemEventInfo> items)
        {
            var wrapperObjects = items.Select(i =>
            {
                var item = i.Item;
                return (item is WrapperObject) ? (WrapperObject)item : new WrapperObject(item) { MappingSettings = this.PipeSettings.Mappings, Language = i.Language };
            }).ToList();

            this.publishingPoint.RemoveItems(wrapperObjects);
            this.publishingPoint.AddItems(wrapperObjects);
        }

        public virtual IQueryable<WrapperObject> GetData()
        {
            var wrapperObjects = this.LoadWrapperObjectItems();
            return wrapperObjects.AsQueryable<WrapperObject>();
        }

        public virtual void ToPublishingPoint()
        {
            var items = new List<PublishingSystemEventInfo>();
            var wrapperObjects = this.LoadWrapperObjectItems();
            foreach (var item in wrapperObjects)
            {
                items.Add(new PublishingSystemEventInfo() { Item = item, ItemAction = RelatedActionsConstants.PublishingPointImported, Language = item.Language });
            }

            this.PushData(items);
            publishNewsItems();
        }
    
        //If option to "Automatically publish imported items" is checked in Alternative publishing, 
        //the article publish date will be set to the moment importer runs instead of the publish date provided in the XML feed. 
        //The code below will publish items with the provided publish date the moment items are imported. 
        //The way it works: method queries for all news items that are in "Master" status (0) and publishes
        //those items by creating a new version with "Live" status (2). The "Master" version will remain, 
        //so there will always be two copies of a news item in the database, Master and Live. Only live is visible. -Ly

        public virtual void publishNewsItems()
           {
                var allNews = GetAllNewsItems();
                foreach (var item in allNews)
                    {
                        var id = item.Id;
                        var date = item.PublicationDate;
                        ModifyNewsPublicationDate(id, date);
                    }

            }
      
            private List<NewsItem> GetAllNewsItems()
            {
            NewsManager newsManager = NewsManager.GetManager();

                return newsManager.GetNewsItems().Where(newsItem => newsItem.Status == ContentLifecycleStatus.Master).ToList();
            }

            private void ModifyNewsPublicationDate(Guid masterNewsId, DateTime pubDate)
            {

            NewsManager newsManager = NewsManager.GetManager();
            newsManager.Provider.SuppressSecurityChecks = true;  //Allows program to access publishing system without being logged in. 

            //Check to make sure "Master" is not null. 

            NewsItem master = newsManager.GetNewsItems().Where(newsItem => newsItem.Id == masterNewsId).FirstOrDefault();

             if (master != null)
              {

                 //Publish the item
                  newsManager.Lifecycle.PublishWithSpecificDate(master, pubDate);
                  master.ApprovalWorkflowState = "Published";
                  newsManager.SaveChanges();

               }

         }

        //video code starts here
        //added by ben

            private bool ValidateVideoPublicKey(string publicKey)
            {
                Regex reg = new Regex("[a-f0-9]{8}", RegexOptions.IgnoreCase);
                return reg.IsMatch(publicKey);
            }

            private bool ValidateGuid(string guid)
            {
                Regex reg = new Regex("[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}", RegexOptions.IgnoreCase);
                return reg.IsMatch(guid);
            }

            private PhotoInstance? GetPhotoInstance(AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticle article, AdferoVideoDotNet.AdferoArticles.ArticlePhotos.AdferoArticlePhotosClient photos, AdferoVideoDotNet.AdferoPhotos.AdferoPhotoClient photoClient, string scaleAxis, int scale)
            {
                PhotoInstance? inst = null;

                AdferoVideoDotNet.AdferoArticles.ArticlePhotos.AdferoArticlePhotoList photoList = photos.ListForArticle(article.Id, 0, 100);
                if (photoList.TotalCount > 0)
                {
                    AdferoVideoDotNet.AdferoArticles.ArticlePhotos.AdferoArticlePhoto apho = photos.Get(photoList.Items[0].Id);
                    int photoId = apho.SourcePhotoId;
                    AdferoVideoDotNet.AdferoPhotos.Photos.AdferoPhoto pho = photoClient.Photos().GetScaleLocationUrl(photoId, scaleAxis, scale);
                    string photoUrl = pho.LocationUri;
                    string photoCaption = photos.Get(photoList.Items[0].Id).Fields["caption"];

                    enumeratedTypes.enumPhotoOrientation ori = enumeratedTypes.enumPhotoOrientation.Landscape;
                    if (scaleAxis == AdferoVideoDotNet.AdferoPhotos.Photos.AdferoScaleAxis.Y)
                        ori = enumeratedTypes.enumPhotoOrientation.Portrait;

                    string cleanedUrl = photoUrl;
                    if (cleanedUrl.IndexOf('?') >= 0)
                        cleanedUrl = cleanedUrl.Substring(0, cleanedUrl.IndexOf('?'));

                    inst = new PhotoInstance()
                    {
                        AltText = photoCaption,
                        Caption = photoCaption,
                        DestinationFileName = Slugify(article.Fields["title"]) + "-" + scale + Path.GetExtension(cleanedUrl),
                        Height = 0,
                        Id = apho.Id,
                        Orientation = ori,
                        Type = enumeratedTypes.enumPhotoInstanceType.Custom,
                        Url = photoUrl,
                        Width = 0
                    };
                }

                return inst;
            }

        private string Slugify(string input)
        {
            Regex alphaNumeric = new Regex("[^a-zA-Z0-9]+");
            return alphaNumeric.Replace(input.ToLower().Trim(), "-");
        }

            struct PhotoInstance
            {
                public int Width, Height, Id;
                public string Url, Caption, AltText;
                public enumeratedTypes.enumPhotoInstanceType Type;
                public enumeratedTypes.enumPhotoOrientation Orientation;
                public string DestinationFileName;
            }


    }
}
