using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var users = User.GetUsers();
            var user = GetData(request);
            int userNameCount = users.Where(x => x.UserName.ToLower() == user.username.ToLower()).Count();
            if (userNameCount != 0) {
                user.isValid = false;
                user.errorPhrase = "Username already in use";
            }
            if (!SendBadResponse(response, user)) {
                User u = new User(user.username, user.password);
                Random rnd = new Random();
                short random = 0;
                while (true) {
                    random = (short)rnd.Next(short.MaxValue);
                    if (users.Where(x => x.Id == random).Count() == 0 && random > 0)
                        break;
                }
                u.Id = random;
                u.AddDevice(request.RemoteEndpoint.Address.ToString());
                User.AddUser(u);
                SendTokenResponse(response, u.Devices[0].Token);
            }
        }

        public static void LoginUser(HttpListenerRequest request, HttpListenerResponse response) {
            string ip = request.RemoteEndpoint.Address.ToString();
            var user = GetData(request);
            if (!SendBadResponse(response, user)) {
                var newUser = User.GetUser(user.username);
                var mac = Helper.GetMacAddress(ip);
                var device = newUser.GetDevice(mac);
                if (device == null) {
                    newUser.AddDevice(ip);
                    User.EditUser(newUser);
                    device = newUser.GetDevice(mac);
                    SendTokenResponse(response, device.Token);
                } else {
                    SendTokenResponse(response, device.Token);
                }

            }
        }

        private static void SendTokenResponse(HttpListenerResponse response, string token) {
            StreamWriter writer = new StreamWriter(response.OutputStream);
            writer.Write("{ \"token\": \"" + token + "\" }");
            response.StatusCode = 200;
        }

        private static bool SendBadResponse(HttpListenerResponse response, UserRequest user) {
            if (!user.isValid) {
                response.StatusCode = 400;
                response.ReasonPhrase = user.errorPhrase;
                return true;
            }
            return false;
        }


        private static UserRequest GetData(HttpListenerRequest request) {
            List<string> phrases = new List<string>() {
                "Enter user name and password in format {\"username\": \"string\",\"password\": \"string\"}",
                "Username or password too short minimum lenght for both is 5 characters",
                "Username must only contain letters a-z, A-Z and numbers 0-9",
                "Input can't be ampty. Enter username and password as POST data in format {\"username\": \"string\",\"password\": \"string\"}" };
            if (request.InputStream == null) {
                return new UserRequest(phrases[3]);
            }
            string content = new StreamReader(request.InputStream).ReadToEnd();
            if (String.IsNullOrEmpty(content)) {
                return new UserRequest(phrases[0]);
            }
            var parsed = JsonConvert.DeserializeObject<UserRequest>(content);
            if (String.IsNullOrEmpty(parsed.password) || String.IsNullOrEmpty(parsed.username)) {
                return new UserRequest(phrases[0]);
            }
            if (parsed.password.Length < 6 && parsed.username.Length < 4) {
                return new UserRequest(phrases[1]);
            }
            if (!Regex.Match(parsed.username, "^[a-zA-Z0-9]*$").Success) {
                return new UserRequest(phrases[2]);
            }
            return parsed;
        }

        private class UserRequest{
            [JsonConstructor]
            public UserRequest() {

            }

            public UserRequest(string username, string password) {
                isValid = true;
                this.username = username;
                this.password = password;
            }
            public UserRequest(string errorMessage) {
                isValid = false;
                errorPhrase = errorMessage;
            }
            public bool isValid = true;
            public string username;
            public string password;
            public string errorPhrase;
        }

    }
}
