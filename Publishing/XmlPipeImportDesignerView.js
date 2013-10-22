Type.registerNamespace("SitefinityWebApp.Publishing");

SitefinityWebApp.Publishing.XmlPipeImportDesignerView = function (element) {
    SitefinityWebApp.Publishing.XmlPipeImportDesignerView.initializeBase(this, [element]);
    this._openMappingSettingsButton = null;
    this._settingsData = {};

    this._refreshing = false;
    this._dataFieldNameControlIdMap = null;
    this._uiNameLabel = null;
    this._radioChoices = null;
    this._selectLanguage = null;
    this._shortDescriptionBase = null;
    this._feedsBaseUrl = null;
    this._urlNameNotSet = null;
    this._radioFormatSettings = ["listRadio_SmartFeed", "listRadio_RSSOnly", "listRadio_AtomOnly", "listRadio_RSSAtom"];
    this._radioOutputSettings = ["listRadio_TitleContent", "listRadio_TitleHeadContent", "listRadio_TitleOnly"];
    this._scheduleTime = null;
    this._scheduleTypeElement = null;
    this._scheduleDaysElement = null;

    this._defaultSettings = {};

}

SitefinityWebApp.Publishing.XmlPipeImportDesignerView.prototype = {
    initialize: function () {
        SitefinityWebApp.Publishing.XmlPipeImportDesignerView.callBaseMethod(this, 'initialize');
        this._radioClickDelegate = Function.createDelegate(this, this._setRssSettings);
        this.get_radioChoices().click(this._radioClickDelegate);
        this.get_scheduleTime().set_datePickerFormat("", "hh:mm:ss");

        if (this._scheduleTypeElement) {
            if (this._selectScheduleTypeDelegate == null)
                this._selectScheduleTypeDelegate = Function.createDelegate(this, this._selectScheduleTypeChanged);
            $addHandler(this._scheduleTypeElement, "change", this._selectScheduleTypeDelegate);
        }


    },
    dispose: function () {
        
        if (this._scheduleTypeElement) {
            $removeHandler(this._scheduleTypeElement, "change", this._selectScheduleTypeDelegate);
            if (this._selectScheduleTypeDelegate != null)
                delete this._selectScheduleTypeDelegate;
        }
        SitefinityWebApp.Publishing.XmlPipeImportDesignerView.callBaseMethod(this, 'dispose');
    },
    refreshUI: function () {
        this._refreshing = true;
        var radios = this.get_radioChoices();
        var radioValue = '';
        this._checkRadioButton(this._radioFormatSettings[this._settingsData.settings.FormatSettings]);
        this._checkRadioButton(this._radioOutputSettings[this._settingsData.settings.OutputSettings]);

        for (var dataFieldName in this._dataFieldNameControlIdMap) {
            var cnt = this._getFieldControl(dataFieldName);
            var fValue = this._settingsData.settings[dataFieldName];
            if (fValue != null && fValue != undefined)
                cnt.set_value(fValue);
        }
        if (this._settingsData.settings.MaxItems == 0) {
            this._checkRadioButton("listRadio_includeAll");
            this.disableElement(this._getFieldControl("MaxItems").get_element());
        }
        else {
            this._checkRadioButton("listRadio_includeNewest");
            this.enableElement(this._getFieldControl("MaxItems").get_element());
        }
        //if (this._settingsData.settings.OutputSettings == 1)
        //    this.enableElement(this._getFieldControl("ContentSize").get_element());
        //else
        //    this.disableElement(this._getFieldControl("ContentSize").get_element());

        if (this._settingsData.settings.UIName != 'undefined')
            this._uiNameLabel.innerHTML = this._settingsData.settings.UIName;
        if (this._settingsData.pipe.UIName != 'undefined')
            this._uiNameLabel.innerHTML = this._settingsData.pipe.UIName;
        this._refreshScheduleData();


        this._refreshing = false;

    },
    applyChanges: function () {
        debugger;
        for (var dataFieldName in this._dataFieldNameControlIdMap) {
            var cnt = this._getFieldControl(dataFieldName);
            if (cnt.get_value() != '' && this.isEnabledElement(cnt.get_element()))
                this._settingsData.settings[dataFieldName] = cnt.get_value();
        }


        //this._settingsData.pipe.UIDescription = this.get_uiDescription();
        this._settingsData.settings.ScheduleType = this.get_scheduleType().val();
        this._settingsData.settings.ScheduleDay = this.get_scheduleDays().val();
        this._settingsData.pipe.ScheduleTime = new Date(this.get_scheduleTime().get_value());

    },
    _refreshScheduleData: function () {
        var settings = this._settingsData.settings;
        if (settings) {
            var scheduleType = settings.ScheduleType;
            if (scheduleType && scheduleType > 0)
                this.get_scheduleType().val(settings.ScheduleType);

            this._showHideScheduledDaysAndTime();
            this.get_scheduleDays().val(settings.ScheduleDay);
        }
        if (this._settingsData.pipe && this._settingsData.pipe.ScheduleTime) {
            this.get_scheduleTime().set_value(new Date(this._settingsData.pipe.ScheduleTime));
        }
    },
    _selectScheduleTypeChanged: function () {
        this._showHideScheduledDaysAndTime();
    },
    _showHideScheduledDaysAndTime: function () {
        // Todo : remove schedule type daily and weekly hardcoded values
        //daily
        this.get_scheduleTime().get_datePicker().hide();
        this.get_scheduleDays().hide();
        if (this.get_scheduleTypeElement().value == "4") {
            this.get_scheduleTime().get_datePicker().show();
        }
        //weekly
        if (this.get_scheduleTypeElement().value == "5") {
            this.get_scheduleDays().show();
            this.get_scheduleTime().get_datePicker().show();
        }
    },

    validate: function () {
        var isValid = true;
        for (var dataFieldName in this._dataFieldNameControlIdMap) {
            var cnt = this._getFieldControl(dataFieldName);
            if (this.isEnabledElement(cnt.get_element())) {
                isValid = isValid && cnt.validate();
            }
        }

        return isValid;
    },
    get_openMappingSettingsButton: function () {
        return this._openMappingSettingsButton;
    },
    set_openMappingSettingsButton: function (val) {
        this._openMappingSettingsButton = val;
    },
    set_controlData: function (value) {
        this._settingsData = value;
        this._setLanguage();
        this.refreshUI();
    },
    get_controlData: function (value) {
        this._getSelectedLanguage();
        return this._settingsData;
    },
    get_uiNameLabel: function () {
        return this._uiNameLabel;
    },
    set_uiNameLabel: function (value) {
        this._uiNameLabel = value;
    },
    _getSelectedLanguage: function () {
        if (this._selectLanguage) {
            var languageSelector = $find(this._selectLanguage.id);
            if (languageSelector) {
                this._clearArray(this._settingsData.pipe.LanguageIds);
                this._settingsData.pipe.LanguageIds.push(languageSelector.get_value());
            }
        }
    },
    _setLanguage: function () {
        if (this._selectLanguage) {
            if (this._settingsData.pipe.LanguageIds && this._settingsData.pipe.LanguageIds.length > 0) {
                var languageSelector = $find(this._selectLanguage.id);
                if (languageSelector) {
                    languageSelector._selectItemByValue(this._settingsData.pipe.LanguageIds);
                }
            }
        }
    },
    _clearArray: function (arr) {
        arr.splice(0, arr.length);
    },
    _setRssSettings: function (sender, args) {
        debugger;
        if (!this._refreshing) {
            var radioID = sender.target.value;
            var index = jQuery.inArray(radioID, this._radioFormatSettings);
            if (index > -1) {
                this._settingsData.settings.FormatSettings = index;
                return;
            }
            index = jQuery.inArray(radioID, this._radioOutputSettings);
            if (index > -1) {
                this._settingsData.settings.OutputSettings = index;
                var func;
                if (index == 1)
                    func = this.enableElement;
                else
                    func = this.disableElement;
                func(this._getFieldControl("ContentSize").get_element());
                return;
            }
            else {
                if (radioID == "listRadio_includeNewest")
                    this.enableElement(this._getFieldControl("MaxItems").get_element());
                if (radioID == "listRadio_includeAll") {
                    this.disableElement(this._getFieldControl("MaxItems").get_element());
                    //this._getFieldControl("MaxItems").set_value(0);
                    this._settingsData.settings.MaxItems = 0;
                }
            }

        }
    },
    _checkRadioButton: function (value) {
        var radios = this.get_radioChoices();
        radios.filter(function (i) { return this.value == value; }).attr('checked', true);
    },
    // gets the reference to the field control by the field name that it edits
    _getFieldControl: function (dataFieldName) {
        return $find(this._dataFieldNameControlIdMap[dataFieldName]);
    },
    // gets all the radio buttons in the container of this control
    get_radioChoices: function () {
        if (!this._radioChoices) {
            this._radioChoices = jQuery(this.get_element()).find('input[type|=radio]');
        }
        return this._radioChoices;
    },
    // gets the object which represents the map of field properties and respective controls
    // that are used to edit them
    set_dataFieldNameControlIdMap: function (value) {
        this._dataFieldNameControlIdMap = value;
    },

    // sets the object which represents the map of field properties and respective controls
    // that are used to edit them
    get_dataFieldNameControlIdMap: function () {
        return this._dataFieldNameControlIdMap;
    },
    enableElement: function (domElement) {
        $(domElement).find('input').each(function () {
            $(this).removeAttr('disabled');
        });
    },
    isEnabledElement: function (domElement) {
        var enabled = false;
        $(domElement).find('input').each(function () {
            enabled = !($(this).attr('disabled'));
        });
        return enabled;
    },
    disableElement: function (domElement) {
        $(domElement).find('input').each(function () {
            $(this).attr('disabled', 'disabled');
        });
    },
    get_selectLanguage: function () {
        return this._selectLanguage;
    },
    set_selectLanguage: function (value) {
        this._selectLanguage = value;
    },
    // generates the UIDescription of the pipe
    get_uiDescription: function () {
        debugger;
        var urlName = this._settingsData.settings.UrlName;
        if (urlName) {
            if (this._feedsBaseUrl.charAt(this._feedsBaseUrl.length - 1) != "/")
                this._feedsBaseUrl = this._feedsBaseUrl + "/";
            var feedUrl = this._feedsBaseUrl + encodeURIComponent(urlName);
            return String.format("{0}<a href=\"{1}\" target=\"_blank\">{1}</a>", this._shortDescriptionBase, feedUrl);
        }
        else {
            return this._urlNameNotSet;
        }
    },
    get_scheduleTime: function () {
        return this._scheduleTime;
    },
    set_scheduleTime: function (val) {
        this._scheduleTime = val;
    },
    get_scheduleType: function () {
        return $(this._scheduleTypeElement);
    },
    get_scheduleTypeElement: function () {
        return this._scheduleTypeElement;
    },
    set_scheduleTypeElement: function (val) {
        this._scheduleTypeElement = val;
    },
    get_scheduleDays: function () {
        return $(this._scheduleDaysElement);
    },
    get_scheduleDaysElement: function () {
        return this._scheduleDaysElement;
    },
    set_scheduleDaysElement: function (val) {
        this._scheduleDaysElement = val;
    }
}

SitefinityWebApp.Publishing.XmlPipeImportDesignerView.registerClass('SitefinityWebApp.Publishing.XmlPipeImportDesignerView',
    Sys.UI.Control, Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesignerBase);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();