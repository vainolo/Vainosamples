using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IWAAuthWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(System.Console.Out));
            IWASite site = new IWASite();
            Task.Run(() => { site.start(); });
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
    class IWASite
    {
        private HttpListener listener;
        public void start()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://+:9090/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Negotiate;
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
                //                response.Headers.Add("Access-Control-Allow-Origin", "*");


                PrincipalContext pc = new PrincipalContext(ContextType.Domain);
                var identity = UserPrincipal.FindByIdentity(pc, context.User.Identity.Name);
                Trace.TraceInformation(identity.UserPrincipalName);

                response.ContentLength64 = Encoding.UTF8.GetBytes(identity.UserPrincipalName).Length;
                byte[] buffer = Encoding.UTF8.GetBytes(identity.UserPrincipalName);
                StreamWriter writer = new StreamWriter(response.OutputStream);
                writer.Write(identity.UserPrincipalName);
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