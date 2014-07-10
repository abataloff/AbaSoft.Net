using System.Net;
using AbaSoft.Net;
using AbaSoft.Net.Rest;
using AbaSoft.Net.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Tools;

namespace AbaSoft.Net_UnitTests
{
    [TestClass]
    public class RestController_tests
    {
        [TestMethod]
        public void InitInvokeInsertedControllersInit_test()
        {
            var _ctrlMock0 = new Mock<IRestController>();
            _ctrlMock0.SetupGet(a_m => a_m.Section).Returns("section_0");
            var _ctrlMock1 = new Mock<IRestController>();
            _ctrlMock1.SetupGet(a_m => a_m.Section).Returns("section_1");
            var _ctrlMock2 = new Mock<IRestController>();
            _ctrlMock2.SetupGet(a_m => a_m.Section).Returns("section_2");
            var _target = new RestContollerMock();
            _target.SetControllers(new[] {_ctrlMock0.Object, _ctrlMock1.Object, _ctrlMock2.Object});
            _target.SetIsContainer(false);

            _target.Init();

            // Init должен вызывать Init вложенных контроллеров
            _ctrlMock0.Verify(a_m => a_m.Init(), Times.Once());
            _ctrlMock1.Verify(a_m => a_m.Init(), Times.Once());
            _ctrlMock2.Verify(a_m => a_m.Init(), Times.Once());
        }

        [TestMethod]
        public void InitInvokeContentControllerInit_test()
        {
            var _ctrlContentMock = new Mock<IRestController>();
            var _target = new RestContollerMock();
            _target.SetContentRestController(_ctrlContentMock.Object);
            _target.SetIsContainer(true);

            _target.Init();

            // Init должен вызвать Init контроллера контента
            _ctrlContentMock.Verify(a_m => a_m.Init(), Times.Once());
        }

        [TestMethod]
        public void ProccessRedirectedIncomemingMessageToInsertedController_test()
        {
            var _ctrlMock0 = new Mock<IRestController>();
            _ctrlMock0.SetupGet(a_m => a_m.Section).Returns("section_0");
            var _ctrlMock1 = new Mock<IRestController>();
            _ctrlMock1.SetupGet(a_m => a_m.Section).Returns("section_1");
            var _ctrlMock2 = new Mock<IRestController>();
            _ctrlMock2.SetupGet(a_m => a_m.Section).Returns("section_2");
            var _requestMock = new Mock<IHttpRequest>();
            _requestMock.SetupGet(a_m => a_m.Url).Returns(new Uri("http://some.net/section_0"));
            var _messageMock = new Mock<IHttpMessage>();
            _messageMock.SetupGet(a_m => a_m.Request).Returns(_requestMock.Object);
            var _target = new RestContollerMock();
            _target.SetControllers(new[] {_ctrlMock0.Object, _ctrlMock1.Object, _ctrlMock2.Object});
            _target.SetIsContainer(false);
            _target.Init();

            _target.Proccess(_messageMock.Object);

            // Входящее сообщение должно перенаправляться вложенному, если это не контейнер
            _ctrlMock0.Verify(a_m => a_m.Proccess(_messageMock.Object), Times.Once());
        }

        [TestMethod]
        public void ProccessRedirectedIncomemingMessageToContentController_test()
        {
            var _requestMock = new Mock<IHttpRequest>();
            _requestMock.SetupGet(a_m => a_m.Url).Returns(new Uri("http://some.net/section_id"));
            var _messageMock = new Mock<IHttpMessage>();
            _messageMock.SetupGet(a_m => a_m.Request).Returns(_requestMock.Object);
            var _ctrlContentMock = new Mock<IRestController>();
            var _target = new RestContollerMock();
            _target.SetContentRestController(_ctrlContentMock.Object);
            _target.SetIsContainer(true);
            _target.Init();

            _target.Proccess(_messageMock.Object);

            // Входящее сообщение должно перенаправляться контент контроллеру, если это контейнер
            _ctrlContentMock.Verify(a_m => a_m.Proccess(_messageMock.Object), Times.Once());
        }

        [TestMethod]
        public void ProccessReturnBadRequest_test()
        {
            var _requestMock = new Mock<IHttpRequest>();
            _requestMock.SetupGet(a_m => a_m.Url).Returns(new Uri("http://some.net/bad_path"));
            var _messageMock = new Mock<IHttpMessage>();
            _messageMock.SetupGet(a_m => a_m.Request).Returns(_requestMock.Object);
            var _responseMock = new Mock<IHttpResponse>();
            _messageMock.SetupGet(a_m => a_m.Response).Returns(_responseMock.Object);
            var _target = new RestContollerMock();
            _target.SetIsContainer(false);
            _target.Init();

            _target.Proccess(_messageMock.Object);

            // На некорректные пути неконтейнер должен отвечать "BadRequest"
            _responseMock.VerifySet(a_m => a_m.StatusCode = HttpStatusCode.BadRequest, Times.Once());
        }

