﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TVSPlayerServer.Classes.TheTVDb;

namespace TVSPlayerServer
{
    public class Series {
        public int Id { get; set; }
        public string SeriesName { get; set; }
        public List<string> aliases = new List<string>();
        public string Banner { get; set; }
        public string Status { get; set; }
        public string FirstAired { get; set; }
        public string Network { get; set; }
        public string Runtime { get; set; }
        public List<string> Genre { get; set; } = new List<string>();
        public string Overview { get; set; }
        public string AirsDayOfWeek { get; set; }
        public string AirsTime { get; set; }
        public string Rating { get; set; }
        public string ImdbId { get; set; }
        public int? TvmazeId { get; set; }
        public double SiteRating { get; set; }
        public int? SiteRatingCount { get; set; }

        public string LibraryPath { get; set; }
        public UserSettings userSettings { get; set; }
        public List<UserSettings> AllUserSettings { get; set; } = new List<UserSettings>();

        public class UserSettings {
            public int UserId { get; set; }
            public bool AutoDownload { get; set; } = false;
            public int PosterId { get; set; }
        }

        /// <summary>
        /// Searches TVMaze API for Series
        /// </summary>
        /// <param name="name">Searched string</param>
        /// <returns>List of Series with basic information or null when error occurs</returns>
        public static List<Series> Search(string name) {
            List<Series> list = new List<Series>();
            name = name.Replace(" ", "+");
            WebRequest wr = WebRequest.Create("http://api.tvmaze.com/search/shows?q=" + name);
            wr.Timeout = 2000;
            HttpWebResponse response = null;
            try {
                response = (HttpWebResponse)wr.GetResponse();
            } catch (WebException e) {
                return null;
            }
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            JArray array;
            try {
                array = JArray.Parse(responseFromServer);
            } catch (Exception e) { return null; }
            foreach (JToken jt in (JToken)array) {
                if (!String.IsNullOrEmpty(jt["show"]["externals"]["thetvdb"].ToString()) && !String.IsNullOrEmpty(jt["show"]["name"].ToString())) {
                    Series s = new Series();
                    s.SeriesName = jt["show"]["name"].ToString();
                    s.FirstAired = jt["show"]["premiered"] != null ? jt["show"]["premiered"].ToString() : "";
                    s.Id = Int32.Parse(jt["show"]["externals"]["thetvdb"].ToString());
                    s.TvmazeId = Int32.Parse(jt["show"]["id"].ToString());
                    list.Add(s);
                }
            }
            return list;
        }

        /// <summary>
        /// Searches for series with best match to string using TVMaze API
        /// </summary>
        /// <param name="name">Any text</param>
        /// <returns>Basic info about Series or null when error occurs</returns>
        public static Series SearchSingle(string name) {
            name = name.Replace(" ", "+");
            WebRequest wr = WebRequest.Create("http://api.tvmaze.com/singlesearch/shows?q=" + name);
            wr.Timeout = 2000;
            HttpWebResponse response = null;
            try {
                response = (HttpWebResponse)wr.GetResponse();
            } catch (WebException e) {
                return null;
            }
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            JObject jObject;
            try {
                jObject = JObject.Parse(responseFromServer);
            } catch (Exception e) {
                return null;
            }
            if (!String.IsNullOrEmpty(jObject["externals"]["thetvdb"].ToString()) && !String.IsNullOrEmpty(jObject["name"].ToString())) {
                Series s = new Series();
                s.SeriesName = jObject["name"].ToString();
                string test = jObject["externals"]["thetvdb"].ToString();
                s.FirstAired = jObject["premiered"] != null ? jObject["premiered"].ToString() : "";
                s.Id = Int32.Parse(test);
                s.TvmazeId = Int32.Parse(jObject["id"].ToString());
                return s;
            }

            return null;
        }

