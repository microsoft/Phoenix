using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Common
{
    public class PortalAPIExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// 
        //*********************************************************************

        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is NotImplementedException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }

            try 
            {
                string reason = Regex.Replace(context.Exception.Message, @"\t|\n|\r", "");
                // Making this as a bad request instead of Internal Server error because the reason phrase for Internal Server error is not able to be customized.
                context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest) 
                {
                    ReasonPhrase = reason
                };

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
                    ResponseSummary(context),
                    httpException,
                    context.Exception.ToString());

                Debug.WriteLine(logMessage);

                // The client typically sees the 200 and generates a parseerror at this point irrespective of whether we rethrow
                // or set ExceptionHandled to true (or to false). We may as well set it to true and save another logging attempt.
                return;
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

        private static string ResponseSummary(HttpActionExecutedContext filterContext)
        {
            if (filterContext == null)
            {
                return "HttpContext is null";
            }

            if (filterContext.Response == null)
            {
                return "HttpContext.Reponse is null";
            }

            StringBuilder responseSummary = new StringBuilder();
            var response = filterContext.Response;
            try
            {
                responseSummary.AppendFormat("\tReasonPhrase: {0}{1}", response.ReasonPhrase, Environment.NewLine);
                responseSummary.AppendFormat("\tVersion: {0}{1}", response.Version, Environment.NewLine);
                responseSummary.AppendFormat("\tContent: {0}{1}", response.Content, Environment.NewLine);
                responseSummary.AppendFormat("\tStatusCode: {0}{1}", response.StatusCode, Environment.NewLine);
                responseSummary.AppendFormat("\tHeader: {0}{1}", response.Headers, Environment.NewLine);
            }
            catch (Exception exception)
            {
                responseSummary.AppendFormat("{0}{0}Exception listing response properties: {1}", Environment.NewLine, exception.ToString());
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
