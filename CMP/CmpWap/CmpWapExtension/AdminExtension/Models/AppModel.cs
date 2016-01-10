using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    public class AppModel
    {
        public const string RegisteredStatus = "Registered";

        //*********************************************************************
        ///
        /// <summary>
        /// Initializes a new instance of the <see cref="AppModel" /> class.
        /// </summary>
        /// 
        //*********************************************************************

        public AppModel()
        {
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Initializes a new instance of the <see cref="AppModel" /> class.
        /// </summary>
        /// <param name="createAppFromApi">The domain name from API.</param>
        /// 
        //*********************************************************************

        public AppModel(App createAppFromApi)
        {
            this.Name = createAppFromApi.Name;
            this.ApplicationId = createAppFromApi.ApplicationId;
            this.IsActive = createAppFromApi.IsActive;
            this.AppCode = createAppFromApi.Code;
            this.Region = createAppFromApi.Region;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Covert to the API object.
        /// </summary>
        /// <returns>The API DomainName data contract.</returns>
        /// 
        //*********************************************************************

        public App ToApiObject()
        {
            return new App()
            {
                Name = this.Name,
                ApplicationId = this.ApplicationId,
                IsActive = this.IsActive,
                Code = this.AppCode,
                Region = this.Region
            };
        }

        public string Name { get; set; }

        /// <summary> </summary>
        public string AppCode { get; set; }


        /// <summary> </summary>
        public int ApplicationId { get; set; }

        /// <summary> </summary>
        public bool IsActive { get; set; }

        /// <summary> </summary>
        public string Region { get; set; }
    }
}