        /// <summary>
        /// Requests full information about Series.
        /// </summary>
        /// <param name="id">ID of Series</param>
        /// <returns>Series with full information or null when error occurs</returns>
        public static Series GetSeries(int id) {
            HttpWebRequest request = GeneralAPI.GetRequest("https://api.thetvdb.com/series/" + id);
            try {
                var response = request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream())) {
                    JObject jObject = JObject.Parse(sr.ReadToEnd());
                    Series series = jObject["data"].ToObject<Series>();
                    series.aliases = series.GetAliases(series.aliases);
                    return series;
                }
            } catch (Exception) {
                return null;
            }

        }

        /// <summary>
        /// Generates aliases for Series. Used when scanning storage for episodes.
        /// </summary>
        /// <param name="aliases">List of few aliases</param>
        /// <returns>List of aliases with (usually) few additions</returns>
        private List<string> GetAliases(List<string> aliases) {
            List<string> aliasesNew = new List<string>();
            Regex reg = new Regex(@"\([0-9]{4}\)");
            Regex reg2 = new Regex(@"\.");
            aliasesNew.Add(SeriesName);
            string temp = SeriesName;
            Match m = reg2.Match(temp);
            while (m.Success) {
                temp = temp.Remove(m.Index, 1);
                aliasesNew.Add(SeriesName.Remove(m.Index, 1));
                m = reg2.Match(temp);
            }
            Match snMatch = reg.Match(SeriesName);
            if (snMatch.Success) {
                aliasesNew.Add(reg.Replace(SeriesName, ""));
            }
            if (aliases != null) {
                foreach (string alias in aliases) {
                    aliasesNew.Add(alias);
                    Match regMatch = reg.Match(alias);
                    if (regMatch.Success) {
                        aliasesNew.Add(reg.Replace(alias, ""));
                    }
                }
            }
            for (int i = 0; i < aliasesNew.Count; i++) {
                if (aliasesNew[i].Contains(" ")) {
                    aliasesNew.Add(aliasesNew[i].Replace(" ", "."));
                }
                if (aliasesNew[i].Contains("'")) {
                    aliasesNew.Add(aliasesNew[i].Replace("'", ""));
                }
            }
            return aliasesNew;
        }

        /// <summary>
        /// Gets id's of all Series updated since parameter "from"
        /// </summary>
        /// <param name="from">Any DateTime from 1.1.1970</param>
        /// <returns>List of id's of Series or null when error occurs</returns>
        public static List<int> GetUpdates(DateTime from) {
            if (from > DateTime.Now) {
                return null;
            }
            HashSet<int> ids = new HashSet<int>();
            List<Tuple<double, double>> timestamps = GenerateTimeStamps(from);
            foreach (Tuple<double, double> timestamp in timestamps) {
                HttpWebRequest request = GeneralAPI.GetRequest("https://api.thetvdb.com/updated/query?fromTime=" + timestamp.Item1 + "&toTime=" + timestamp.Item2);
                try {
                    var response = request.GetResponse();
                    using (var sr = new StreamReader(response.GetResponseStream())) {
                        JObject jObject = JObject.Parse(sr.ReadToEnd());
                        foreach (JToken jt in jObject["data"]) {
                            ids.Add(Int32.Parse((string)jt["id"]));
                        }
                    }
                } catch (WebException e) {
                    return null;
                }
            }
            return ids.ToList();
        }

        /// <summary>
        /// Generates List of tupples of Unix timestamps. From any DateTime to 1 more week.
        /// </summary>
        /// <param name="from">Any DateTime from 1.1.1970</param>
        /// <returns>List of timestamps in Tupple. Item1 is parameter from. Item2 is from + 1 week</returns>
        private static List<Tuple<double, double>> GenerateTimeStamps(DateTime from) {
            List<Tuple<double, double>> timestamps = new List<Tuple<double, double>>();
            do {
                if (from.AddDays(7) > DateTime.Now) {
                    timestamps.Add(new Tuple<double, double>(ConvertToUnixTimestamp(from), ConvertToUnixTimestamp(DateTime.Now)));
                } else {
                    timestamps.Add(new Tuple<double, double>(ConvertToUnixTimestamp(from), ConvertToUnixTimestamp(from.AddDays(7))));
                }
                from = from.AddDays(7);
            } while (from < DateTime.Now);
            return timestamps;
        }

        /// <summary>
        /// Converts DateTime to Unix (Epoch) timestamp
        /// </summary>
        /// <param name="date">Any DateTime from 1.1.1970</param>
        /// <returns>Unix/Epoch timestamp</returns>
        private static double ConvertToUnixTimestamp(DateTime date) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

    }
}
