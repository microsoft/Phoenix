(function (global, undefined) {
    "use strict";

    var extensions = [{
        name: "CmpWapExtensionAdminExtension",
        displayName: "Azure VM Cloud",
        //iconUri: "/Content/CmpWapExtensionAdmin/TestTeam.png",
        iconUri: "/Content/CmpWapExtensionAdmin/VirtualMachines.png",
        iconShowCount: false,
        iconTextOffset: 11,
        iconInvertTextColor: true,
        displayOrderHint: 51
    }];

    global.Shell.Internal.ExtensionProviders.addLocal(extensions);
})(this);