using System;
using AbaSoft.Net.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbaSoft.Net;
using Moq;
using Tools;

namespace AbaSoft.Net_UnitTests
{
    [TestClass]
    public class RestServer_tests
    {
        [TestMethod]
        public void StartInvokeHttpStart_test()
        {
            var _httpServerMock = new Mock<IHttpServer>();
            var _rootRestControllerMock = new Mock<RestController>();

            var _targert = new RestServer(_httpServerMock.Object, _rootRestControllerMock.Object);
            _targert.Start();

            _httpServerMock.Verify(a_m=>a_m.Start(),Times.Once());
        }

        [TestMethod]
        public void StopInvokeHttpStop_test()
        {
            var _httpServerMock = new Mock<IHttpServer>();
            var _rootRestControllerMock = new Mock<RestController>();

            var _targert = new RestServer(_httpServerMock.Object, _rootRestControllerMock.Object);
            _targert.Stop();

            _httpServerMock.Verify(a_m => a_m.Stop(), Times.Once());
        }

        [TestMethod]
        public void HttpMessageReceivedInvokeRestRootProccess_test()
        {
            var _httpServerMock = new Mock<IHttpServer>();
            var _rootRestControllerMock = new Mock<IRestController>();
            var _message = new Mock<IHttpMessage>();
            var _targert = new RestServer(_httpServerMock.Object, _rootRestControllerMock.Object);
            _targert.Start();

            _httpServerMock.Raise(a_m => a_m.MessageReceived += null, new UniversalEventArgs<IHttpMessage>(_message.Object));

            _rootRestControllerMock.Verify(a_m=>a_m.Proccess(_message.Object), Times.Once());
        }
    }
}
