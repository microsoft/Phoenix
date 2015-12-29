// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Common
{
    /// <summary>
    /// Exception from the portal that will be serialized to a Json format that the client expects
    /// </summary>
    [Serializable]
    public class PortalException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        /// <value>
        /// The HTTP status code.
        /// </value>
        public HttpStatusCode httpStatusCode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PortalException(string message)
            : base(message)
        {
            this.httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Initialized a PortalException with a message, and a user specificed HTTP status code.
        /// </summary>
        /// <param name="message">The message that may be displayed in the UI</param>
        /// <param name="httpStatusCode">The HTTP Status code to reutrn to the caller</param>
        public PortalException(string message, System.Net.HttpStatusCode httpStatusCode)
            : base(message)
        {
            this.httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initialized a PortalException with a message, and an internal exception (that is logged). Uses the default HttpStatusCode of InternalServerError. 
        /// </summary>
        /// <param name="message">The message that may be displayed in the UI</param>
        /// <param name="innerException">The inner exception, useful for logging scenario's</param>
        public PortalException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.httpStatusCode = System.Net.HttpStatusCode.BadRequest;
        }

        /// <summary>
        /// Initialized a PortalException with a message, exception and a caller specificed Http Status Code
        /// </summary>
        /// <param name="message">The message that may be displayed in the UI</param>
        /// <param name="innerException">The inner exception, useful for logging scenario's</param>
        /// <param name="httpStatusCode">The HTTP Status code to reutrn to the caller</param>
        public PortalException(string message, Exception innerException, System.Net.HttpStatusCode httpStatusCode)
            : base(message, innerException)
        {
            this.httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Required override to add in the serialized parameters
        /// </summary>
        /// <param name="info">serialization information</param>
        /// <param name="context">streaming context</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("httpStatusCode", (int)this.httpStatusCode);

            base.GetObjectData(info, context);
        }
    }
}
