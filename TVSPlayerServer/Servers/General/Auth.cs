using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static bool IsAuthorized(HttpListenerRequest request, out User user) {
            if (IsRequestLocal(request)) {
                var ip = request.RemoteEndpoint.Address.ToString();
                var token = GetToken(request);
                if (token != null) {
                    var usr = User.GetUserByToken(token);
                    var device = usr?.Devices.Where(x => x.Token == token).FirstOrDefault();
                    if (device != null) {
                        if ((Settings.SecurityLevel == 1 && usr.LastLogin > DateTime.Now.AddHours(-1)) || Settings.SecurityLevel == 2) {
                            if (device.MacAddress == Helper.GetMacAddress(ip)) {
                                user = EditUser(usr, ip);
                                return true;
                            }
                        } else {
                            user = EditUser(usr, ip);
                            return true;
                        }
                    }
                }
            }
            if (Settings.LoggingLevel == 2) {
                ConsoleLog.WriteLine("Request was not authorized from " + request.RemoteEndpoint.Address);
            }
            user = null;
            return false;

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
                ConsoleLog.WriteLine("Successful register attempt from " + request.RemoteEndpoint.Address.ToString());
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
            } else if (Settings.LoggingLevel == 2) {
                ConsoleLog.WriteLine("Unsuccessful register attempt from " + request.RemoteEndpoint.Address.ToString());
            }
        }

        private static User EditUser(User usr, string ip) {
            usr.LastLogin = DateTime.Now;
            usr.LastLoginIP = ip;
            User.EditUser(usr);
            return usr;
        }

        public static void LoginUser(HttpListenerRequest request, HttpListenerResponse response) {
            string ip = request.RemoteEndpoint.Address.ToString();
            var user = GetData(request);
            if (!SendBadResponse(response, user)) {
                var newUser = User.GetUser(user.username);
                if (GetHash(user.password) == newUser.Password) {
                    ConsoleLog.WriteLine("Successful log in attempt from " + request.RemoteEndpoint.Address.ToString());
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
                } else {
                    user.isValid = false;
                    user.errorPhrase = "Bad password";
                    SendBadResponse(response, user);
                }            
            } else if(Settings.LoggingLevel == 2) {
                ConsoleLog.WriteLine("Unsuccessful log in attempt from " + request.RemoteEndpoint.Address.ToString());
            }
        }

        private static void SendTokenResponse(HttpListenerResponse response, string token) {
            StreamWriter writer = new StreamWriter(response.OutputStream);
            writer.Write(token);
            writer.Flush();
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

        public static bool IsRequestLocal(HttpListenerRequest request) {
            return (request.RemoteEndpoint.AddressFamily != AddressFamily.InterNetwork || request.RemoteEndpoint.AddressFamily != AddressFamily.InterNetworkV6);
        }

        private static string GetToken(HttpListenerRequest request) {
            if (!request.Headers.ContainsKey("Token")) {
                return null;
            }
            string token = request.Headers["Token"];
            if (token.Length != 64) {
                return null;
            }
            return token;
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

        private static string GetHash(string text) {
            using (SHA512 sha = SHA512.Create()) {
                var hashedBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hash;
            }
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
