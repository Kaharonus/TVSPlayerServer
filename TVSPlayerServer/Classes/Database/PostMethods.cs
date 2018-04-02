using System;
using System.Collections.Generic;
using System.Text;
using TVSPlayerServer.Classes.Database;
using System.Linq;
using static TVSPlayerServer.GetMethods;
using System.Threading.Tasks;

namespace TVSPlayerServer
{
    class PostMethods {

        public static string AddSeries(int seriesId, object json, User user) {
            var series = Database.Data.Select(x => x.Series).Where(x => x.Id == seriesId).FirstOrDefault();
            if (series == null) {
                List<Task<object>> tasks = new List<Task<object>>();
                tasks.Add(Task.Run(() => (object)Series.GetSeries(seriesId)));
                tasks.Add(Task.Run(() => (object)Episode.GetEpisodes(seriesId)));
                tasks.Add(Task.Run(() => (object)Actor.GetActors(seriesId)));
                tasks.Add(Task.Run(() => (object)Poster.GetPosters(seriesId)));
                tasks.WaitAll();
                Database db = new Database();
                var s = (Series)tasks[0].Result;
                s.UserIds.Add(user.Id);
                db.Series = s;
                db.Episodes = (List<Episode>)tasks[1].Result;
                db.Actors = (List<Actor>)tasks[2].Result;
                db.Posters = (List<Poster>)tasks[3].Result;
                Database.AddItem(db);
            } else {
                if (!series.UserIds.Contains(user.Id)) {
                    series.UserIds.Add(user.Id);
                    EditSeries(series);
                }
            }
            return "Added";
        }

        private static void EditSeries(Series series) {
            var data = Database.Data;
            int index = data.IndexOf(data.Where(x => x.Series.Id == series.Id).FirstOrDefault());
            if (index != -1) {
                Database.Data[index].Series = series;
                DatabaseFiles.Write("Series", Database.GetSeries());
            }
     
        }

        public static string RemoveSeries(int seriesId, object json, User user) {
            int index = Database.Data.IndexOf(Database.Data.Where(x => x.Series.Id == seriesId).FirstOrDefault());
            if (index != -1) { 
                if (Database.Data[index].Series.UserIds.Count > 1) {
                    Database.Data[index].Series.UserIds.Remove(seriesId);
                    DatabaseFiles.Write("Series", Database.GetSeries());
                } else {
                    Database.RemoveItem(Database.Data[index]);
                }
            }
            return "Removed";
        }
    }
}
