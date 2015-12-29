var PlanServices;
(function (PlanServices) {
    var Models;
    (function (Models) {
        var Setting = (function () {
            function Setting() {
            }
            Object.defineProperty(Setting.prototype, "Id", {
                get: function () {
                    return this._id;
                },
                set: function (value) {
                    this._id = value;
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(Setting.prototype, "IsSelected", {
                get: function () {
                    return this._isSelected;
                },
                set: function (value) {
                    this._isSelected = value;
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(Setting.prototype, "Name", {
                get: function () {
                    return this._name;
                },
                set: function (value) {
                    this._name = value;
                },
                enumerable: true,
                configurable: true
            });
            return Setting;
        })();
        Models.Setting = Setting;
    })(Models = PlanServices.Models || (PlanServices.Models = {}));
})(PlanServices || (PlanServices = {}));
