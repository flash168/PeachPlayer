using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Esprima.Ast;
using HtmlAgilityPack;
using Jint;
using Jint.Native;
using Jint.Runtime;
using RestSharp;

namespace Peach.DataAccess
{
    //html解析器
    public class Parser
    {
        RestClient client;
        public Parser()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            var options = new RestClientOptions()
            {
                RemoteCertificateValidationCallback = (a, c, d, v) => true,
                MaxTimeout = 100000,
                ThrowOnAnyError = true,  //设置不然不会报异常
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36"
            };
            client = new RestClient(options);
            client.AddDefaultHeader("Content-Type", "application/json");
            client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
        }


        /// <summary>
        /// 自动智能拼接href 比如 /1/index.html 会自动在前面拼接网页的host
        /// </summary>
        /// <param name="this"></param>
        /// <param name="arguments">url,  rule,  urlKey</param>
        /// <returns></returns>
        public JsValue pd1(JsValue @this, JsValue[] arguments)
        {
            var url = arguments.At(0)?.ToString();
            var rule = arguments.At(1)?.ToString();
            var urlKey = arguments.At(2)?.ToString();

            return arguments.At(0);
        }

        public string pd(string html, string parse, string uri)
        {
            return parse;
        }



        //urlJoin/joinUrl智能链接拼接函数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="this"></param>
        /// <param name="arguments"> parent,  child</param>
        /// <returns></returns>
        public JsValue joinUrl1(JsValue @this, JsValue[] arguments)
        {
            var parent = arguments.At(0)?.ToString();
            var child = arguments.At(1)?.ToString();

            try
            {
                if (string.IsNullOrWhiteSpace(parent)) return child;

                return new Uri(new Uri(parent), child).ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
        public string joinUrl(string parent, string child)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parent)) return child;

                return new Uri(new Uri(parent), child).ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 解析html获取列表 pdfa(html,'#list li') 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="rule"></param>
        /// <returns>得到html源码里id为list下面的li列表</returns>
        public JsValue pdfa1(JsValue @this, JsValue[] arguments)
        {
            var html = arguments.At(0)?.ToString();
            var rule = arguments.At(1)?.ToString();

            //Document doc = cache.getPdfa(html);
            //rule = parseHikerToJq(rule, false);
            //String[] parses = rule.split(" ");
            //Elements elements = new Elements();
            //for (String parse : parses)
            //{
            //    elements = parseOneRule(doc, parse, elements);
            //    if (elements.isEmpty()) return Collections.emptyList();
            //}
            List<String> items = new List<String>();
            //for (Element element : elements) items.add(element.outerHtml());
            return "";
        }
        public List<string> pdfa(string html, string _rules)
        {
            var data = new List<string>();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNodeCollection nodes = null;
            string[] rules = _rules.Split("&&");
            foreach (var rule in rules)
            {
                nodes = parseOneRule(htmlDoc, rule);
                // 遍历节点并获取 li 列表
                foreach (HtmlNode node in nodes)
                {
                    if (node != null)
                    {
                        data.Add(node.InnerHtml);
                    }
                }

            }
            return data;
        }
        private HtmlNodeCollection parseOneRule(HtmlDocument htmlDoc, string rule)
        {
            var rue = rule.Substring(1);
            var cc = rule.Substring(0, 1);

            // 选择包含特定 class 的节点
            HtmlNodeCollection nodes = null;
            if (cc == ".")
                nodes = htmlDoc.DocumentNode.SelectNodes($"//div[contains(@class,'{rue}')]");
            else if (cc == "#")
                nodes = htmlDoc.DocumentNode.SelectNodes($"//div[contains(@id,'{rue}')]");
            else
                nodes = htmlDoc.DocumentNode.SelectNodes($"//{rue}");
            return nodes;
        }


        /// <summary>
        /// 解析元素 pdfh(html,'a&&href')
        /// </summary>
        /// <param name="html"></param>
        /// <param name="rule"></param>
        /// <param name="addUrl"></param>
        /// <returns>得到html里a标签下的href属性</returns>
        public string pdfh(string html, string rule, string addUrl)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var a = htmlDoc.DocumentNode.SelectSingleNode("//a");
            return a.InnerText;


            var er = a.Attributes["href"].Value;
            if (rule.Equals("body&&Text") || rule.Equals("Text"))
            {
                return a.Attributes["href"].Value;
            }
            else if (rule.Equals("body&&Html") || rule.Equals("Html"))
            {
                return a.InnerHtml;
            }

