using System;
using Tools;
using MessageReceivedEventHandler = System.EventHandler<Tools.UniversalEventArgs<AbaSoft.Net.IHttpMessage>>;

namespace AbaSoft.Net
{
    public interface IHttpServer
    {
        event MessageReceivedEventHandler MessageReceived;
        void Start();
        void Stop();
    }
}