using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TVSPlayerServer
{
    class Database{
        private static List<Database> Data { get; set; } = new List<Database>();
        private Series Series { get; set; }
        private List<Episode> Episodes { get; set; }
        private List<Poster> Posters { get; set; }
        private List<Actor> Actors { get; set; }

        private static List<Series> GetSeries() {
            return Data.Select(x => x.Series).ToList();
        }

        public static List<Series> GetSeries(User user) {
            return GetSeries().Where(x=>x.UserIds.Contains(user.Id)).ToList();
        }

        public static Series GetSeries(int seriesId, User user) {
            var data = GetSeries().Where(x => x.Id == seriesId).FirstOrDefault();
            if (data == null || String.IsNullOrEmpty(data.ImdbId)) {
                data = Series.GetSeries(seriesId);
                Data.Add(new Database { Series = data });
            }
            return data;
        }

    }
    
}
