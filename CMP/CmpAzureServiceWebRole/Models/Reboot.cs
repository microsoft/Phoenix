using System.ComponentModel.DataAnnotations.Schema;

namespace CmpAzureServiceWebRole.Models
{
    public partial class Reboot
    {
        public int ID { get; set; }
        [ForeignKey("VmDeploymentRequest")]
        public int VmDeploymentRequestID { get; set; }
        public virtual CmpInterfaceModel.Models.VmDeploymentRequest VmDeploymentRequest { get; set; }  // Navigation property
        public int Param { get; set; }

    }
}