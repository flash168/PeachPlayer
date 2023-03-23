﻿using System;
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
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Peach.DataAccess
{
    public static class Ext
    {
        public static List<HtmlNode> Select(this HtmlNode node, string rule)
        {
            // 选择包含特定 class 的节点"//span[@class='" + ClassToGet + "']"
            List<HtmlNode> nodes = null;
            if (string.IsNullOrWhiteSpace(rule)) return nodes;
            if (rule.StartsWith('.') || rule.StartsWith('#'))
            {
                var rue = rule.Substring(1);
                var cc = rule.Substring(0, 1);

                if (cc == ".")
                    nodes = node.SelectNodes($"//*[contains(@class,'{rue}')]")?.ToList();
                else if (cc == "#")
                    nodes = node.SelectNodes($"//*[contains(@id,'{rue}')]")?.ToList();
                else
                    nodes = node.SelectNodes($"//{rue}")?.ToList();
            }
            else
            {
                string[] ruls = rule.Split(":");
                nodes = node.SelectNodes($"//{ruls[0]}")?.ToList();

                if (rule.Contains("gt")|| rule.Contains("lt"))
                {
                    var gt = new Regex(@"(?<=gt\()[\d]+(?=\))").Match(rule);
                    var lt = new Regex(@"(?<=lt\()[\d]+(?=\))").Match(rule);
                    int st = 0;
                    int et = nodes.Count;

                    if (gt.Success)
                        st = int.Parse(gt.Value);
                    if (lt.Success)
                        et = int.Parse(lt.Value);
                    nodes = nodes.ToList().GetRange(st, et - st).ToList();
                }
            }
            return nodes;
        }

        public static List<HtmlNode> eq(this List<HtmlNode> Elements, int index)
        {
            return Elements?.Count > index ? new List<HtmlNode>() { Elements[index] } : new List<HtmlNode>();
        }

    }

    //html解析器
    public class HtmlParser
    {
        RestClient client;
        public HtmlParser()
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
            string rContent = "";
            try
            {
                RestResponse? response;
                if (method?.ToLower() == "post")
                    response = client.Post(request);
                else
                    response = client.Get(request);
                var trw = response.Cookies;
                rContent = response.Content;
            }
            catch (Exception)
            { }
            var jsValue = new { headers = _headers, content = rContent };
            return jsValue;
        }

        private static Regex p = new Regex("url\\((.*?)\\)", RegexOptions.Multiline | RegexOptions.Singleline);
        private static Regex NOAdd_INDEX = new Regex(":eq|:lt|:gt|:first|:last|^body$|^#");
        private static Regex URLJOIN_ATTR = new Regex("(url|src|href|-original|-src|-play|-url)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);


        public string joinUrl(string parent, string child)
        {
            if (string.IsNullOrWhiteSpace(parent))
            {
                return child;
            }

            Uri url;
            string q = parent;
            try
            {
                url = new Uri(new Uri(parent), child);
                q = url.ToString();
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
            //        if (q.Contains("#")) {
            //            q = q.ReplaceAll("^(.+?)#.*?$", "$1");
            //        }
            return q;
        }

        public class Painfo
        {
            public string nparse_rule;
            public int nparse_index;
            public List<string> excludes;
        }

        private Painfo getParseInfo(string nparse)
        {
            /*
             根据传入的单规则获取 parse规则，索引位置,排除列表  -- 可以用于剔除元素,支持多个，按标签剔除，按id剔除等操作
             :param nparse:
             :return:*/
            Painfo painfo = new Painfo();
            //List<string> excludes = new ArrayList<>();  //定义排除列表默认值为空
            //int nparse_index;  //定义位置索引默认值为0
            painfo.nparse_rule = nparse; //定义规则默认值为本身
            if (nparse.Contains(":eq"))
            {
                painfo.nparse_rule = nparse.Split(":")[0];
                string nparse_pos = nparse.Split(":")[1];

                if (painfo.nparse_rule.Contains("--"))
                {
                    string[] rules = painfo.nparse_rule.Split("--");
                    painfo.excludes = rules.ToList();// new(Arrays.asList(rules));
                    painfo.excludes.RemoveAt(0);
                    painfo.nparse_rule = rules[0];
                }
                else if (nparse_pos.Contains("--"))
                {
                    string[] rules = nparse_pos.Split("--");
                    painfo.excludes = rules.ToList();// new ArrayList<>(Arrays.asList(rules));
                    painfo.excludes.RemoveAt(0);
                    nparse_pos = rules[0];
                }

                try
                {
                    painfo.nparse_index = int.Parse(nparse_pos.Replace("eq(", "").Replace(")", ""));
                }
                catch (Exception e1)
                {
                    painfo.nparse_index = 0;
                }
            }
            else
            {
                if (nparse.Contains("--"))
                {
                    string[] rules = painfo.nparse_rule.Split("--");
                    painfo.excludes = rules.ToList();// new ArrayList<>(Arrays.asList(rules));
                    painfo.excludes.RemoveAt(0);
                    painfo.nparse_rule = rules[0];
                }
            }
            return painfo;
        }

        //pd
        public string parseDom(string html, string rule)
        {
            return parseDomForUrl(html, rule, "");

        }
        //pdfh
        public string parseDomForUrl(string html, string rule, string Add_url)
        {
            if (string.IsNullOrEmpty(html)) return "";
            HtmlDocument doc = new();
            doc.LoadHtml(html);//pdfh_doc =  Jsoup.parse(html);
            if (rule.Equals("body&&Text") || rule.Equals("Text"))
                return doc.Text;
            else if (rule.Equals("body&&Html") || rule.Equals("Html"))
                return doc.DocumentNode.InnerHtml;

            string option = "";
            if (rule.Contains("&&"))
            {
                string[] rs = rule.Split("&&");
                option = rs[rs.Length - 1];
                List<string> excludes = rs.ToList();// new ArrayList<>(Arrays.asList(rs));
                excludes.RemoveAt(rs.Length - 1);
                rule = string.Join("&&", excludes);// TextUtils.join("&&", excludes);
            }
            rule = parseHikerToJq(rule, true);
            string[] parses = rule.Split(" ");
            List<HtmlNode> ret = null;
            foreach (string nparse in parses)
            {
                ret = parseOneRule(doc, nparse, ret?[0]);
                if (ret == null || ret.Count <= 0) return "";
            }
            if (string.IsNullOrWhiteSpace(option))
                return ret?[0].OuterHtml;
            if (option.Equals("Text"))
                return ret[0]?.InnerText;
            else if (option.Equals("Html"))
                return ret[0]?.InnerHtml;
            else //(JSUtils.isNotEmpty(option))
            {
                string result = ret[0]?.GetAttributeValue(option, "");
                if (option.ToLower().Contains("style") && result.Contains("url("))
                {
                    Match m = p.Match(result);
                    if (m.Success)
                        result = m.Groups[1]?.Value;
                }
                if (string.IsNullOrWhiteSpace(result) && !string.IsNullOrEmpty(Add_url))// (JSUtils.isNotEmpty(result) && JSUtils.isNotEmpty(Add_url))
                {
                    // 需要自动urljoin的属性
                    Match m = URLJOIN_ATTR.Match(option);
                    //if (isUrl(option)) {
                    if (m.Success)
                    {
                        if (result.Contains("http"))
                            result = result.Substring(result.IndexOf("http"));
                        else
                            result = joinUrl(Add_url, result);
                    }
                }
                return result;
            }

        }
        //pdfa
        public List<string> parseDomForArray(string html, string rule)
        {
            List<string> eleHtml = new();
            HtmlDocument doc = new();
            doc.LoadHtml(html);

            rule = parseHikerToJq(rule, false);
            string[] parses = rule.Split(" ");
            List<HtmlNode> ret = null;
            foreach (var pars in parses)
            {
                ret = parseOneRule(doc, pars, ret?[0]);
                if (ret == null) return eleHtml;
            }
            for (int i = 0; i < ret.Count(); i++)
            {
                eleHtml.Add(ret[i].OuterHtml);
            }
            return eleHtml;
        }
        //pdfl
        public List<string> parseDomForList(string html, string rule, string list_text, string list_url, string urlKey)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            rule = parseHikerToJq(rule, false);
            string[] parses = rule.Split(" ");
            List<HtmlNode> ret = null;
            foreach (string pars in parses)
            {
                ret = parseOneRule(doc, pars, ret?[0]);
                if (ret == null) return new();
            }
            List<string> new_vod_list = new();
            for (int i = 0; i < ret.Count; i++)
            {
                string it = ret[i].OuterHtml;
                new_vod_list.Add(parseDomForUrl(it, list_text, "").Trim() + '$' + parseDomForUrl(it, list_url, urlKey));
            }
            return new_vod_list;
        }



        private string parseHikerToJq(string parse, bool first)
        {
            /*
             海阔解析表达式转原生表达式,自动补eq,如果传了first就最后一个也取eq(0)
            :param parse:
            :param first:
            :return:
            */
            // 不自动加eq下标索引
            if (parse.Contains("&&"))
            {
                string[] parses = parse.Split("&&");  //带&&的重新拼接
                List<string> new_parses = new();  //构造新的解析表达式列表
                for (int i = 0; i < parses.Length; i++)
                {
                    string[] pss = parses[i].Split(" ");
                    string ps = pss[pss.Length - 1];  //如果分割&&后带空格就取最后一个元素
                    Match m = NOAdd_INDEX.Match(ps);  // Matcher m = NOAdd_INDEX.matcher(ps);
                    //if (!isIndex(ps)) {
                    if (!m.Success)
                    {
                        if (!first && i >= parses.Length - 1)
                        {  //不传first且遇到最后一个,不用补eq(0)
                            new_parses.Add(parses[i]);
                        }
                        else
                        {
                            new_parses.Add(parses[i] + ":eq(0)");
                        }
                    }
                    else
                    {
                        new_parses.Add(parses[i]);
                    }
                }
                parse = string.Join(" ", new_parses);// TextUtils.join(" ", new_parses);
            }
            else
            {
                string[] pss = parse.Split(" ");
                string ps = pss[pss.Length - 1];  //如果分割&&后带空格就取最后一个元素
                //Matcher m = NOAdd_INDEX.matcher(ps); 
                Match m = NOAdd_INDEX.Match(ps);
                //if (!isIndex(ps) && first) {
                if (!m.Success && first)
                {
                    parse = parse + ":eq(0)";
                }
            }
            return parse;
        }

        private List<HtmlNode> parseOneRule(HtmlDocument doc, string parse, HtmlNode ret)
        {
            List<HtmlNode> nodes;
            Painfo info = getParseInfo(parse);
            if (parse.Contains(":eq"))
            {
                if (ret == null)
                {
                    if (info.nparse_index < 0)
                    {
                        var r = doc.DocumentNode.Select(info.nparse_rule);
                        nodes = r.eq(r.Count + info.nparse_index);
                    }
                    else
                    {
                        nodes = doc.DocumentNode.Select(info.nparse_rule).eq(info.nparse_index);
                    }
                }
                else
                {
                    if (info.nparse_index < 0)
                    {
                        var r = ret.Select(info.nparse_rule);
                        nodes = r.eq(r.Count() + info.nparse_index);
                    }
                    else
                    {
                        nodes = ret.Select(info.nparse_rule).eq(info.nparse_index);
                    }
                }
            }
            else
            {
                if (ret == null)
                    nodes = doc.DocumentNode.Select(parse);
                else
                    nodes = ret.Select(parse);
            }
            if (info.excludes != null && nodes != null)
            {
                foreach (var exclude in info.excludes)
                {
                    nodes.RemoveAll(s => s.Select(exclude)?.Count > 0);
                }
            }
            return nodes;
        }
    }
}