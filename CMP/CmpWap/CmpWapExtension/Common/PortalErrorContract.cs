// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Configuration;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Common
{
    /// <summary>
    /// The error data contract that will be serialized and send back to the client.
    /// </summary>
    [DataContract]
    public class PortalErrorContract
    {
        //*********************************************************************
        ///
        /// <summary>
        /// Convert an exception object of the PortalErrorContract
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The PortalErrorContract.</returns>
        /// 
        //*********************************************************************

        public static PortalErrorContract FromException(Exception exception)
        {
            var error = new PortalErrorContract();

            PortalException extensionException = exception as PortalException;

            if (extensionException != null)
            {
                error.message = extensionException.Message;
                error.httpStatusCode = extensionException.httpStatusCode.ToString();
            }
            else
            {
                // If we cannot positivly identify the exception type, we cannot show the message
                // in the exception to the end user. The exception (and it's messsage) will be 
                // logged to MDS so nothing is lost.
                string stdMessage = "Try again. Contact support if the problem persists...";
                error.message = stdMessage + exception.Message;
                error.httpStatusCode = System.Net.HttpStatusCode.InternalServerError.ToString();
                error.operationTrackingId = string.Empty;
            }

            var debugModeSetting = ConfigurationManager.AppSettings["Microsoft.Azure.Portal.Configuration.PortalConfiguration.DevelopmentMode"];
            if (string.Equals(debugModeSetting, "true"))
            {
                error.stackTrace = exception.ToString();
            }

            return error;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets Exception message
        /// </summary>
        /// 
        //*********************************************************************

        [DataMember]
        public string message { get; private set; }

        //*********************************************************************
        ///
        /// <summary>
        /// Error Message suitable to display to users in the HTML UI. 
        /// </summary>
        /// <remarks>
        /// A signifigant amount of client code looks for "ErrorMessage" on the response (with the Upper Camel Casing)
        /// </remarks>
        /// 
        //*********************************************************************

        [DataMember]
        public string ErrorMessage { get { return this.message; } }

        /// <summary>
        /// Gets Http Status code / RDFE Error code
        /// </summary>
        [DataMember]
        public string httpStatusCode { get; private set; }

        /// <summary>
        /// The tracking id used by Ops to get more detail about the failed operation
        /// </summary>
        [DataMember]
        public string operationTrackingId { get; private set; }

        /// <summary>
        /// Gets Exception Stack Trace
        /// </summary>
        [DataMember]
        public string stackTrace { get; private set; }
    }
}
