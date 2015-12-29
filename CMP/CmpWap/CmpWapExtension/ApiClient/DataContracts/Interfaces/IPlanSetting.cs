namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts.Interfaces
{
    public interface IPlanSetting
    {
        int Id { get; set; }

        bool IsSelected { get; set; }

        string Name { get; set; }
    }
}