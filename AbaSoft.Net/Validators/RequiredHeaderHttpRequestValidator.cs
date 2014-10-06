using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AbaSoft.Net.Validators
{
    public class RequiredHeaderHttpRequestValidator : IHttpRequestValidator
    {
        private static readonly Dictionary<string, RequiredHeaderHttpRequestValidator> rules =
            new Dictionary<string, RequiredHeaderHttpRequestValidator>();

        private readonly string header;

        public RequiredHeaderHttpRequestValidator(string a_header, HttpStatusCode a_errorStatusCode = HttpStatusCode.BadRequest,
            string a_errorMessage = null)
        {
            header = a_header;
            ErrorStatusCode = a_errorStatusCode;
            ErrorMessage = a_errorMessage ?? string.Empty;
        }

        public string ErrorMessage { get; private set; }

        public HttpStatusCode ErrorStatusCode { get; private set; }

        public static RequiredHeaderHttpRequestValidator[] GetArray(params string[] a_headers)
        {
            return a_headers.Select(Get).ToArray();
        }

        public static RequiredHeaderHttpRequestValidator Get(HttpRequestHeader a_header,
            HttpStatusCode a_errorStatusCode = HttpStatusCode.BadRequest,
            string a_errorMessage = null)
        {
            return Get(a_header.ToString(), a_errorStatusCode, a_errorMessage);
        }

        public static RequiredHeaderHttpRequestValidator Get(string a_header)
        {
            return Get(a_header, HttpStatusCode.BadRequest, null);
        }

        public static RequiredHeaderHttpRequestValidator Get(string a_header, HttpStatusCode a_errorStatusCode, string a_errorMessage)
        {
            if (!rules.ContainsKey(a_header))
                rules.Add(a_header, new RequiredHeaderHttpRequestValidator(a_header, a_errorStatusCode, a_errorMessage));
            return rules[a_header];
        }

        public bool Validate(IHttpRequest a_request)
        {
            return a_request.Headers.Get(header) != null;
        }
    }
}