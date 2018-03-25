using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace TVSPlayerServer
{
    static class Auth{
        public static bool IsAuthorized(HttpListenerRequest request) {
            /*if (request.RemoteEndpoint.AddressFamily != AddressFamily.InterNetwork || request.RemoteEndpoint.AddressFamily != AddressFamily.InterNetworkV6) {         
                return false;
            } else {
                return true;
            }*/
            return true;
        }

        public static void RegisterUser(HttpListenerRequest request, HttpListenerResponse response) {
            User u = new User("test", "test");
            DatabaseFiles.Read("");
        }

        

    }
}
