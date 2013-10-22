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
                    //    (item as Telerik.Sitefinity.GenericContent.Model.Content).Organizer.AddTaxa("Category", Category.Id);
                    //}
                    //catch (Exception e){ 
                    //}
                    var tagName = propertyValue.GetValue(wrapperObj).ToString();
                    var tagList = taxonomyManager.GetTaxa<FlatTaxon>().Where(t => t.Name == tagName);
                    if (tagList.Any())
                    {
                        (item as Telerik.Sitefinity.GenericContent.Model.Content).Organizer.AddTaxa("Tags", tagList.FirstOrDefault().Id);
                    }
                    else {
                        var newTag = taxonomyManager.CreateTaxon<FlatTaxon>();
                        newTag.Name = tagName;
                        newTag.Title = tagName;
                        newTag.Description = "";
                        taxonomyManager.GetTaxonomies<FlatTaxonomy>().Where(t => t.Name == "Tags").First().Taxa.Add(newTag);
                        taxonomyManager.SaveChanges();
                        (item as Telerik.Sitefinity.GenericContent.Model.Content).Organizer.AddTaxa("Tags", newTag.Id);
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