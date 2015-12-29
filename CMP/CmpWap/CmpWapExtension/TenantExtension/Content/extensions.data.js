(function (global, undefined) {
    "use strict";

    $('head').append('<meta http-equiv="Pragma" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
    $('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Expires" content="-1" />');

    var extensions = [{
            name: "CmpWapExtensionTenantExtension",
            displayName: "Azure VMs",
            iconUri: "/Content/CmpWapExtensionTenant/CmpWapExtensionTenant.png",
            iconShowCount: false,
            iconTextOffset: 11,
            iconInvertTextColor: true,
            displayOrderHint: 2
        }];

    global.Shell.Internal.ExtensionProviders.addLocal(extensions);
})(this);
