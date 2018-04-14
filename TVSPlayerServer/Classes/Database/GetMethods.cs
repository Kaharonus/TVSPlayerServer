using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TVSPlayerServer.Classes.Database;


namespace TVSPlayerServer
{
    class GetMethods{

        public static List<Series> SearchSeries(string query, User user) {
            return Series.Search(query);
        }

        public static List<Series> GetSeries(User user) {
            var fullSeries = Database.GetSeries();
            var series = new List<Series>();
            foreach (var ser in fullSeries) {
                var data = ser.AllUserSettings.Where(x => x.UserId == user.Id).FirstOrDefault();
                if (data != null) {
                    ser.userSettings = data;
                    ser.AllUserSettings = null;
                    series.Add(ser);
                }
            }
            return series;
        }

        public static Series GetSeries(int seriesId, User user) {
            var data = Database.GetSeries().Where(x => x.Id == seriesId).FirstOrDefault();
            if (data == null) {
                data = Series.GetSeries(seriesId);
            } else {
                var set = data.AllUserSettings.Where(x => x.UserId == user.Id).FirstOrDefault();
                if (set != null) {
                    data.userSettings = set;
                }
                data.AllUserSettings = null;
            }
            return data;
        }

        public static List<Episode> GetEpisodes(int seriesId, User user) {
            var data = Database.Data.Where(x => x.Series.Id == seriesId).FirstOrDefault();
            if (data != null) {
                foreach (var ep in data.Episodes) {
                    var set = ep.AllUserSettings.Where(x => x.UserId == user.Id).FirstOrDefault();
                    if (set != null) {
                        ep.userSettings = set;
                    }
                    ep.AllUserSettings = null;
                }
                return data.Episodes;
            }
            return new List<Episode>();
        }

        public static Episode GetEpisode(int seriesId, int episodeId, User user) {
            var data = Database.Data.Where(x => x.Series.Id == seriesId).FirstOrDefault();
            if (data != null) {
                var ep = data.Episodes.Where(x => x.Id == episodeId).FirstOrDefault();
                if (ep != null) {
                    var set = ep.AllUserSettings.Where(x => x.UserId == user.Id).FirstOrDefault();
                    if (set != null) {
                        ep.userSettings = set;
                    }
                    ep.AllUserSettings = null;
                    return ep;
                }
            }
            return new Episode();
        }
    }
}
