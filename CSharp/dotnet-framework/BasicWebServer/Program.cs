using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(System.Console.Out));
            BasicWebServer server = new BasicWebServer();
            Task.Run(() => { server.start(); });
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }

        }
    }
    class BasicWebServer
    {
        private HttpListener listener;
        public void start()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:9090/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Ntlm;
            listener.Start();
            Console.WriteLine("Listener 9090 started");
            handleRequests();
        }

        private void handleRequests()
        {
            IAsyncResult result = listener.BeginGetContext(ListenerCallback, listener);
            Console.WriteLine("Listening...");
            while (true)
            {
                result.AsyncWaitHandle.WaitOne();
                Console.WriteLine("Request processed asyncronously.");
                result = listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
            }
            listener.Close();
        }
        public void ListenerCallback(IAsyncResult result)
        {
            try
            {
                HttpListener listener = result.AsyncState as HttpListener;
                if (!listener.IsListening)
                    return; ;
                HttpListenerContext context = listener.EndGetContext(result);
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                //response.ContentLength64 = Encoding.UTF8.GetBytes(identity.UserPrincipalName).Length;
                //byte[] buffer = Encoding.UTF8.GetBytes(identity.UserPrincipalName);
                StreamWriter writer = new StreamWriter(response.OutputStream);
                writer.Write("Hello");
                writer.Close();
                response.Close();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }
    }
}
