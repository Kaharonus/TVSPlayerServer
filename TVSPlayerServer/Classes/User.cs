using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace TVSPlayerServer
{
    class User{
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; private set; }
        public DateTime LastLogin { get; set; }
        public string LastLoginIP { get; set; }
        List<UserDevice> Devices { get; set; } = new List<UserDevice>();

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
            Devices.Add(UserDevice.Create(ipAddress));
        }


        class UserDevice {
            public PhysicalAddress MacAddress { get; set; }
            public string Token { get; set; }
            public static UserDevice Create(string ip) {
                return new UserDevice {
                    Token = GenerateToken(),
                    MacAddress = Helper.GetMacAddress(ip)
                };
            }

            private static string GenerateToken() {
                var rnd = RandomNumberGenerator.Create();
                string password = "";
                char[] randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-".ToCharArray();
                for (int i = 0; i < 32; i++) {
                    byte[] data = new byte[4];
                    rnd.GetBytes(data);
                    int generatedValue = Math.Abs(BitConverter.ToInt32(data, 0));
                    password += randomChars[generatedValue % randomChars.Length];
                }
                return password;
            }
        }

        
    }
}
