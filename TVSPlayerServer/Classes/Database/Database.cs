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
            /*await Task.Run(async () => {
                var series = await DatabaseFiles.Read<List<Series>>("Series");
                foreach (var item in series) {
                    Database db = new Database();
                    db.Series = item;
                    db.Episodes = await DatabaseFiles.Read<List<Episode>>(item.Id + "\\Episodes");
                    db.Actors = await DatabaseFiles.Read<List<Actor>>(item.Id + "\\Actors");
                    db.Posters = await DatabaseFiles.Read<List<Poster>>(item.Id + "\\Posters");
                    Data.Add(db);
                }
            });*/
        }

        public async static void SaveDatabase() {
            var series = GetSeries();
            await DatabaseFiles.Write("Series", series);
            foreach (var db in Data) {
                Task[] tasks = new Task[] {
                    DatabaseFiles.Write(db.Series.Id + "\\Episodes", db.Episodes),
                    DatabaseFiles.Write(db.Series.Id + "\\Posters", db.Actors),
                    DatabaseFiles.Write(db.Series.Id + "\\Actors", db.Posters),
                };
                tasks.StartAll();
                await tasks.WaitAll();
            }
        }
    }
  
}
