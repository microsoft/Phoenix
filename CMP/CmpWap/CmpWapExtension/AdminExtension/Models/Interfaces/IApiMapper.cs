namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.Interfaces
{
    public interface IApiMapper<out T>
    {
        T ToApiObject();
    }
}