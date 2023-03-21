using Peach.DataAccess.Models;
using System.Collections.ObjectModel;

namespace Peach.DataAccess
{
    public class APIConfig
    {
        //静态单例
        public static APIConfig Instance { get; set; }


        private List<SitesItem> sites;
        private List<LivesItem> lives;
        private List<ParsesItem> parses;

        public APIConfig()
        {
            sites = new List<SitesItem>();
            lives = new List<LivesItem>();
            parses = new List<ParsesItem>();
            Instance = this;
        }

        //加载本地/网络配置文件
        public async Task<string> LoadConfig(string path)
        {
            if (path.StartsWith("http"))
            {
               // var api = new ApiConfigClient(path);
            }
            else
            {

            }
            return "";
        }


        private void Assignment(string data)
        {
            //sites.Add();
            //lives.Add();
            //parses.Add();
        }










    }
}