namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Interfaces
{
    public interface IPlanSetting<T> : IPlanOption where T : IPlanOption
    {
        bool IsSelected { get; }
    }
}