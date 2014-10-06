using System.IO;
using AbaSoft.Net.Validators;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AbaSoft.Net.Rest
{
    public abstract class RestController : IRestController
    {
        protected readonly Dictionary<string, List<IHttpRequestValidator>> rules;

        private  Dictionary<string,IRestController> dicControllers;

        protected RestController()
        {
            rules = new Dictionary<string, List<IHttpRequestValidator>>
            {
                {WebRequestMethods.Http.Get, new List<IHttpRequestValidator>()},
                {WebRequestMethods.Http.Post, new List<IHttpRequestValidator>()},
                {WebRequestMethods.Http.Put, new List<IHttpRequestValidator>()},
            };
        }

        public abstract string Section { get; }

        protected abstract int level { get; }

        protected abstract IEnumerable<IRestController> controllers { get; }

        protected abstract bool isContainer { get; }

        protected abstract IRestController contentRestController { get; }

        public virtual void Init()
        {
            dicControllers = new Dictionary<string, IRestController>();

            if (isContainer)
            {
                contentRestController.Init();
            }
            else
            {
                if (controllers != null)
                {
                    foreach (var _controller in controllers)
                    {
                        _controller.Init();
                        dicControllers.Add(_controller.Section, _controller);
                    }
                }
            }
        }

        public void Proccess(IHttpMessage a_context)
        {
            var _request = a_context.Request;
            // Example: /section1/section2 - length=3
            // Example: /section1/section2/ - length=3
            // Example: /section1/section2/section3 - length=4
            var _segments = _request.Url.Segments;
            // Если в пути есть вложенные уровни
            if (_segments.Length > (level + 1))
            {
                // Если это контейнер
                if (isContainer)
                {
                    // перенаправляем запрос контроллеру сущности
                    contentRestController.Proccess(a_context);
                }
                else
                {
                    // иначе перенаправляем запрос вложенным контроллерам
                    var _subSection = _segments[level + 1].TrimEnd('/');
                    if (dicControllers.ContainsKey(_subSection))
                    {
                        dicControllers[_subSection].Proccess(a_context);
                    }
                    else
                    {
                        setCode(a_context.Response, HttpStatusCode.BadRequest);
                    }
                }
            }
            else
            {
                // иначе этот запрос данному контроллеру
                proccessInThisController(a_context);
            }
        }

        private void proccessInThisController(IHttpMessage a_context)
        {
            var _request = a_context.Request;
            var _response = a_context.Response;
            var _method = _request.HttpMethod;

            HttpStatusCode _errorStatusCode;
            string _errorMessage;
            if (valid(_request,out _errorStatusCode, out _errorMessage))
            {
                afterValidate(_request);
                switch (_method)
                {
                    case WebRequestMethods.Http.Get:
                        get(a_context);
                        break;
                    case WebRequestMethods.Http.Put:
                        put(a_context);
                        break;
                    case WebRequestMethods.Http.Post:
                        post(a_context);
                        break;
                }
            }
            else
            {
                setCode(_response, _errorStatusCode);
            }
        }

        protected virtual void afterValidate(IHttpRequest a_request)
        {
        }

        private bool valid(IHttpRequest a_request, out HttpStatusCode a_errorStutusCode,out string a_errorMessage)
        {
            foreach (var _httpRequesValidator in rules[a_request.HttpMethod])
            {
                if (!_httpRequesValidator.Validate(a_request))
                {
                    a_errorStutusCode = _httpRequesValidator.ErrorStatusCode;
                    a_errorMessage = _httpRequesValidator.ErrorMessage;
                    return false;
                }
            }
            a_errorStutusCode = HttpStatusCode.OK;
            a_errorMessage = string.Empty;
            return true;
        }

        protected virtual void get(IHttpMessage a_context)
        {
            setCode(a_context.Response, HttpStatusCode.BadRequest);
        }

        protected virtual void post(IHttpMessage a_context)
        {
            setCode(a_context.Response, HttpStatusCode.BadRequest);
        }

        protected virtual void put(IHttpMessage a_context)
        {
            setCode(a_context.Response, HttpStatusCode.BadRequest);
        }

        protected static void setCode(IHttpResponse a_response, HttpStatusCode a_statusCode)
        {
            a_response.StatusCode = a_statusCode;
        }

        protected static void setContent(IHttpResponse a_response, byte[] a_buffer)
        {
            a_response.ContentLength64 = a_buffer.Length;
            try
            {
                var _output = a_response.OutputStream;
                _output.Write(a_buffer, 0, a_buffer.Length);
                _output.Close();
            }
            catch
            {
                throw new HttpListenerException((int) HttpStatusCode.RequestTimeout);
            }
        }

        protected static void setContent(IHttpResponse a_response, Stream a_contentStream)
        {
            try
            {
                a_contentStream.CopyTo(a_response.OutputStream);
                a_response.ContentLength64 = a_contentStream.Length;
            }
            catch
            {
                throw new HttpListenerException((int) HttpStatusCode.RequestTimeout);
            }
        }
    }
}