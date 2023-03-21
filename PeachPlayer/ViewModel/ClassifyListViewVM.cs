using Peach.DataAccess;
using Peach.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeaPlayer.ViewModel
{
    internal class ClassifyListViewVM : NotifyProperty
    {
        private ObservableCollection<VideoModel> dataList;
        public ObservableCollection<VideoModel> DataList
        {
            get { return dataList; }
            set { SetProperty(ref dataList, value); }
        }


        public ClassifyListViewVM(string tag)
        {
            if (tag.Equals("-1"))
            {
                GetData();
            }
            else
            {
                GetDataByClass(tag);
            }
        }

        async void GetData()
        {
            var datas = await LeaderServices.Instance.GetHomeVod();
            if (datas != null)
                DataList = new ObservableCollection<VideoModel>(datas.List);
        }

        async void GetDataByClass(string tid)
        {
            var datas = await LeaderServices.Instance.GetCategory(tid, "1", "", "");
            if (datas != null)
                DataList = new ObservableCollection<VideoModel>(datas.List);
        }
    }
}
