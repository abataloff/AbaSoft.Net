using System;
using System.IO;
using System.Net;

namespace AbaSoft.Net
{
    internal class HttpResponse : IHttpResponse
    {
        protected HttpListenerResponse response;
        protected IHttpRequest request;

        internal static AbaSoft.Net.IHttpResponse Create(HttpListenerResponse a_response, IHttpRequest a_request)
        {
            var _retVal = new HttpResponse {response = a_response, request = a_request};
            return _retVal;
        }

        public long ContentLength64
        {
            get { return (int) response.ContentLength64; }
            set { response.ContentLength64 = value; }
        }

        public Stream OutputStream
        {
            get { return response.OutputStream; }
        }

        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode) response.StatusCode; }
            set { response.StatusCode = (int) value; }
        }

        public string StatusDescription
        {
            get { return response.StatusDescription; }
            set { response.StatusDescription = value; }
        }

        public void AddHeader(string p1, string p2)
        {
            response.AddHeader(p1,p2);
        }

        public void Close()
        {
            response.Close();
        }

        public Guid RequestTraceIdentifier { get { return request.RequestTraceIdentifier; } }
    }
}
