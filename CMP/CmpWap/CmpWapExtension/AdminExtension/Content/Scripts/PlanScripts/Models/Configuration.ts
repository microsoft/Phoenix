module PlanServices.Models {
    import ISetting = Models.ISetting;
    
    export class Configuration {
        private _operatingSystems: Array<ISetting>;
        private _vmSizes: Array<ISetting>;
		private _azureRegions: Array<ISetting>;
        private _azureSubscriptionSettings: Array<ISetting>;

        get OperatingSystems(): Array<ISetting> {
            return this._operatingSystems;
        }

        set OperatingSystems(osSettings: Array<ISetting>) {
            this._operatingSystems = osSettings;
        }

        get VmSizes(): Array<ISetting> {
            return this._vmSizes;
        }

        set VmSizes(vmSizesSettings: Array<ISetting>) {
            this._vmSizes = vmSizesSettings;
        }

		get AzureRegions(): Array<ISetting> {
            return this._azureRegions;
        }

        set AzureRegions(vmSizesSettings: Array<ISetting>) {
            this._azureRegions = vmSizesSettings;
        }

        get AzureSubscriptions():Array<ISetting> {
            return this._azureSubscriptionSettings;
        }

        set AzureSubscriptions(azureSubscriptionSettings: Array<ISetting>) {
            this._azureSubscriptionSettings = azureSubscriptionSettings;
        }
    }
} 