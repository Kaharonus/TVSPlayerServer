using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace TVSPlayerServer
{
    class RequestParser {
        public static string Parse(HttpListenerRequest request, HttpListenerResponse response, User user) {
            var query = ParseQuery(request.Url.Query);
            var type = typeof(Database);
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public).ToList();
            methods = methods.Where(x => x.Name == request.Url.AbsolutePath.Remove(0, 1)).ToList();
            foreach (var method in methods) {
                var parameters = method.GetParameters().ToList();
                parameters.RemoveAt(parameters.Count - 1);
                if (parameters.Select(x => x.Name).All(query.Keys.Contains) && parameters.Count == query.Keys.Count) {
                    List<object> funcParams = new List<object>();
                    foreach (var param in parameters) {
                        if (param.ParameterType == typeof(Int32)) {
                            funcParams.Add(int.Parse(query[param.Name]));
                        } else {
                            funcParams.Add(query[param.Name]);
                        }                       
                    }
                    funcParams.Add(user);
                    var res = method.Invoke(new object(), funcParams.ToArray());
                    if (res != null) {
                        return JsonConvert.SerializeObject(res);
                    }
                }
            }
            return null;
        }

        public static Dictionary<string, string> ParseQuery(string query){
            query = query.Remove(0, 1);
            var dictionary = new Dictionary<string, string>();
            foreach (var part in query.Split('@')) {
                var partsOfPart = part.Split('=');
                dictionary.Add(partsOfPart[0], partsOfPart[1]);
            }
            return dictionary;
        }

        private static Action NotFound() {
            return () => { };
        }
    }
}
