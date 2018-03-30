using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TVSPlayerServer
{
    class User{

        private static List<User> Users { get; set; } = new List<User>();

        public short Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime LastLogin { get; set; }
        public string LastLoginIP { get; set; }
        public List<UserDevice> Devices { get; set; } = new List<UserDevice>();

        [JsonConstructor]
        public User() { }

        public User(string userName, string password) {
            UserName = userName;
            SetPassword(password);
        }

        public void SetPassword(string password) {
            using (SHA512 sha = SHA512.Create()) {
                var hashedBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                Password = hash;
            }
        }

        public void AddDevice(string ipAddress) {
            Devices.Add(UserDevice.Create(this,ipAddress));
        }

        public UserDevice GetDevice(string macAddress) {
            return Devices.Where(x => x.MacAddress == macAddress).FirstOrDefault();
        }

        #region Static Methods

        public async static void LoadUsers() {
            Users = await DatabaseFiles.Read<List<User>>("Users");
        }

        public static User GetUser(short id) {
            return Users.FirstOrDefault(x => x.Id == id);
        }

        public static User GetUser(string username) {
            return Users.FirstOrDefault(x => x.UserName == username);
        }

        public static User GetUserByToken(string token) {
            short id = BitConverter.ToInt16(Convert.FromBase64String(token.Substring(60)), 0);
            return GetUser(id);
        }

        public static List<User> GetUsers() {
            return Users;
        }

        public static void AddUser(User user) {
            Users.Add(user);
            DatabaseFiles.Write("Users", Users);
        }

        public static void EditUser(User user) {
            var orig = Users.SingleOrDefault(x => x.Id == user.Id);
            if (orig != null) {
                Users.RemoveAll(x => x.Id == user.Id);
                Users.Add(user);
                DatabaseFiles.Write("Users", Users);
            }
        }

        public static void DeleteUser(User user) {
            var orig = Users.SingleOrDefault(x => x.Id == user.Id);
            if (orig != null) {
                Users.RemoveAll(x => x.Id == user.Id);
                DatabaseFiles.Write("Users", Users);
            }
        }

        #endregion

        public class UserDevice {
            public string MacAddress { get; set; }
            public string Token { get; set; }
            public static UserDevice Create(User user,string ip) {
                return new UserDevice {
                    Token = GenerateToken(user.Id),
                    MacAddress = Helper.GetMacAddress(ip)
                };
            }

            private static string GenerateToken(short id) {
                var rnd = RandomNumberGenerator.Create();
                string password = "";
                char[] randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-".ToCharArray();
                for (int i = 0; i < 60; i++) {
                    byte[] data = new byte[4];
                    rnd.GetBytes(data);
                    int generatedValue = Math.Abs(BitConverter.ToInt32(data, 0));
                    password += randomChars[generatedValue % randomChars.Length];
                }
                password += Convert.ToBase64String(BitConverter.GetBytes(id));
                return password;
            }
        }

        
    }
}
