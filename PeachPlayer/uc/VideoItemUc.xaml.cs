using Peach.DataAccess.Models;
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

namespace PeaPlayer.uc
{
    /// <summary>
    /// VideoItemUc.xaml 的交互逻辑
    /// </summary>
    public partial class VideoItemUc : UserControl
    {
        public VideoItemUc()
        {
            InitializeComponent();
        }


        public VideoModel ItemData
        {
            get { return (VideoModel)GetValue(ItemDataProperty); }
            set { SetValue(ItemDataProperty, value); }
        }
        public static readonly DependencyProperty ItemDataProperty = DependencyProperty.Register("ItemData", typeof(VideoModel), typeof(VideoItemUc), new PropertyMetadata(null, OnItemDataChanged));

        private static void OnItemDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (VideoItemUc)d;
            var item = (VideoModel)e.NewValue;
            if (item != null)
            {
                control.title.Content = item.Vod_name;
                control.score.Content = item.Vod_douban_score;
                control.mark.Content = item.Vod_remarks;
                control.image.Source = new BitmapImage(new System.Uri(item.Vod_pic, System.UriKind.RelativeOrAbsolute));
            }
        }

    }
}
