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
    var vmId = "";
    var subscriptionRegisteredToService;
    var vmName = "";
    var addressFromVm;
    var vmStatus;
    var attachedDisks = [];
    var detachedDisks = [];
    var curDisks = 0;
    var vmSize = "";
    var vmOpUrl = "/CmpWapExtensionTenant/VmOp", vmIpUrl = "/CmpWapExtensionTenant/OpenRDP";
    var ext, areaRender, dataRender, IsVmdeleted;
    function loadTab(extension, renderArea, renderData) {
        vmId = renderData.Id;
        vmName = renderData.name;
        addressFromVm = renderData.AddressFromVm;
        subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("CmpWapExtension");
        if (renderData.AddressFromVm != null || (IsVmdeleted != true && IsVmdeleted != undefined)) {
            var Columns = [
                { name: "DISK", field: "DiskName" },
                { name: "TYPE", field: "Type" },
                { name: "HOSTCACHE", field: "HostCaching" },
                { name: "VHD", field: "MediaLink" }
            ];
            ext = extension;
            areaRender = renderArea;
            dataRender = renderData;
            global.Shell.UI.Spinner.show();
            var promise = global.CmpWapExtensionTenantExtension.Controller.getVmdashboardData(renderData.Id);
            promise.done(function (value) {
                updateContextualCommands('');
                $(".vm-dashboard-usageAndLinked").css("display", "block");
                $(".vm-dashboard-attached-devices").css("display", "block");
                $("#errordashboard").css("display", "none");
                global.CmpWapExtensionTenantExtension.Controller.vmDahsboardData = value.data;
                vmStatus = value.data.Status;
                attachedDisks = [];
                $.each(value.data.DataVirtualHardDisks, function (index, disk) {
                    attachedDisks[attachedDisks.length] = disk.DiskName;
                });
                value.data.DataVirtualHardDisks.push(value.data.OSVirtualHardDisk);
                curDisks = value.data.DataVirtualHardDisks.length - 1;
                vmSize = value.data.RoleSize;
                updateContextualCommands(vmStatus);
                grid = renderArea.find(".gridContainer")
                    .wazObservableGrid("destroy")
                    .wazObservableGrid({
                    lastSelectedRow: null,
                    data: value.data.DataVirtualHardDisks,
                    columns: Columns,
                    emptyListOptions: {
                        extensionName: "CmpWapExtensionTenantExtension",
                        templateName: "FileSharesTabEmpty"
                    }
                });
                addressFromVm = value.data.InternalIP;
                _attachLayout(renderArea, value.data);
                _attachquickglanceLayout(renderArea, value.data);
            })
                .fail(function (val) {
                global.Shell.UI.Spinner.hide();
                // delete option to delete vm record
                Exp.UI.Commands.Contextual.add(new Exp.UI.Command("deleteVm", "Delete", Exp.UI.CommandIconDescriptor.getWellKnown("delete"), true, null, onDeleteOnException));
                Exp.UI.Commands.update();
            })
                .always(function () {
                global.Shell.UI.Spinner.hide();
            });
        }
        else {
            $(".vm-dashboard-usageAndLinked").css("display", "none");
            $(".vm-dashboard-attached-devices").css("display", "none");
            var $errlbl = $("#errordashboard");
            $errlbl.css("display", "block");
            if (renderData.StatusCode == "Exception") {
                $errlbl.text(renderData.StatusMessage);
                // delete option to delete vm record
                Exp.UI.Commands.Contextual.add(new Exp.UI.Command("deleteVm", "Delete", Exp.UI.CommandIconDescriptor.getWellKnown("delete"), true, null, onDeleteOnException));
                Exp.UI.Commands.update();
            }
            if (renderData.AddressFromVm == null && renderData.StatusCode != "Exception") {
                $errlbl.text("VM is being provisioned, please wait or contact support team.");
                // delete option to delete vm record
                Exp.UI.Commands.Contextual.add(new Exp.UI.Command("deleteVm", "Delete", Exp.UI.CommandIconDescriptor.getWellKnown("delete"), true, null, onDeleteOnException));
                Exp.UI.Commands.update();
            }
            else if (IsVmdeleted) {
                $errlbl.text(vmName + " VM is Deleted.");
            }
        }
    }
    //*************************************************************************
    // Adds the core usage diagram to the view
    //*************************************************************************
    function _attachLayout(container, data) {
        /// Helper function for addressing usage overview layout
        container = $(container);
        var newVMUsageQuotas = [];
        newVMUsageQuotas.push({
            name: data.RoleName,
            usedByThis: parseInt(data.Cores),
            usedByAll: parseInt(data.Subscription.CurrentCoreCount),
            totalQuota: parseInt(data.Subscription.MaximumCoreCount),
            unitLabelLong: "CORE(S)",
            unitLabel: "CORE(S)"
        });
        container.find(".vm-dashboard-usage")
            .autolayout("destroy")
            .fxUsageBars({
            thisEntityName: data.RoleName,
            otherEntityName: "OTHER ROLES",
            totalEntityName: "AVAILABLE",
            quotas: newVMUsageQuotas
        });
    }
    //*************************************************************************
    // Adds the Quick Glance tab to the view
    //*************************************************************************
    function _attachquickglanceLayout(renderArea, data) {
        $(".vm-dashboard-quickglance-info").css("display", "block");
        $("#vmstatus").text(data.Status);
        $("#vmreq-queue-status").text(data.QueueStatus);
        $("#dnsname").text(data.DNSName);
        $("#hostname").text(data.RoleName);
        $("#internalip").text(data.InternalIP);
        $("#vmsize").text(data.RoleSize);
        $("#rdpcert").text(data.RDPCertificateThumbprint);
        $("#deployid").text(data.DeploymentID);
        $("#subscriptionname").text(data.Subscription.SubscriptionName);
        $("#subscriptionId").text(data.Subscription.SubscriptionID);
    }
    //*************************************************************************
    // Cleans up the view
    //*************************************************************************
    function cleanUp() {
        if (grid) {
            grid.wazObservableGrid("destroy");
            grid = null;
        }
    }
    //*************************************************************************
    // Updates the application bar according the virtual machine state
    //*************************************************************************
    function updateContextualCommands(Status) {
        Status = Status.toLowerCase();
        switch (Status) {
            case 'running':
                setContextualCommands(true, true, false, true, true, false, false);
                break;
            case 'stopped / deallocated':
                setContextualCommands(false, false, true, false, true, false, false);
                break;
            case 'starting':
                setContextualCommands(false, false, false, false, true, false, false);
                break;
            case 'started':
                setContextualCommands(true, true, false, true, true, false, false);
                break;
            case 'creatingVM':
                setContextualCommands(false, false, false, false, true, false, false);
                break;
            case 'deallocated':
                setContextualCommands(false, false, true, false, true, false, false);
                break;
            case '':
                break;
            default:
                setContextualCommands(true, true, true, true, true, false, false);
                break;
        }
    }
    //*************************************************************************
    // Function:
    //*************************************************************************
    function UpdateByQueueStatus(queueStatus, vmStatus) {
        switch (queueStatus) {
            case 'NA':
                updateContextualCommands(vmStatus);
                break;
            case 'Exception':
                updateContextualCommands(vmStatus);
                break;
            case 'Complete':
                updateContextualCommands(vmStatus);
                break;
            case 'Submitted':
                updateContextualCommands(vmStatus);
            default:
                setContextualCommands(false, false, false, false, false, false, false);
                break;
        }
    }
    //*************************************************************************
    // Sets the application bar command states
    //*************************************************************************
    function setContextualCommands(enableConnect, enableRestart, enableStart, enableStop, enableDelete, enableAttach, enableDetach) {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Contextual.add(new Exp.UI.Command("showDnsManager", "Connect", Exp.UI.CommandIconDescriptor.getWellKnown("browse"), enableConnect, null, onConnect));
        Exp.UI.Commands.Contextual.add(new Exp.UI.Command("restartVm", "Restart", Exp.UI.CommandIconDescriptor.getWellKnown("reset"), enableRestart, null, onRestartVm));
        Exp.UI.Commands.Contextual.add(new Exp.UI.Command("startVm", "Start", Exp.UI.CommandIconDescriptor.getWellKnown("start"), enableStart, null, onStartVm));
        Exp.UI.Commands.Contextual.add(new Exp.UI.Command("stopVm", "Shutdown", Exp.UI.CommandIconDescriptor.getWellKnown("shutdown"), enableStop, null, onStopScenarios));
        Exp.UI.Commands.Contextual.add(new Exp.UI.Command("deleteVm", "Delete", Exp.UI.CommandIconDescriptor.getWellKnown("delete"), enableDelete, null, onDeleteScenarios));
        //Exp.UI.Commands.Contextual.add(new Exp.UI.Command("attachdisk", "attach", Exp.UI.CommandIconDescriptor.getWellKnown("attachdisk"), enableAttach, null, onAttachdisk));
        //Exp.UI.Commands.Contextual.add(new Exp.UI.Command("detachdisk", "detach", Exp.UI.CommandIconDescriptor.getWellKnown("detachdisk"), enableDetach, null, onDetachScenarios));
        Exp.UI.Commands.update();
    }
    //*************************************************************************
    // Attempts to connect to the machine
    //*************************************************************************
    function onConnect() {
        if (addressFromVm) {
            var connectConfirmation = new Shell.UI.Notifications.Confirmation("The portal is retrieving the .rdp file. You will receive a prompt to open or save the file shortly.");
            connectConfirmation.setActions([Shell.UI.Notifications.Buttons.yes()]);
            Shell.UI.Notifications.add(connectConfirmation);
            window.location.href = window.location.protocol + "//" + window.location.host + "/" + vmIpUrl + "?vmIp=" + addressFromVm;
        }
        else {
            Shell.UI.Notifications.add("Something wrong with the VM creation. No IP address assigned.", Shell.UI.InteractionSeverity.error);
        }
    }
    //*************************************************************************
    // Updates the UI when the process has completed
    //*************************************************************************
    function commandComplete(data, progressOperation, message) {
        if (data.Opcode == "DELETE" || data.Opcode == "DELETEFROMSTORAGE" || data.Opcode == "DELETEONEXCEPTION") {
            global.CmpWapExtensionTenantExtension.Controller.loadMainPage();
            data = data.data;
        }
        else {
            if (vmName == data.value.data.Name.split('.')[0]) {
                loadTab(ext, areaRender, dataRender);
            }
        }
        progressOperation.complete(message, Shell.UI.InteractionSeverity.information);
    }
    //*************************************************************************
    // Updates the UI when an error has occurred
    //*************************************************************************
    function commandError(message, progressOperation, messageDetail) {
        progressOperation.complete(message, Shell.UI.InteractionSeverity.error, Shell.UI.InteractionBehavior.ok, (message ? { detailData: messageDetail } : null));
    }
    //*************************************************************************
    // Sends a restart request to the API
    //*************************************************************************
    function restartCommandSend(item, progressOperation, targetVMName) {
        $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "RESTART", VmId: vmId, sData: "", iData: 0 })
            .done(function (data) {
            refreshVMData(vmId, "RESTART", data, progressOperation);
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
            var messageDetail = JSON.parse(jqXHR.responseText).message;
            commandError("Restarting the VM " + targetVMName + " Failed.", progressOperation, messageDetail);
            updateContextualCommands(vmStatus);
        });
    }
    function deleteVMOnExceptionCommandSend(item, progressOperation, targetVMName) {
        $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "DELETEONEXCEPTION", VmId: vmId, sData: "", iData: 0 })
            .done(function (data) {
            IsVmdeleted = true;
            var response = {
                Opcode: "DELETEONEXCEPTION",
                data: data
            };
            commandComplete(response, progressOperation, "Successfully deleted record of VM " + targetVMName);
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
            var messageDetail = JSON.parse(jqXHR.responseText).message;
            commandError("Deleting entry for the VM " + targetVMName + " Failed.", progressOperation, messageDetail);
            IsVmdeleted = false;
            setContextualCommands(false, false, false, false, false, false, false);
        });
    }
    //*************************************************************************
    // Sends a request to delete the machine and associated disks to the API
    //*************************************************************************
    function deleteVMWithDiskCommandSend(item, progressOperation, targetVMName) {
        $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "DELETEFROMSTORAGE", VmId: vmId, sData: "", iData: 0 })
            .done(function (data) {
            IsVmdeleted = true;
            refreshVMData(vmId, "DELETEFROMSTORAGE", data, progressOperation);
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
            var messageDetail = JSON.parse(jqXHR.responseText).message;
            commandError("Deleting the VM " + targetVMName + " Failed.", progressOperation, messageDetail);
            IsVmdeleted = false;
            updateContextualCommands(vmStatus);
        });
    }
    //*************************************************************************
    // Sends a request to delete the machine to the API
    //*************************************************************************
    function deleteVMWithoutDiskCommandSend(item, progressOperation, targetVMName) {
        $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "DELETE", VmId: vmId, sData: "", iData: 0 })
            .done(function (data) {
            IsVmdeleted = true;
            refreshVMData(vmId, "DELETE", data, progressOperation);
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
            var messageDetail = JSON.parse(jqXHR.responseText).message;
            commandError("Deleting the VM " + targetVMName + " Failed.", progressOperation, messageDetail);
            IsVmdeleted = false;
            updateContextualCommands(vmStatus);
        });
    }
    //*************************************************************************
    // Sends a command to start the machine to the UI
    //*************************************************************************
    function startCommandSend(item, progressOperation, targetVMName) {
        $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "START", VmId: vmId, sData: "", iData: 0 })
            .done(function (data) {
            refreshVMData(vmId, "START", data, progressOperation);
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
            var messageDetail = JSON.parse(jqXHR.responseText).message;
            commandError("Starting the VM " + targetVMName + " Failed.", progressOperation, messageDetail);
            updateContextualCommands(vmStatus);
        });
    }
    //*************************************************************************
    // Sends a command to deallocate a machine to the API
    //*************************************************************************
    function deallocateCommandSend(item, progressOperation, targetVMName) {
        $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "DEALLOCATE", VmId: vmId, sData: "", iData: 0 })
            .done(function (data) {
            refreshVMData(vmId, "DEALLOCATE", data, progressOperation);
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
            var messageDetail = JSON.parse(jqXHR.responseText).message;
            commandError("Deallocating the VM " + targetVMName + " Failed.", progressOperation, messageDetail);
            updateContextualCommands(vmStatus);
        });
    }
    //*************************************************************************
    // Sends a command to stop a machine to the API
    //*************************************************************************
    function stopCommandSend(item, progressOperation, targetVMName) {
        $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "STOP", VmId: vmId, sData: "", iData: 0 })
            .done(function (data) {
            refreshVMData(vmId, "STOP", data, progressOperation);
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
            var messageDetail = JSON.parse(jqXHR.responseText).message;
            commandError("Stopping the VM " + targetVMName + " Failed.", progressOperation, messageDetail);
            updateContextualCommands(vmStatus);
        });
    }
    //*************************************************************************
    // Restarts the machine and updates the UI
    //*************************************************************************
    function performRestart(item, targetVMName) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation(
        // Title of operation
        "Restarting VM " + targetVMName + " ...", 
        // Initial status. null = default
        null, 
        // Is indeterministic? (Does it NOT provide a % complete)
        true);
        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);
        setContextualCommands(true, false, false, false, true, false, false);
        restartCommandSend(item, progressOperation, targetVMName);
    }
    //*************************************************************************
    // Starts the machine and updates the UI
    //*************************************************************************
    function onStartVm(item) {
        // store the current vmName as tab may change between notifications
        var targetVMName = vmName;
        var startConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to Start the VM " + targetVMName + "?");
        // Note you could have multiple options – setActions takes an array
        startConfirmation.setActions([Shell.UI.Notifications.Buttons.yes(function () {
                performStart(item, targetVMName);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(startConfirmation);
    }
    //*************************************************************************
    // Starts the machine and updates the UI
    //*************************************************************************
    function performStart(item, targetVMName) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation(
        // Title of operation
        "Starting VM " + targetVMName + " ...", 
        // Initial status. null = default
        null, 
        // Is indeterministic? (Does it NOT provide a % complete)
        true);
        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);
        setContextualCommands(false, false, false, false, true, false, false);
        startCommandSend(item, progressOperation, targetVMName);
    }
    //*************************************************************************
    // Function: Stop VM Scenarios
    //*************************************************************************
    var onStopScenarios = function (item) {
        // store the current vmName as tab may change between notifications
        var targetVMName = vmName;
        cdm.stepWizard({
            extension: "CmpWapExtensionTenantExtension",
            steps: [
                {
                    template: "ShutDownVMDialogBox",
                    onStepCreated: function () {
                        //wizard = this;
                        var optiondesc = {
                            stopvm: "In this option, the OS in the VM is stopped and the VM services are unavailable, " +
                                "but the VM continues to reserve the compute and network resources that Azure provisioned for it. " +
                                "Azure continues to charge for the VM core hours while it’s Stopped, based on the size of the VM and " +
                                "the image you selected to create it. You continue to accrue charges for the VM’s cloud service and the " +
                                "storage needed for the VM’s OS disk and any attached data disks. Temporary (scratch) disk storage on the VM is free. " +
                                "The static internal IP assigned to the VM is preserved.",
                            deallocvm: "In this option, you not only stop the VM’s OS, you also free up the hardware and network resources Azure previously provisioned for it. " +
                                "Azure doesn’t charge for the VM core hours while it’s Stopped (Deallocated). You continue to accrue charges for the Azure " +
                                "storage needed for the VM’s OS disk and any attached data disks. The static internal IP assigned to the VM will still be preserved."
                        };
                        $("#vmstop-option-desc").text(optiondesc.stopvm);
                        var types = [{ text: "Stop VM", value: "stopVM" },
                            { text: "Deallocate VM", value: "deallocateVM" }];
                        $("#vm-stop-radio").fxRadio({
                            value: types[0],
                            values: types,
                            change: function (event, args) {
                                if (args.value.value == "stopVM") {
                                    $("#vmstop-option-desc").text(optiondesc.stopvm);
                                }
                                else if (args.value.value == "deallocateVM") {
                                    $("#vmstop-option-desc").text(optiondesc.deallocvm);
                                }
                            }
                        });
                    },
                    onComplete: function () {
                        //helper to get the selected value
                        var getSelectedValue = function () {
                            return $("#vm-stop-radio").fxRadio("value");
                        };
                        switch (getSelectedValue().value) {
                            case "stopVM":
                                onStopVm(item, targetVMName);
                                break;
                            case "deallocateVM":
                                onDeallocateVm(item, targetVMName);
                                break;
                        }
                    }
                }
            ]
        }, { size: "small" });
    };
    //*************************************************************************
    // Deallocates the machine
    //*************************************************************************
    var onDeallocateVm = function (item, targetVMName) {
        var stopConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to deallocate the VM " + targetVMName + "?");
        // Note you could have multiple options – setActions takes an array
        stopConfirmation.setActions([Shell.UI.Notifications.Buttons.yes(function () {
                performDeallocate(item, targetVMName);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(stopConfirmation);
    };
    //*************************************************************************
    // Stops the virtual machine
    //*************************************************************************
    var onStopVm = function (item, targetVMName) {
        var stopConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to Stop the VM " + targetVMName + "?");
        // Note you could have multiple options – setActions takes an array
        stopConfirmation.setActions([Shell.UI.Notifications.Buttons.yes(function () {
                performStop(item, targetVMName);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(stopConfirmation);
    };
    //*************************************************************************
    // Deallocates the machine and updates the UI
    //*************************************************************************
    var performDeallocate = function (item, targetVMName) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation(
        // Title of operation
        "Deallocating VM " + targetVMName + " ...", 
        // Initial status. null = default
        null, 
        // Is indeterministic? (Does it NOT provide a % complete)
        true);
        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);
        setContextualCommands(false, false, false, false, true, false, false);
        deallocateCommandSend(item, progressOperation, targetVMName);
    };
    //*************************************************************************
    // Stops the virtual machine and updates the UI
    //*************************************************************************
    var performStop = function (item, targetVMName) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation(
        // Title of operation
        "Stopping VM " + targetVMName + " ...", 
        // Initial status. null = default
        null, 
        // Is indeterministic? (Does it NOT provide a % complete)
        true);
        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);
        setContextualCommands(false, false, false, false, true, false, false);
        stopCommandSend(item, progressOperation, targetVMName);
    };
    function onDeleteOnException(item) {
        var deleteConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to delete entry for the VM " + vmName + "?");
        // Note you could have multiple options – setActions takes an array
        deleteConfirmation.setActions([Shell.UI.Notifications.Buttons.yes(function () {
                //global.CmpWapExtensionTenantExtension.FileSharesTab.loadTab(global.CmpWapExtensionTenantExtension.Controller.mainDashboardrenderData, global.CmpWapExtensionTenantExtension.Controller.mainDashboardrenderArea);
                // Create a new Progress Operation object
                var progressOperation = new Shell.UI.ProgressOperation(
                // Title of operation
                "Deleting entry for VM " + vmName + " ...", 
                // Initial status. null = default
                null, 
                // Is indeterministic? (Does it NOT provide a % complete)
                true);
                // This adds the progress operation we set up earlier to the visible list of PrOp's
                Shell.UI.ProgressOperations.add(progressOperation);
                setContextualCommands(false, false, false, false, false, false, false);
                deleteVMOnExceptionCommandSend(item, progressOperation, vmName);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(deleteConfirmation);
    }
    //*************************************************************************
    // Function: Delete VM Scenarios
    //*************************************************************************
    function onDeleteScenarios(item) {
        // store the current vmName as tab may change between notifications
        var targetVMName = vmName;
        cdm.stepWizard({
            extension: "CmpWapExtensionTenantExtension",
            steps: [
                {
                    template: "DeleteVMDialogBox",
                    onStepCreated: function () {
                        //wizard = this;
                        // options description
                        var optiondesc = {
                            deletedisk: "This option deletes the disk attached to the VM." //,
                        };
                        $("#vmdelete-option-desc").text(optiondesc.deletedisk); //tie description to textbox
                        var types = [{ text: "Delete VM with disk", value: "deleteVManddisk" } //,
                        ];
                        $("#vm-delete-radio").fxRadio({
                            value: types[0],
                            values: types,
                            change: function (event, args) {
                                if (args.value.value == "deleteVManddisk") {
                                    $("#vmdelete-option-desc").text(optiondesc.deletedisk);
                                }
                                //else if (args.value.value == "deallocateVMnodisk") {
                                //	$("#vmdelete-option-desc").text(optiondesc.keepdisk);
                                //}
                            }
                        });
                    },
                    onComplete: function () {
                        //helper to get the selected value
                        var getSelectedValue = function () {
                            return $("#vm-delete-radio").fxRadio("value");
                        };
                        switch (getSelectedValue().value) {
                            case "deleteVManddisk":
                                onDeleteVMkWithDisk(item, targetVMName);
                                break;
                        }
                    }
                }
            ]
        }, { size: "small" });
    }
    //*************************************************************************
    // Deletes the machine and its disks and updates the UI
    //*************************************************************************
    function onDeleteVMkWithDisk(item, targetVMName) {
        var deleteConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to delete the VM " + targetVMName + " with disks ?");
        // Note you could have multiple options – setActions takes an array
        deleteConfirmation.setActions([Shell.UI.Notifications.Buttons.yes(function () {
                performDeleteVMWithDisk(item, targetVMName);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(deleteConfirmation);
    }
    //*************************************************************************
    // Deletes the machine, keeps the disks and updates the UI
    //*************************************************************************
    function onDeleteVMWithoutDisk(item, targetVMName) {
        var deleteConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to delete the VM without " + targetVMName + " disks ?");
        // Note you could have multiple options – setActions takes an array
        deleteConfirmation.setActions([Shell.UI.Notifications.Buttons.yes(function () {
                performDeleteVMWithoutDisk(item, targetVMName);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(deleteConfirmation);
    }
    //*************************************************************************
    // Deletes the machine and its disks and updates the UI
    //*************************************************************************
    function performDeleteVMWithDisk(item, targetVMName) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation(
        // Title of operation
        "Deleting VM " + targetVMName + " ...", 
        // Initial status. null = default
        null, 
        // Is indeterministic? (Does it NOT provide a % complete)
        true);
        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);
        setContextualCommands(false, false, false, false, false, false, false);
        deleteVMWithDiskCommandSend(item, progressOperation, targetVMName);
    }
    //*************************************************************************
    // Deletes the machine, keeps the disks and updates the UI
    //*************************************************************************
    function performDeleteVMWithoutDisk(item, targetVMName) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation(
        // Title of operation
        "Deleting VM " + targetVMName + " ...", 
        // Initial status. null = default
        null, 
        // Is indeterministic? (Does it NOT provide a % complete)
        true);
        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);
        setContextualCommands(false, false, false, false, false, false, false);
        deleteVMWithoutDiskCommandSend(item, progressOperation, targetVMName);
    }
    //*************************************************************************
    // Function: Detach Disk Scenarios
    //*************************************************************************
    function onDetachScenarios(item) {
        // store the current vmName as tab may change between notifications
        var targetVMName = vmName;
        cdm.stepWizard({
            extension: "CmpWapExtensionTenantExtension",
            steps: [
                {
                    template: "DetachDisk",
                    onStepCreated: function () {
                        var select = $("#disk-detach-select");
                        if (select.prop) {
                            var options = select.prop("options");
                        }
                        else {
                            var options = select.attr("options");
                        }
                        $("option", select).remove();
                        $.each(attachedDisks, function (index, disk) {
                            options[options.length] = new Option(disk, disk);
                        });
                        select.val(options);
                        var optiondesc = {
                            deletedisk: "This option deletes the disk after being detached from the virtual machine.",
                            keepdisk: "This option keeps the disk in a storage account after being detached from the virtual machine."
                        };
                        $("#diskdetach-option-desc").text(optiondesc.deletedisk); //tie description to textbox
                        var types = [{ text: "Delete disk", value: "detachanddeletedisk" },
                            { text: "Keep disk", value: "detachdisk" }];
                        $("#disk-detach-radio").fxRadio({
                            value: types[0],
                            values: types,
                            change: function (event, args) {
                                if (args.value.value == "detachanddeletedisk") {
                                    $("#diskdetach-option-desc").text(optiondesc.deletedisk);
                                }
                                else if (args.value.value == "detachdisk") {
                                    $("#diskdetach-option-desc").text(optiondesc.keepdisk);
                                }
                            }
                        });
                    },
                    onComplete: function () {
                        //helper to get the selected value
                        var getSelectedValue = function () {
                            return $("#disk-detach-radio").fxRadio("value");
                        };
                        var diskName = $("#disk-detach-select").val();
                        switch (getSelectedValue().value) {
                            case "detachanddeletedisk":
                                onDetachAndDeleteDisk(item, diskName, targetVMName);
                                break;
                            case "detachdisk":
                                onDetachDisk(item, diskName, targetVMName);
                                break;
                        }
                    }
                }
            ]
        }, { size: "small" });
    }
    //*************************************************************************
    // Deletes the disk and updates the UI
    //*************************************************************************
    function onDetachAndDeleteDisk(item, diskName, targetVMName) {
        var deleteConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to detach and delete this disk from " + targetVMName + "?");
        // Note you could have multiple options – setActions takes an array
        deleteConfirmation.setActions([Shell.UI.Notifications.Buttons.yes(function () {
                performDetachAndDeleteDisk(item, diskName, targetVMName);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(deleteConfirmation);
    }
    //*************************************************************************
    // Detaches the disk and updates the UI
    //*************************************************************************
    function onDetachDisk(item, diskName, targetVMName) {
        var deleteConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to detach this disk from " + targetVMName + "?");
        // Note you could have multiple options – setActions takes an array
        deleteConfirmation.setActions([Shell.UI.Notifications.Buttons.yes(function () {
                performDetachDisk(item, diskName, targetVMName);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(deleteConfirmation);
    }
    //*************************************************************************
    // Deletes the disk and updates the UI
    //*************************************************************************
    function performDetachAndDeleteDisk(item, diskName, targetVMName) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation(
        // Title of operation
        "Detaching and deleting disk from " + targetVMName + "...", 
        // Initial status. null = default
        null, 
        // Is indeterministic? (Does it NOT provide a % complete)
        true);
        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);
        detachAndDeleteDiskCommandSend(item, diskName, progressOperation, targetVMName);
    }
    //*************************************************************************
    // Detaches the disk and updates the UI
    //*************************************************************************
    function performDetachDisk(item, diskName, targetVMName) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation(
        // Title of operation
        "Detaching disk from " + targetVMName + "...", 
        // Initial status. null = default
        null, 
        // Is indeterministic? (Does it NOT provide a % complete)
        true);
        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);
        detachDiskCommandSend(item, diskName, progressOperation, targetVMName);
    }
    //*************************************************************************
    // Sends a delete disk request to the API
    //*************************************************************************
    function detachAndDeleteDiskCommandSend(item, diskName, progressOperation, targetVMName) {
        var disksList = JSON.stringify([{ DiskName: diskName }]);
        $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "DETACHANDDELETE", VmId: vmId, disks: disksList, sData: "", iData: 0 })
            .done(function (data) {
            refreshVMData(vmId, "DETACHANDDELETE", data, progressOperation);
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
            var messageDetail = JSON.parse(jqXHR.responseText).message;
            commandError("Detaching and deleting the disk from " + targetVMName + " failed.", progressOperation, messageDetail);
            updateContextualCommands(vmStatus);
        });
    }
    //*************************************************************************
    // Sends a detach disk request to the UI
    //*************************************************************************
    function detachDiskCommandSend(item, diskName, progressOperation, targetVMName) {
        var disksList = JSON.stringify([{ DiskName: diskName }]);
        $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "DETACH", VmId: vmId, disks: disksList, sData: "", iData: 0 })
            .done(function (data) {
            refreshVMData(vmId, "DETACH", data, progressOperation);
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
            var messageDetail = JSON.parse(jqXHR.responseText).message;
            commandError("Detaching the disk from " + targetVMName + " failed.", progressOperation, messageDetail);
            updateContextualCommands(vmStatus);
        });
    }
    //*************************************************************************
    // Restarts the machine and updates the UI
    //*************************************************************************
    function onRestartVm(item) {
        // store the current vmName as tab may change between notifications
        var targetVMName = vmName;
        var restartConfirmation = new Shell.UI.Notifications.Confirmation("Are you sure you want to restart the VM " + targetVMName + "?");
        // Note you could have multiple options – setActions takes an array
        restartConfirmation.setActions([Shell.UI.Notifications.Buttons.yes(function () {
                performRestart(item, targetVMName);
            }), Shell.UI.Notifications.Buttons.no(function () {
            })]);
        Shell.UI.Notifications.add(restartConfirmation);
    }
    //*************************************************************************
    // Presents a UI to attach a disk
    //*************************************************************************
    function onAttachdisk(item) {
        // store the current vmName as tab may change between notifications
        var targetVMName = vmName;
        var diskvals = []; //disk values set got from the api
        cdm.stepWizard({
            extension: "CmpWapExtensionTenantExtension",
            steps: [
                {
                    template: "AttachDisk",
                    onStepCreated: function () {
                        //wizard = this;
                        //option description
                        //TODO description part
                        var optiondesc = {
                            newDisk: "In this option, a new disk (storage) is attached to the virtual machine. The user is required to format the disk seperately, as in the Azure Portal.",
                            existingDisk: "Add an existing detached disk from the subscription." //,
                        };
                        // ***Populate UI elements***
                        var promise = global.CmpWapExtensionTenantExtension.Controller.getVmdashboardData(vmId);
                        $("#vmdisk-option-desc").text(optiondesc.newDisk);
                        // UI configuration. Initial display
                        $("#vm-attachdisk-new").show();
                        $("#vm-attachdisk-existing").hide();
                        $("#vm-growdisk-existing").hide();
                        //types of scenarios
                        var types = [
                            { text: "Attach New disk", value: "newdisk" },
                            { text: "Attach Existing disk", value: "existingdisk" }
                        ];
                        // { text: "Grow Existing disk", value: "growdisk" }];
                        $("#vm-attachdisk-radio").fxRadio({
                            value: types[0],
                            values: types,
                            change: function (event, args) {
                                if (args.value.value == "newdisk") {
                                    $("#vm-attachdisk-new").show();
                                    $("#vmdisk-option-desc").text(optiondesc.newDisk);
                                    $("#vm-attachdisk-existing").hide();
                                    $("#vm-growdisk-existing").hide();
                                }
                                else if (args.value.value == "existingdisk") {
                                    $("#vm-attachdisk-new").hide();
                                    $("#vm-attachdisk-existing").show();
                                    $("#vm-growdisk-existing").hide();
                                    $("#vmdisk-option-desc").text(optiondesc.existingDisk);
                                    var select = $("#attachexistingdisk");
                                    if (select.prop) {
                                        var options = select.prop("options");
                                    }
                                    else {
                                        var options = select.attr("options");
                                    }
                                    $("option", select).remove();
                                    $.each(detachedDisks, function (index, disk) {
                                        options[options.length] = new Option(disk, disk);
                                    });
                                    select.val(options);
                                }
                                else if (args.value.value == "growdisk") {
                                    var listItems = "";
                                    promise.done(function (value) {
                                        value.data.DataVirtualHardDisks.forEach(function (disk) {
                                            listItems += "<option value='" + disk.DiskName + "' selected='selected'>" + disk.DiskLabel + "</option>";
                                            diskvals.push(disk); // push disk values in
                                        });
                                        $("#growexistingdisk").html(listItems);
                                        $("#vm-attachdisk-new").hide();
                                        $("#vm-attachdisk-existing").hide();
                                        $("#vm-growdisk-existing").show();
                                        //$("#vmdisk-option-desc").text(optiondesc.growDisk);
                                        $('#growexistingdisk option[selected="selected"]').removeAttr('selected');
                                        // mark the first option as selected
                                        $("#growexistingdisk option:first").attr('selected', 'selected');
                                        // initial dropdown slider mapping
                                        invokeSlider($("#slider-growdisk"), parseInt(value.data.DataVirtualHardDisks[0].LogicalDiskSizeInGB), parseInt(value.data.DataVirtualHardDisks[0].LogicalDiskSizeInGB), parseInt(value.data.DataVirtualHardDisks[0].LogicalDiskSizeInGB));
                                    });
                                }
                            }
                        });
                    },
                    onComplete: function () {
                        //helper to get the selected value
                        var getSelectedValue = function () {
                            return $("#vm-attachdisk-radio").fxRadio("value");
                        };
                        switch (getSelectedValue().value) {
                            case "newdisk":
                                onAttachDisk(item, targetVMName);
                                break;
                            case "existingdisk":
                                onAttachExistingDisk(item, targetVMName);
                                break;
                            case "growdisk":
                                //onGrowDisk(targetVMName);
                                break;
                        }
                    }
                }
            ]
        }, { size: "mediumplus" });
        // new disk slider 
        $("#VmServrName").val(targetVMName);
        invokeSlider($("#slider-newdisk"), 5, 1, 1);
        // grow disk slider on change
        $("#growexistingdisk").change(function () {
            for (var i = 0; i < diskvals.length; i++) {
                if ($("select option:selected").text() == diskvals[i].DiskLabel) {
                    invokeSlider($("#slider-growdisk"), parseInt(diskvals[i].LogicalDiskSizeInGB), parseInt(diskvals[i].LogicalDiskSizeInGB), parseInt(diskvals[i].LogicalDiskSizeInGB));
                    break;
                }
            }
        });
    }
    //*************************************************************************
    // invoke a new slider based on the DOM element.
    //*************************************************************************
    function invokeSlider(slider, val, min, slidmin) {
        // destroying slider and cerating a new one as existing slider modification is causing UI issues
        slider.fxSlider("destroy");
        slider.fxSlider({
            value: val,
            min: min,
            max: 1024,
            slidableMin: slidmin,
            slidableMax: 1024,
            change: function (event, args) {
                $("#sliderCurrentValueOnChangedSpan").text(args.value);
            }
        });
    }
    //*************************************************************************
    // Attaches a new disk and updates the UI
    //*************************************************************************
    function onAttachDisk(item, targetVMName) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation(
        // Title of operation
        "Attaching disk to " + targetVMName + "...", 
        // Initial status. null = default
        null, 
        // Is indeterministic? (Does it NOT provide a % complete)
        true);
        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);
        AttachDisk(item, progressOperation, targetVMName);
    }
    //*************************************************************************
    // Sends a request to attach a new disk to the API
    //*************************************************************************
    function AttachDisk(item, progressOperation, targetVMName) {
        var diskSize = $("#slider-newdisk").fxSlider("value");
        var maxDisks = 0;
        diskSize = parseInt(diskSize);
        var vmOpUrl = "/CmpWapExtensionTenant/VmOp";
        var lstdisks = JSON.stringify([{ HostCaching: "None", LogicalDiskSizeInGB: diskSize }]);
        //Verify number of disks and allowed
        $.each(sizeInfoList, function (i, value) {
            if (value.Name == vmSize) {
                maxDisks = value.MaxDataDiskCount;
                return;
            }
        });
        if (maxDisks < (curDisks + 1)) {
            var msg = "Attempted to attach too many disks to the virtual machine. The maximum number of data disks currently permitted is " + maxDisks + ". The current number of data disks is " + curDisks + ". The operation is attempting to add 1 additional data disk.";
            commandError("Attaching the disk to " + targetVMName + " failed.", progressOperation, msg);
        }
        else {
            $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "ADDISK", VmId: vmId, sData: "", iData: 0, disks: lstdisks })
                .done(function (data) {
                refreshVMData(vmId, "ADDISK", data, progressOperation);
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                var messageDetail = JSON.parse(jqXHR.responseText).message;
                commandError("Attaching the disk to " + targetVMName + " failed.", progressOperation, messageDetail);
                updateContextualCommands(vmStatus);
            });
        }
    }
    //*************************************************************************
    // Attaches an existing disk and updates the UI
    //*************************************************************************
    function onAttachExistingDisk(item, targetVMName) {
        // Create a new Progress Operation object
        var progressOperation = new Shell.UI.ProgressOperation(
        // Title of operation
        "Attaching disk to " + targetVMName + "...", 
        // Initial status. null = default
        null, 
        // Is indeterministic? (Does it NOT provide a % complete)
        true);
        // This adds the progress operation we set up earlier to the visible list of PrOp's
        Shell.UI.ProgressOperations.add(progressOperation);
        attachExistingDisk(item, progressOperation, targetVMName);
    }
    //*************************************************************************
    // Sends a request to attach an existing disk to the UI
    //*************************************************************************
    function attachExistingDisk(item, progressOperation, targetVMName) {
        //Verify number of disks and allowed
        var maxDisks = 0;
        $.each(sizeInfoList, function (i, value) {
            if (value.Name == vmSize) {
                maxDisks = value.MaxDataDiskCount;
                return;
            }
        });
        if (maxDisks < (curDisks + 1)) {
            var msg = "Attempted to attach too many disks to the virtual machine. The maximum number of data disks currently permitted is " + maxDisks + ". The current number of data disks is " + curDisks + ". The operation is attempting to add 1 additional data disk.";
            commandError("Attach the disk to " + targetVMName + " failed.", progressOperation, msg);
        }
        else {
            var diskName = $("#attachexistingdisk").val();
            var disksList = JSON.stringify([{ DiskName: diskName }]);
            $.post(vmOpUrl, { subscriptionId: subscriptionRegisteredToService[0].id, Opcode: "ATTACHEXISTING", VmId: vmId, disks: disksList, sData: "", iData: 0 })
                .done(function (data) {
                refreshVMData(vmId, "ATTACHEXISTING", data, progressOperation);
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                var messageDetail = JSON.parse(jqXHR.responseText).message;
                commandError("Attaching the disk to " + targetVMName + " failed.", progressOperation, messageDetail);
                updateContextualCommands(vmStatus);
            });
        }
    }
    //*************************************************************************
    // Refreshes the data in the main grid
    //*************************************************************************
    function forceRefreshGridData() {
        try {
            // When we navigate to the tab, sometimes this method is called before observableGrid is not intialized, which will throw exception.
            grid.wazObservableGrid("refreshData");
        }
        catch (e) {
        }
    }
    // This block of code performs Ajax long polling and refreshes after every 10 seconds and fetches the vm dashboard data
    function refreshVMData(vmId, opType, data, progressOperation) {
        var queueData;
        setContextualCommands(false, false, false, false, false, false, false);
        (function invoke() {
            var promise = global.CmpWapExtensionTenantExtension.Controller.getVmOpsQueueTask(vmId);
            promise.done(function (value) {
                if (queueData != value.data.StatusCode) {
                    queueData = value.data.StatusCode;
                    $("#vmreq-queue-status").text(value.data.StatusCode);
                }
                if (queueData == "Complete" || queueData == "Exception") {
                    if (queueData == "Complete") {
                        var response = {
                            Opcode: opType,
                            data: data,
                            value: value
                        };
                        commandComplete(response, progressOperation, "Successfully performed operation " + opType + " on the VM " + value.data.Name);
                    }
                    else {
                        commandError("Failed to perform operation " + opType + " on the VM " + value.data.Name, progressOperation, value.data.StatusMessage);
                        updateContextualCommands(vmStatus);
                    }
                }
                else {
                    setTimeout(invoke, 20000);
                    return;
                }
            }).
                fail(function (jqXHR, textStatus, errorThrown) {
                commandError("Failed to perform operation " + opType + " on the VM ", progressOperation, errorThrown);
                updateContextualCommands(vmStatus);
            });
        })();
    }
    global.CmpWapExtensionTenantExtension = global.CmpWapExtensionTenantExtension || {};
    global.CmpWapExtensionTenantExtension.VMDashboardTab = {
        loadTab: loadTab,
        cleanUp: cleanUp,
        forceRefreshGridData: forceRefreshGridData,
        statusIcons: statusIcons
    };
})(jQuery, this);
