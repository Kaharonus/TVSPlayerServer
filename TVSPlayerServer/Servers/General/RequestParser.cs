using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TVSPlayerServer
{
    class RequestParser {
        public static string ParseGet(HttpListenerRequest request, User user) {
            var methods = GetMethods(typeof(GetMethods), request);
            return RunMethod(request, user, methods);
        }

        public static string ParsePut(HttpListenerRequest request, User user) {
            var methods = GetMethods(typeof(PutMethods), request);
            return RunMethod(request, user, methods);
        }

        private static string RunMethod(HttpListenerRequest request, User user, List<MethodInfo> methods) {
            var query = ParseQuery(request.Url.Query);
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
                        }                    }                   
                    funcParams.Add(user);                   
                    var res = method.Invoke(new object(), funcParams.ToArray());
                    if (res != null) {
                        return JsonConvert.SerializeObject(res);
                    }
                }
            }
            return null;
        }

        private static List<MethodInfo> GetMethods(Type type, HttpListenerRequest request) {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            return methods.Where(x => x.Name == request.Url.AbsolutePath.Remove(0, 1)).ToList();
        }

        private static object GetObject(string strInput) {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || (strInput.StartsWith("[") && strInput.EndsWith("]"))) {
                try {
                    return JsonConvert.DeserializeObject(strInput);
                } catch (Exception ex) {
                    return null;
                }
            } else {
                return null;
            }
        }

        private static Dictionary<string, string> ParseQuery(string query){
            query = query.Remove(0, 1);
            var dictionary = new Dictionary<string, string>();
            foreach (var part in query.Split('@')) {
                var partsOfPart = part.Split('=');
                dictionary.Add(partsOfPart[0], partsOfPart[1]);
            }
            return dictionary;
        }

    }
}
