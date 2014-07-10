using System;
using System.Collections.Specialized;
using AbaSoft.Net.Rest;
using AbaSoft.Net.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbaSoft.Net;
using Moq;
using Tools;

namespace AbaSoft.Net_UnitTests
{
    [TestClass]
    public class RequiredHeaderHttpRequestValidator_tests
    {
        [TestMethod]
        public void Common_test()
        {
            var _target = new RequiredHeaderHttpRequestValidator("NameHeader");
            var _requestMock = new Mock<IHttpRequest>();
            var _headers = new NameValueCollection();
            _requestMock.SetupGet(a_m => a_m.Headers).Returns(_headers);

            Assert.IsFalse(_target.Validate(_requestMock.Object));

            _headers.Add("NameHeader", "Value");
            Assert.IsTrue(_target.Validate(_requestMock.Object));
        }
    }
}