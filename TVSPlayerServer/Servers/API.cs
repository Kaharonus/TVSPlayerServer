﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttpListener = System.Net.Http.HttpListener;
using NETStandard.HttpListener;

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
        }

        public void Start() {
            if (Listener == null) {
                Listener = new HttpListener(IPAddress.Parse(IP), Port);
                Listener.Request += (s, ev) => Listen(ev);
            }
            Listener.Start();
        }

        public void Listen(HttpListenerRequestEventArgs context) {
            Task.Run(async () => {
                var request = context.Request;
                var response = context.Response;
                //Create auth methods
                if (true) {
                    if (request.HttpMethod == HttpMethod.Get.Method) {                      
                        ConsoleLog.WriteLine("GET Request recieved from " + request.RemoteEndpoint.Address.ToString());
                    } else if (request.HttpMethod == HttpMethod.Post.Method) {
                        ConsoleLog.WriteLine("POST Request recieved from " + request.RemoteEndpoint.Address.ToString());
                    } else {
                        response.MethodNotAllowed();
                    }
                } else {
                    response.Forbidden();
                }
                response.Close();
            });
        }        

    }
}