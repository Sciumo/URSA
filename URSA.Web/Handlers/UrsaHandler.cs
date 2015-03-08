﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using URSA.Web.Description;
using URSA.Web.Http;
using URSA.Web.Http.Converters;
using URSA.Web.Http.Description;

namespace URSA.Web.Handlers
{
    /// <summary>Provides a connection between URSA framework and standard ASP.net pipeline.</summary>
    /// <typeparam name="T">Type of controller exposed.</typeparam>
    public class UrsaHandler<T> : IHttpHandler, IRouteHandler where T : IController
    {
        private RequestHandler _requestHandler;

        /// <summary>Initializes a new instance of the <see cref="UrsaHandler{T}" /> class.</summary>
        public UrsaHandler()
        {
            _requestHandler = new RequestHandler();
        }

        /// <inheritdoc />
        public bool IsReusable { get { return true; } }

        /// <inheritdoc />
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        /// <inheritdoc />
        public void ProcessRequest(HttpContext context)
        {
            context.Response.TrySkipIisCustomErrors = true;
            if ((context.Request.Url.Segments.Any()) && (context.Request.Url.Segments.Skip(1).First() == EntityConverter.DocumentationStylesheet))
            {
                context.Response.ContentType = "text/xsl";
                using (var source = new StreamReader(GetType().Assembly.GetManifestResourceStream("URSA.Web.DocumentationStylesheet.xslt")))
                {
                    context.Response.Output.Write(source.ReadToEnd());
                }

                return;
            }

            var headers = new HeaderCollection();
            foreach (string headerName in context.Request.Headers)
            {
                ((IDictionary<string, string>)headers)[headerName] = context.Request.Headers[headerName];
            }

            var requestInfo = new RequestInfo(
                Verb.Parse(context.Request.HttpMethod),
                context.Request.Url,
                context.Request.InputStream,
                headers);
            var response = _requestHandler.HandleRequest(requestInfo);
            context.Response.ContentEncoding = context.Response.HeaderEncoding = response.Encoding;
            context.Response.StatusCode = (int)response.Status;
            foreach (var header in response.Headers)
            {
                switch (header.Name)
                {
                    case Header.ContentType:
                        context.Response.ContentType = header.Value;
                        break;
                    case Header.ContentLength:
                        break;
                    default:
                        context.Response.Headers.Add(header.Name, header.Value);
                        break;
                }
            }

            response.Body.CopyTo(context.Response.OutputStream);
        }
    }
}