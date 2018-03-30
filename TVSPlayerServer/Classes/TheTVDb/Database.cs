using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TVSPlayerServer
{
    class Database{
        private static List<Database> Data { get; set; } = new List<Database>();
        private Series Series { get; set; }

        public static List<Series> GetSeries(User user) {
            return Data.Select(x => x.Series).Where(x=>x.UserIds.Contains(user.Id)).ToList();
        }

        private static List<Series> GetSeries() {
           return Data.Select(x => x.Series).ToList();
        }

        public static Series GetSeries(int seriesId, User user) {
            var data = GetSeries().Where(x => x.Id == seriesId).FirstOrDefault();
            if (data == null) {
                data = Series.GetSeries(seriesId);
                Data.Add(new Database { Series = data });
            }
            return data;
        }

    }
    
}
