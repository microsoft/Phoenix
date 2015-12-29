using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CmpAzureServiceWebRole
{
    public class RequireClientCertAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext context)
        {
            // do authorization based on the principal.
            var principal = Thread.CurrentPrincipal;
            if (principal == null || !principal.IsInRole("Administrators"))
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }
    }

    //*************************************************************************
    ///
    /// <summary>
    /// Handler for client certificates incoming from client requests
    ///
    /// </summary>
    /// http://blogs.msdn.com/b/hongmeig1/archive/2012/05/11/how-to-access-clientcertificate-in-a-host-agnostic-manner.aspx
    /// http://blogs.msdn.com/b/hongmeig1/rss.aspx
    /// 
    //*************************************************************************

    public class CustomCertificateMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var cert = request.GetClientCertificate();

            if (cert != null)
            {
                if (cert.Subject.Contains("Some Name you are expecting"))
                {
                    Thread.CurrentPrincipal =
                        new GenericPrincipal(
                            new GenericIdentity(cert.Subject), new[] { "Administrators" });
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }



    public class WebHostClientCertAuth
    {
    }
}