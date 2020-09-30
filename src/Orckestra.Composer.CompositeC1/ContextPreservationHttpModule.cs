using System;
using System.Threading;
using System.Web;

namespace Orckestra.Composer.CompositeC1
{
    public class ContextPreservationHttpModule : IHttpModule
    {
        public static AsyncLocal<HttpContext> PreservedHttpContext { get; } = new AsyncLocal<HttpContext>();

        public void Init(HttpApplication context)
        {
            context.BeginRequest += SaveContext;
            context.AuthenticateRequest += SaveContext;
            context.PostAuthenticateRequest += SaveContext;
            context.AuthorizeRequest += SaveContext;
            context.PostAuthorizeRequest += SaveContext;
            context.ResolveRequestCache += SaveContext;
            context.PostResolveRequestCache += SaveContext;
            context.PostMapRequestHandler += SaveContext;
            context.AcquireRequestState += SaveContext;
            context.PostAcquireRequestState += SaveContext;
            context.PreRequestHandlerExecute += SaveContext;
            context.PostRequestHandlerExecute += SaveContext;
            context.ReleaseRequestState += SaveContext;
            context.PostReleaseRequestState += SaveContext;
            context.UpdateRequestCache += SaveContext;
            context.PostUpdateRequestCache += SaveContext;
            context.LogRequest += SaveContext;
            context.PostLogRequest += SaveContext;
            context.PreSendRequestContent += SaveContext;
            context.EndRequest += SaveContext;
        }

        private void SaveContext(object sender, EventArgs args)
        {
            var context = ((HttpApplication)sender).Context;

            PreservedHttpContext.Value = context;
        }

        public void Dispose()
        {
        }
    }
}