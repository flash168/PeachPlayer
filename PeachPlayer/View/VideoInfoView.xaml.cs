using Peach.DataAccess;
using Peach.DataAccess.Models;
using PeaPlayer.ViewModel;
using PeaPlayer.Windows;
using System;
using System.Collections.Generic;
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

namespace PeaPlayer.View
{
    /// <summary>
    /// VideoInfoView.xaml 的交互逻辑
    /// </summary>
    public partial class VideoInfoView : UserControl
    {
        VideoInfoViewVM vm;
        public VideoInfoView(VideoModel video)
        {
            InitializeComponent();
            this.DataContext = vm = new VideoInfoViewVM(video);

        }


        private async void Btn_ItemPlay(object sender, RoutedEventArgs e)
        {
            var flag = (sender as Button).Tag as PlayerData;
            var info = (sender as Button).DataContext as JiShuInfo;
            if (flag != null && info != null)
            {
                new PlayerWin(flag, info.Name).Show();
            }
        }




        private void Btn_ItemDown(object sender, RoutedEventArgs e)
        {
            var info = (sender as Button).DataContext as JiShuInfo;
            if (info != null)
            {
                //M3u8Services.Instance.AddM3u8DownTask(vm.Move, info.Purl, info.Name);
            }
        }

    }
}