            String option = "";
            if (rule.Contains("&&"))
            {
                var rs = rule.Split("&&").ToList();
                option = rs.Last();
                rs.RemoveAt(rs.Count - 1);
                rule = string.Join("&&", rs);
            }
            var er2 = a.InnerText;
            return er2;
            //rule = parseHikerToJq(rule, true);
            //String[] parses = rule.split(" ");
            //Elements elements = new Elements();
            //for (String parse : parses)
            //{
            //    elements = parseOneRule(doc, parse, elements);
            //    if (elements.isEmpty()) return "";
            //}
            //if (TextUtils.isEmpty(option)) return elements.outerHtml();
            //if (option.equals("Text"))
            //{
            //    return elements.text();
            //}
            //else if (option.equals("Html"))
            //{
            //    return elements.html();
            //}
            //else
            //{
            //    String result = elements.attr(option);
            //    if (option.toLowerCase().contains("style") && result.contains("url("))
            //    {
            //        Matcher matcher = p1.matcher(result);
            //        if (matcher.find()) result = matcher.group(1);
            //    }
            //    if (!TextUtils.isEmpty(result) && !TextUtils.isEmpty(addUrl) && p3.matcher(option).find())
            //    {
            //        if (result.contains("http")) result = result.substring(result.indexOf("http"));
            //        else result = joinUrl(addUrl, result);
            //    }
            //    return result;
            //}
        }

        /// <summary>
        /// 二级封装高性能选集列表解析
        /// </summary>
        /// <param name="html"></param>
        /// <param name="rule"></param>
        /// <param name="texts"></param>
        /// <param name="urls"></param>
        /// <param name="urlKey"></param>
        /// <returns></returns>
        public JsValue pdfl1(JsValue @this, JsValue[] arguments)
        {
            var html = arguments.At(0)?.ToString();
            var rule = arguments.At(1)?.ToString();
            var texts = arguments.At(2)?.ToString();
            var urls = arguments.At(3)?.ToString();
            var urlKey = arguments.At(4)?.ToString();
            //String[] parses = parseHikerToJq(rule, false).split(" ");
            //Elements elements = new Elements();
            //for (String parse : parses)
            //{
            //    elements = parseOneRule(cache.getPdfa(html), parse, elements);
            //    if (elements.isEmpty()) return Collections.emptyList();
            //}
            // var items = new JsValue[] { 1, "foo" };
            //for (Element element : elements)
            //{
            //    html = element.outerHtml();
            //    items.add(pdfh(html, texts, "").trim() + '$' + pdfh(html, urls, urlKey));
            //}
            return "";
        }

        public List<string> pdfl(string html, string rule, string texts, string urls, string urlKey)
        {
            return new List<string>();
        }

        /// <summary>
        /// okhttp封装的html请求，给js调用http请求的
        /// </summary>
        /// <param name="url"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public object request(string url, JsValue arguments)
        {
            Uri uri = new Uri(url);
            string Host = uri.Host;
            var method = arguments.AsObject()["method"]?.ToString();
            var _headers = arguments.AsObject()["headers"].AsObject();
            var Referer = _headers["Referer"]?.ToString();
            var UserAgent = _headers["User-Agent"]?.ToString();
            var Cookie = _headers["Cookie"]?.ToString();

            var request = new RestRequest(url);
            if (string.IsNullOrEmpty(UserAgent))
                UserAgent = "Mozilla/5.0 (Linux; Android 11; M2007J3SC Build/RKQ1.200826.002; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/77.0.3865.120 MQQBrowser/6.2 TBS/045714 Mobile Safari/537.36";
            request.AddHeader("User-Agent", UserAgent);
            if (!string.IsNullOrEmpty(Referer))
                request.AddHeader("Referer", Referer);

            if (!string.IsNullOrEmpty(Cookie) && !Cookie.Equals("undefined"))
            {
                string[] cooks = Cookie.Split(';');
                foreach (var item in cooks)
                {
                    string[] cook = item.Split('=');
                    if (cook.Length == 2)
                        client.AddCookie(cook[0].Trim(), cook[1].Trim(), "/", Host);
                }
            }
            RestResponse? response;
            if (method?.ToLower() == "post")
                response = client.Post(request);
            else
                response = client.Get(request);
            var trw = response.Cookies;
            var jsValue = new { headers = _headers, content = response.Content };
            return jsValue;
        }



    }
}
