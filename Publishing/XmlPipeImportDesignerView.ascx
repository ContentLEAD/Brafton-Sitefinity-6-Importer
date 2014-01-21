<%@ Control Language="C#"  %>
<%@ Register Assembly="Telerik.Sitefinity" Namespace="Telerik.Sitefinity.Web.UI"
    TagPrefix="sitefinity" %>
<%@ Register Assembly="Telerik.Sitefinity" TagPrefix="sfFields" Namespace="Telerik.Sitefinity.Web.UI.Fields" %>
<div class="sfContentViews">
    <sitefinity:SitefinityLabel id="uiNameLabel" runat="server" WrapperTagName="h1" HideIfNoText="false"
        CssClass="sfSepTitle" />
    
    <sfFields:LanguageChoiceField id="languageChoiceField" runat="server" DisplayMode="Write" RenderChoicesAs="DropDown" MutuallyExclusive="true" Title="<%$ Resources:LocalizationResources, Language %>" WrapperTag="div" CssClass="sfFormCtrl" />
    
    <sfFields:TextField ID="urlName" runat="server" Title="<%$Resources:PublishingMessages, RssFeedUrlName %>"
        DisplayMode="Write" Expanded="true" DataFieldName="UrlName" WrapperTag="div"
        ExpandText="Change" CssClass="sfFormCtrl">
        <validatordefinition required="false" messagecssclass="sfError" requiredviolationmessage="<%$Resources:PublishingMessages, ValidationMessageUrlCannotBeEmpty %>" />
    </sfFields:TextField>
    
    <h2>
        <asp:Literal runat="server" ID="itemLimitsLabel" Text="<%$Resources:PublishingMessages, LiteralLimitTheItems %>" /></h2>
    <ul class="sfRadioList">
        <li>
            <asp:RadioButton runat="server" ID="listRadio_includeNewest" Checked="false" GroupName="FeedItemsLimits"
                Text="<%$Resources:PublishingMessages, RadioIncludeTheNewest %> " />
            <sfFields:TextField ID="maxItemsCount" runat="server" DataFieldName="MaxItems" DisplayMode="Write"
                CssClass="sfInlineWrapper sfSetNumberPropery">
                <validatordefinition expectedformat="Integer" required="true" messagecssclass="sfError"
                    integerviolationmessage="<%$Resources:PublishingMessages, ValidationWholeNumber %>"
                    requiredviolationmessage="<%$Resources:PublishingMessages, ValidationEnterWholeNumber %>"
                    minvalueviolationmessage="<%$Resources:PublishingMessages, ValidationNumberGreaterThanZero %>" />
            </sfFields:TextField>
            <asp:Literal runat="server" ID="maxItemsCountLabel" Text="<%$Resources:PublishingMessages, ItemsCount %>" />
        </li>
        <li>
            <asp:RadioButton runat="server" ID="listRadio_includeAll" Checked="false" GroupName="FeedItemsLimits"
                Text="<%$Resources:PublishingMessages, RadioIncludeAllPublishedItems %>" />
        </li>
    </ul>
    <div class="sfFormCtrl">
        <asp:Label runat="server" ID="Label1" Text="<%$Resources:PublishingMessages, Mapping %>" CssClass="sfTxtLbl" />
        <a href="javascript:void(0);" id="openMappingSettings" runat="server" class="sfLinkBtn sfChange">
            <asp:Label runat="server" ID="lMappingSettings" Text="<%$Resources:PublishingMessages, MappingSettings %>" CssClass="sfLinkBtnIn" />
        </a>
        <%--<a href="javascript:void(0);" id="A1" runat="server" class="sfLinkBtn sfChange">
            <asp:Label runat="server" ID="Label2" Text="<%$Resources:PublishingMessages, MappingSettings %>"
                CssClass="sfLinkBtnIn" />
        </a>--%>
    </div>
    <div class="sfFormCtrl">
        <asp:Label runat="server" Text="Schedule publication updates interval" ID="schedulePublicationUpdateIntervals" CssClass="sfTxtLbl" />
        <div class="sfCompositeDdlCtrl">
            <asp:DropDownList runat="server" ID="ddlScheduleType">
                <asp:ListItem Value="0" Text="None"></asp:ListItem>
                <asp:ListItem Value="1" Text="5 minutes"></asp:ListItem>
                <asp:ListItem Value="2" Text="30 minutes"></asp:ListItem>
                <asp:ListItem Value="3" Text="1 hour"></asp:ListItem>
                <asp:ListItem Value="4" Text="Daily at..."></asp:ListItem>
                <asp:ListItem Value="5" Text="Weekly on..."></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div id="divDays" class="sfInlineBlock">
            <asp:DropDownList runat="server" ID="ddlDays" />
        </div>
        <div id="divTime" class="sfInlineBlock">
            <sfFields:DateField ID="dtpScheduleTime" runat="server" DisplayMode="Write" CssClass="sfTimePickerBasedOnDateTimePicker" />
        </div>
    </div>
</div>
