using Peach.DataAccess.Models;
using PeachPlayer.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Peach.DataAccess.Services
{

    public class MacCmsServices
    {
        private static object lockValue = 0;
        private static HttpCommunication instance;
        public static HttpCommunication Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockValue)
                    {
                        if (instance == null)
                        {
                            instance = new HttpCommunication();
                        }
                    }
                }
                return instance;
            }
        }
    }



    public class HttpCommunication
    {
        HttpHelper http;
        public HttpCommunication()
        {
            http = new HttpHelper();
        }


        string url = "/api.php/provide/vod/";
        public Task<HttpResult<CmsAPIModel>> GeClass()
        {
            return http.GetMessageAsy<CmsAPIModel>(url, null, new Dictionary<string, string>() { { "ac", "list" } });
        }
        //推荐
        public Task<HttpResult<CmsAPIModel>> GeRecommend(int pg = 1)
        {
            return http.GetMessageAsy<CmsAPIModel>(url, null, new Dictionary<string, string>() { { "ac", "detail" }, { "pg", pg.ToString() } });
        }

        public Task<HttpResult<CmsAPIModel>> GeMoveInfo(string ids, int pg = 1)
        {
            return http.GetMessageAsy<CmsAPIModel>(url, null, new Dictionary<string, string>() { { "ac", "detail" }, { "pg", pg.ToString() }, { "ids", ids } });
        }

        public Task<HttpResult<CmsAPIModel>> GetListData(int t, int pg = 1)
        {
            return http.GetMessageAsy<CmsAPIModel>(url, null, new Dictionary<string, string>() { { "ac", "detail" }, { "pg", pg.ToString() }, { "t", t.ToString() } });
        }

        public Task<HttpResult<CmsAPIModel>> GetSearch(string search, int pg = 1)
        {
            return http.GetMessageAsy<CmsAPIModel>(url, null, new Dictionary<string, string>() { { "ac", "detail" }, { "pg", pg.ToString() }, { "wd", search } });
        }


    }
}
