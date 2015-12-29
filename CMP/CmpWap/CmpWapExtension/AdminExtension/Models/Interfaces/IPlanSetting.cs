namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.Interfaces
{
    public interface IPlanSetting
    {
        int Id { get; set; }

        bool IsSelected { get; set; }

        string Name { get; set; }
    }
}