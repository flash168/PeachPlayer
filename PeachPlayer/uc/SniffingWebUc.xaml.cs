using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Interops.Signatures;
using Vlc.DotNet.Wpf;

namespace PeaPlayer.uc
{
    /// <summary>
    /// SniffingWebUc.xaml 的交互逻辑
    /// </summary>
    public partial class SniffingWebUc : UserControl
    {
        public event Action<string> OnResponseReceived;
        public SniffingWebUc()
        {
            InitializeComponent();
            InitializeAsync();
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        }

        private void WebView_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            webView.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceivedAsync;
        }

        private void CoreWebView2_WebResourceResponseReceivedAsync(object? sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            var url = e.Request.Uri;
            Debug.WriteLine(url);
            if (url.Contains(".m3u8"))
            {
                OnResponseReceived?.Invoke(url);
            }
        }

        async void InitializeAsync()
        {
            var webView2Environment = await CoreWebView2Environment.CreateAsync();
            await webView.EnsureCoreWebView2Async(webView2Environment);
            Debug.WriteLine("----EnsureCoreWebView2Async end");
            if (!string.IsNullOrEmpty(purl))
            {
                GoUrl(purl);
                purl = "";
            }
        }
        //懒得处理异步，初始化好后才能跳转网页
        static string purl = "";
        public void GoUrl(string url)
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.Navigate(url);
            }
            else
                purl = url;
        }

    }
}
