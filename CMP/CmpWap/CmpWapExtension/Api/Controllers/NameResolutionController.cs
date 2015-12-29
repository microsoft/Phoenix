//*****************************************************************************
// File: NameResolutionController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used to validate security groups for provisioning.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    /// This class is used to validate security groups for provisioning.
    /// </remarks>
    public class NameResolutionController : ApiController
    {
        /// <summary>
        /// This class is used to validate security groups for provisioning.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="securitygroups"></param>
        /// <returns>returns an object determining security group validity status</returns>
        [HttpGet]
        public SecurityGroupResult ValidateSecurityGroup(string subscriptionId,string securitygroups)
        {
            /*
            ActiveDirectory activeDirectory = new ActiveDirectory();
            var arr = securitygroups.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var status = false;
            var returnResult = "";
            foreach (var item in arr)
            {
                var adGroup = activeDirectory.GetADSecurityGroup(item, false);
                if (adGroup.ResolvedOk)
                {
                    status = true;
                    if (!String.IsNullOrEmpty(adGroup.PrincipalName))
                    {
                        returnResult += adGroup.Domain + "\\" + adGroup.PrincipalName + ";";
                    }
                    else if (!String.IsNullOrEmpty(adGroup.GroupName))
                    {
                        returnResult += adGroup.Domain + "\\" + adGroup.GroupName + ";";
                    }

                }
            }


            if (!status)
                return (new SecurityGroupResult { Status = status, Result = null });
            else
                return (new SecurityGroupResult { Status = status, Result = returnResult.TrimEnd(';') });
             */

            //Quick workaround to get this to return a default value. Method above will need to be reviewed if this
            //has to return something new.
            return (new SecurityGroupResult {Status = false, Result = null});

        }
    }
}
