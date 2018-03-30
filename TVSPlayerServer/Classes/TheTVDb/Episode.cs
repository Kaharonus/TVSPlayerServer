using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TVSPlayerServer.Classes.TheTVDb;

namespace TVSPlayerServer
{
    public class Episode {

        public int Id { get; set; }
        public int? AiredSeason { get; set; }
        public int? AiredEpisodeNumber { get; set; }
        public string EpisodeName { get; set; }
        public string FirstAired { get; set; }
        public List<string> GuestStars { get; set; } = new List<string>();
        public string Director { get; set; }
        public List<string> Directors { get; set; } = new List<string>();
        public List<string> Writers { get; set; } = new List<string>();
        public string Overview { get; set; }
        public string ShowUrl { get; set; }
        public int? AbsoluteNumber { get; set; }
        public string Filename { get; set; }
        public int? SeriesId { get; set; }
        public int? AirsAfterSeason { get; set; }
        public int? AirsBeforeSeason { get; set; }
        public int? AirsBeforeEpisode { get; set; }
        public string ImdbId { get; set; }
        public double SiteRating { get; set; }
        public int? SiteRatingCount { get; set; }

        public long ContinueAt { get; set; }
        public bool Finished { get; set; }
        public string Thumbnail { get; set; }
        public List<ScannedFile> Files { get; set; } = new List<ScannedFile>();

        public class ScannedFile {
            public enum FileType { Video, Subtitles };
            public FileType Type { get; set; }
            public string OriginalName { get; set; }
            public string NewName { get; set; }
            public long? TimeStamp { get; set; }
        }

        /// <summary>
        /// Requests basic information about all episodes from TVDb API
        /// </summary>
        /// <param name="id">TVDb id of Series</param>
        /// <returns>List of Episodes with only basic information or null when error occurs</returns>
        public static List<Episode> GetEpisodes(int id) {
            List<Episode> list = new List<Episode>();
            int page = 1;
            while (true) {
                try {
                    HttpWebRequest request = GeneralAPI.GetRequest("https://api.thetvdb.com/series/" + id + "/episodes?page=" + page);
                    var response = request.GetResponse();
                    using (var sr = new StreamReader(response.GetResponseStream())) {
                        JObject jObject = JObject.Parse(sr.ReadToEnd());
                        foreach (JToken jt in jObject["data"]) {
                            list.Add(jt.ToObject<Episode>());
                        }
                        page++;
                    }
                } catch (WebException ex) {
                    if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null) {
                        var resp = (HttpWebResponse)ex.Response;
                        if (resp.StatusCode == HttpStatusCode.NotFound) { return list; }
                    } else {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Requests full information about Episode from TVDb API
        /// </summary>
        /// <param name="id">TVDb ID of Episode</param>
        /// <returns>Episode object or null when error occurs</returns>
        public static Episode GetEpisode(int id) {
            HttpWebRequest request = GeneralAPI.GetRequest("https://api.thetvdb.com/episodes/" + id);
            try {
                var response = request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream())) {
                    JObject jObject = JObject.Parse(sr.ReadToEnd());
                    return jObject["data"].ToObject<Episode>();
                }
            } catch (WebException e) {
                return null;
            }
        }
    }
}
