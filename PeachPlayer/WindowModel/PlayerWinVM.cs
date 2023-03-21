using Peach.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Peach.DataAccess;
using PeaPlayer.uc;
using System.IO;
using Vlc.DotNet.Wpf;
using Vlc.DotNet.Core;

namespace PeaPlayer.ViewModel
{
    internal class PlayerWinVM : NotifyProperty
    {
        private PlayerData move;
        public PlayerData Move { get => move; set { SetProperty(ref move, value); } }

        VlcControl vlcVideo;

        string selji;
        public PlayerWinVM(PlayerData video, string ji)
        {
            SniffingService.Instance.OnResponseReceived += View2_OnResponseReceived;
            Move = video;
            selji = ji;
        }

        public void InitVLC(ContentControl contentCtrl)
        {
            if (this.vlcVideo?.SourceProvider?.MediaPlayer != null)
            {
                this.vlcVideo.SourceProvider.MediaPlayer.PositionChanged -= MediaPlayer_PositionChanged;
                this.vlcVideo.SourceProvider.MediaPlayer.LengthChanged -= MediaPlayer_LengthChanged;
            }
            this.vlcVideo = new VlcControl();
            contentCtrl.Content = this.vlcVideo;
            var libDirectory = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\LibVlc");
            this.vlcVideo.SourceProvider.CreatePlayer(libDirectory);//创建视频播放器
            this.vlcVideo.SourceProvider.MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;//视频的定位移动事件
            this.vlcVideo.SourceProvider.MediaPlayer.LengthChanged += MediaPlayer_LengthChanged;//播放视频源的视频长度

            GetVideoInfo(Move.Title, Move.Collection.FirstOrDefault(s => s.Name == selji)?.Purl);
        }

        private void MediaPlayer_LengthChanged(object? sender, VlcMediaPlayerLengthChangedEventArgs e)
        {
        }

        private void MediaPlayer_PositionChanged(object? sender, VlcMediaPlayerPositionChangedEventArgs e)
        {
        }


        async void GetVideoInfo(string flag, string id)
        {
            var data = await LeaderServices.Instance.GetPlayInfo(flag, id);
            if (data.jx == 1)
            {
                //去解析
            }
            else
            {
                if (data.parse == 1)
                    SniffingService.Instance.GoUrl(data.url);
                else
                    Play(data.url);
            }
        }

        public void Play(string videourl)
        {
            vlcVideo.SourceProvider.MediaPlayer.Play(new Uri(videourl));
        }

        public void Pause()
        {
            vlcVideo.SourceProvider.MediaPlayer.Pause();
        }

        private void View2_OnResponseReceived(string obj)
        {
            Play(obj);
        }

    }

}
