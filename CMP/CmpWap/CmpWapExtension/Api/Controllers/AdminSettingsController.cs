//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    public class AdminSettingsController : ApiController
    {
        public static AdminSettings settings;

        static AdminSettingsController()
        {
            settings = new AdminSettings
            {
                EndpointAddress = "http://dummyservice",
                Username = "testUser",
                Password = "Password"
            };
        }

        [HttpGet]
        public AdminSettings GetAdminSettings()
        {
           return settings;
        }

        [HttpPut]
        public void UpdateAdminSettings(AdminSettings newSettings)
        {
            if (newSettings == null)
            {
                throw new ArgumentNullException("Given input is null");
            }

            settings = newSettings;
        }
    }
}
