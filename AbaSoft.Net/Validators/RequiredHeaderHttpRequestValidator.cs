using System.Collections.Generic;
using System.Linq;

namespace AbaSoft.Net.Validators
{
    public class RequiredHeaderHttpRequestValidator : IHttpRequestValidator
    {
        private static readonly Dictionary<string, RequiredHeaderHttpRequestValidator> rules =
            new Dictionary<string, RequiredHeaderHttpRequestValidator>();

        private readonly string header;

        public RequiredHeaderHttpRequestValidator(string a_header)
        {
            header = a_header;
        }

        public static RequiredHeaderHttpRequestValidator[] GetArray(params string[] a_headers)
        {
            return a_headers.Select(Get).ToArray();
        }

        public static RequiredHeaderHttpRequestValidator Get(string a_header)
        {
            if (!rules.ContainsKey(a_header))
                rules.Add(a_header, new RequiredHeaderHttpRequestValidator(a_header));
            return rules[a_header];
        }

        public bool Validate(IHttpRequest a_request)
        {
            return a_request.Headers.Get(header) != null;
        }
    }
}