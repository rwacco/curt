using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace curt
{
    internal class WebAPIServer
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private BotData _botData;
        private HttpListener _listener;

        public WebAPIServer(IPAddress ipAddress, int port, BotData data)
        {
            _ipAddress = ipAddress;
            _port = port;
            _botData = data;

            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:1234/");
            _listener.Start();
            Console.WriteLine("Listening for connections on http://localhost:1234/");

        }

        public async Task StartServer()
        {
            bool runServer = true;

            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await _listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                // Write the response info
                string disableSubmit = !runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes("Test");
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }
    }

    namespace WebAPI.Models
    {
        public class MOTD
        {
            public string text { get; }
        }
    }
}
