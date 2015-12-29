//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Data model for VM size tenant view
    /// </summary>    
    public class CreateSizeModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSizeModel" /> class.
        /// </summary>
        public CreateSizeModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSizeModel" /> class.
        /// </summary>
        /// <param name="createSizeFromApi">The size from API.</param>
        public CreateSizeModel(VmSize createSizeFromApi)
        {
            this.VMSizeId = createSizeFromApi.VmSizeId;
            this.Name = createSizeFromApi.Name;
            Cores = createSizeFromApi.Cores;
            this.Description = createSizeFromApi.Description;
         
            this.IsActive = createSizeFromApi.IsActive;
              Memory = createSizeFromApi.Memory;
              MaxDataDiskCount = createSizeFromApi.MaxDataDiskCount;
           
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API size data contract.</returns>
        public VmSize ToApiObject()
        {
            return new VmSize()
            {
                VmSizeId = this.VMSizeId,
                Name = this.Name,
                Description = this.Description,

                Cores = this.Cores,
                IsActive = this.IsActive,
                Memory = this.Memory,
                MaxDataDiskCount = this.MaxDataDiskCount,
               
            };
        }

        /// <summary>
        /// The ID of the VM size
        /// </summary>
        public int VMSizeId { get; set; }

        /// <summary>
        /// The name of the size
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the size
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// The number of cores in the size
        /// </summary>
        public int Cores { get; set; }
        
        /// <summary>
        /// Amount of memory in the size
        /// </summary>
        public int Memory { get; set; }
        
        /// <summary>
        /// Maximum number of data disks supported by the size
        /// </summary>
        public int MaxDataDiskCount { get; set; }
        
        /// <summary>
        /// Whether or not the size is active
        /// </summary>
        public bool IsActive { get; set; }
    }

    public class CreateSizeComparer : IEqualityComparer<CreateSizeModel>
    {
        public bool Equals(CreateSizeModel x, CreateSizeModel y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the objects' properties are equal.
            return x.Name == y.Name
                   && x.Description == y.Description
                   && x.VMSizeId == y.VMSizeId
                   && x.Cores == y.Cores
                   && x.Memory == y.Memory
                   && x.MaxDataDiskCount == y.MaxDataDiskCount;
        }

        public int GetHashCode(CreateSizeModel size)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(size, null)) return 0;

            //Get hash code for each field if it is not null. 
            int hashName = size.Name == null ? 0 : size.Name.GetHashCode();
            int hashDescription = size.Description == null ? 0 : size.Description.GetHashCode();
            int hashVmSizeId = size.VMSizeId.GetHashCode();
            int hashCores = size.Cores.GetHashCode();
            int hashMemory = size.Memory.GetHashCode();
            int hashMaxDataDiskCount = size.MaxDataDiskCount.GetHashCode();

            //Calculate the hash code for the size. 
            return hashName ^ hashDescription ^ hashVmSizeId ^ hashCores ^ hashMemory ^ hashMaxDataDiskCount;
        }
    }
}