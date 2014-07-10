namespace AbaSoft.Net
{
    public interface IHttpMessage
    {
        IHttpRequest Request { get; }

        IHttpResponse Response { get; }
    }
}
