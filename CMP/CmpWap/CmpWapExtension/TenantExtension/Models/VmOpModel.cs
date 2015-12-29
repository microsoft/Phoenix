//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System.Collections.Generic;


namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Data model for virtual machine operations
    /// </summary>    
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
        /// <param name="createVmFromApi">The virtual machine from API.</param>
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
            this.Vmsize = createVmFromApi.Vmsize;
            this.IsMultiOp = createVmFromApi.IsMultiOp;
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API virtual machine data contract.</returns>
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
                StatusMessage = this.StatusMessage,
                Vmsize = this.Vmsize,
                disks = this.disks,
                IsMultiOp = this.IsMultiOp
            };
        }

        /// <summary>
        /// The ID of the operation
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The opcode for the operation
        /// </summary>
        public string Opcode { get; set; }
        public bool IsMultiOp { get; set; }

        /// <summary>
        /// The CMP request ID associated with the operation
        /// </summary>
        public int VmId { get; set; }

        /// <summary>
        /// Data associated with the operation
        /// todo: Rename
        /// </summary>
        public string sData { get; set; }

        /// <summary>
        /// Data associated with the operation
        /// todo: Rename
        /// </summary>
        public int iData { get; set; }

        /// <summary>
        /// The virtual machine configuration
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// The status code of the virtual machine
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// The status message for the virtual machine
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// The size of the virtual machine
        /// </summary>
        public string Vmsize { get; set; }

        /// <summary>
        /// Any disks associated with the operation
        /// </summary>
        public List<object> disks { get; set; }
    }
}