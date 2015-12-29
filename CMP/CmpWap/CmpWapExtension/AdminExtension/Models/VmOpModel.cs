using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    public class VmOpModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="VmOpModel" /> class.
        /// </summary>
        public VmOpModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmOpModel" /> class.
        /// </summary>
        /// <param name="createVmFromApi">The domain name from API.</param>
        public VmOpModel(VmOp createVmFromApi)
        {
            this.Id = createVmFromApi.Id;
            this.Opcode = createVmFromApi.Opcode;
            this.VmId = createVmFromApi.VmId;
            this.sData = createVmFromApi.sData;
            this.iData = createVmFromApi.iData;
            this.Config = createVmFromApi.Config;
            this.StatusCode = createVmFromApi.StatusCode;
            this.StatusMessage = createVmFromApi.StatusMessage;

        }

        /// <summary>
        /// Covert to the API object.
        /// </summary>
        /// <returns>The API DomainName data contract.</returns>
        public VmOp ToApiObject()
        {
            return new VmOp()
            {
                Id = this.Id,
                Opcode = this.Opcode,
                VmId = this.VmId,
                sData = this.sData,
                iData = this.iData,
                Config = this.Config,
                StatusCode = this.StatusCode,
                StatusMessage = this.StatusMessage
            };
        }

        public int Id { get; set; }
        public string Opcode { get; set; }
        public int VmId { get; set; }
        public string sData { get; set; }
        public int iData { get; set; }
        public string Config { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }
}
