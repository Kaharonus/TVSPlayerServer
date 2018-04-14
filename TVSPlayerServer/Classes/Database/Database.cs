using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace TVSPlayerServer.Classes.Database
{
    class Database{
        public static List<Database> Data { get; set; } = new List<Database>();

        public Series Series { get; set; } = new Series();
        public List<Episode> Episodes { get; set; } = new List<Episode>();
        public List<Poster> Posters { get; set; } = new List<Poster>();
        public List<Actor> Actors { get; set; } = new List<Actor>();
        public List<FileInformation> Files { get; set; } = new List<FileInformation>();

        public static void AddItem(Database db) {
            Data.Add(db);
        }
        public static void RemoveItem(Database db) {
            Data.Remove(db);
        }

        public static void EditItem(Database db) {
            var data = Data.Where(x => x.Series.Id == db.Series.Id).FirstOrDefault();
            if (data != null) {
                Data[Data.IndexOf(data)] = db;
            }
        }

        public static List<Series> GetSeries() {
            return Data.Select(x => x.Series).ToList();
        }

        public static int GetDefaultPosterId(int seriesId) {
            var data = Data.Where(x => x.Series.Id == seriesId).FirstOrDefault();
            if (data.Posters.Count > 0) {
                return data.Posters[0].Id;
            } else {
                return 0;
            }
        }


        public async static Task<Database> CreateDatabase(int seriesId) {
            List<Task<object>> tasks = new List<Task<object>> {
                Task.Run(() => (object)Series.GetSeries(seriesId)),
                Task.Run(() => (object)Episode.GetEpisodes(seriesId)),
                Task.Run(() => (object)Actor.GetActors(seriesId)),
                Task.Run(() => (object)Poster.GetPosters(seriesId))
            };
            await tasks.WaitAll();
            return new Database {
                Series = (Series)tasks[0].Result,
                Episodes = (List<Episode>)tasks[1].Result,
                Actors = (List<Actor>)tasks[2].Result,
                Posters = (List<Poster>)tasks[3].Result
            };
        }

        public async static Task LoadDatabase() {
            var series = (await DatabaseFiles.Read("Series")).ToObject<List<Series>>();
            series = series ?? new List<Series>();
            foreach (var item in (List<Series>)series) {
                Database db = new Database();
                db.Series = item;
                db.Episodes = (await DatabaseFiles.Read(item.Id + "\\Episodes")).ToObject<List<Episode>>();
                db.Actors = (await DatabaseFiles.Read(item.Id + "\\Actors")).ToObject<List<Actor>>();
                db.Posters = (await DatabaseFiles.Read(item.Id + "\\Posters")).ToObject<List<Poster>>();
                db.Files = (await DatabaseFiles.Read(item.Id + "\\Files")).ToObject<List<FileInformation>>();
                Data.Add(db);
            }
        }

        public static void SaveDatabase() {
            var series = GetSeries();
            DatabaseFiles.Write("Series", series);
            foreach (var db in Data) {
                DatabaseFiles.Write(db.Series.Id + "\\Episodes", db.Episodes);
                DatabaseFiles.Write(db.Series.Id + "\\Posters", db.Posters);
                DatabaseFiles.Write(db.Series.Id + "\\Actors", db.Actors);
                DatabaseFiles.Write(db.Series.Id + "\\Files", db.Files);

            }
        }
    }
  
}
