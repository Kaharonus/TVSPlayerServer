using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TVSPlayerServer.Classes.TheTVDb;

namespace TVSPlayerServer
{
    public class Poster
    {
        public int Id { get; set; }
        public string KeyType { get; set; }
        public string SubKey { get; set; }
        public string FileName { get; set; }
        public string Resolution { get; set; }
        public RatingsInfo ratingsInfo { get; set; }
        public string Thumbnail { get; set; }
        public class RatingsInfo {
            public double Average { get; set; }
            public int? Count { get; set; }
        }

        /// <summary>
        /// Request information about posters
        /// </summary>
        /// <param name="id">TVDb id of Series</param>
        /// <returns>List of Poster objects or null when error occurs</returns>
        public static List<Poster> GetPosters(int id) {
            HttpWebRequest request = GeneralAPI.GetRequest("https://api.thetvdb.com/series/" + id + "/images/query?keyType=poster");
            try {
                var response = request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream())) {
                    List<Poster> list = new List<Poster>();
                    JObject jObject = JObject.Parse(sr.ReadToEnd());
                    foreach (JToken jt in jObject["data"]) {
                        list.Add(jt.ToObject<Poster>());
                    }
                    return list.OrderByDescending(y => y.ratingsInfo.Count).ToList();
                }
            } catch (WebException e) {
                return null;
            }
        }

        /// <summary>
        /// Request information about posters in a season
        /// </summary>
        /// <param name="id">TVDb id of Series</param>
        /// <param name="season">Any season that the Series has</param>
        /// <returns>List of Poster objects or null when error occurs</returns>
        public static List<Poster> GetPostersForSeason(int id, int season) {
            HttpWebRequest request = GeneralAPI.GetRequest("https://api.thetvdb.com/series/" + id + "/images/query?keyType=season&subKey=" + season);
            try {
                var response = request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream())) {
                    List<Poster> list = new List<Poster>();
                    JObject jObject = JObject.Parse(sr.ReadToEnd());
                    foreach (JToken jt in jObject["data"]) {
                        list.Add(jt.ToObject<Poster>());
                    }
                    return list.OrderByDescending(y => y.ratingsInfo.Count).ToList();
                }
            } catch (WebException e) {
                return null;
            }
        }

        /// <summary>
        /// Request information about default fanart
        /// </summary>
        /// <param name="id">TVDb id of Series</param>
        /// <returns></returns>
        public static Poster GetFanArt(int id) {
            HttpWebRequest request = GeneralAPI.GetRequest("https://api.thetvdb.com/series/" + id + "/images/query?keyType=fanart");
            try {
                var response = request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream())) {
                    List<Poster> list = new List<Poster>();
                    JObject jObject = JObject.Parse(sr.ReadToEnd());
                    foreach (JToken jt in jObject["data"]) {
                        list.Add(jt.ToObject<Poster>());
                    }
                    List<Poster> sorted = list.OrderByDescending(y => y.ratingsInfo.Count).ToList();
                    return sorted[0];
                }
            } catch (WebException e) {
                return null;
            }
        }

    }
}
