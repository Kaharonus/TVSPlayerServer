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

        public static string AddSeries(int seriesId,  User user) {
            var data = Database.Data.Where(x => x.Series.Id == seriesId).FirstOrDefault();
            if (data == null) {
                var db = Database.CreateDatabase(seriesId).Result;
                db.Series.UserIds.Add(user.Id);
                Database.AddItem(db);
            } else if(!data.Series.UserIds.Contains(user.Id)) {
                data.Series.UserIds.Add(user.Id);
                Database.EditItem(data);
            }
            return "Added";
        }
       
    }
}
