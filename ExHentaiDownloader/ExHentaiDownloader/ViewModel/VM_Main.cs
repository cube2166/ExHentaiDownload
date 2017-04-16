using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExHentaiDownloader.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using ExHentaiDownloader.Http;
using ExHentaiDownloader.Command;
using ExHentaiDownloader.Dialog;

namespace ExHentaiDownloader.ViewModel 
{
    public class VM_Main : INotifyPropertyChanged
    {
        #region Private
        private M_Main _main;
        private Task _task;
        private ObservableCollection<VM_Comic> _comicCollect;
        private const string RequestUrl = "http://exhentai.org/?";

        //private 
        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange(string ss)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(ss));
        }
        #endregion

        #region Create
        public VM_Main()
        {
            _main = new M_Main();
            OnLoading(RequestUrl, CookieHelper.GetCookie());
            //            _comicCollect = new ObservableCollection<VM_Comic>();
            _main.ProgressData.ProgressIndeterminate = true;
            _main.ProgressData.ProgressStatus = Brushes.Green;
            _main.ProgressData.ProgressValue = 0;
            _main.ProgressData.ProgressVisibility = Visibility.Collapsed;
        }
        #endregion

        #region Property

        #region MainData
        public string KeyWord
        {
            get { return _main.MainData.KeyWord; }
            set
            {
                if(_main.MainData.KeyWord!=value)
                {
                    _main.MainData.KeyWord = value;
                    OnPropertyChange("KeyWord");
                }
            }
        }
        #endregion

        #region ProgressData
        public int ProgressValue
        {
            get { return _main.ProgressData.ProgressValue; }
            set
            {
                if (_main.ProgressData.ProgressValue != value)
                {
                    _main.ProgressData.ProgressValue = value;
                    OnPropertyChange("ProgressValue");
                }
            }
        }
        public SolidColorBrush ProgressStatus
        {
            get { return _main.ProgressData.ProgressStatus; }
            set
            {
                if (_main.ProgressData.ProgressStatus != value)
                {
                    _main.ProgressData.ProgressStatus = value;
                    OnPropertyChange("ProgressStatus");
                }
            }
        }
        public bool ProgressIndeterminate
        {
            get { return _main.ProgressData.ProgressIndeterminate; }
            set
            {
                if (_main.ProgressData.ProgressIndeterminate != value)
                {
                    _main.ProgressData.ProgressIndeterminate = value;
                    OnPropertyChange("ProgressIndeterminate");
                }
            }
        }
        public Visibility ProgressVisibility
        {
            get { return _main.ProgressData.ProgressVisibility; }
            set
            {
                if (_main.ProgressData.ProgressVisibility != value)
                {
                    _main.ProgressData.ProgressVisibility = value;
                    OnPropertyChange("ProgressVisibility");
                }
            }
        }
        #endregion

        #region ComicCollect
        public ObservableCollection<VM_Comic> ComicCollect
        {
            get { return _comicCollect; }
            set
            {
                if(_comicCollect != value)
                {
                    _comicCollect = value;
                    OnPropertyChange("ComicCollect");
                }
            }
        }
        #endregion

        #endregion

        #region Method
        private async void OnLoading(string uri, string cookie)
        {
            try
            {
                ProgressVisibility = Visibility.Visible;
                ComicCollect = await ParseHelper.GetMainListAsync(uri, cookie);
                ProgressVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
            }
        }
        private async Task SearchKeyWord(string keyword, string cookie)
        {
            try
            {
                ComicCollect.Clear();
                ProgressVisibility = Visibility.Visible;
                for (int ii = 0; ii < 5; ii++)
                {
                    string url = RequestUrl + "&page=" + (ii).ToString() + "&f_search=" + keyword;

                    var morelist = await ParseHelper.GetMainListAsync(url, cookie);
                    foreach (var item in morelist)
                    {
                        ComicCollect.Add(item);
                    }
                }
                ProgressVisibility = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                ProgressVisibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Command
        public const string SearchClickStr = "SearchClick";
        private CommandHandler _SearchClick;
        public CommandHandler SearchClick
        {
            get
            {
                return _SearchClick ?? (_SearchClick = new CommandHandler(async x =>
                {
                    await SearchKeyWord(KeyWord, CookieHelper.GetCookie());
                }));
            }
        }

        private CommandHandler _openComic;
        public CommandHandler OpenComic
        {
            get
            {
                return (_openComic) ?? (_openComic = new CommandHandler(x =>
                {
                    VM_Comic temp = x as VM_Comic;
                    if (x == null) return;
                    new W_ShowComic(temp).ShowDialog();
                }));
            }
        }

        public Func<object, string, Task> LoginWindowDelegate;

        private void ExecuteDelegate(string ss)
        {
            if (LoginWindowDelegate != null)
                LoginWindowDelegate(this, ss);
        }
        #endregion
    }
}
