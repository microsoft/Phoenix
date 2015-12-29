module PlanServices.Models {
    export class Setting implements ISetting{
        private _id: number;
        private _isSelected: boolean;
        private _name: string;
        
        get Id(): number {
            return this._id;
        }
        set Id(value: number) {
            this._id = value;
        }

        get IsSelected(): boolean {
            return this._isSelected;
        }
        set IsSelected(value: boolean) {
            this._isSelected = value;
        }

        get Name(): string {
            return this._name;
        }
        set Name(value: string) {
            this._name = value;
        }
    }
} 