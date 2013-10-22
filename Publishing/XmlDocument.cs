using System;
using System.Linq;

namespace SitefinityWebApp.Publishing
{
    public class XmlDocument
    {
        public XmlDocument()
        {
            this.Id = Guid.Empty;
            this.Title = String.Empty;
            this.Content = String.Empty;
            this.Date = String.Empty;
            this.Image = String.Empty;
        }

        public XmlDocument(Guid id, string title, string content, string pubdate, string imageurl, string categories)
            : this()
        {
            this.Id = id;
            this.Title = title;
            this.Content = content;
            this.Image = imageurl;
            this.Date = pubdate;
            this.Categories = categories;
        }

        public Guid Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public string Categories
        {
            get;
            set;
        }

        public string Image
        {
            get;
            set;
        }

        public string Date
        {
            get;
            set;
        }
    }
}