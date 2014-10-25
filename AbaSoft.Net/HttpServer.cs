using System;
using System.Linq;
using System.Net;
using System.Threading;
using NLog;
using Tools;

namespace AbaSoft.Net
{
    public class HttpServer : IHttpServer
    {
        public event EventHandler<UniversalEventArgs<IHttpMessage>> MessageReceived;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly HttpListener listener;
        private readonly Thread thread;

        public HttpServer(string[] a_prefixes)
        {
            logger.Debug("Инициализация сервера по префиксам: {0}", string.Join("; ", a_prefixes));

            listener = new HttpListener();
            foreach (var _prefix in a_prefixes)
                listener.Prefixes.Add(_prefix);

            thread = new Thread(method);

            ShowHeadersInLog = false;
            SendAcaHeaders = true;
        }

        public bool ShowHeadersInLog { get; set; }

        public bool SendAcaHeaders { get; set; }

        public void Start()
        {
            logger.Debug("Запуск сервера");

            thread.Start();

            logger.Debug("Сервер запущен");
        }

        public void Stop()
        {
            logger.Debug("Остановка сервера");

            //listener.Stop();
            thread.Abort();

            logger.Debug("Сервер остановлен");
        }

        private void method()
        {
            listener.Start();

            while (true)
            {
                var _result = listener.BeginGetContext(listenerCallback, listener);
                _result.AsyncWaitHandle.WaitOne();
            }
        }

        private readonly object listnerCallbackLockObject = new object();

        private void listenerCallback(IAsyncResult a_result)
        {
            lock (listnerCallbackLockObject)
            {
                if (MessageReceived != null)
                {
                    var _listener = (HttpListener) a_result.AsyncState;
                    var _context = _listener.EndGetContext(a_result);
                    var _msg = HttpMessage.Create(_context);

                    logRequest(_msg.Request, ShowHeadersInLog);

                    var _response = (HttpResponse) _msg.Response;

                    if (SendAcaHeaders)
                    {
                        _response.AddHeader("Access-Control-Allow-Origin", "*");
                        _response.AddHeader("Access-Control-Allow-Headers", "Authorization, Content-Type");
                        _response.AddHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT");
                    }

                    // OPTIONS запросы не должны обрабатываться
                    if (_msg.Request.HttpMethod != "OPTIONS" && MessageReceived != null)
                    {
                        try
                        {
                            MessageReceived.Invoke(this, new UniversalEventArgs<IHttpMessage>(_msg));
                        }
                        catch (HttpListenerException _httpListenerException)
                        {
                            _response.StatusCode = (HttpStatusCode) _httpListenerException.ErrorCode;
                        }
                    }

                    logResponse(_msg.Response);

                    _response.Close();
                }
            }
        }

        private static void logResponse(IHttpResponse a_response)
        {
            logger.Debug("Response[{0}]: ContentLength:{1}, ErrorStatusCode:{2}, StatusDescription:{3}",
                a_response.RequestTraceIdentifier.ToString().Remove(0, 19),
                a_response.ContentLength64, a_response.StatusCode, a_response.StatusDescription);
        }

        private void logRequest(IHttpRequest a_request, bool a_showHeaders)
        {
            var _headers = a_showHeaders
                ? a_request.Headers.AllKeys.Select(a_headerKey => string.Format("{0}: {1}", a_headerKey, a_request.Headers[a_headerKey]))
                : new string[0];

            logger.Debug("Request[{0}]: Uri:{1}, IP:{2} Method:{3}{4}",
                a_request.RequestTraceIdentifier.ToString().Remove(0, 19),
                a_request.Url, "NONE", a_request.HttpMethod, a_showHeaders ? " \nHeaders:\n" + string.Join("\n", _headers) : "");
        }
    }
}
