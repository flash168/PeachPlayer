using Peach.DataAccess.Models;
using PeaPlayer.uc;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PeaPlayer.View
{
    /// <summary>
    /// ClassifyListView.xaml 的交互逻辑
    /// </summary>
    public partial class SearchListView : Page
    {
        SearchListViewVM vm;

        public SearchListView(string wd)
        {
            InitializeComponent();
            vm = new SearchListViewVM(wd);
            this.DataContext = vm;
        }

        private void list_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void Btn_VideoItem(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as VideoModel;
            if (item != null)
            {
                NavigationService.GetNavigationService(this).Navigate(new VideoInfoView(item));
            }
        }
    }
}
