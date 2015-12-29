using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Interfaces;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public class PlanSetting<T> : IPlanSetting<T> where T : IPlanOption
    {
        public int Id
        {
            get { return Option.Id; }
        }

        public bool IsSelected { get; set; }

        public string Name
        {
            get { return Option.Name; }
        }

        public T Option { get; set; }
    }
}