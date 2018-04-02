using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TVSPlayerServer.Classes.Database;


namespace TVSPlayerServer
{
    class GetMethods{

        public static List<Series> GetSeries(User user) {
            return Database.GetSeries().Where(x => x.UserIds.Contains(user.Id)).ToList();
        }

        public static Series GetSeries(int seriesId, User user) {
            var data = Database.GetSeries().Where(x => x.Id == seriesId).FirstOrDefault();
            if (data == null || String.IsNullOrEmpty(data.ImdbId)) {
                data = Series.GetSeries(seriesId);
            }
            return data;
        }

        public static List<Series> SearchSeries(string query, User user) {
            return Series.Search(query);
        }
       
    }
}
