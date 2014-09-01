using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace AbaSoft.Net
{
    internal class HttpRequest : IHttpRequest
    {
        private HttpListenerRequest request;

        internal static IHttpRequest Create(HttpListenerRequest a_request)
        {
            var _retVal = new HttpRequest {request = a_request};
            return _retVal;
        }

        public long ContentLength64
        {
            get { return request.ContentLength64; }
        }

        public NameValueCollection Headers
        {
            get { return request.Headers; }
        }

        public void AddHeader(string a_name, string a_value)
        {
            request.Headers.Add(a_name, a_value);
        }

        public string HttpMethod
        {
            get { return request.HttpMethod; }
        }

        public Stream InputStream
        {
            get { return request.InputStream; }
        }

        public NameValueCollection QueryString
        {
            get { return request.QueryString; }
        }

        public Uri Url
        {
            get { return request.Url; }
        }

        public Guid RequestTraceIdentifier
        {
            get { return request.RequestTraceIdentifier; }
        }
    }
}
