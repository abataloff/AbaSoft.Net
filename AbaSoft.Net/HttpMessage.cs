namespace AbaSoft.Net
{
    internal class HttpMessage : IHttpMessage
    {
        internal static AbaSoft.Net.IHttpMessage Create(System.Net.HttpListenerContext a_context)
        {
            var _request = HttpRequest.Create(a_context.Request);
            var _respone = HttpResponse.Create(a_context.Response, _request);
            var _retVal = new HttpMessage
            {
                Request = _request,
                Response = _respone,
            };
            return _retVal;
        }

        public IHttpRequest Request { get; private set; }

        public IHttpResponse Response { get; private set; }
    }
}
