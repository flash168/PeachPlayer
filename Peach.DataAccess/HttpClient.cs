using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;


namespace Pea.DataAccess
{
    public class HttpService
    {
        private static object lockValue = 0;
        private static HttpClient instance;
        public static HttpClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockValue)
                    {
                        if (instance == null)
                        {
                            instance = new HttpClient(GlobalCache.ServiceBase, GlobalCache.FileUrl, GlobalCache.Ledger);
                        }
                    }
                }

                return instance;
            }
        }
    }
    public class HttpClient
    {
        public HttpClient()
        {

            jsetting.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            jsetting.NullValueHandling = NullValueHandling.Ignore;
            jsetting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsetting.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
        }


        JsonSerializerSettings jsetting = new JsonSerializerSettings();


        string BaseUrl { get; set; }
        long Ledger { get; set; }
        public string FileUrl { get; set; }


        /// <summary>
        /// GET获取接口数据
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="UrlSegments">请求地址栏数据，问号前，斜杠隔开（可空）</param>
        /// <param name="QueryString">请求地址栏参数 问号后，XXX=XX（可空）</param>
        /// <param name="Body">提交数据</param>
        /// <returns></returns>
        public Task<HttpResult<T>> GetMessageAsy<T>(string url, Dictionary<string, string> UrlSegments = null, Dictionary<string, string> QueryString = null, object Body = null)
        {
            return Request<T>(url, Method.GET, UrlSegments, QueryString, Body);
        }

        /// <summary>
        /// POST请求服务
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="Body">提交数据</param>
        /// <param name="UrlSegments">请求地址栏数据，问号前，斜杠隔开（可空）</param>
        /// <param name="QueryString">请求地址栏参数 问号后，XXX=XX（可空）</param>
        /// <param name="timeout">请求超时（可空）</param>
        /// <returns></returns>
        public Task<HttpResult<T>> PostMessageAsy<T>(string url, object Body, Dictionary<string, string> UrlSegments = null, Dictionary<string, string> QueryString = null)
        {
            return Request<T>(url, Method.POST, UrlSegments, QueryString, Body);
        }

        /// <summary>
        /// PATCH请求服务
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="Body">提交数据</param>
        /// <param name="UrlSegments">请求地址栏数据，问号前，斜杠隔开（可空）</param>
        /// <param name="QueryString">请求地址栏参数 问号后，XXX=XX（可空）</param>
        /// <param name="timeout">请求超时（可空）</param>
        /// <returns></returns>
        public Task<HttpResult<T>> PATCHMessageAsy<T>(string url, object Body, Dictionary<string, string> UrlSegments = null, Dictionary<string, string> QueryString = null)
        {
            return Request<T>(url, Method.PATCH, UrlSegments, QueryString, Body);
        }

        /// <summary>
        /// Put请求服务
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="Body">提交数据</param>
        /// <param name="UrlSegments">请求地址栏数据，问号前，斜杠隔开（可空）</param>
        /// <param name="QueryString">请求地址栏参数 问号后，XXX=XX（可空）</param>
        /// <param name="timeout">请求超时（可空）</param>
        /// <returns></returns>
        public Task<HttpResult<T>> PutMessageAsy<T>(string url, object Body, Dictionary<string, string> UrlSegments = null, Dictionary<string, string> QueryString = null)
        {
            return Request<T>(url, Method.PUT, UrlSegments, QueryString, Body);
        }

        /// <summary>
        /// Delete请求服务
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="Body">提交数据</param>
        /// <param name="UrlSegments">请求地址栏数据，问号前，斜杠隔开（可空）</param>
        /// <param name="QueryString">请求地址栏参数 问号后，XXX=XX（可空）</param>
        /// <param name="timeout">请求超时（可空）</param>
        /// <returns></returns>
        public Task<HttpResult<T>> DeleteMessageAsy<T>(string url, Dictionary<string, string> UrlSegments = null, object Body = null, Dictionary<string, string> QueryString = null)
        {
            return Request<T>(url, Method.DELETE, UrlSegments, QueryString, Body);
        }




        /// <summary>
        /// 上传文件请求服务（POST File）
        /// </summary>
        /// <param name="module">项目名称（根据业务类型）使用前在web版创建文件夹后在这里传文件夹名称</param>
        /// <param name="filePath">文件本地路径</param>
        /// <returns>请求成功 "code":8200, "message":"Success" data </returns>
        public async Task<PostFileResModel> PostFileMessage(string module, string filePath)
        {
            var error = new PostFileResModel() { Code = "", Message = "上传文件路径错误!" };
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return error;

            var base64Img = Convert.ToBase64String(File.ReadAllBytes(filePath));
            return await PostFileMessage(module, Path.GetFileName(filePath), base64Img);
        }

        /// <summary>
        /// 上传文件请求服务（POST File）
        /// </summary>
        /// <param name="module">项目名称（根据业务类型）使用前在web版创建文件夹后在这里传文件夹名称</param>
        /// <param name="fileName">文件名称（带后缀 xxx.png）</param>
        /// <param name="base64Img">文件转base64后的字符串</param>
        /// <returns>请求成功 "code":8200, "message":"Success" data</returns>
        public async Task<PostFileResModel> PostFileMessage(string module, string fileName, string base64Img)
        {
            var error = new PostFileResModel() { Code = "", Message = "上传文件错误!" };
            if (string.IsNullOrEmpty(base64Img) || string.IsNullOrEmpty(fileName)) return error;
            string media_type = fileName.Substring(fileName.LastIndexOf(".") + 1);
            try
            {
                var file = new[] { new { original_name = Guid.NewGuid() + "." + media_type, file = base64Img, media_type = media_type } };
                var datas = new { project = Ledger.ToString(), module = module, files = file };

                string json = JsonConvert.SerializeObject(datas, Formatting.Indented, jsetting);
                var client = new RestClient(FileUrl);
                var request = new RestRequest("/ossupload/uploadByBinary", Method.POST);
                request.AddHeader("Authorization", "Bearer " + GlobalCache.Token);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var restResponse = await client.ExecuteAsync(request);
                LogService.Instance.Write(restResponse.Content, ErrorType.Trace);
                if (restResponse.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<PostFileResModel>(restResponse.Content, jsetting);
                }
                else
                {
                    error.Message = "失败！" + restResponse.ErrorMessage;
                    return error;
                }
            }
            catch (Exception e)
            {
                error.Message = e.Message;
                LogService.Instance.Write(e.Message, ErrorType.Error);
                return error;
                // throw e;
            }
        }





        private async Task<HttpResult<T>> Request<T>(string url, Method method, Dictionary<string, string> UrlSegments, Dictionary<string, string> QueryString, object Body)
        {
            HttpResult<T> req = new HttpResult<T>();
            try
            {
                Body = ObjTrimString(Body);
                var client = new RestClient(BaseUrl);
                var request = new RestRequest("/api" + url, method);
                request.RequestFormat = DataFormat.Json;
                //request.AddQueryParameter("ledger", Ledger + "");
                request.AddHeader("Authorization", "Bearer " + GlobalCache.Token);
                if (UrlSegments != null)
                {
                    foreach (var item in UrlSegments)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                            request.AddUrlSegment(item.Key, item.Value.Trim());
                    }
                }
                if (QueryString != null)
                {
                    foreach (var item in QueryString)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                            request.AddQueryParameter(item.Key, item.Value.Trim());
                    }
                }

                if (Body != null)
                {
                    string json = JsonConvert.SerializeObject(Body, Formatting.None, jsetting);
                    request.AddParameter("application/json", json, ParameterType.RequestBody);
                }
                var restResponse = await client.ExecuteAsync(request);
                LogService.Instance.Write(restResponse.Content, ErrorType.Info);
                req.Code = restResponse.StatusCode;
                if (restResponse.StatusCode == HttpStatusCode.OK)
                {
                    req.IsSuccessful = true;
                    if (typeof(T).Name is "String" || typeof(T).Name is "string")
                    {
                        req.Content = (T)Convert.ChangeType(restResponse.Content, typeof(T));
                    }
                    else
                    {
                        req.Content = JsonConvert.DeserializeObject<T>(restResponse.Content, jsetting);
                    }
                    return req;
                }
                else if (restResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    req.Message = "@@服务器暂时不可用，通常是由于过多加载或维护!";
                    LogService.Instance.Write($"请求接口异常：{restResponse.ResponseUri?.AbsoluteUri}\r\n异常信息：{restResponse.Content}");
                    return req;
                }
                else
                {
                    string data = JsonConvert.DeserializeObject<HttpResult<string>>(restResponse.Content, jsetting)?.Message;
                    if (data == null) { data = restResponse.ErrorMessage; }
                    req.Message = "@@" + data;
                    var errorinfo = $"请求接口异常：{restResponse.ResponseUri?.AbsoluteUri}\r\n";
                    errorinfo += $"参数Body：{(Body == null ? string.Empty : JsonConvert.SerializeObject(Body, Formatting.None, jsetting))}\r\n";
                    errorinfo += $"Content：{restResponse.Content}\r\n";
                    errorinfo += $"ErrorMessage：{restResponse.ErrorMessage}\r\n";
                    LogService.Instance.Write(errorinfo);
                    return req;
                }
            }
            catch (Exception e)
            {
                LogService.Instance.Write(e.Message);
                throw e;
            }
        }
        //去掉实体内字段空格
        private static object ObjTrimString(object t)
        {
            try
            {
                if (t == null)
                {
                    return default;
                }

                Type type = t.GetType();
                if (type.Name == "Dictionary`2")
                {
                    if (t is Dictionary<string, string> keyValues)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        foreach (KeyValuePair<string, string> kvp in keyValues)
                        {
                            dic.Add(kvp.Key, kvp.Value.Trim());
                        }
                        t = dic;
                    }
                }
                else if (type.Name == "List`1")
                {
                    if (t is List<string> vs)
                    {
                        for (int i = 0; i < vs.Count; i++)
                        {
                            vs[i] = vs[i].Trim();
                        }

                        t = vs;
                    }
                }
                else
                {
                    PropertyInfo[] props = type.GetProperties();
                    Parallel.ForEach(props, p =>
                    {
                        if (p.PropertyType.Name.Equals("String") && p.CanWrite)
                        {
                            var tmp = (string)p.GetValue(t, null);
                            if (tmp != null)
                            {
                                p.SetValue(t, tmp?.Trim(), null);
                            }
                        }
                    });
                }
                return t;
            }
            catch (Exception e)
            {
                LogService.Instance.WriteError(e);
                return default;
            }
        }
    }
}
