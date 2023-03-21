using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peach.DataAccess.Models
{
    public class CmsAPIModel : NotifyProperty
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 数据列表
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pagecount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string limit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<VideoModel> list { get; set; }

        public List<ListClass> Class { get; set; }

    }



    public class ListClass
    {
        public string Type_id { get; set; }
        public string Type_name { get; set; }
    }


    public class DrpyHomeInfo
    {
        public List<ListClass> Class { get; set; }
        public object filters { get; set; }
    }

    //{"parse":1,"url":"https://1080p.tv/vodplay/551971-1-1/","jx":0}
    public class DrpyPlay
    {
        public int parse { get; set; }
        public string url { get; set; }
        public int jx { get; set; }

    }




}
