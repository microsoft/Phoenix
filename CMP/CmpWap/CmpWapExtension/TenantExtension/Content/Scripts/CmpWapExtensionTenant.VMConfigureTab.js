(function ($, global, undefined) {
    "use strict";

    $('head').append('<meta http-equiv="Pragma" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
    $('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Expires" content="-1" />');

    var grid, statusIcons = {
        Registered: {
            text: "Registered",
            iconName: "complete"
        },
        Default: {
            iconName: "spinner"
        }
    };

    var sizeInfoList, vMId, vmName, vMSize, vMNewSize, subscriptionRegisteredToService, getSizeInfoListUrl = "/CmpWapExtensionTenant/ListVmSizes", vmOpUrl = "/CmpWapExtensionTenant/VmOp";

    //*************************************************************************
    // Initializes the view
    //*************************************************************************
    function loadTab(extension, renderArea, renderData) {
        if (renderData.AddressFromVm != null) {
            $("#configurevmsection").css("display", "block");
            $("#errorconfigure").css("display", "none");
            vMId = renderData.Id;
            vmName = renderData.name;
            vMSize = renderData.VmSize;
            subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("CmpWapExtension");
            fetchSizeInfoList(subscriptionRegisteredToService[0].id);
            clearContextualCommands();
        } else {
            $("#configurevmsection").css("display", "none");
            $("#errorconfigure").css("display", "block");
            $("#errorconfigure").text("VM is being provisioned, please wait or contact support team.");
        }
        //$("#size").prop("disabled", true);         // addsa- disabled resize functionality is enabled
    }

    //*************************************************************************
    // (Re)loads the virtual machine size tabs
    //*************************************************************************
    function refreshData() {
        //vMId = renderData.Id;
        //vmName = renderData.name;
        //vMSize = renderData.VmSize;
        subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("CmpWapExtension");
        fetchSizeInfoList(subscriptionRegisteredToService[0].id);
        clearContextualCommands();
        $("#sample-tablist").fxTabList({
            //Tabs to show
            values: [
                { text: "BASIC", value: "Basic" },
                { text: "STANDARD", value: "Standard" }
            ],
            //The corresponding "panels" that the tab values  map to.  This is matched based on ordering between values and panels
            panels: [$("#tab1"), $("#tab2")]
        });
    }

    //*************************************************************************
    // Sets and loads the virtual machine size data
    //*************************************************************************
    function gotTheSizeInfoList(data) {
        sizeInfoList = data;
        populateConfigureUiElements();
    }

    //*************************************************************************
    // Fetches the virtual machine size data
    //*************************************************************************
    function fetchSizeInfoList(subscriptionId) {
        $.post(getSizeInfoListUrl, { subscriptionIds: subscriptionId }).done(function (data) {
            gotTheSizeInfoList(data.data);
        });
    }

    //*************************************************************************
    // Loads the virtual machine size data into the view
    //*************************************************************************
    function populateConfigureUiElements() {
        var listItems = "";

        listItems = "";
        for (var i = 0; i < sizeInfoList.length; i++) {
            if (sizeInfoList[i].Name == vMSize) {
                listItems += "<option value='" + sizeInfoList[i].Name + "' selected='selected'>" + sizeInfoList[i].Description + "</option>";
            } else {
                listItems += "<option value='" + sizeInfoList[i].Name + "'>" + sizeInfoList[i].Description + "</option>";
            }
        }
        $("#size").html(listItems);

        $("#size").change(function () {
            vmSizeChanged();
        });
    }

    //*************************************************************************
    // Updates the application bar whenever a size changes
    //*************************************************************************
    function vmSizeChanged() {
        updateContextualCommands(true, true);
    }

    //*************************************************************************
    // Prompts user to resize a virtual machine
    //*************************************************************************
    function onVmReSize(item) {
        vMNewSize = $("#size").val();
        var resizeConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to resize the VM " + vmName + "?");

        // Note you could have multiple options – setActions takes an array
        resizeConfirmation.setActions([
            Shell.UI.Notifications.Buttons.yes(function () {
                performResize(item);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(resizeConfirmation);
    }

    //*************************************************************************
    // Discards any pending changes
    //*************************************************************************
    function onDiscard(item) {
        $("#size").val(vMSize);
        clearContextualCommands();
    }

    //*************************************************************************
    // Resizes a virtual machine
    //*************************************************************************
    function performResize(item) {
        var currentvmData = global.CmpWapExtensionTenantExtension.Controller.vmDahsboardData;
        var currentvmdatadiskcount = currentvmData.DataVirtualHardDisks.length - 1;
        var newskumaxdiskcount, newskucorecount;
        $.each(sizeInfoList, function (i, val) {
            if (val.Name == vMNewSize) {
                newskumaxdiskcount = val.MaxDataDiskCount;
                newskucorecount = val.Cores;
            }
        });
        var subscriptioncorecountlimit = parseInt(currentvmData.Subscription.MaximumCoreCount);
        var subscriptioncurrentcorecount = parseInt(currentvmData.Subscription.CurrentCoreCount);
        var currentvmcorecount = parseInt(currentvmData.Cores);
        var requestedcorecount = newskucorecount - currentvmData.Cores;

        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation("Resizing VM " + vmName + " ...", null, true);

        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);
        if (newskumaxdiskcount < currentvmdatadiskcount) {
            var additionaldisks = currentvmdatadiskcount - newskumaxdiskcount;
            var msg = "Too many data disks specified for virtual machine. The maximum number of data disks currently permitted is " + newskumaxdiskcount + ". The current number of data disks is " + currentvmdatadiskcount + ". The operation is attempting to add " + additionaldisks + " additional data disks.";
            commandError("Failed to configure virtual machine" + vmName + ".", progressOperation, msg);
        } else if (requestedcorecount > 0 && ((subscriptioncurrentcorecount + requestedcorecount) > subscriptioncorecountlimit)) {
            var msg = "The subscription policy limit for resource type 'cores count' was exceeded. The limit for resource type 'cores count' is " + subscriptioncorecountlimit + " per subscription, the current count is " + subscriptioncurrentcorecount + ", and the requested increment is " + requestedcorecount + ".";
            commandError("Failed to configure virtual machine" + vmName + ".", progressOperation, msg);
        } else {
            resizeCommandSend(item, progressOperation);
        }
        clearContextualCommands();
    }

    //*************************************************************************
    // Sends a resize command to the API
    //*************************************************************************
    function resizeCommandSend(item, progressOperation) {
        $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "RESIZE", VmId: vMId, sData: "", iData: 0, Vmsize: vMNewSize }).done(function (data) {
            commandComplete(data, progressOperation, "Successfully resized the VM " + vmName + ".");
        }).fail(function (jqXHR, textStatus, errorThrown) {
            var messageDetail = JSON.parse(jqXHR.responseText).message;
            commandError("Resizing the VM " + vmName + " Failed .", progressOperation, messageDetail);
        });
    }

    //*************************************************************************
    // Updates the UI to reflect the new virtual machine size
    //*************************************************************************
    function commandComplete(data, progressOperation, message) {
        progressOperation.complete(message, Shell.UI.InteractionSeverity.information);
        vMSize = vMNewSize;
    }

    //*************************************************************************
    // Updates the UI to indicate that an error occurred
    //*************************************************************************
    function commandError(message, progressOperation, messageDetail) {
        progressOperation.complete(message, Shell.UI.InteractionSeverity.error, Shell.UI.InteractionBehavior.ok, (message ? { detailData: messageDetail } : null));

        $("#size").val(vMSize);
    }

    //*************************************************************************
    // Clears out the UI
    //*************************************************************************
    function cleanUp() {
    }

    //*************************************************************************
    // Empties the application bar
    //*************************************************************************
    function clearContextualCommands() {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.update();
    }

    //*************************************************************************
    // Sets the availability of the application bar items
    //*************************************************************************
    function updateContextualCommands(enableSave, enableDiscard) {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Contextual.add(new Exp.UI.Command("save", "SAVE", Exp.UI.CommandIconDescriptor.getWellKnown("save"), enableSave, null, onVmReSize));
        Exp.UI.Commands.Contextual.add(new Exp.UI.Command("discard", "DISCARD", Exp.UI.CommandIconDescriptor.getWellKnown("discard"), enableDiscard, null, onDiscard));
        Exp.UI.Commands.update();
    }

    global.CmpWapExtensionTenantExtension = global.CmpWapExtensionTenantExtension || {};
    global.CmpWapExtensionTenantExtension.VMConfigureTab = {
        loadTab: loadTab,
        cleanUp: cleanUp,
        statusIcons: statusIcons
    };
})(jQuery, this);
