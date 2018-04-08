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
            var series = Database.GetSeries().Where(x => x.UserIds.Contains(user.Id)).ToList();
            series.ForEach(x => x.UserIds = new List<int>());
            return series;
        }

        public static Series GetSeries(int seriesId, User user) {
            var data = Database.GetSeries().Where(x => x.Id == seriesId).FirstOrDefault();
            if (data == null) {
                data = Series.GetSeries(seriesId);
            }
            data.UserIds = new List<int>();
            return data;
        }

        public static List<Episode> GetEpisode(int seriesId, User user) {
            var data = Database.Data.Where(x => x.Series.Id == seriesId).FirstOrDefault();
            if (data != null) {
                foreach (var ep in data.Episodes) {
                    ep.UserTimeStamp = ep.AllUserTimeStamp.TryGetValue(user.Id, out long value) ? value : 0;
                    ep.AllUserTimeStamp = new Dictionary<short, long>();
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
                    ep.UserTimeStamp = ep.AllUserTimeStamp.TryGetValue(user.Id, out long value) ? value : 0;
                    ep.AllUserTimeStamp = new Dictionary<short, long>();
                    return ep;
                }
            }
            return new Episode();
        }
    }
}
