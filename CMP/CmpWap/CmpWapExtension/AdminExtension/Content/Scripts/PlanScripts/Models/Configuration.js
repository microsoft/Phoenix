var PlanServices;
(function (PlanServices) {
    var Models;
    (function (Models) {
        var Configuration = (function () {
            function Configuration() {
            }
            Object.defineProperty(Configuration.prototype, "OperatingSystems", {
                get: function () {
                    return this._operatingSystems;
                },
                set: function (osSettings) {
                    this._operatingSystems = osSettings;
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(Configuration.prototype, "VmSizes", {
                get: function () {
                    return this._vmSizes;
                },
                set: function (vmSizesSettings) {
                    this._vmSizes = vmSizesSettings;
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(Configuration.prototype, "AzureSubscriptions", {
                get: function () {
                    return this._azureSubscriptionSettings;
                },
                set: function (azureSubscriptionSettings) {
                    this._azureSubscriptionSettings = azureSubscriptionSettings;
                },
                enumerable: true,
                configurable: true
            });
            return Configuration;
        })();
        Models.Configuration = Configuration;
    })(Models = PlanServices.Models || (PlanServices.Models = {}));
})(PlanServices || (PlanServices = {}));
