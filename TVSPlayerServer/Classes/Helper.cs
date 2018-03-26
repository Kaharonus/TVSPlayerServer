using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Threading;
using System.Net.Sockets;

namespace TVSPlayerServer
{
    public static class Extentions {

        public static string ChopOffBefore(this string s, string Before) {//Usefull function for chopping up strings
            int End = s.ToUpper().IndexOf(Before.ToUpper());
            if (End > -1) {
                return s.Substring(End + Before.Length);
            }
            return s;
        }



        public static string ChopOffAfter(this string s, string After) {//Usefull function for chopping up strings
            int End = s.ToUpper().IndexOf(After.ToUpper());
            if (End > -1) {
                return s.Substring(0, End);
            }
            return s;
        }

        public static string ReplaceIgnoreCase(this string Source, string Pattern, string Replacement) {// using \\$ in the pattern will screw this regex up
                                                                                                        //return Regex.Replace(Source, Pattern, Replacement, RegexOptions.IgnoreCase);

            if (Regex.IsMatch(Source, Pattern, RegexOptions.IgnoreCase))
                Source = Regex.Replace(Source, Pattern, Replacement, RegexOptions.IgnoreCase);
            return Source;
        }

    }
    public static class Helper {


        public static string GetMyIP() {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        /// <summary>
        /// Returns your IP
        /// </summary>
        /// <param name="ignoreThis">Does not matter what you enter here. I just had to combat C#s inability to overload functions by return value</param>
        /// <returns></returns>
        public static IPAddress GetMyIP(int ignoreThis = 0) {
            IPAddress localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address;
            }
            return localIP;
        }

        public static PhysicalAddress GetMacAddress(string ip) {
            if (ip == GetMyIP()) {
                return NetworkInterface.GetAllNetworkInterfaces().Single(x => x.GetIPProperties().UnicastAddresses.Where(y => y.Address.ToString() == ip).Count() > 0).GetPhysicalAddress();
            } else {
                Process pProcess = new Process();
                pProcess.StartInfo.FileName = "arp";
                pProcess.StartInfo.Arguments = "-a ";
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.CreateNoWindow = true;
                pProcess.Start();
                string cmdOutput = pProcess.StandardOutput.ReadToEnd();
                string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";
                var result = Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase).Cast<Match>().Where(x => x.Groups["ip"].Value == ip).FirstOrDefault();
                if (result != null) {
                    return PhysicalAddress.Parse(result.Groups["mac"].Value.ToUpper());
                } else {
                    return PhysicalAddress.None;
                }
            }
           
        }
       
    }
}
