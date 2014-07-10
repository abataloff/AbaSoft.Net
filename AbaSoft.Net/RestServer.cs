using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbaSoft.Net.Rest;
using Tools;

namespace AbaSoft.Net
{
    public class RestServer
    {
        private readonly IHttpServer httpServer;
        private readonly IRestController rootRestController;

        public RestServer(IHttpServer a_httpServer, IRestController a_rootRestController)
        {
            httpServer = a_httpServer;

            rootRestController = a_rootRestController;
            rootRestController.Init();
        }

        public void Start()
        {
            httpServer.Start();
            httpServer.MessageReceived += httpServer_MessageReceived;
        }

        public void Stop()
        {
            httpServer.Stop();
        }

        void httpServer_MessageReceived(object a_sender, UniversalEventArgs<IHttpMessage> a_e)
        {
            rootRestController.Proccess(a_e.Target);
        }
    }
}
