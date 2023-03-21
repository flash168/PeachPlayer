using Esprima.Ast;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Peach.DataAccess.Models;
using RestSharp;
using RestSharp.Serializers.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace Peach.DataAccess
{
    public static class ApiConfigServices
    {
        private static object lockObj = 0;
        private static ApiConfigClient instance = null;
        public static ApiConfigClient Instance
        {
            get
            {
                if (instance == null)
                    lock (lockObj)
                        if (instance == null)
                            instance = new ApiConfigClient();
                return instance;
            }
        }
    }


    public class ApiConfigClient : IDisposable
    {
        private readonly RestClient _client;
        private Root ConfigRoot;
        public ApiConfigClient()
        {
            JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Include,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Newtonsoft.Json.Formatting.None,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            var options = new RestClientOptions()
            {
                MaxTimeout = 100000,
                ThrowOnAnyError = true,  //设置不然不会报异常
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36"
            };
            _client = new RestClient(options);
            _client.AddDefaultHeader("Content-Type", "application/json");
            _client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            // _client.UseNewtonsoftJson();
        }

        public async Task<bool> LoadConfig(string url)
        {
            var rest = new RestRequest(url);
            var data = await _client.GetAsync(rest);

            if (data.IsSuccessful && !string.IsNullOrEmpty(data.Content))
            {
                //JObject jo = JObject.Parse(data.Content);
                //var si = jo["sites"]?.ToArray();
                //var site = new SitesItem();
                //foreach (var item in si)
                //{
                //    ConfigRoot.Sites.Add(site);
                //}
                //var site =  jo["sites"].ToArray()[191].ToString();

                //var dd1 = JsonConvert.DeserializeObject<SitesItem>(site);


                var dd = JsonConvert.DeserializeObject<Root>(data.Content);
                if (data != null)
                    ConfigRoot = dd;
            }
            return data != null;
        }


        //获取所有站点
        public List<SitesItem> GetSites(int type = -1)
        {
            if (type < 0)
                return ConfigRoot.Sites;
            else
                return ConfigRoot.Sites.Where(s => s.Type == type).ToList();
        }

        public async Task<string?> GetApi(string url)
        {
            var res = await _client.GetAsync(new RestRequest(url));
            if (res.IsSuccessful)
            {
                return res.Content;
            }
            return null;
        }


        public async Task<SitesItem> GetSite(string key)
        {
            var data = ConfigRoot.Sites.FirstOrDefault(s => s.Key.Equals(key));
            if (data != null)
            {
                if (data.Api.ToLower().StartsWith("http")&& string.IsNullOrWhiteSpace(data.ApiData))
                {
                    var api = await GetApi(data.Api);
                    if (!string.IsNullOrWhiteSpace(api))
                        data.ApiData = api;
                }
            }
            return data;
        }

        //获取所有解析
        public List<ParsesItem> GetParses(string key)
        {
            return ConfigRoot.Parses;
        }

        //获取直播

        //获取Ads

        //public async Task<Leader> GetLeader(string key)
        //{
        //    var site = ConfigRoot.Sites.FirstOrDefault(s => s.Key.Equals(key));
        //    var dp = await GetApi(site.Api);
        //    var ex = await GetApi(site.Ext?.ToString());
        //    return new Leader(dp, ex);
        //}



        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
