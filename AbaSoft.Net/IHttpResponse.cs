using System.IO;

namespace AbaSoft.Net
{
    public interface IHttpResponse
    {
        System.Net.HttpStatusCode StatusCode { get; set; }

        long ContentLength64 { get; set; }

        Stream OutputStream { get; }

        string StatusDescription { get; set; }

        void AddHeader(string a_name, string a_value);

        void Close();
    }
}
