using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttpListener = System.Net.Http.HttpListener;
using NETStandard.HttpListener;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TVSPlayerServer
{
    class API {

        public int Port { get; set; }
        public string IP { get; set; }
        public bool IsRunning { get; set; }
        private HttpListener Listener { get; set; }

        public API(int port) {
            Port = port;
            IsRunning = false;
            IP = Helper.GetMyIP();

        }


        public void Stop() {
            Listener.Close();
            ConsoleLog.WriteLine("API Stopped");

        }

        public void Start() {
            if (Listener == null) {
                Listener = new HttpListener(System.Net.IPAddress.Parse(IP), Port);
                Listener.Request += (s, ev) => Listen(ev);
            }
            Listener.Start();
            ConsoleLog.WriteLine("API Started @ " + IP + ":" + Port);
        }

        public void Listen(HttpListenerRequestEventArgs context) {
            Task.Run(async () => {
                var request = context.Request;
                var response = context.Response;
                var match = Regex.Match(request.Url.LocalPath, "/register/?");
                if (match.Success && request.HttpMethod == HttpMethods.Post) {
                    Auth.RegisterUser(request, response);
                } else if (Regex.Match(request.Url.LocalPath, "/login/?").Success && request.HttpMethod == HttpMethods.Post) {
                    Auth.LoginUser(request, response);
                } else if (Auth.IsAuthorized(request, out User user)) {
                    if (request.HttpMethod == HttpMethod.Get.Method) {
                        response = HandleGet(request, response, user);
                    } else if (request.HttpMethod == HttpMethod.Post.Method) {
                        response = HandlePost(request, response, user);
                    } else {
                        response.MethodNotAllowed();
                    }

                } else {
                    response.Forbidden();
                }
                response.Close();
            });
        }

        private static HttpListenerResponse HandleGet(HttpListenerRequest request, HttpListenerResponse response, User user){
            var result = RequestParser.ParseGet(request, user);
            if (result != null) {
                StreamWriter writer = new StreamWriter(response.OutputStream);
                writer.Write(result);
                writer.Flush();
                ConsoleLog.WriteLine("GET Request recieved from " + request.RemoteEndpoint.Address.ToString());
            } else {
                response.NotFound();
            }
            return response;
        }
        private static HttpListenerResponse HandlePost(HttpListenerRequest request, HttpListenerResponse response, User user) {
            var result = RequestParser.ParsePost(request, user);
            if (result != null) {
                StreamWriter writer = new StreamWriter(response.OutputStream);
                writer.Write(result);
                writer.Flush();
                ConsoleLog.WriteLine("POST Request recieved from " + request.RemoteEndpoint.Address.ToString());
            } else {
                response.NotFound();
            }
            return response;
        }
    }
}
