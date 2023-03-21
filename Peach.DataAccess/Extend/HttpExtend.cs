﻿using RestSharp;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


namespace Peach.DataAccess.Extend
{
    public interface IProgress<T>
    {
        void Report(T data);
    }

    public class Progress<T> : IProgress<T>
    {
        private readonly SynchronizationContext _context;

        public Progress()
        {
            _context = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        public Progress(Action<T> action) : this()
        {
            ProgressReported += action;
        }

        void IProgress<T>.Report(T data)
        {
            var action = ProgressReported;
            if (action != null)
            {
                _context.Post(arg => action((T)arg), data);
            }
        }

        public event Action<T> ProgressReported;
    }

    public struct DownloadDataStruct
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Complete { get; set; }
        public long CurrentSize { get; set; }
        public long FinalFileSize { get; set; }
    }

    public struct DownloadData
    {
        public long Id { get; set; }
        public DownStatus Status { get; set; }
        public long CurrentSize { get; set; }
        public long FinalFileSize { get; set; }
    }
    public enum DownStatus
    {
        [Description("待加队列")]//删除
        Wait = 1,
        [Description("等待下载")]//取消
        Start = 2,
        [Description("下载中")]//取消
        DownLoading = 4,
        [Description("已完成")]//删除 打开
        Complete = 8,
        [Description("异常")]//删除 重新下载
        Error = 16,
        [Description("已取消")]//删除 重新下载
        Cancel = 32
    }


    public class HikFileStream : FileStream
    {
        public HikFileStream(string path) : base(path, FileMode.Create, FileAccess.Write, FileShare.None) { }

        public long CurrentSize { get; private set; }
        public event EventHandler Progress;

        public override void Write(byte[] array, int offset, int count)
        {
            base.Write(array, offset, count);
            CurrentSize += count;
            var h = Progress;
            h?.Invoke(this, EventArgs.Empty); //WARN: THIS SHOULD RETURNS ASAP!
        }
    }

    public static class HttpExtend
    {
        public static bool DownLoad(string baseurl, string file, string path, long tid, CancellationTokenSource cts, Action<DownloadData> action)
        {
            var tempFile = Path.Combine(path, file);
            string p = Path.GetDirectoryName(tempFile);
            if (!Directory.Exists(p))
                Directory.CreateDirectory(p);

            if (File.Exists(tempFile))
                File.Delete(tempFile);

            DownloadData data = new DownloadData() { Id = tid };
            try
            {
                HttpWebRequest req = WebRequest.Create(baseurl) as HttpWebRequest;
                HttpWebResponse response = req.GetResponse() as HttpWebResponse;

                data.FinalFileSize = response.ContentLength;
                using (var writer = new HikFileStream(tempFile))
                {
                    writer.Progress += (w, arg) =>
                    {
                        if (cts.IsCancellationRequested)
                        {
                            response.Close();
                            data.Status = DownStatus.Cancel;
                            action.Invoke(data);
                        }
                        data.Status = DownStatus.DownLoading;
                        data.CurrentSize = writer.CurrentSize;
                        action.Invoke(data);
                    };
                    Stream stream = response.GetResponseStream();
                    stream.CopyTo(writer);

                    if (data.CurrentSize == data.FinalFileSize)
                    {
                        data.Status = DownStatus.Complete;
                        action.Invoke(data);
                    }
                }
            }
            catch (Exception ex)
            {
                if (cts.IsCancellationRequested)
                    data.Status = DownStatus.Cancel;
                else
                    data.Status = DownStatus.Error;
                action.Invoke(data);
                return false;
            }
            return true;
        }


    }
}
