using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Peach.DataAccess.Extend
{
    public class M3u8Extend
    {
        /// <summary>
        /// 根据ffmpeg【下载】输出内容读下载取详细信息
        /// </summary>
        /// <param name="info"></param>
        public void ShowDownM3u8Info(string info)
        {
            //Regex regex = new Regex(@"(\d\d[.:]){3}\d\d", RegexOptions.Compiled | RegexOptions.Singleline);//取视频时长以及Time属性
            // label5.Text = "[总时长：" + regex.Match(info).Value + "]";
            //var time = regex.Matches(info);
            //if (time.Count > 0)
            //{ label6.Text = "[已下载：" + time.OfType<Match>().Last() + "]"; }
            //Regex fps = new Regex(@"(\S+)\sfps", RegexOptions.Compiled | RegexOptions.Singleline);//取视频帧数
            //Regex resolution = new Regex(@"\d{2,}x\d{2,}", RegexOptions.Compiled | RegexOptions.Singleline);//取视频分辨率
            // label7.Text = "[视频信息：" + resolution.Match(info).Value + "，" + fps.Match(info).Value + "]";
            //if (time.Count > 0)
            //{
            //    Double All = Convert.ToDouble(Convert.ToDouble(label5.Text.Substring(5, 2)) * 60 * 60 + Convert.ToDouble(label5.Text.Substring(8, 2)) * 60
            //    + Convert.ToDouble(label5.Text.Substring(11, 2)) + Convert.ToDouble(label5.Text.Substring(14, 2)) / 100);
            //    Double Downloaded = Convert.ToDouble(Convert.ToDouble(label6.Text.Substring(5, 2)) * 60 * 60 + Convert.ToDouble(label6.Text.Substring(8, 2)) * 60
            //    + Convert.ToDouble(label6.Text.Substring(11, 2)) + Convert.ToDouble(label6.Text.Substring(14, 2)) / 100);

            //    Double Progress = (Downloaded / All) * 100;

            //    if (Progress > 100)  //防止进度条超过百分之百
            //    {
            //        Progress = 100;
            //    }
            //    ProgressBar.Value = Convert.ToInt32(Progress);

            //    this.Text = "M3U8 Downloader  by：nilaoda [0.1.1]" + "     已完成：" + String.Format("{0:F}", Progress) + "%";
            //}


        }




    }
}
