using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace TVSPlayerServer
{
    static class Auth {
        public static bool IsAuthorized(HttpListenerRequest request) {
            /*if (request.RemoteEndpoint.AddressFamily != AddressFamily.InterNetwork || request.RemoteEndpoint.AddressFamily != AddressFamily.InterNetworkV6) {         
                return false;
            } else {
                return true;
            }*/
            return true;
        }

        public static void RegisterUser(HttpListenerRequest request, HttpListenerResponse response) {
            List<string> phrases = new List<string>() {
                "Enter user name and password in format {\"username\": \"string\",\"password\": \"string\"}",
                "Username or password too short minimum lenght for both is 5 characters",
                "Username must only contain letters a-z, A-Z and numbers 0-9"
            };
            int errorIndex = 0;
            var str = request.InputStream;
            if (request.InputStream != null) {
                string content = new StreamReader(str).ReadToEnd();
                if (!String.IsNullOrEmpty(content)) {
                    var parsed = JsonConvert.DeserializeObject<UserRequest>(content);
                    if (!String.IsNullOrEmpty(parsed.password) && !String.IsNullOrEmpty(parsed.username)) {
                        if (parsed.password.Length > 5 && parsed.username.Length > 3) {
                            if (Regex.Match(parsed.username, "^[a-zA-Z0-9]*$").Success) {
                                User u = new User(parsed.username, parsed.password);
                                u.AddDevice(request.RemoteEndpoint.Address.ToString());
                                User.AddUser(u);
                            } else {
                                errorIndex = 2;
                            }
                        } else {
                            errorIndex = 1;
                        }
                    } else {
                        errorIndex = 0;
                    }
                } else {
                    errorIndex = 0;
                }
            } else {
                errorIndex = 0;
            }

            response.ReasonPhrase = phrases[errorIndex];
            response.StatusCode = 400;
            response.Close();
                
        }

        public static void LoginUser(HttpListenerRequest request, HttpListenerResponse response) {

        }

        private class UserRequest{
            public string username;
            public string password;
        }
        

    }
}
