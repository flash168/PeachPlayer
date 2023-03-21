using Peach.DataAccess;
using Peach.DataAccess.Models;
using PeaPlayer.ViewModel;
using System;
using System.Collections.Generic;
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
using Vlc.DotNet.Core.Interops.Signatures;
using Vlc.DotNet.Wpf;

namespace PeaPlayer.View
{
    /// <summary>
    /// VideoInfoView.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerView : UserControl
    {
        PlayerViewVM vm;
        public PlayerView(VideoModel video)
        {
            InitializeComponent();
            this.DataContext = vm = new PlayerViewVM(video);
            contentCtrl.Content = vm.InitVLC();
        }



    }
}
