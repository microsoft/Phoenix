// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Common
{
    /// <summary>
    /// An attribute used to decorate a Portal MVC controller to enable default error handling and error serialization.
    /// </summary>
    public class PortalExceptionHandlerAttribute : HandleErrorAttribute
    {
        //*********************************************************************
        ///
        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The action-filter context.</param>
        /// 
        //*********************************************************************

        public override void OnException(ExceptionContext filterContext)
        {
            Debug.WriteLine(filterContext.Exception.ToString());

            try
            {
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                try
                {
                    filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                }
                catch (HttpException httpException)
                {
                    // Attempting to change the response status code after the response has started streaming to the client causes
                    // an exception. We can't fix the stream the from this state, so ensure the original exception is logged.
                    string logMessage = string.Format(CultureInfo.InvariantCulture,
                        "Unable to report exception to the client. A HttpException occured while attempting to set the response status code to InternalServerError. " +
                        "This can occur if the exception is thrown after the response to the client has started down the wire.{0}{0}" +
                        "The response was in this state: {0}{1}{0}{0}" +
                        "The HttpException was: {2}{0}{0}" +
                        "The original exception was already logged and is repeated here for convenience only: {3}",
                        Environment.NewLine,
                        ResponseSummary(filterContext),
                        httpException,
                        filterContext.Exception.ToString());

                    Debug.WriteLine(logMessage);

                    // The client typically sees the 200 and generates a parseerror at this point irrespective of whether we rethrow
                    // or set ExceptionHandled to true (or to false). We may as well set it to true and save another logging attempt.
                    filterContext.ExceptionHandled = true;
                    return;
                }
                
                // Handle aggregate exceptions
                var aggregateException = filterContext.Exception as AggregateException;
                if (aggregateException != null)
                {
                    filterContext.Exception = aggregateException.Flatten().InnerException;
                }

                PortalException extensionException = filterContext.Exception as PortalException;
                if (extensionException != null)
                {
                    filterContext.HttpContext.Response.StatusCode = (int)extensionException.httpStatusCode;
                    filterContext.HttpContext.Response.StatusDescription = NormalizeString(extensionException.Message);
                }

                filterContext.Result = new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = PortalErrorContract.FromException(filterContext.Exception)
                };

                filterContext.ExceptionHandled = true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string ResponseSummary(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext == null)
            {
                return "HttpContext is null";
            }

            if (filterContext.HttpContext.Response == null)
            {
                return "HttpContext.Reponse is null";
            }

            StringBuilder responseSummary = new StringBuilder();
            HttpResponseBase response = filterContext.HttpContext.Response;
            try
            {
                responseSummary.AppendFormat("\tIsClientConnected: {0}{1}", response.IsClientConnected, Environment.NewLine);
                responseSummary.AppendFormat("\tBufferOutput: {0}{1}", response.BufferOutput, Environment.NewLine);
                responseSummary.AppendFormat("\tIsRequestBeingRedirected: {0}{1}", response.IsRequestBeingRedirected, Environment.NewLine);
                responseSummary.AppendFormat("\tStatus: {0}{1}", response.Status, Environment.NewLine);
                responseSummary.AppendFormat("\tStatusCode: {0}{1}", response.StatusCode, Environment.NewLine);
            }
            catch (Exception exception)
            {
                responseSummary.AppendFormat("{0}{0}Exception listing response properties: {1}", Environment.NewLine, exception.ToString());
            }

            try
            {
                if (response.Cookies != null)
                {
                    responseSummary.AppendLine("\tCookies:");
                    foreach (string key in response.Cookies.AllKeys)
                    {
                        HttpCookie cookie = response.Cookies[key];
                        // Cookie values are obmitted so that information like RPS tickets isn't written to the logs (we do the same thing with request cookies)
                        responseSummary.AppendFormat("\t\tName={0}: Expires={1}, Domain={2}, HttpOnly={3}, Path={4}, Secure={5} Value=[Redacted] ({6} characters){7}",
                            key, cookie.Expires.ToString(CultureInfo.InvariantCulture), cookie.Domain, cookie.HttpOnly, cookie.Path, cookie.Secure,
                            (cookie.Value ?? string.Empty).Length, Environment.NewLine);
                    }
                }
            }
            catch (Exception exception)
            {
                responseSummary.AppendFormat("{0}{0}Exception listing response cookies: {1}", Environment.NewLine, exception.ToString());
            }

            try
            {
                if (response.Headers != null)
                {
                    responseSummary.AppendLine("\tHeaders:");
                    foreach (string key in response.Headers.AllKeys)
                    {
                        responseSummary.AppendFormat("\t\t{0} = {1} ({2} characters) {3}",
                            key, key == "Set-Cookie" ? "[Redacted]" : response.Headers[key], (response.Headers[key] ?? string.Empty).Length, Environment.NewLine);
                    }
                }
            }
            catch (Exception exception)
            {
                responseSummary.AppendFormat("{0}{0}Exception listing response headers: {1}", Environment.NewLine, exception.ToString());
            }

            return responseSummary.ToString();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringToNormalize"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string NormalizeString(string stringToNormalize)
        {
            char[] carriageCharacters = new char[] { '\r', '\n', '\t' };

            //http://msdn.microsoft.com/en-us/library/system.web.httpresponse.statusdescription.aspx
            //ArgumentOutOfRangeException  - The selected value has a length greater than 512.
            stringToNormalize = stringToNormalize.Substring(0, Math.Min(stringToNormalize.Length, 512));

            foreach (char character in carriageCharacters)
            {
                stringToNormalize = stringToNormalize.Replace(character, ' ');
            }

            return stringToNormalize;
        }
    }
}
