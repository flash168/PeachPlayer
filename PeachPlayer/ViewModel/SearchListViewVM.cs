using Peach.DataAccess.Models;
using Peach.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PeaPlayer.ViewModel
{

    internal class SearchListViewVM : NotifyProperty
    {
        private ObservableCollection<VideoModel> dataList;
        public ObservableCollection<VideoModel> DataList
        {
            get { return dataList; }
            set { SetProperty(ref dataList, value); }
        }

        public SearchListViewVM(string wd)
        {
            GetData(wd);
        }

        async void GetData(string wd)
        {
            var data = await LeaderServices.Instance.Search(wd);
            if (data != null && data.List.Count > 0)
                DataList = data.List;
        }


    }

}

