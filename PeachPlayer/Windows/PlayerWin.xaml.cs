using Peach.DataAccess.Models;
using PeaPlayer.ViewModel;
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
using System.Windows.Shapes;

namespace PeaPlayer.Windows
{
    /// <summary>
    /// PlayerWin.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerWin : Window
    {
        PlayerWinVM vm;
        public PlayerWin(PlayerData data,string ji)
        {
            InitializeComponent();
            this.DataContext = vm = new PlayerWinVM(data, ji);
             vm.InitVLC(contentCtrl);
        }


    }
}
