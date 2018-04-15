using System;
using System.Collections.Generic;
using System.Text;
using TVSPlayerServer.Classes.Database;
using System.Linq;
using static TVSPlayerServer.GetMethods;
using System.Threading.Tasks;

namespace TVSPlayerServer
{
    class PutMethods {

        public static string AddSeries(int seriesId, User user) {
            var data = Database.Data.Where(x => x.Series.Id == seriesId).FirstOrDefault();
            if (data == null) {
                var db = Database.CreateDatabase(seriesId).Result;
                Database.AddItem(db);
                AddSeries(seriesId, user);
            } else if(data.Series.AllUserSettings.Where(x=>x.UserId == user.Id).Count() == 0) {
                data.Series.AllUserSettings.Add(new Series.UserSettings() {
                    UserId = user.Id,
                    PosterId = Database.GetDefaultPosterId(seriesId)
                });
                Database.EditItem(data);
            }
            return "Added";
        }
       
    }
}
