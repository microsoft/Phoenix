(function($, global, undefined) {
    "use strict";

function VmDashboardTabOpened(extension, renderArea, renderData) {
    global.CmpWapExtensionTenantExtension.VMDashboardTab.VmsDashboardTabLoaded(extension, renderArea, renderData);
}

//function VmConfigureTabOpened(renderArea)
//{
//    global.CmpWapExtensionTenantExtension.VmsConfigureTab.VmsConfigureTabOpened("");
//}

global.CmpWapExtensionTenantExtension = global.CmpWapExtensionTenantExtension || {};
global.CmpWapExtensionTenantExtension.Navigation = {
    VmDashboardTabOpened: VmDashboardTabOpened
};
})(jQuery, this);