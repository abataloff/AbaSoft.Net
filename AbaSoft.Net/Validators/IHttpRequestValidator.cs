using System.Net;

namespace AbaSoft.Net.Validators
{
    public interface IHttpRequestValidator
    {
        bool Validate(IHttpRequest a_request);
        string ErrorMessage { get; }
        HttpStatusCode ErrorStatusCode { get; }
    }
}
