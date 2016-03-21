/*
/// <reference path="scripts/CmpWapExtensionTenant.createwizard.js" />
/// <reference path="scripts/CmpWapExtensionTenant.controller.js" />*/
/*globals window,jQuery,Shell, CmpWapExtensionTenantExtension, Exp*/

declare var jQuery;
declare var osInfoList;
declare var Exp;
declare var Shell;
declare var cdm;
declare var AccountsAdminExtension;
declare var offersListSelector;
declare var valContainerSelector;
declare var resources;

(function ($, global, undefined?) {
    "use strict";

    $('head').append('<meta http-equiv="Pragma" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
    $('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Expires" content="-1" />');

    var serverRoleList;
    var sizeInfoList;
    var serviceCategoryList;
    var serverroleDrivemappingList;
    var networkNicList;
    var resourcegroupList;
    var iisroleservicesList;
    var sqlanalysisservicemodesList;
    var sqlcollationList;
    var subscriptionMappingsList;
    var subscriptionRegionOSMapping;
    var subscriptionRegionSizeMapping;
    var sqlVersionList;
    var environmenttypeList;
    var targetRegionsList;
    var domainList;
    var appList;
    var predefDriveVals = {};
    var subscriptionId;
    var resources = [], CmpWapExtensionTenantExtensionActivationInit, subscriptionRegisteredToService, accountAdminLiveEmailId, navigation, selectedrow, lastselecteddrive, serviceName = "CmpWapExtension", defaultdriveslist = ["D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"], extradrivenames = [], defaultsqlcollation = "SQL_Latin1_General_CP1_CI_AS";

    var allSubscriptionIds: string[];
    var getDomainlistUrl = "/CmpWapExtensionTenant/ListDomains", getResourceGroupsUrl = "/CmpWapExtensionTenant/LisResourceGroups", getSizeInfoListUrl = "/CmpWapExtensionTenant/ListVmSizes", getOsInfoListUrl = "/CmpWapExtensionTenant/ListOSs", getTargetRegionsListUrl = "/CmpWapExtensionTenant/ListTargetRegions", getAppListUrl = "/CmpWapExtensionTenant/ListApps", getEnvironmenttypeListUrl = "/CmpWapExtensionTenant/ListEnvironments", getServiceCategoryListUrl = "/CmpWapExtensionTenant/ListCategories", getServerRoleListUrl = "/CmpWapExtensionTenant/ListServerRoles", getNetworkNICListUrl = "/CmpWapExtensionTenant/LisNetworkNICs", getServerRoleDriverMappingListUrl = "/CmpWapExtensionTenant/LisServerRoleDriveMappings", getSqlCollationListUrl = "/CmpWapExtensionTenant/ListSqlCollations", getSqlVersionListUrl = "/CmpWapExtensionTenant/ListSqlVersions", getiisroleservicesurl = "/CmpWapExtensionTenant/ListIISRoleServices", getsqlanalysisservicemodesurl = "/CmpWapExtensionTenant/ListSQLAnalysisServiceModes", getSubscriptionMappingsUrl = "/CmpWapExtensionTenant/ListSubscriptionMappings", getVMOSMappingsUrl = "/CmpWapExtensionTenant/ListVMRegionOSMappings";
    var getVMSizeMappingsUrl = "/CmpWapExtensionTenant/ListVMRegionSizeMappings";
    //*************************************************************************
    // Clears view when navigating away from the page
    //*************************************************************************
    function onNavigateAway() {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Global.clear();
        Exp.UI.Commands.update();
    }

    //*************************************************************************
    // Loads the settings view
    //*************************************************************************
    function loadSettingsTab(extension, renderArea, renderData) {
        global.CmpWapExtensionTenantExtension.SettingsTab.loadTab(renderData, renderArea);
    }

    //*************************************************************************
    // Loads the virtual machine details view
    //*************************************************************************
    function loadVMDashboardTab(extension, renderArea, renderData) {
        var initData = {};

        global.CmpWapExtensionTenantExtension.VMDashboardTab.loadTab(extension, renderArea, renderData);
    }

    //*************************************************************************
    // Loads the view to configure a virtual machine
    //*************************************************************************
    function loadVMConfigureTab(extension, renderArea, renderData) {
        var initData = {};

        global.CmpWapExtensionTenantExtension.VMConfigureTab.loadTab(extension, renderArea, renderData);
    }

    //*************************************************************************
    // Loads the list of virtual machines for the navigation bar
    //*************************************************************************
    function loadVMNavigationItemsDataFunction(data, originalPath, extension) {
        //retrieve the cached data to populate the vms in the left horizontal tab after first level navigation
        var items = $.map(global.CmpWapExtensionTenantExtension.Controller.getVmsDataSet().data, function (value) {
            return $.extend(value, {
                name: value.Name,
                displayName: value.Name,
                uniqueId: value.Name,
                navigationPath: {
                    type: value.Type,
                    name: value.Name
                }
            });
        });

        return {
            data: items,
            backNavigation: {
                // id of the tab registered in navigation.
                view: "fileShares"
            }
        };
    }

    //*************************************************************************
    // Loads the file share view
    //*************************************************************************
    function fileSharesTab(extension, renderArea, renderData) {
        global.CmpWapExtensionTenantExtension.Controller.mainDashboardrenderArea = renderArea;
        global.CmpWapExtensionTenantExtension.Controller.mainDashboardrenderData = renderData;
        global.CmpWapExtensionTenantExtension.FileSharesTab.loadTab(renderData, renderArea);
    }

    //*************************************************************************
    // Called when navigating to the extension
    //*************************************************************************
    function onNavigating(context) {
        var destinationItem = context.destination.item;

        // We are navigating to drill downs for a container
        if (destinationItem) {
            if (destinationItem.type === "VMs") {
                //selectedContainerId = destinationItem.name;
            }
        }
    }

    global.CmpWapExtensionTenantExtension = global.CmpWapExtensionTenantExtension || {};

    navigation = {
        tabs: [
            {
                id: "fileShares",
                displayName: "File Shares",
                template: "FileSharesTab",
                activated: fileSharesTab
            }
        ],
        types: [
            {
                name: "VMs",
                dataFunction: loadVMNavigationItemsDataFunction,
                tabs: [
                    {
                        id: "VMDashboardTab",
                        displayName: "Dashboard",
                        template: "VMDashboardTab",
                        activated: function (extension, renderArea, renderData) {
                            loadVMDashboardTab(extension, renderArea, renderData);
                        }
                    }//,
                    //{
                    //    id: "VMConfigureTab",
                    //    displayName: "Configure",
                    //    template: "VMConfigureTab",
                    //    activated: function (extension, renderArea, renderData) {
                    //        loadVMConfigureTab(extension, renderArea, renderData);
                    //    }
                    //}
                ]
            }
        ]
    };

    //*************************************************************************
    // Updates the administrator settings
    // todo: Rename
    //*************************************************************************
    function callback(newEndpointUrl, newUsername, newPassword) {
        var newSettings = $.extend(true, {}, global.CmpWapExtensionAdminExtension.Controller.getCurrentAdminSettings());
        newSettings.EndpointAddress = newEndpointUrl;
        newSettings.Username = newUsername;
        newSettings.Password = newPassword;

        return global.CmpWapExtensionAdminExtension.Controller.updateAdminSettings(newSettings);
    }

    var selectedDriveLetters, selectedDriveSizes;
    var driveletters = ['D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W'];

    //*************************************************************************
    // Updates the drive letters for the virtual machine
    //*************************************************************************
    function driveLetterChanged() {
        //var eventSource = this;
        //var targetId = eventSource.event.target.id;
        //var targetVal = eventSource.event.target.value;
        //targetNumber = eventSource.event.target.valueAsNumber;
        var str = "";
        selectedDriveLetters = [];

        $(".VmDriveLetter option:selected").each(function () {
            if (0 < ($(this).text()).length) {
                if (-1 < str.indexOf($(this).text())) {
                    alert("Duplicate Drive Letter");

                    //this.event.target.selectedIndex = 0;
                    this.selectedIndex = 0;
                }
                str += $(this).text();
                selectedDriveLetters.push($(this).text());
            }
        });
    }

    //*************************************************************************
    // Updates the size for the virtual machine
    //*************************************************************************
    function vmSizeChanged() {
        var selectedVmSize = $("#VmSize").val();

        sizeInfoList.forEach(function (value, index) {
            if (selectedVmSize == sizeInfoList[index].Name) {
                populateSizeDescription(index);
                populateDriveList(sizeInfoList[index].DataDiskCount, sizeInfoList[index].DiskSizeOS);
                return;
            }
        });
    }

    //*************************************************************************
    // Loads the list of operating systems for a source image
    //*************************************************************************
    function vmSourceImageChanged() {
        var selectedImage = $("#VmSourceImage").val();

        osInfoList.forEach(function (value, index) {
            if (selectedImage == osInfoList[index].Name) {
                populateVmSourceImageDescription(index);
                return;
            }
        });
    }

    //*************************************************************************
    // Changes the region list to the single region of the selecetd app
    //*************************************************************************
    function vmSelectedAppChanged() {
        var selectedApp = $("#VmAppNameSelect").val();

        if (appList) {
            for (var index = 0; index < appList.length; index++) {
                if (selectedApp == appList[index].Name) {
                    setVmRegionList(appList[index].Region);
                    return;
                }
            }
        };

        setVmRegionList(null);
    }

    function setVmRegionList(selectedRegion) {
        var listItems = "";

        if (selectedRegion) {
            listItems += "<option value='" + selectedRegion + "'>" + selectedRegion + "</option>";
            $("#VmRegion").html(listItems);
            $("#VmRegion").selectedIndex = 0;
        }
        else if (targetRegionsList) {
            for (var i = 0; i < targetRegionsList.length; i++) {
                listItems += "<option value='" + targetRegionsList[i].Name + "'>" + targetRegionsList[i].Description + "</option>";
            }
            $("#VmRegion").html(listItems);
        }
    }

    //*************************************************************************
    // Updates the drive size list
    //*************************************************************************
    function driveSizeChanged() {
        //var eventSource = this;
        //var targetId = eventSource.event.target.id;
        //var targetVal = eventSource.event.target.value;
        //targetNumber = eventSource.event.target.valueAsNumber;
        selectedDriveSizes = [];

        $(".VmDriveSize").each(function () {
            selectedDriveSizes.push(this.value);
        });
    }

    //*************************************************************************
    // Loads the list of drives for a virtual machine
    //*************************************************************************
    function populateDriveList(diskCount, cDriveSize) {
        var listItems = "<tbody><tr><th style='background-color: #628DB5; color: #FFF; font-weight: bold; width: 20%; text-align: left;'>ID</th>" + "<th style='background-color: #628DB5; color: #FFF; font-weight: bold; width: 20%; text-align: left;'>Size (GB)</th></tr>" + "<tr><td>C</td><td>" + cDriveSize + "</td></tr>";

        for (var index = 0; index < diskCount; index++) {
            listItems += buildDriveLine(index);
        }
        listItems += "</tbody>";
        $("#VmDriveList").html(listItems);
    }

    function buildDriveLine(index) {
        var driveLine = "<tr><td><select class='VmDriveLetter' id='VmDriveLetter" + index + "'><option value=''></option>";

        $(driveletters).each(function () {
            driveLine += "</option><option value='" + this + "'>" + this + "</option>";
        });

        driveLine += "<td style='vertical-align:middle'><input class='VmDriveSize' id='VmDriveSize" + index + "' type='range' min='20' max='1000' step='10' data-val='true' width='2' value='20' /></td></tr>";
        return driveLine;
    }

    //*************************************************************************
    // Builds an XML string representing a disk
    //*************************************************************************
    function buildDiskSpecXml() {
        //<Drives><Drive><Letter>C</Letter><Role>OS</Role><SizeInGB>100</SizeInGB><TypeCode>D</TypeCode><TypeName>Dynamic VHD</TypeName><BlockSize>Default</BlockSize></Drive>
        //<Drive><Letter>E</Letter><Role>Data</Role><SizeInGB>20</SizeInGB><TypeCode>D</TypeCode><TypeName>Dynamic VHD</TypeName><BlockSize>Default</BlockSize></Drive>
        //<Drive><Letter>F</Letter><Role>Data</Role><SizeInGB>30</SizeInGB><TypeCode>D</TypeCode><TypeName>Dynamic VHD</TypeName><BlockSize>Default</BlockSize></Drive></Drives>
        var extradrivesdata = $("#extradrives").fxGrid("option", "data");

        var predefineddrives = $("#predefineddrives").fxGrid("option", "data");

        var diskSpec = "<Drives>";
        if (null != predefineddrives) {
            predefineddrives.forEach(function (value, index) {
                if (1 == value.name.length) {
                    diskSpec += "<Drive><Letter>" + value.name + "</Letter><Role>Data</Role><SizeInGB>" + value.size + "</SizeInGB><TypeCode>D</TypeCode><TypeName>Dynamic VHD</TypeName><BlockSize>Default</BlockSize></Drive>";
                }
            });
        }

        if (null != extradrivesdata)
            extradrivesdata.forEach(function (value, index) {
                if (1 == value.drivenames.selecteddrive.length) {
                    diskSpec += "<Drive><Letter>" + value.drivenames.selecteddrive + "</Letter><Role>Data</Role><SizeInGB>" + value.drivesize + "</SizeInGB><TypeCode>D</TypeCode><TypeName>Dynamic VHD</TypeName><BlockSize>Default</BlockSize></Drive>";
                }
            });

        diskSpec += "</Drives>";

        return diskSpec;
    }

    //*************************************************************************
    // Loads a description of the virtual machine source image
    //*************************************************************************
    function populateVmSourceImageDescription(index) {
        $("#SourceImageDescription").html(osInfoList[index].Description);
    }

    //*************************************************************************
    // Loads the description of a virtual machine size
    //*************************************************************************
    function populateSizeDescription(index) {
        $("#SizeDescription").html(sizeInfoList[index].Description);
        $("#SizeCpuCoreCount").html(sizeInfoList[index].CpuCoreCount);
        $("#SizeRamMB").html(sizeInfoList[index].RamMB);
        $("#SizeDiskSizeOS").html(sizeInfoList[index].DiskSizeOS);
        $("#SizeDiskSizeTemp").html(sizeInfoList[index].DiskSizeTemp);
        $("#SizeDataDiskCount").html(sizeInfoList[index].DataDiskCount);
    }

    //*************************************************************************
    // Initializes the UI
    //*************************************************************************
    function populateUiElements() {
        //$(document).ready(function () {
        try {
            if (domainList) {
                var listItems = "";
                for (var i = 0; i < domainList.length; i++) {
                    listItems += "<option data-Id='" + domainList[i].Id + "' value='" + domainList[i].Name + "'>" + domainList[i].DisplayName + "</option>";
                }
                $("#VmDomain").html(listItems);
            }
            // populateskudropdown(sizeInfoList);
            if (osInfoList) {
                listItems = "";
                for (var i = 0; i < osInfoList.length; i++) {
                    listItems += "<option value='" + osInfoList[i].Name + "'>" + osInfoList[i].Name + "</option>";
                }
                $("#VmSourceImage").html(listItems);
            }
            if (subscriptionRegisteredToService) {
                listItems = "";
                for (var i = 0; i < subscriptionRegisteredToService.length; i++) {
                    listItems += "<option value='" + subscriptionRegisteredToService[i].id + "'>" + subscriptionRegisteredToService[i].SubscriptionName.toString() + "</option>";
                }
                $("#SubscriptionName").html(listItems);
            }
            //if (subscriptionMappingsList) {
            //    listItems = "";
            //    for (var i = 0; i < subscriptionMappingsList.length; i++) {
            //        listItems += "<option value='" + subscriptionMappingsList[i].id + "'>" + subscriptionRegisteredToService[i].SubscriptionName.toString() + "</option>";
            //    }
            //    $("#SubscriptionName").html(listItems);
            //}
            if (targetRegionsList) {
                listItems = "";
                for (var i = 0; i < targetRegionsList.length; i++) {
                    listItems += "<option value='" + targetRegionsList[i].Name + "'>" + targetRegionsList[i].Description + "</option>";
                }
                $("#VmRegion").html(listItems);
            }

            /*listItems = "";
            for (var i = 0; i < serviceCategoryList.length; i++) {
            listItems += "<option value='" + serviceCategoryList[i].Name + "'>" + serviceCategoryList[i].Name + "</option>";
            }
            $("#vm-config-service").html(listItems);*/
            if (environmenttypeList) {
                listItems = "";
                for (var i = 0; i < environmenttypeList.length; i++) {
                    listItems += "<option data-Id='" + environmenttypeList[i].EnvironmentTypeId + "' value='" + environmenttypeList[i].Name + "'>" + environmenttypeList[i].Name + "</option>";
                }
                $("#VmEnvironment").html(listItems);
            }
            /* addsa - Commented out. Modified dynamically below based on domain selection
            listItems = "";
            for (var i = 0; i < networkNicList.length; i++) {
            listItems += "<option data-Id='" + networkNicList[i].NetworkNICId + "' value='" + networkNicList[i].Name + "'>" + networkNicList[i].Name + "</option>";
            }
            $("#VmNic1").html(listItems); */
            fetchAppInfoList(allSubscriptionIds).then(function () {
                if (appList) {
                    listItems = "";
                    for (var i = 0; i < appList.length; i++) {
                        listItems += "<option value='" + appList[i].Name + "'>" + "</option>";
                    }
                    $("#VmAppNameSelectTxt").html(listItems);
                }
            });

            /* $("#VmAppNameSelect").change(function() {
            var vmAppName = $("#VmAppNameSelect option:selected").text();
            $("#VmAppNameSelecttxt").text(vmAppName);
            })
            .change();*/
            /*
            listItems = "";
            for (var i = 0; i < serverRoleList.length; i++) {
            listItems += "<option value='" + serverRoleList[i].ServerRoleId + "'>" + serverRoleList[i].Description + "</option>";
            }
            $("#VmServerRole").html(listItems);*/
            if (sqlVersionList) {
                listItems = "";
                for (var i = 0; i < sqlVersionList.length; i++) {
                    listItems += "<option value='" + sqlVersionList[i].Name + "'>" + sqlVersionList[i].Description + "</option>";
                }
                $("#VmSqlversion").html(listItems);
            }
            if (sqlcollationList) {
                listItems = "";
                for (var i = 0; i < sqlcollationList.length; i++) {
                    listItems += "<option value='" + sqlcollationList[i].Name + "'>" + sqlcollationList[i].Description + "</option>";
                }
                $("#VmSqlcollation").html(listItems);
            }
            if (iisroleservicesList){
                listItems = "";
                for (var i = 0; i < iisroleservicesList.length; i++) {
                    listItems += "<option value='" + iisroleservicesList[i].Name + "'>" + iisroleservicesList[i].Description + "</option>";
                }
                $("#IIsRoleservices").html(listItems);
            }
            if (sqlanalysisservicemodesList) {
                listItems = "";
                for (var i = 0; i < sqlanalysisservicemodesList.length; i++) {
                    listItems += "<option value='" + sqlanalysisservicemodesList[i].Name + "'>" + sqlanalysisservicemodesList[i].Description + "</option>";
                }
                $("#sqlanalysismode").html(listItems);
            }

            //populateSizeDescription(0);
            //populateVmSourceImageDescription(0);
            $("#VmSize").change(function () {
                vmSizeChanged();
            });
            $("#VmSourceImage").change(function () {
                vmSourceImageChanged();
            });
            //** thing ***
            $("#VmAppNameSelect").change(function () {
                vmSelectedAppChanged();
            });

        }
        catch (err) {
            alert(err.message);
        }
        //});
    }

    //*************************************************************************
    // Loads the view to create a new virtual machine
    //*************************************************************************
    function ShowCreateVmWizard(currentUserInfo) {
        var promise, wizardContainerSelector = ".hw-create-fileshare-container";

        var prevvalue = null;
        var valid = true;

        var values = [{ text: "Yes", value: "true" }, { text: "No", value: "false" }];
        cdm.stepWizard({
            extension: "CmpWapExtensionTenantExtension",
            steps: [
                {
                    template: "CreateVM1",
                    // Called when the step is first created
                    onStepCreated: function () {
                        //wizard = this;
                    },
                    // Called before the wizard moves to the next step
                    onNextStep: function () {
                        var regex = /^[a-zA-Z0-9]{4,14}$/;
                        if (!regex.test($("#VmAppNameSelect").val())) {
                            valid = false;
                            $("#lblmessage").css("display", "block");
                            $("#lblmessage").text("Enter valid alphanumeric name above 4 and below 15 characters long.");
                        } else {
                            valid = true;
                        }
                        if (Shell.UI.Validation.validateContainer(".hw-create-fileshare-container")) {
                            if (!valid) {
                                return false;
                            } else {
                                valid = true;
                            }
                        } else {
                            return false;
                        }
                    },
                    // Called each time the step is displayed
                    onStepActivate: function () {
                        populateUiElements();
                        $("#lblmessage").css("display", "none");
                        valid = true;
                    }
                },
                {
                    template: "CreateVM2",
                    // Called when the step is first created
                    onStepCreated: function () {
                        //wizard = this;
                        //var role = $("#VmServerRole");
                        var selectedrole = 3;
                        var drivecount = $.grep(serverroleDrivemappingList, function (val) {
                            return val.ServerRoleId == selectedrole;
                        });
                        var filteredskulist = $.grep(sizeInfoList, function (val) {
                            return val.MaxDataDiskCount >= drivecount.length + 1;
                        });

                        populateskudropdown(filteredskulist);

                        /////////////////////////////////////////
                        // Only populate if domain values are present in the database
                        if (domainList != null && domainList.length > 0) {
                            $("#isDomainjoined").show();
                            $("#VmDomains").show();
                            $("VmNics").show();

                            var nics = [];
                            var listItems = "";
                            networkNicList.forEach(function (nic) {
                                if ($.inArray(nic.Name, nics) == -1) {
                                    nics.push(nic.Name);
                                    listItems += "<option data-Id='" + nic.NetworkNICId + "' value='" + nic.Name + "'>" + nic.Name + "</option>";
                                }
                            });
                            nics = [];
                            $("#VmNic1").html(listItems);
                        } else {
                            $("#isDomainjoined").hide();
                            $("#UserLabel").text("Username");
                            $("#VmDomains").hide();
                            $("#VmNics").hide();
                        }

                        var domainTypes = [{ text: "Yes", value: true }, { text: "No", value: false }];
                        $("#domain-joined-radio").fxRadio({
                            value: domainTypes[1],
                            values: domainTypes,
                            change: function (event, args) {
                                if (args.value.value) {
                                    $("#UserLabel").text("Server Admin Security Group");
                                    $("#VmDomains").show();
                                    $("#VmNics").show();
                                } else {
                                    $("#UserLabel").text("Username");
                                    $("#VmDomains").hide();
                                    $("#VmNics").hide();
                                }
                            }
                        });

                        //Temporary fix to disable domain joining. 
                        $("#isDomainjoined").hide();
                        $("#UserLabel").text("Username");
                        $("#VmDomains").hide();
                        $("#VmNics").hide();

                        $("#VmDomain").on("change", function () {
                            var vmDomain = $("#VmDomain").val();
                            var sqlIntallRadio = $("#sqlraido").fxRadio("value").value;
                            var listItems = "";
                            var domainSetFlag = false;

                            for (var i = 0; i < networkNicList.length; i++) {
                                if (vmDomain == networkNicList[i].ADDomain) {
                                    domainSetFlag = true;
                                    listItems += "<option data-Id='" + networkNicList[i].NetworkNICId + "' value='" + networkNicList[i].Name + "'>" + networkNicList[i].Name + "</option>";
                                }
                            }

                            if (domainSetFlag == false) {
                                var nics = [];
                                networkNicList.forEach(function (nic) {
                                    if ($.inArray(nic.Name, nics) == -1) {
                                        nics.push(nic.Name);
                                        listItems += "<option data-Id='" + nic.NetworkNICId + "' value='" + nic.Name + "'>" + nic.Name + "</option>";
                                    }
                                });
                                nics = [];
                            }
                            $("#VmNic1").html(listItems);
                        });

                        /////////////////////////////////////////
                        $("#VmAdminGroup").keydown(function () {
                            $("#lblmessageStatus").css("display", "none");
                        });

                        $("#VmAdminGroup").on("change", function () {
                            var vmAdminName = $("#VmAdminGroup").val();

                            vmAdminName = $.trim(vmAdminName);
                            vmAdminName = vmAdminName.toLowerCase();

                            var foundInvalidStr = false;
                            var invalidStrs = [/admin/g]; //Add more invalid strings
                            var length = invalidStrs.length;

                            while (length--) {
                                if (invalidStrs[length].test(vmAdminName)) {
                                    foundInvalidStr = true;
                                    break;
                                }
                            }

                            if (foundInvalidStr == true) {
                                $("#lblmessageStatus").css("display", "block");
                                $("#lblmessageStatus").text("Please enter valid username without containing 'admin'");
                                valid = false;
                            }
                            else {
                                $("#lblmessageStatus").css("display", "none");
                                valid = true;
                            }
                        });
                        $("#VmAdminGroup").blur(function () {
                            if (($("#domain-joined-radio").fxRadio("value").value) && (domainList != null && domainList.Count > 0)) {
                                var vmAdminName = $("#VmAdminGroup").val();
                                var currentgroups = vmAdminName;
                                if ($.trim(vmAdminName) != "" && prevvalue != vmAdminName) {
                                    $("#loading").css("display", "inline-block");

                                    vmAdminName = removewaq(vmAdminName);

                                    prevvalue = currentgroups;
                                    var promise = global.CmpWapExtensionTenantExtension.Controller.nameResolution(vmAdminName);
                                    promise.done(function (value) {
                                        $("#loading").css("display", "none");
                                        var status = { status: value.data.Status, result: value.data.Result };
                                        if (value.data.Status) {
                                            $("#lblmessage").css("display", "none");
                                            $("#VmAdminGroup").val(status.result);
                                            valid = true;
                                        } else {
                                            $("#lblmessage").css("display", "block");
                                            $("#lblmessage").text("Enter valid security group alias");
                                            valid = false;
                                        }
                                    });

                                    promise.fail(function (jqXHR, textStatus) {
                                        $("#loading").css("display", "none");
                                        $("#lblmessage").css("display", "block");
                                        $("#lblmessage").text("Aliase(s) did not Resolve");
                                        valid = false;
                                    });
                                }
                            }
                        });
                           
                        $("#VmSourceImage").on("change", function () {                            
                            $("#lblRegionOsMappingStatus").css("display", "none");
                        });
                        $("#VmSize").on("change", function () {
                            $("#lblRegionSizeMappingStatus").css("display", "none");
                        });
                    },
                    // Called before the wizard moves to the next step
                    onNextStep: function () {
                       var selectedImage = $("#VmSourceImage").val();
                        var selectedSize = $("#VmSize").val();
                        var selectedRegion = $("#VmRegion").val();

                        var validMapping = false;
                        var osId = 0;
                        var sizeId = 0;
                        var subId = subscriptionId;
                        var selectedRegionId = 0;

                        var ASubId = subscriptionMappingsList[0].AzureSubscriptionId;

                        osInfoList.forEach(function (value, index) {
                            if (selectedImage == osInfoList[index].Name) {
                                osId = osInfoList[index].VmOsId;
                            }
                        });

                        sizeInfoList.forEach(function(value, index) {
                            if (selectedSize == sizeInfoList[index].Name) {
                                sizeId = sizeInfoList[index].VMSizeId;
                                }
                         });

                        targetRegionsList.forEach(function (value, index) {
                            if (selectedRegion == targetRegionsList[index].Name) {
                                selectedRegionId = targetRegionsList[index].AzureRegionId;
                            }
                        });

                        var regionOsIds = [selectedRegionId, osId];
                        if (selectedRegionId > 0 && osId > 0) {
                            fetchVMOSMappings(subId, regionOsIds);
                        }                        

                        var regionSizeIds = [selectedRegionId, sizeId];
                        if (selectedRegionId > 0 && sizeId > 0) {
                            fetchVMSizeMappings(subId, regionSizeIds);
                        }

                        if (subscriptionRegionOSMapping == false) {
                            $("#lblRegionOsMappingStatus").css("display", "block");
                            $("#lblRegionOsMappingStatus").text("Please select a different OS for the selected region");
                            valid = false;
                        } else {
                            $("#lblRegionOsMappingStatus").css("display", "none");
                            valid = true;
                        }

                        if (subscriptionRegionSizeMapping == false) {
                            $("#lblRegionSizeMappingStatus").css("display", "block");
                            $("#lblRegionSizeMappingStatus").text("Please select a different SKU for the selected region");
                            valid = false;
                        } else if (valid == true){
                            $("#lblRegionSizeMappingStatus").css("display", "none");
                            valid = true;
                        }
                        if (Shell.UI.Validation.validateContainer(".hw-create-fileshare-container")) {
                            if (!valid) {
                                return false;
                            } else {
                                valid = true;
                            }
                        } else {
                            return false;
                        }
                    },
                    // Called each time the step is displayed
                    onStepActivate: function () {
                        //Shell.UI.Validation.setValidationContainer(wizardContainerSelector);
                        Shell.UI.Validation.setValidationContainer(".hw-create-fileshare-container");
                        /* $("#msitradio").fxRadio(
                        {
                        value: values[0], //default value
                        values: values
                        
                        }); */
                    }
                },
                /*{
                template: "CreateVM3",
                // Called when the step is first created
                onStepCreated: function () {
                //wizard = this;
                $("#VmSqladmin").keydown(function () {
                $("#lblmessageSql").css("display", "none");
                });
                
                $("#VmSqladmin").blur(function () {
                
                var vmSqlAdminName = $("#VmSqladmin").val();
                var currentgroups = vmSqlAdminName;
                if ($.trim(vmSqlAdminName) != "") {
                $("#loadingSql").css("display", "inline-block");
                vmSqlAdminName = removewaq(vmSqlAdminName);
                var promise = global.CmpWapExtensionTenantExtension.Controller.nameResolution(vmSqlAdminName);
                promise.done(function (value) {
                $("#loadingSql").css("display", "none");
                var status = { status: value.data.Status, result: value.data.Result };
                if (value.data.Status) {
                $("#lblmessageSql").css("display", "none");
                $("#VmSqladmin").val(status.result);
                valid = true;
                }
                else {
                $("#lblmessageSql").css("display", "block");
                $("#lblmessageSql").text("Enter valid security group alias");
                valid = false;
                }
                });
                
                promise.fail(function (jqXHR, textStatus) {
                $("#loadingSql").css("display", "none");
                $("#lblmessageSql").css("display", "block");
                $("#lblmessageSql").text("Aliase(s) did not Resolve");
                valid = false;
                });
                }
                
                });
                
                var defaultcollationindex;
                $.each(sqlcollationList, function (i, value) {
                if (value.Name == defaultsqlcollation) {
                defaultcollationindex = i;
                return;
                }
                });
                
                
                if (defaultcollationindex != -1)
                $("#VmSqlcollation")[0].selectedIndex = defaultcollationindex;
                },
                // Called before the wizard moves to the next step
                onNextStep: function () {
                
                if (Shell.UI.Validation.validateContainer(".hw-create-roles-container")) {
                if (!valid) {
                return false;
                }
                else {
                valid = true;
                }
                }
                else
                return false;
                },
                // Called each time the step is displayed
                onStepActivate: function () {
                
                // un comment once iis sql has been integrated
                var sql = $("#sqlconfig");
                var iis = $("#iisconfig");
                var sqlsubsection = $("#sqlconfigsection");
                var iissubsection = $("#iissubsection");
                var noconfig = $("#noconfig");
                var selectedrole = 3; //$("#VmServerRole")[0].selectedIndex;
                Shell.UI.Validation.setValidationContainer(".hw-create-roles-container");
                
                //This code to be deleted once IIS SQL is configured with CMP WAP
                // var previewconfig = $("#previewconfig");
                //iis.css("display", "none");
                //sql.css("display", "none");
                //noconfig.css("display", "block");
                
                // addsa - The commented out code below to be used when we have CMP WAP integrated with IIS SQL
                // Until then, we skip IIS SQL installation in the UI and proceed ahead.
                
                if (selectedrole == 0)
                {
                iis.css("display", "none");
                sql.css("display", "block");
                noconfig.css("display", "none");
                }
                else if (selectedrole == 2)
                {
                iis.css("display", "block");
                sql.css("display", "none");
                noconfig.css("display", "none");
                }
                else
                {
                iis.css("display", "none");
                sql.css("display", "none");
                noconfig.css("display", "block");
                }
                
                if(iis.css('display') == 'block')
                {
                
                $("#iisradio").fxRadio(
                {
                value: values[0], //default value
                values: values,
                change: function (event, args) {
                if (args.value.text == "Yes") {
                iissubsection.css('display', "block");
                }
                else {
                iissubsection.css('display', "none");
                }
                }
                })
                }
                
                if(sql.css('display') == 'block')
                {
                $("#sqlraido").fxRadio(
                {
                value: values[0],
                values: values,
                change: function (event, args) {
                if(args.value.text=="Yes")
                {
                sqlsubsection.css('display', "block");
                }
                else
                {
                sqlsubsection.css('display', "none");
                }
                }
                
                
                })
                
                
                var sqlanalysissection = $("#sqlanalysissection");
                $("#sqlcheckboxanalysis").fxCheckBox({
                value: false,
                change: function (event, args) {
                if (args.value) {
                sqlanalysissection.css('display', "block");
                }
                else {
                sqlanalysissection.css('display', "none");
                }
                }
                
                });
                
                
                $("#sqlcheckboxIntegration").fxCheckBox({
                value: false
                });
                
                
                $("#sqlcheckboxreplication").fxCheckBox({
                value: false
                
                });
                }
                }
                },*/
                {
                    template: "CreateVM4",
                    //data: data,
                    // Called when the step is first created
                    onStepCreated: function () {
                        //wizard = this;
                    },
                    // Called before the wizard moves to the next step
                    onNextStep: function () {
                        //return Shell.UI.Validation.validateContainer(".hw-create-storage-container");
                    },
                    // Called each time the step is displayed
                    onStepActivate: function () {
                        $("#extradrives").fxGrid("destroy");

                        populateCreateVM4Uielements();

                        Shell.UI.Validation.setValidationContainer(".hw-create-storage-container");
                    }
                }
            ],
            onComplete: function () {
                if (!Shell.UI.Validation.validateContainer(".hw-create-storage-container")) {
                    return false;
                }

                var osCode = "Windows"; //*** ["Linux","Windows"] added to support Linux. 
                var azureApiName = "RDFE"; //*** ["RDFE","ARM"] added to support ARM

                var vmAppName = $("#VmAppNameSelect").val();
                var vmAppId = $("#VmAppNameSelect").val();
                var vmServerName = $("#VmServerName").val();
                var vmDomain = $("#VmDomain").val();
                var vmAdminName = $("#VmAdminGroup").val();
                var vmAdminPassword = $("#VmAdminPassword").val();
                var vmSourceImage = $("#VmSourceImage").val();
                var vmSize = $("#VmSize").val();
                var vmRegion = $("#VmRegion").val();

                var subscriptionId = $('#SubscriptionName').val();
                //for (var item in resourcegroupList) {
                //    if (item.) {

                //    }
                //}

                //Changed codebase for beanstalk
                var vmRole = 3;
                var envResourcegroupname = "";
                if (($("#domain-joined-radio").fxRadio("value").value) && (domainList != null && domainList.Count > 0)) {
                    var domainId = $("#VmDomain option:selected").data("id");

                    var envId = $("#VmEnvironment option:selected").data("id");

                    var nicId = $("#VmNic1 option:selected").data("id");

                    $.each(resourcegroupList, function (i, val) {
                        if (val.DomainId == parseInt(domainId) && val.EnvironmentTypeId == parseInt(envId) && val.NetworkNICId == parseInt(nicId))
                         {
                            envResourcegroupname = val.Name;
                        }
                    });
                } else {
                    envResourcegroupname = "DEFAULT";
                    vmDomain = null;
                }

                var vmDiskSpec = buildDiskSpecXml();
                var vmConfig = "";
                var vmTagData = "";
                var servicecategory = "Basic";
                var nic1 = $("#VmNic1").val();
                var Msitmonitored = false;

                var sqlConfig, iisconfig, environmentclass;

                //var role = $("#VmServerRole");
                var selectedrole = 3;
                environmentclass = $("#VmEnvironment").val();

                // addsa - to be replaced with the code below once IIS SQL is integrated
                iisconfig = null;
                sqlConfig = null;

                if (selectedrole == 0) {
                    iisconfig = null;
                    sqlConfig = {
                        InstallSql: $("#sqlraido").fxRadio("value").value,
                        InstallAnalysisServices: $("#sqlcheckboxanalysis").fxCheckBox("value"),
                        InstallReplicationServices: $("#sqlcheckboxreplication").fxCheckBox("value"),
                        InstallIntegrationServices: $("#sqlcheckboxIntegration").fxCheckBox("value"),
                        SqlInstancneName: $("#VmSqlinstance").val(),
                        Collation: $("#VmSqlcollation").val(),
                        Version: $("#VmSqlversion").val(),
                        AdminGroups: $("#VmSqladmin").val(),
                        AnalysisServicesMode: $("#sqlanalysismode").val()
                    };
                } else if (selectedrole == 2) {
                    sqlConfig = null;
                    iisconfig = {
                        InstallIis: $("#iisradio").fxRadio("value").value,
                        RoleServices: $("#IIsRoleservices").val()
                    };
                } else {
                    iisconfig = null;
                    sqlConfig = null;
                }

                promise = global.CmpWapExtensionTenantExtension.Controller.createVm(subscriptionId, vmAppName, vmAppId, envResourcegroupname,
                    vmServerName, vmDomain, vmAdminName, vmAdminPassword, vmSourceImage, vmSize, vmRegion, vmRole, vmDiskSpec, vmConfig,
                    vmTagData, servicecategory, nic1, Msitmonitored, sqlConfig, iisconfig, environmentclass, accountAdminLiveEmailId, osCode, azureApiName);

                global.waz.interaction.showProgress(promise, {
                    initialText: "Submitting VM request...",
                    successText: "Successfully submitted VM request.",
                    failureText: "Failed to submit VM request."
                });

                destroyWidgets();
            }
        }, {
                size: "mediumplus"
            });
    }

    //*************************************************************************
    // Loads the dropdown list of available SKUs
    //*************************************************************************
    function populateskudropdown(sizeList) {
        var listItems = "";
        for (var i = 0; i < sizeList.length; i++) {
            listItems += "<option value='" + sizeList[i].Name + "' data-maxdiskcount='" + sizeList[i].MaxDataDiskCount + "'>" + sizeList[i].Description + ", MaxDataDiskCount-" + sizeList[i].MaxDataDiskCount + "</option>";
        }
        $("#VmSize").html(listItems);
    }

    //*************************************************************************
    // Removes a virtual machine administrator
    // todo: Rename
    //*************************************************************************
    function removewaq(vmAdminName) {
        var splitgroups = vmAdminName.split(';');
        splitgroups = $.map(splitgroups, function (group, index) {
            var waqindex = group.indexOf('\\');
            if (waqindex != -1)
                return group.substring(waqindex + 1);
            else
                return group;
        });
        vmAdminName = splitgroups.join(';');
        return vmAdminName;
    }

    //*************************************************************************
    // Cleans up the UI
    //*************************************************************************
    function destroyWidgets() {
        $("#sqlraido").fxRadio("destroy");

        $("#sqlcheckboxanalysis").fxCheckBox("destroy");

        $("#sqlcheckboxIntegration").fxCheckBox("destroy");

        $("#sqlcheckboxreplication").fxCheckBox("destroy");

        $("#predefineddrives").fxGrid("destroy");

        $("#extradrives").fxGrid("destroy");

        $("#iisradio").fxRadio("destroy");
        //   $("#msitradio").fxRadio("destroy");
    }

    //*************************************************************************
    // Populates elements for the virtual machine creation dialog
    //*************************************************************************
    function populateCreateVM4Uielements() {
        selectedrow = null;
        $("#extradrivessection").undelegate("#grid-add", "click", adddrives);
        $("#extradrivessection").undelegate("#grid-remove", "click", removedrives);

        // $("#extradrives").undelegate(".drivenames", "click", modifydrivenames);
        $("#extradrives").undelegate(".drivesize", "blur", storeextradrivesize);
        $("#extradrives").undelegate(".drivenames", "blur", storeextradrivename);
        $("#predefineddrives").undelegate(".drivesize", "blur", modifypredefdrivesize);

        $("#extradirvescheckbox").fxCheckBox({
            value: false,
            change: function (event, data) {
                //setSkipQuickStart(data.value);
                var extradivessection = $("#extradrivessection");
                if (data.value) {
                    extradivessection.css("display", "block");
                } else {
                    $("#extradrives").fxGrid("option", "data", []);
                    extradivessection.css("display", "none");
                }
            }
        });

        var drives = [{ name: "C", size: "100" }];
        var drivenames = [];

        // var role = $("#VmServerRole");
        var selectedrole = 3;
        var filtereddrives = $.grep(serverroleDrivemappingList, function (n) {
            return n.ServerRoleId == selectedrole;
        });

        $.each(filtereddrives, function (i, val) {
            drives.push({ name: val.Drive, size: val.MemoryInGB });
            drivenames.push(val.Drive);
        });

        filtereddrives.length = filtereddrives.length == 0 ? 1 : filtereddrives.length;

        var maxdiskcountforrsku = $("#VmSize option:selected").data("maxdiskcount");
        var maxadditionaldisksallowed = maxdiskcountforrsku - filtereddrives.length;

        $("#lbladditionaldisks").text(maxadditionaldisksallowed);
        var defaultdrivename;

        //filtereddrives = $.grep(filtereddrives,)
        extradrivenames = $(defaultdriveslist).not(drivenames).get();

        // Drives grid populated based on Role and SKU size
        $("#predefineddrives").fxGrid({
            columns: [
                {
                    name: "Drive*", field: "name",
                    formatter: function (value) {
                        var changedtext = "<select style='width:50px' disabled>";
                        changedtext += "<option >" + value + "</option>";
                        changedtext += "</select>";
                        defaultdrivename = value;
                        return changedtext;
                    },
                    cssClass: "gridcolumnwidth"
                },
                {
                    name: "Storage(GB)*",
                    field: "size",
                    formatter: function (val) {
                        var changedText = defaultdrivename == "C" ? "<input type='text'  style='width:50px' value=" + val + " disabled></input>" : "<input id=" + defaultdrivename + " type='text' class='fx-textbox fx-validation fx-editablecontrol drivesize' data-val='true' data-val-required='drive size is required' data-val-regex='disk size should contain numbers greater than 0' data-val-regex-pattern='^[1-9][0-9]*$' style='width:50px' value=" + val + " ></input>";

                        // : "<input id=" + defaultdrivename + " type='text' class='fx-textbox fx-validation fx-editablecontrol drivesize' data-val='true' data-val-required='drive size is required' data-val-regex='disk size should contain numbers' data-val-regex-pattern='^[0-9]+' style='width:50px' value=" + val + " ></input>";
                        return changedText;
                    },
                    cssClass: "gridcolumnwidth"
                }
            ],
            /*  rowSelect: function (e, args) {
            //Bind an event when a row is selected
            selectedrow = null;
            selectedrow = args.selected;
            },*/
            data: drives,
            selectable: true,
            multiselect: false
        });

        // Extra drives test data
        var extradrivesdata = [];
        var exselecteddrive;

        //Extradrives grid
        $("#extradrives").fxGrid({
            columns: [
                {
                    name: "Drive*", field: "drivenames",
                    formatter: function (value) {
                        var changedtext = "<select  style='width:50px' class='drivenames'>";
                        var isselect = false;
                        $.each(value.names, function (index, item) {
                            changedtext += "<option>" + item + "</option>";
                        });
                        changedtext += "</select>";
                        exselecteddrive = null;
                        exselecteddrive = value.selecteddrive;
                        return changedtext;
                    },
                    cssClass: "gridcolumnwidth"
                },
                {
                    name: "Storage(GB)*",
                    field: "drivesize",
                    formatter: function (value) {
                        // var changedText = "<input type='text' style='width:50px' class='fx-textbox fx-validation fx-editablecontrol drivesize' data-val='true' data-val-required='drive size is required' data-val-regex='disk size should contain numbers' data-val-regex-pattern='^[0-9]+'  value=" + value + "></input>";
                        var changedText = "<input id=" + exselecteddrive + " type='text' style='width:50px' class='fx-textbox fx-validation fx-editablecontrol drivesize' data-val='true' data-val-required='drive size is required' data-val-regex='disk size should contain numbers greater than 0' data-val-regex-pattern='^[1-9][0-9]*$'  value=" + value + "></input>";
                        return changedText;
                    },
                    cssClass: "gridcolumnwidth"
                },
                {
                    formatter: function (value) {
                        var changedText = " <a id='grid-remove' href='#'  onclick='javascript:return false'>[*] Close</a>";
                        return changedText;
                    },
                    cssClass: "gridclosecolumn"
                }
            ],
            rowSelect: function (e, args) {
                //Bind an event when a row is selected
                selectedrow = null;
                selectedrow = args.selected;
            },
            data: extradrivesdata,
            selectable: true
        });

        $("#extradrivessection").delegate("#grid-add", "click", adddrives);
        $("#extradrivessection").delegate("#grid-remove", "click", removedrives);

        // $("#extradrives").delegate(".drivenames", "click",  modifydrivenames);
        $("#extradrives").delegate(".drivesize", "blur", storeextradrivesize);
        $("#extradrives").delegate(".drivenames", "blur", storeextradrivename);
        $("#predefineddrives").delegate(".drivesize", "blur", modifypredefdrivesize);
    }

    //*************************************************************************
    // Get drive size selected by user
    //*************************************************************************
    /*
    function getUserSelectedSizeForPredefDrives() {
    predefDriveVals[defaultdrivename] = document.getElementById("dvals").value;
    }
    */
    //*************************************************************************
    // Adds drives to the creation UI
    //*************************************************************************
    function adddrives() {
        var extradrivesdata = $("#extradrives").fxGrid("option", "data");

        var removelink = $("#grid-remove");
        var selecteddrives = [];

        $.each(extradrivesdata, function (index, value) {
            if (value.drivenames.selecteddrive != "")
                selecteddrives.push(value.drivenames.selecteddrive);
        });

        var arr = $.grep(extradrivenames, function (value, i) {
            return $.inArray(value, selecteddrives) == -1;
        });

        var diskcount = $("#lbladditionaldisks").text();

        if (extradrivesdata.length < parseInt(diskcount)) {
            $.observable(extradrivesdata).insert(extradrivesdata.length, {
                drivenames: { names: arr, selecteddrive: arr[0] },
                drivesize: ""
            });
        }
        Shell.UI.Validation.setValidationContainer(".hw-create-storage-container");
        if (extradrivesdata.length == 0) {
            removelink.css("display", "none");
        } else {
            removelink.css("display", "block");
        }
    }

    //*************************************************************************
    // Removes drives from the creation UI
    //*************************************************************************
    function removedrives() {
        var rowsmetadata = $("#extradrives").fxGrid("getAllRowMetadata");
        var extradrivesdata = $("#extradrives").fxGrid("option", "data");
        var selectedrowarray = $.grep(rowsmetadata, function (element, index) {
            return (element.selected == true);
        });
        if (selectedrowarray.length > 0) {
            var index = $.inArray(selectedrowarray[0].dataItem, extradrivesdata);

            $.observable(extradrivesdata).remove(index);
        }

        if (extradrivesdata.length == 0) {
            $("#grid-remove").css("display", "none");
        }
    }

    //*************************************************************************
    // Modifies the drive names in the UI
    //*************************************************************************
    function modifydrivenames(e) {
        var rowdata = $("#extradrives").fxGrid("getAllRowMetadata");
        var data = $("#extradrives").fxGrid("option", "data");

        //  var lastselecteddrive = this.value;
        var index = this.selectedIndex;
        var currentvalue = this.value;
        var selecteddrives = [];

        var extradrives = extradrivenames;
        var itemindex = extradrives.indexOf(currentvalue);
        extradrives.splice(itemindex, 1);

        //$.each(data, function (i, val) { val.drivenames.names = extradrives; });
        $.each(data, function (index, value) {
            if (value.drivenames.selecteddrive != currentvalue)
                selecteddrives.push(value.drivenames.selecteddrive);
        });

        var $this = this;
        $.each($this, function (i, val) {
            if (val != null) {
                var index = $.inArray(val.innerText, selecteddrives);

                if (index >= 0) {
                    $this[i].removeNode();
                }
            }
        });
        //   this = $this;
        /* var filtereddrives = $(extradrives).not(selecteddrives).get();
        
        $.each(data, function (i, val) { val.drivenames.names = filtereddrives; });
        //var rowindex = $.inArray(selectedrow, rowdata);
        //if (rowindex != -1) {
        
        //    data[rowindex].drivenames.selecteddrive = this.value;
        //   // var valueindex= data[rowindex].drivenames.names.indexOf(this.value);
        
        //  var  filtereddata = $.grep(data, function (value, i) {
        //        return $.grep(value.drivenames.names, function (item) {
        //            return item != currentvalue;
        //        })
        //    })
        //    if (lastselecteddrive != this.value)
        //    {
        //        data = $.map(data, function (value, i) {
        //            value.drivenames.names.splice(valueindex, 0, this.value);
        //        })
        //    }
        
        // $.observable(data).replaceAll(data);
        $("#extradrives").fxDataGrid("refresh");
        
        $("#extradrives").fxDataGrid("bind");
        //    $("#extradrives").fxGrid("option", "data", data);
        //}
        
        */
    }

    //*************************************************************************
    // Update a preloaded drive size
    //*************************************************************************
    function modifypredefdrivesize(e) {
        var rowdata = null;
        var griddata = null;
        var index = -1;

        rowdata = $("#predefineddrives").fxGrid("getAllRowMetadata");
        griddata = $("#predefineddrives").fxGrid("option", "data");

        for (var i = 0; i < rowdata.length; i++) {
            if (this.id == rowdata[i].dataItem.name) {
                index = i;
                break;
            }
        }

        if (index != -1) {
            griddata[index].size = this.value;
            $.observable(griddata);
        }
    }

    //*************************************************************************
    // Saves an additional drive size
    //*************************************************************************
    function storeextradrivesize(e) {
        var rowdata = null;
        var griddata = null;
        var index = -1;

        rowdata = $("#extradrives").fxGrid("getAllRowMetadata");
        griddata = $("#extradrives").fxGrid("option", "data");

        for (var i = 0; i < rowdata.length; i++) {
            if (this.id == rowdata[i].dataItem.drivenames.selecteddrive) {
                index = i;
                break;
            }
        }

        if (index != -1) {
            griddata[index].drivesize = this.value;
            $.observable(griddata);
        }
    }

    //*************************************************************************
    // Saves an extra drive name
    //*************************************************************************
    function storeextradrivename(e) {
        var rowdata = null;
        var griddata = null;
        var index = -1;

        rowdata = $("#extradrives").fxGrid("getAllRowMetadata");
        griddata = $("#extradrives").fxGrid("option", "data");

        for (var i = 0; i < rowdata.length; i++) {
            if (this.id == rowdata[i].dataItem.drivenames.selecteddrive) {
                index = i;
                break;
            }
        }

        if (index != -1) {
            griddata[index].drivenames.selecteddrive = this.value;
            $.observable(griddata);
        }
    }

    //*************************************************************************
    // Launches the virtual machine creation dialog
    //*************************************************************************
    function executeCreateVm() {
        ShowCreateVmWizard("xx");
    }

    //*************************************************************************
    // Launches the virtual machine migration dialog
    //*************************************************************************
    function executeMigrateVm() {
        ShowCreateVmWizard("xx");
    }

    //*************************************************************************
    // Initializes the Phoenix CMP WAP extension
    //*************************************************************************
    CmpWapExtensionTenantExtensionActivationInit = function () {
        var subs = Exp.Rdfe.getSubscriptionList(), CmpWapExtensionExtension = $.extend(this, global.CmpWapExtensionTenantExtension);

        subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("CmpWapExtension");

        // Don't activate the extension if user doesn't have a plan that includes the service.
        if (subscriptionRegisteredToService.length === 0) {
            return false;
        }

        //subscriptionRegisteredToService.forEach(function (sub) {
        //    allSubscriptionIds.push(sub.id);
        //});

        allSubscriptionIds = subscriptionRegisteredToService.map(x => x.id);
        subscriptionId = subscriptionRegisteredToService.length ? subscriptionRegisteredToService[0].id : null;
        this.subscriptionId = subscriptionRegisteredToService.length ? subscriptionRegisteredToService[0].id : null;
        accountAdminLiveEmailId = subscriptionRegisteredToService[0].AccountAdminLiveEmailId;

        $.extend(CmpWapExtensionExtension, {
            viewModelUris: [CmpWapExtensionExtension.Controller.listVMsUrl],
            displayName: "Azure VMs",
            navigationalViewModel: {
                uri: CmpWapExtensionExtension.Controller.listVMsUrl,
                ajaxData: function () {
                    return global.Exp.Rdfe.getSubscriptionIdsRegisteredToService(serviceName);
                }
            },
            displayStatus: global.waz.interaction.statusIconHelper(global.CmpWapExtensionTenantExtension.FileSharesTab.statusIcons, "Status"),
            menuItems: [
                {
                    name: "FileShares",
                    displayName: "Azure VMs",
                    url: "#Workspaces/CmpWapExtensionTenantExtension",
                    preview: "createPreview",
                    subMenu: [
                        {
                            name: "Create",
                            displayName: "Create Azure VM",
                            description: "Create a new build-in-cloud Azure VM",
                            execute: executeCreateVm
                        }
                    ]
                }
            ],
            getResources: function () {
                return resources;
            }
        });

        CmpWapExtensionExtension.onNavigating = onNavigating;
        CmpWapExtensionExtension.onNavigateAway = onNavigateAway;
        CmpWapExtensionExtension.navigation = navigation;

        Shell.UI.Pivots.registerExtension(CmpWapExtensionExtension, function () {
            Exp.Navigation.initializePivots(this, this.navigation);
        });

        Exp.TypeRegistry.add("CmpWapExtensionTenantExtension", navigation.types);

        // Finally activate and give "the" CmpWapExtensionExtension the activated extension since a good bit of code depends on it
        $.extend(global.CmpWapExtensionTenantExtension, Shell.Extensions.activate(CmpWapExtensionExtension));

        //Load domain data
        console.log(subscriptionRegisteredToService);
        fetchDomainNames(allSubscriptionIds);
        fetchSizeInfoList(allSubscriptionIds);
        fetchOsInfoList(allSubscriptionIds);
        fetchRegionInfoList(allSubscriptionIds);
        fetchAppInfoList(allSubscriptionIds);
        fetchResourceGroupList(allSubscriptionIds);

        fetchServerRolesList(allSubscriptionIds);
        fetchServerRoleDriveMappingList(allSubscriptionIds);
        fetchServiceCategoriesList(allSubscriptionIds);
        fetchSQLCollationsList(allSubscriptionIds);
        fetchSQlVersionsList(allSubscriptionIds);
        fetchEnvironmentTypesList(allSubscriptionIds);
        fetchNetworkNICList(allSubscriptionIds);
        fetchIISRoleServicesList(allSubscriptionIds);
        fetchSQlAnalysisServiceModesList(allSubscriptionIds);
        fetchSubscriptionMappings(allSubscriptionIds);
        //setting commands to be performed on multiple VMs here
        //setContextualCommands(true, true); // commented out for future use
    };

    //*************************************************************************
    // Updates the application bar commands
    //*************************************************************************
    var setContextualCommands = function (stopBool, deallocbool) {
        Exp.UI.Commands.Contextual.add(new Exp.UI.Command("stopVm", "Stop VM", Exp.UI.CommandIconDescriptor.getWellKnown("shutdown"), stopBool, null, onStopScenarios));
        Exp.UI.Commands.Contextual.add(new Exp.UI.Command("stopVm", "Deallocate", Exp.UI.CommandIconDescriptor.getWellKnown("shutdown"), deallocbool, null, onDeallocateScenarios));
    };

    //*************************************************************************
    // Stops all virtual machines
    //*************************************************************************
    var onStopScenarios = function (item) {
        onStopVm(item);
    };

    //*************************************************************************
    // Deallocate all virtual machines
    //*************************************************************************
    var onDeallocateScenarios = function (item) {
        onDeallocateVm(item);
    };

    //*************************************************************************
    // Updates UI to verify user wants to deallocate their machines
    //*************************************************************************
    var onDeallocateVm = function (item) {
        var stopConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to deallocate all the VMs ?");

        // Note you could have multiple options – setActions takes an array
        stopConfirmation.setActions([
            Shell.UI.Notifications.Buttons.yes(function () {
                performDeallocate(item);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(stopConfirmation);
    };

    //*************************************************************************
    // Updates UI to verify user wants to stop their machines
    //*************************************************************************
    var onStopVm = function (item) {
        var stopConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to Stop all the VMs ?");

        // Note you could have multiple options – setActions takes an array
        stopConfirmation.setActions([
            Shell.UI.Notifications.Buttons.yes(function () {
                performStop(item);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(stopConfirmation);
    };

    //*************************************************************************
    // Deallocates the machines and updates the UI
    //*************************************************************************
    var performDeallocate = function (item) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation("Deallocating VM(s)...", null, true);

        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);

        setContextualCommands(false, false);
        deallocateCommandSend(item, progressOperation);
    };

    //*************************************************************************
    // Stops the machines and updates the UI
    //*************************************************************************
    var performStop = function (item) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation("Stopping VM(s)...", null, true);

        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);

        setContextualCommands(false, false);
        stopCommandSend(item, progressOperation);
    };

    //*************************************************************************
    // Sends a command to stop the virtual machines to the API
    //*************************************************************************
    var stopCommandSend = function (item, progressOperation) {
        //$.post("/CmpWapExtensionTenant/VmOp", { subscriptionId: subscriptionRegisteredToService, Opcode: "STOP", isMultiOp: true })
        global.CmpWapExtensionTenantExtension.Controller.postVmOps({ subscriptionId: subscriptionRegisteredToService, Opcode: "STOP", IsMultiOp: true }).done(function (data) {
            progressOperation.complete("Successfully sent stop VM(s) to the queue.", Shell.UI.InteractionSeverity.information);
            setContextualCommands(false, false);
        }).fail(function (jqXHR, textStatus, errorThrown) {
                var messageDetail = JSON.parse(jqXHR.responseText).message;

                //commandError("Stopping the VM(s)", progressOperation, messageDetail);
                progressOperation.complete("Stopping the VM(s) failed", Shell.UI.InteractionSeverity.error, Shell.UI.InteractionBehavior.ok, ("Stopping the VM(s) failed" ? { detailData: messageDetail } : null));

                setContextualCommands(true, true);
            });
    };

    //*************************************************************************
    // Sends a command to deallocate the virtual machines to the API
    //*************************************************************************
    var deallocateCommandSend = function (item, progressOperation) {
        //$.post("/CmpWapExtensionTenant/VmOp", { subscriptionId: subscriptionRegisteredToService, Opcode: "DEALLOCATE", IsMultiOp: true })
        global.CmpWapExtensionTenantExtension.Controller.postVmOps({ subscriptionId: subscriptionRegisteredToService, Opcode: "DEALLOCATE", IsMultiOp: true }).done(function (data) {
            progressOperation.complete("Successfully sent Deallocate VM(s) to the queue.", Shell.UI.InteractionSeverity.information);
            setContextualCommands(false, false);
        }).fail(function (jqXHR, textStatus, errorThrown) {
                var messageDetail = JSON.parse(jqXHR.responseText).message;

                //commandError("Deallocating the VM(s)", progressOperation, messageDetail);
                progressOperation.complete("Deallocating the VM(s) failed", Shell.UI.InteractionSeverity.error, Shell.UI.InteractionBehavior.ok, ("Deallocating the VM(s) failed" ? { detailData: messageDetail } : null));

                setContextualCommands(true, true);
            });
    };

    //*************************************************************************
    // Saves the list of Internet Information Services roles
    //*************************************************************************
    function gotIISRoleServicesList(data) {
        iisroleservicesList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of Internet Information Services roles from the API
    //*************************************************************************
    function fetchIISRoleServicesList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getiisroleservicesurl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotIISRoleServicesList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of SQL Analysis Service modes
    //*************************************************************************
    function gotSQlAnalysisServiceModesList(data) {
        sqlanalysisservicemodesList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of SQL Analysis Service modes from the API
    //*************************************************************************
    function fetchSQlAnalysisServiceModesList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getsqlanalysisservicemodesurl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotSQlAnalysisServiceModesList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of SQL Server versions
    //*************************************************************************
    function gotSQlVersionsList(data) {
        sqlVersionList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of SQL Server versions from the API
    //*************************************************************************
    function fetchSQlVersionsList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getSqlVersionListUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotSQlVersionsList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of domains
    //*************************************************************************
    function gotTheDomainListData(data) {
        domainList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of domains from the API
    //*************************************************************************
    function fetchDomainNames(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getDomainlistUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotTheDomainListData(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of virtual machine sizes
    //*************************************************************************
    function gotTheSizeInfoList(data) {
        sizeInfoList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of virtual machine sizes
    //*************************************************************************
    function fetchSizeInfoList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getSizeInfoListUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotTheSizeInfoList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of operating systems
    //*************************************************************************
    function gotTheOsInfoList(data) {
        osInfoList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of operating systems from the UI
    //*************************************************************************
    function fetchOsInfoList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getOsInfoListUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotTheOsInfoList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of regions
    //*************************************************************************
    function gotTheRegionInfoList(data) {
        targetRegionsList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of regions from the API
    //*************************************************************************
    function fetchRegionInfoList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getTargetRegionsListUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotTheRegionInfoList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of applications
    //*************************************************************************\
    function gotTheAppInfoList(data) {
        appList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of applications from the API
    //*************************************************************************
    function fetchAppInfoList(allSubscriptionIds: string[]) {
        return $.ajax({
            type: 'POST',
            url: getAppListUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotTheAppInfoList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of resource groups
    //*************************************************************************
    function gotTheResourceGroupList(data) {
        resourcegroupList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of resource groups from the API
    //*************************************************************************
    function fetchResourceGroupList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getResourceGroupsUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotTheResourceGroupList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of environments
    //*************************************************************************
    function gotTheEnvironmentList(data) {
        environmenttypeList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of environments from the API
    //*************************************************************************
    function fetchEnvironmentTypesList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getEnvironmenttypeListUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotTheEnvironmentList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of server roles
    //*************************************************************************
    function gotTheServerRoleList(data) {
        serverRoleList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of server roles from the API
    //*************************************************************************
    function fetchServerRolesList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getServerRoleListUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function(data) {
                gotTheServerRoleList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of service categories
    //*************************************************************************
    function gotTheServiceCategoryList(data) {
        serviceCategoryList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of service categories from the API
    //*************************************************************************
    function fetchServiceCategoriesList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getServiceCategoryListUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotTheServiceCategoryList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of network interface controllers
    //*************************************************************************
    function gotTheNetworkNICList(data) {
        networkNicList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of network interface controllers from the API
    //*************************************************************************
    function fetchNetworkNICList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getNetworkNICListUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotTheNetworkNICList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of drive mappings for server roles
    //*************************************************************************
    function gotServerRoleDriveMappingList(data) {
        serverroleDrivemappingList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of drive mapping for server roles from the API
    //*************************************************************************
    function fetchServerRoleDriveMappingList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getServerRoleDriverMappingListUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotServerRoleDriveMappingList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of SQL collations
    //*************************************************************************
    function gotSQLCollationsList(data) {
        sqlcollationList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of SQL collations from the API
    //*************************************************************************
    function fetchSQLCollationsList(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getSqlCollationListUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
                gotSQLCollationsList(data.data);
            });
    }

    //*************************************************************************
    // Saves the list of Subscription Mappings
    //*************************************************************************
    function gotSubscriptionMappingsList(data) {
        subscriptionMappingsList = data;
        //alert("init " + JSON.stringify(data));
    }

    //*************************************************************************
    // Fetches the list of mappings between WAP and Azure Subscriptions
    //*************************************************************************
    function fetchSubscriptionMappings(allSubscriptionIds: string[]) {
        $.ajax({
            type: 'POST',
            url: getSubscriptionMappingsUrl,
            contentType: 'application/json',
            data: JSON.stringify(allSubscriptionIds)
        }).done(function (data) {
            this.subscriptionMappingsList = data;
                gotSubscriptionMappingsList(data);
            });
    }

       function gotRegionOSMappingsList(data) {
        subscriptionRegionOSMapping = data;
    }

    //*************************************************************************
    // Fetches the list of mappings between Regions and OS
    //*************************************************************************
    var fetchVMOSMappings = function(allSubscriptionIds, ids) {

        var mappingInfo = { "subscriptionId": allSubscriptionIds, "Ids": ids };
        $.ajax({
            type: 'POST',
            url: getVMOSMappingsUrl,
            contentType: 'application/json',
            data: JSON.stringify(mappingInfo),
            async: false
        }).done(function (data) {
            gotRegionOSMappingsList(data);
        });
    }

    function gotRegionSizeMappingsList(data) {
        subscriptionRegionSizeMapping = data;
    }

    //*************************************************************************
    // Fetches the list of mappings between Regions and Size
    //*************************************************************************
    var fetchVMSizeMappings = function(allSubscriptionIds, ids) {

        var mappingInfo = { "subscriptionId": allSubscriptionIds, "Ids": ids };
        $.ajax({
            type: 'POST',
            url: getVMSizeMappingsUrl,
            contentType: 'application/json',
            data: JSON.stringify(mappingInfo),
            async: false
        }).done(function (data) {
            gotRegionSizeMappingsList(data);
        });
    }

    //*************************************************************************
    // Returns the Quick Create menu item for creating file shares
    //*************************************************************************
    function getQuickCreateFileShareMenuItem() {
        return {
            name: "QuickCreate",
            displayName: "Create Azure VM",
            description: "Create Azure VM",
            template: "quickCreateWithRdfe",
            label: resources[0].CreateMenuItem,
            opening: function () {
                AccountsAdminExtension.AccountsTab.renderListOfHostingOffers(offersListSelector);
            },
            open: function () {
                // Enables As-You-Type validation experience on a container specified
                Shell.UI.Validation.setValidationContainer(valContainerSelector);

                // Enables password complexity feedback experience on a container specified
                Shell.UI.PasswordComplexity.parse(valContainerSelector);
            },
            ok: function (object) {
                var dialogFields = object.fields, isValid = this.validateAccount();

                if (isValid) {
                    this.createAccountWithRdfeCore(dialogFields);
                }
                return false;
            },
            cancel: function (dialogFields) {
                // you can return false to cancel the closing
            }
        };
    }

    //*************************************************************************
    // Updates the UI to indicate an error occurred
    //*************************************************************************
    function commandError(message, progressOperation, messageDetail) {
        progressOperation.complete(message, Shell.UI.InteractionSeverity.error, Shell.UI.InteractionBehavior.ok, (message ? { detailData: messageDetail } : null));
    }

    Shell.Namespace.define("CmpWapExtensionTenantExtension", {
        serviceName: serviceName,
        init: CmpWapExtensionTenantExtensionActivationInit,
        getQuickCreateFileShareMenuItem: getQuickCreateFileShareMenuItem
    });
})(jQuery, this);
