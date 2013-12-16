using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using Telerik.Sitefinity.Descriptors;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Utilities;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Publishing.Model;
using Telerik.Sitefinity.Publishing.Pipes;

namespace SitefinityWebApp.Publishing
{
    public class CustomContentOutboundPipe : ContentOutboundPipe
    {
        protected override void SetPropertiesThroughPropertyDescriptor(IContent item, WrapperObject wrapperObj)
        {
            var properties = TypeDescriptor.GetProperties(wrapperObj);
            TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
            foreach (PropertyDescriptor propertyValue in properties)
            {
                if (propertyValue.Name.Equals("Categories")) {
                    //var Category = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(t => t.Taxonomy.Name == "Categories").FirstOrDefault();
                    //try
                    //{
                    //   (item as Telerik.Sitefinity.GenericContent.Model.Content).Organizer.AddTaxa("Category", Category.Id);
                    //}
                    //catch (Exception e){ 
                    //}
                    var tagName = propertyValue.GetValue(wrapperObj).ToString();
                    var newtagname = tagName.Replace(" ", "").Replace("&", "");
                    var tagList = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(t => t.Name == newtagname);
                    if (tagList.Any())
                    {
                      (item as Telerik.Sitefinity.GenericContent.Model.Content).Organizer.AddTaxa("Category", tagList.FirstOrDefault().Id);
                    }
                    else {
                        var catTaxonomy = taxonomyManager.GetTaxonomies<HierarchicalTaxonomy>().Where(t => t.Name == "Categories").SingleOrDefault();
                       var parentTaxonomy = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(t => t.Name == "News").Single(); 
                       var newCat = taxonomyManager.CreateTaxon<HierarchicalTaxon>();
                        newCat.Name = newtagname;
                        newCat.Title = tagName;
                        newCat.Description = "";
                        newCat.UrlName = newtagname;
                        newCat.Taxonomy = catTaxonomy;
                        parentTaxonomy.Subtaxa.Add(newCat);
                        //taxonomyManager.GetTaxonomies<HierarchicalTaxonomy>().Where(t => t.Name == "news").First().Taxa.Add(newCat);
                        taxonomyManager.SaveChanges();
                        tagList = taxonomyManager.GetTaxa<HierarchicalTaxon>().Where(t => t.Name == newtagname);
                        (item as Telerik.Sitefinity.GenericContent.Model.Content).Organizer.AddTaxa("Category", tagList.FirstOrDefault().Id);
                    }
                    try
                    {
                        
                    }
                    catch (Exception e)
                    {
                    }
                }
                else
                {
                    var propertyDescriptor = this.ContentItemTypeDescriptros.Find(propertyValue.Name, false);

                    if (propertyDescriptor == null) continue;

                    if (propertyDescriptor is LstringPropertyDescriptor)
                    {
                        // HACK - the property value can be Lstring, TextSyndicationContent or string in this case.
                        var value = "";
                        if (propertyValue.GetValue(wrapperObj) is Lstring)
                        {
                            value = ((Lstring)propertyValue.GetValue(wrapperObj)).Value;
                        }
                        else
                        {
                            if (propertyValue.GetValue(wrapperObj) is TextSyndicationContent)
                            {
                                value = ((TextSyndicationContent)propertyValue.GetValue(wrapperObj)).Text;
                            }
                            else
                            {
                                value = (string)propertyValue.GetValue(wrapperObj);
                            }
                        }
                        ((LstringPropertyDescriptor)propertyDescriptor).SetString(item, value, this.GetWrapperObjectCulture(wrapperObj));
                        continue;
                    }

                    if (!propertyDescriptor.IsReadOnly)
                    {
                        if (!propertyDescriptor.PropertyType.IsList() && !propertyDescriptor.PropertyType.IsCollection() &&
                            !propertyDescriptor.PropertyType.IsDictionary())
                        {
                            (item as IDynamicFieldsContainer).SetValue(propertyValue.Name, propertyValue.GetValue(wrapperObj));
                        }
                    }
                }
            }

        }
    }
}