        [TestMethod]
        public void ProccessReturnBadRequestInternal_test()
        {
            var _requestMock = new Mock<IHttpRequest>();
            _requestMock.SetupGet(a_m => a_m.Url).Returns(new Uri("http://some.net/"));
            _requestMock.SetupGet(a_m => a_m.HttpMethod).Returns("GET");
            var _messageMock = new Mock<IHttpMessage>();
            _messageMock.SetupGet(a_m => a_m.Request).Returns(_requestMock.Object);
            var _responseMock = new Mock<IHttpResponse>();
            _messageMock.SetupGet(a_m => a_m.Response).Returns(_responseMock.Object);
            var _target = new RestContollerMock();
            _target.SetIsContainer(false);
            _target.Init();
            var _requestValidatorMock = new Mock<IHttpRequestValidator>();
            _requestValidatorMock.Setup(a_m => a_m.Validate(_requestMock.Object)).Returns(false);
            _target.AddRule("GET", _requestValidatorMock.Object);

            _target.Proccess(_messageMock.Object);

            // Если валидация не пройдена контроллер должен отвечать "BadRequest"
            _responseMock.VerifySet(a_m => a_m.StatusCode = HttpStatusCode.BadRequest, Times.Once());
        }

        [TestMethod]
        public void IncomingMessageValidation_test()
        {
            var _requestMock = new Mock<IHttpRequest>();
            _requestMock.SetupGet(a_m => a_m.Url).Returns(new Uri("http://some.net/"));
            _requestMock.SetupGet(a_m => a_m.HttpMethod).Returns("GET");
            var _messageMock = new Mock<IHttpMessage>();
            _messageMock.SetupGet(a_m => a_m.Request).Returns(_requestMock.Object);
            var _responseMock = new Mock<IHttpResponse>();
            _messageMock.SetupGet(a_m => a_m.Response).Returns(_responseMock.Object);
            var _target = new RestContollerMock();
            _target.SetIsContainer(false);
            _target.Init();
            var _requestValidatorMock = new Mock<IHttpRequestValidator>();
            _target.AddRule("GET", _requestValidatorMock.Object);

            _target.Proccess(_messageMock.Object);

            // Контроллер должен вызывать валидацию
            _requestValidatorMock.Verify(a_m => a_m.Validate(_requestMock.Object), Times.Once());
        }

        [TestMethod]
        public void ProccessInvokeInternalPut_test()
        {
            var _requestMock = new Mock<IHttpRequest>();
            _requestMock.SetupGet(a_m => a_m.Url).Returns(new Uri("http://some.net/"));
            _requestMock.SetupGet(a_m => a_m.HttpMethod).Returns("PUT");
            var _messageMock = new Mock<IHttpMessage>();
            _messageMock.SetupGet(a_m => a_m.Request).Returns(_requestMock.Object);
            var _responseMock = new Mock<IHttpResponse>();
            _messageMock.SetupGet(a_m => a_m.Response).Returns(_responseMock.Object);
            var _target = new RestContollerMock();
            _target.SetIsContainer(false);
            _target.Init();
            var _requestValidatorMock = new Mock<IHttpRequestValidator>();
            _requestValidatorMock.Setup(a_m => a_m.Validate(_requestMock.Object)).Returns(true);
            _target.AddRule("PUT", _requestValidatorMock.Object);
            var _actual = false;
            _target.PutCelled +=
                delegate(object a_sender, UniversalEventArgs<IHttpMessage> a_context)
                {
                    _actual = (a_context.Target == _messageMock.Object);
                };

            _target.Proccess(_messageMock.Object);

            Assert.IsTrue(_actual);
        }

        [TestMethod]
        public void ProccessInvokeInternalGet_test()
        {
            var _requestMock = new Mock<IHttpRequest>();
            _requestMock.SetupGet(a_m => a_m.Url).Returns(new Uri("http://some.net/"));
            _requestMock.SetupGet(a_m => a_m.HttpMethod).Returns("GET");
            var _messageMock = new Mock<IHttpMessage>();
            _messageMock.SetupGet(a_m => a_m.Request).Returns(_requestMock.Object);
            var _responseMock = new Mock<IHttpResponse>();
            _messageMock.SetupGet(a_m => a_m.Response).Returns(_responseMock.Object);
            var _target = new RestContollerMock();
            _target.SetIsContainer(false);
            _target.Init();
            var _requestValidatorMock = new Mock<IHttpRequestValidator>();
            _requestValidatorMock.Setup(a_m => a_m.Validate(_requestMock.Object)).Returns(true);
            _target.AddRule("GET", _requestValidatorMock.Object);
            var _actual = false;
            _target.GetCelled +=
                delegate(object a_sender, UniversalEventArgs<IHttpMessage> a_context)
                {
                    _actual = (a_context.Target == _messageMock.Object);
                };

            _target.Proccess(_messageMock.Object);

            Assert.IsTrue(_actual);
        }

