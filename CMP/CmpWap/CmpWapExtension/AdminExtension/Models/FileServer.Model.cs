// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{    
    /// <summary>
    /// This is a model class which contains data contract we send to Controller which then shows up in UI
    /// FileServerModel contains data contract of data which shows up in "File Servers" tab inside CmpWapExtension Admin Extension
    /// </summary>
    public class FileServerModel
    {
        /// <summary>
        /// This is hidden in UI but can be used to identify individual file server records in grid
        /// </summary>
        public int FileServerId { get; set; }

        /// <summary>
        /// FileServerName maps to "Name" column in "File Servers" tab
        /// </summary>
        public string FileServerName { get; set; }

        /// <summary>
        /// TotalSpace maps to "Total Space" column in "File Servers" tab
        /// Specify space in GB
        /// </summary>
        public int TotalSpace { get; set; }

        /// <summary>
        /// FreeSpace maps to "Free Space" column in "File Servers" tab        
        /// </summary>
        public int FreeSpace { get; set; }

        /// <summary>
        /// DefaultSize maps to "Default Share Size" column in "File Servers" tab
        /// </summary>
        public int DefaultSize { get; set; }

        public FileServerModel(FileServer fileServer)
        {
            this.FileServerId = fileServer.FileServerId;
            this.FileServerName = fileServer.FileServerName;
            this.TotalSpace = fileServer.TotalSpace;
            this.FreeSpace = fileServer.FreeSpace;
            this.DefaultSize = fileServer.DefaultSize;
        }
    }
}
