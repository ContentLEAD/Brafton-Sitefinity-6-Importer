using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Publishing.Web.Services.Data;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.ControlDesign;
using Telerik.Sitefinity.Web.UI.Fields;

namespace SitefinityWebApp.Publishing
{
    public class XmlPipeImportDesignerView : ControlDesignerBase
    {
        private string providerName;
        public XmlPipeImportDesignerView()
        {
        }

        public static readonly string layoutTemplatePath = "~/Publishing/XmlPipeImportDesignerView.ascx";
        public override string LayoutTemplatePath
        {
            get
            {
                return layoutTemplatePath;
            }
            set
            {
                base.LayoutTemplatePath = value;
            }
        }

        protected override string LayoutTemplateName
        {
            get
            {
                return null;
            }
        }

        protected override void InitializeControls(GenericContainer container)
        {
            this.providerName = PublishingManager.GetProviderNameFromQueryString();
            this.UrlName.Description = String.Format(Res.Get<PublishingMessages>().FeedsBaseUrlPattern, PublishingManager.GetFeedsBaseURl());
            this.FillDaysDropDown();
        }

        protected virtual void FillDaysDropDown()
        {
            var ddlDays = Container.GetControl<DropDownList>("ddlDays", true);
            for (int i = 0; i < 7; i++)
            {
                DayOfWeek translatedDay = (DayOfWeek)i;
                ddlDays.Items.Add(new ListItem() { Text = translatedDay.ToString(), Value = i.ToString() });
            }
        }


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            this.MaxItemsCount.ValidatorDefinition.MinValue = minMaxItemsCount;
            //this.ContentSize.ValidatorDefinition.MinValue = minContentSize;
        }

        public TextField MaxItemsCount
        {
            get
            {
                return Container.GetControl<TextField>("maxItemsCount", true);
            }
        }

        //public TextField ContentSize
        //{
        //    get
        //    {
        //        return Container.GetControl<TextField>("contentSize", true);
        //    }
        //}

        public ITextControl UINameLabel
        {
            get
            {
                return Container.GetControl<ITextControl>("uiNameLabel", true);
            }
        }

        public FieldControl UrlName
        {
            get
            {
                return this.Container.GetControl<FieldControl>("urlName", true);
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        /// <summary>
        /// Gets the language selector.
        /// </summary>
        /// <value>The language selector.</value>
        protected virtual LanguageChoiceField LanguageSelector
        {
            get
            {
                return this.Container.GetControl<LanguageChoiceField>("languageChoiceField", true);
            }
        }


        protected virtual Control OpenMappingSettingsDialog
        {
            get { return this.Container.GetControl<Control>("openMappingSettings", true); }
        }

        public override IEnumerable<ScriptReference> GetScriptReferences()
        {
            var res = PageManager.GetScriptReferences(ScriptRef.JQuery);
            var assemblyName = this.GetType().Assembly.GetName().ToString();

            var telerikAssemblyName = typeof(Telerik.Sitefinity.Web.UI.Fields.TextField).Assembly.GetName().FullName;

            res.Add(new ScriptReference("Telerik.Sitefinity.Web.UI.ControlDesign.Scripts.IDesignerViewControl.js", telerikAssemblyName));
            res.Add(new ScriptReference("SitefinityWebApp.Publishing.XmlPipeImportDesignerView.js", assemblyName));

            return res;
        }

        public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor desc = new ScriptControlDescriptor(this.GetType().FullName, this.ClientID);
            desc.AddElementProperty("openMappingSettingsButton", this.OpenMappingSettingsDialog.ClientID);

            Dictionary<string, string> fieldControlsMap = new Dictionary<string, string>();

            foreach (FieldControl fieldControl in this.Container.GetControls<FieldControl>().Values)
            {
                if (!string.IsNullOrEmpty(fieldControl.DataFieldName))
                    fieldControlsMap.Add(fieldControl.DataFieldName, fieldControl.ClientID);
            }
            desc.AddProperty("dataFieldNameControlIdMap", fieldControlsMap);
            desc.AddElementProperty("uiNameLabel", ((Control)this.UINameLabel).ClientID);
            desc.AddProperty("_shortDescriptionBase", Res.Get<PublishingMessages>().PipeSettingsShortDescriptionBase);
            desc.AddProperty("_urlNameNotSet", Res.Get<PublishingMessages>().PipeSettingsUrlNameNotSet);
            desc.AddProperty("_feedsBaseUrl", PublishingManager.GetFeedsBaseURl());
            desc.AddElementProperty("selectLanguage", this.LanguageSelector.ClientID);

            var settings = new Dictionary<string, object>();
            var defaults = PublishingSystemFactory.GetPipe(XmlInboundPipe.PipeName).GetDefaultSettings();
            settings.Add("settings", defaults);
            settings.Add("pipe", new WcfPipeSettings(XmlInboundPipe.PipeName, this.providerName));

            desc.AddProperty("_settingsData", settings);

            desc.AddElementProperty("scheduleTypeElement", Container.GetControl<DropDownList>("ddlScheduleType", true).ClientID);
            desc.AddElementProperty("scheduleDaysElement", Container.GetControl<DropDownList>("ddlDays", true).ClientID);
            desc.AddComponentProperty("scheduleTime", Container.GetControl<DateField>("dtpScheduleTime", true).ClientID);


            return new[] { desc };
        }

        protected const int minMaxItemsCount = 1;
        protected const int minContentSize = 1;
    }
    }