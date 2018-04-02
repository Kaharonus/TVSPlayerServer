using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TVSPlayerServer.Classes.Database
{
    class Database{
        public static List<Database> Data { get; set; } = new List<Database>();
        public Series Series { get; set; } = new Series();
        public List<Episode> Episodes { get; set; } = new List<Episode>();
        public List<Poster> Posters { get; set; } = new List<Poster>();
        public List<Actor> Actors { get; set; } = new List<Actor>();

        public async static void LoadDatabase() {
            var series = await DatabaseFiles.Read<List<Series>>("Series");
            foreach (var item in series) {
                Database db = new Database();
                db.Series = item;
                db.Episodes = await DatabaseFiles.Read<List<Episode>>(item.Id + "\\Episodes");
                db.Actors = await DatabaseFiles.Read<List<Actor>>(item.Id + "\\Actors");
                db.Posters = await DatabaseFiles.Read<List<Poster>>(item.Id + "\\Posters");
                Data.Add(db);
            }
        }
        public static void AddItem(Database db) {
            Data.Add(db);
            DatabaseFiles.Write("Series", GetSeries());
            DatabaseFiles.Write(db.Series.Id + "\\Episodes", db.Episodes);
            DatabaseFiles.Write(db.Series.Id + "\\Posters", db.Actors);
            DatabaseFiles.Write(db.Series.Id + "\\Actors", db.Posters);
        }
        public static void RemoveItem(Database db) {
            Data.Remove(db);
            DatabaseFiles.Write("Series", GetSeries());
            DatabaseFiles.Write(db.Series.Id + "\\Episodes", new object());
            DatabaseFiles.Write(db.Series.Id + "\\Posters", new object());
            DatabaseFiles.Write(db.Series.Id + "\\Actors", new object());

        }
        public static List<Series> GetSeries() {
            return Data.Select(x => x.Series).ToList();
        }
    }
  
}
