using Jint;
using System.Diagnostics;
using Jint.Native.Object;
using Jint.Native;
using Newtonsoft.Json;
using Peach.DataAccess.Models;
using Jint.Runtime;

namespace Peach.DataAccess
{
    public class LeaderServices
    {
        private static object lockObj = 0;
        private static Leader instance = null;
        public static Leader Instance
        {
            get
            {
                if (instance == null)
                    lock (lockObj)
                        if (instance == null)
                            instance = new Leader();
                return instance;
            }
        }
    }

    public class Leader
    {
        public Leader() { }

        // var assemblyDirectory = new DirectoryInfo(AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory);
        string tph = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

        Parser parser = new Parser();
        HtmlParser hparser = new HtmlParser();
        public bool InitLeader(string api, string drpy, string ext)
        {
            string apiurl = api.Substring(0, api.LastIndexOf('/') + 1);

            drpy = drpy.Replace("console.log", "consolelog");

            engine = new Engine(cfg =>
            {
                cfg.AllowClr();
                //cfg.EnableModules(tp);
                cfg.EnableModules(new RequireModuleLoader(apiurl, tph));
            }).SetValue("consolelog", new Action<object>(g => { Debug.WriteLine(g); }));

            // 将 C# 函数转换为 JavaScript 函数，并将其添加到 engine 中
            engine.SetValue("pd", new Func<string, string, string>(hparser.parseDom));
            engine.SetValue("pdfh", new Func<string, string, string, string>(hparser.parseDomForUrl));
            engine.SetValue("pdfl", new Func<string, string, string, string, string, List<string>>(hparser.parseDomForList));
            engine.SetValue("pdfa", new Func<string, string, List<string>>(hparser.parseDomForArray));
            engine.SetValue("joinUrl", new Func<string, string, string>(hparser.joinUrl));
            engine.SetValue("req", new Func<string, JsValue, object>(hparser.request));

            engine.SetValue("local", new Local());

            try
            {
                engine.AddModule("drpyModel", drpy);
                ns = engine.ImportModule("drpyModel");
                var init = ns.Get("default");

                var result = engine.Invoke(init.Get("init"), ext);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        Engine engine;
        ObjectInstance ns;


        public Task<DrpyHomeInfo> GetHome(string fidier)
        {
            Task<DrpyHomeInfo> task = Task.Factory.StartNew(_fidier =>
            {
                var rest = engine.Invoke(ns["default"].Get("home"), _fidier).AsString();
                return ConvertData<DrpyHomeInfo>(rest);
            }, fidier);
            return task;
        }
        public Task<VideoListModel> GetHomeVod()
        {
            return Task.Factory.StartNew(() =>
            {
                var rest = engine.Invoke(ns["default"].Get("homeVod")).AsString();
                return ConvertData<VideoListModel>(rest);
            });
        }

        public Task<VideoListModel> GetCategory(string tid, string pg, string filter, string obj)
        {
            return Task.Factory.StartNew(() =>
            {
                var rest = engine.Invoke(ns["default"].Get("category"), tid, pg, filter, obj).AsString();
                return ConvertData<VideoListModel>(rest);
            });
        }

        public Task<VideoListModel> Search(string fidier)
        {
            return Task.Factory.StartNew(_filter =>
            {
                var rest = engine.Invoke(ns["default"].Get("search"), _filter, true).AsString();
                return ConvertData<VideoListModel>(rest);
            }, fidier);
        }


        //play search detail
        public Task<VideoListModel> GetDetails(string ids)
        {
            return Task.Factory.StartNew(_ids =>
            {
                var rest = engine.Invoke(ns["default"].Get("detail"), _ids).AsString();
                return ConvertData<VideoListModel>(rest);
            }, ids);
        }

        public Task<DrpyPlay> GetPlayInfo(string flag, string id)
        {
            return Task.Factory.StartNew(_id =>
            {
                //flag线路名, id, array(vipFlags)全局配置需要解析的标识列表flags
                var rest = engine.Invoke(ns["default"].Get("play"), flag, _id, "").AsString();
                return ConvertData<DrpyPlay>(rest);
            }, id);
        }

        private T ConvertData<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
            }
            return default(T);
        }


    }
}
