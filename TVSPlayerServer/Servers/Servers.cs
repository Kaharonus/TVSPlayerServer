using System;
using System.Collections.Generic;
using System.Text;

namespace TVSPlayerServer
{
    class Servers{
        public static MediaServer Media { get; set; }
        public static API Api { get; set; }

        public static void LoadMedia(int port) {
            Media = new MediaServer(port, @"E:\");
        }
        public static void LoadApi(int port) {
            Api = new API(port);
        }
    }
}