        [TestMethod]
        public void ProccessInvokeInternalPost_test()
        {
            var _requestMock = new Mock<IHttpRequest>();
            _requestMock.SetupGet(a_m => a_m.Url).Returns(new Uri("http://some.net/"));
            _requestMock.SetupGet(a_m => a_m.HttpMethod).Returns("POST");
            var _messageMock = new Mock<IHttpMessage>();
            _messageMock.SetupGet(a_m => a_m.Request).Returns(_requestMock.Object);
            var _responseMock = new Mock<IHttpResponse>();
            _messageMock.SetupGet(a_m => a_m.Response).Returns(_responseMock.Object);
            var _target = new RestContollerMock();
            _target.SetIsContainer(false);
            _target.Init();
            var _requestValidatorMock = new Mock<IHttpRequestValidator>();
            _requestValidatorMock.Setup(a_m => a_m.Validate(_requestMock.Object)).Returns(true);
            _target.AddRule("POST", _requestValidatorMock.Object);
            var _actual = false;
            _target.PostCelled +=
                delegate(object a_sender, UniversalEventArgs<IHttpMessage> a_context)
                {
                    _actual = (a_context.Target == _messageMock.Object);
                };

            _target.Proccess(_messageMock.Object);

            Assert.IsTrue(_actual);
        }

        [TestMethod]
        public void ProccessInvokeInternalAfterValidate_test()
        {
            var _requestMock = new Mock<IHttpRequest>();
            _requestMock.SetupGet(a_m => a_m.Url).Returns(new Uri("http://some.net/"));
            _requestMock.SetupGet(a_m => a_m.HttpMethod).Returns("GET");
            var _messageMock = new Mock<IHttpMessage>();
            _messageMock.SetupGet(a_m => a_m.Request).Returns(_requestMock.Object);
            var _responseMock = new Mock<IHttpResponse>();
            _messageMock.SetupGet(a_m => a_m.Response).Returns(_responseMock.Object);
            var _target = new RestContollerMock();
            _target.SetIsContainer(false);
            _target.Init();
            var _requestValidatorMock = new Mock<IHttpRequestValidator>();
            _requestValidatorMock.Setup(a_m => a_m.Validate(_requestMock.Object)).Returns(true);
            _target.AddRule("GET", _requestValidatorMock.Object);
            var _actual = false;
            _target.AfterValidateCelled +=
                delegate(object a_sender, UniversalEventArgs<IHttpRequest> a_request)
                {
                    _actual = (a_request.Target == _requestMock.Object);
                };

            _target.Proccess(_messageMock.Object);

            // После валидации должен вызваться метод afterValidate
            Assert.IsTrue(_actual);
        }

        private class RestContollerMock : RestController
        {
            public override string Section
            {
                get { return ""; }
            }

            protected override int level
            {
                get { return 0; }
            }

            private IEnumerable<IRestController> h_controllers;

            protected override IEnumerable<IRestController> controllers
            {
                get { return h_controllers; }
            }

            public void SetControllers(IEnumerable<IRestController> a_controllers)
            {
                h_controllers = a_controllers;
            }

            private bool h_isContainer;

            protected override bool isContainer
            {
                get { return h_isContainer; }
            }

            public void SetIsContainer(bool a_value)
            {
                h_isContainer = a_value;
            }

            private IRestController h_contentRestController;

            protected override IRestController contentRestController
            {
                get { return h_contentRestController; }
            }

            public void SetContentRestController(IRestController a_controller)
            {
                h_contentRestController = a_controller;
            }

            public void AddRule(string a_httpMethod, IHttpRequestValidator a_requestValidator)
            {
                rules[a_httpMethod].Add(a_requestValidator);
            }

            public event EventHandler<UniversalEventArgs<IHttpMessage>> GetCelled;

            protected override void get(IHttpMessage a_context)
            {
                base.get(a_context);
                if (GetCelled != null)
                    GetCelled.Invoke(this, new UniversalEventArgs<IHttpMessage>(a_context));
            }

            public event EventHandler<UniversalEventArgs<IHttpMessage>> PutCelled;

            protected override void put(IHttpMessage a_context)
            {
                base.put(a_context);
                if (PutCelled != null)
                    PutCelled.Invoke(this, new UniversalEventArgs<IHttpMessage>(a_context));
            }

            public event EventHandler<UniversalEventArgs<IHttpMessage>> PostCelled;

            protected override void post(IHttpMessage a_context)
            {
                base.post(a_context);
                if (PostCelled != null)
                    PostCelled.Invoke(this, new UniversalEventArgs<IHttpMessage>(a_context));
            }

            public event EventHandler<UniversalEventArgs<IHttpRequest>> AfterValidateCelled;

            protected override void afterValidate(IHttpRequest a_request)
            {
                base.afterValidate(a_request);
                if (AfterValidateCelled != null)
                    AfterValidateCelled.Invoke(this, new UniversalEventArgs<IHttpRequest>(a_request));
            }
        }
    }
}