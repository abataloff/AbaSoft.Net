using System.IO;

namespace AbaSoft.Net
{
    public interface IHttpResponse
    {
        System.Net.HttpStatusCode StatusCode { get; set; }

        int ContentLength64 { get; set; }

        Stream OutputStream { get; }

        string StatusDescription { get; set; }
    }
}
