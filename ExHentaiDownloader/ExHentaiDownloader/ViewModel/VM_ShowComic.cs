using ExHentaiDownloader.Command;
using ExHentaiDownloader.Dialog;
using ExHentaiDownloader.Http;
using ExHentaiDownloader.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExHentaiDownloader.ViewModel
{
    public class VM_ShowComic : INotifyPropertyChanged
    {
        #region Private
        private M_ShowComic _showComic;
        private CancellationTokenSource _cts;
        private Task _task;
        private ObservableCollection<VM_Comic> _comicCollect;
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
        public VM_ShowComic(VM_Comic temp)
        {
            _cts = new CancellationTokenSource();
            _showComic = new M_ShowComic();
            _showComic.ShowComicData.ComicLink = temp.ComicLink;
            _showComic.ShowComicData.ComicName = temp.ComicName;

            _showComic.ProgressData.ProgressIndeterminate = true;
            _showComic.ProgressData.ProgressStatus = Brushes.Green;
            _showComic.ProgressData.ProgressValue = 0;
            _showComic.ProgressData.ProgressVisibility = Visibility.Collapsed;

            OnLoading(ComicLink, CookieHelper.GetCookie());
        }
        public VM_ShowComic()
        {
            _cts = new CancellationTokenSource();
            _showComic = new M_ShowComic();
            _showComic.ShowComicData.ComicLink = null;
            _showComic.ShowComicData.ComicName = null;

            _showComic.ProgressData.ProgressIndeterminate = true;
            _showComic.ProgressData.ProgressStatus = Brushes.Green;
            _showComic.ProgressData.ProgressValue = 0;
            _showComic.ProgressData.ProgressVisibility = Visibility.Collapsed;

 //           OnLoading(ComicLink, CookieHelper.GetCookie());
        }
        #endregion

        #region Property
        #region ShowComic
        public string ComicLink
        {
            get { return _showComic.ShowComicData.ComicLink; }
            set
            {
                if(_showComic.ShowComicData.ComicLink != value)
                {
                    _showComic.ShowComicData.ComicLink = value;
                    OnPropertyChange("ComicLink");
                }
            }
        }
        public string ComicName
        {
            get { return _showComic.ShowComicData.ComicName; }
            set
            {
                if (_showComic.ShowComicData.ComicName != value)
                {
                    _showComic.ShowComicData.ComicName = value;
                    OnPropertyChange("ComicName");
                }
            }
        }
        public BitmapImage ComicImage
        {
            get { return _showComic.ShowComicData.ComicImage; }
            set
            {
                if (_showComic.ShowComicData.ComicImage != value)
                {
                    _showComic.ShowComicData.ComicImage = value;
                }
            }
        }
        public string ImageLink
        {
            get { return _showComic.ShowComicData.ImageLink; }
            set
            {
                if (_showComic.ShowComicData.ImageLink != value)
                {
                    _showComic.ShowComicData.ImageLink = value;
                    OnPropertyChange("ImageLink");
                    //if (_task.Status == TaskStatus.Running)
                    //    _task.Dispose();
                    _task = Task.Run(async () =>
                    {
                        await App.Current.Dispatcher.BeginInvoke(new Action(async () =>
                        {
                            try
                            {
                                ComicImage = await GetImage(value);
                                OnPropertyChange("ComicImage");
                            }
                            catch (Exception)
                            {
                            }

                        }));
                    });
                }
            }
        }
        #endregion

        #region Progress
        public int ProgressValue
        {
            get { return _showComic.ProgressData.ProgressValue; }
            set
            {
                if (_showComic.ProgressData.ProgressValue != value)
                {
                    _showComic.ProgressData.ProgressValue = value;
                    OnPropertyChange("ProgressValue");
                }
            }
        }
        public SolidColorBrush ProgressStatus
        {
            get { return _showComic.ProgressData.ProgressStatus; }
            set
            {
                if (_showComic.ProgressData.ProgressStatus != value)
                {
                    _showComic.ProgressData.ProgressStatus = value;
                    OnPropertyChange("ProgressStatus");
                }
            }
        }
        public bool ProgressIndeterminate
        {
            get { return _showComic.ProgressData.ProgressIndeterminate; }
            set
            {
                if (_showComic.ProgressData.ProgressIndeterminate != value)
                {
                    _showComic.ProgressData.ProgressIndeterminate = value;
                    OnPropertyChange("ProgressIndeterminate");
                }
            }
        }
        public Visibility ProgressVisibility
        {
            get { return _showComic.ProgressData.ProgressVisibility; }
            set
            {
                if (_showComic.ProgressData.ProgressVisibility != value)
                {
                    _showComic.ProgressData.ProgressVisibility = value;
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
                if (_comicCollect != value)
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
                VM_Comic tempVM = new VM_Comic { ComicLink = this.ComicLink, ComicName = this.ComicName };
                ComicCollect = await ParseHelper.ParseDeepSearch(tempVM, cookie, _cts.Token);
                ProgressVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
            }
        }
        private async Task<BitmapImage> GetImage(string link)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers["Cookie"] = CookieHelper.GetCookie();
                var tt = client.DownloadDataTaskAsync(link);


                byte[] bit = await tt;
                return ByteArrayToBitmapImage(bit);
            }

        }

        private BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            using (MemoryStream ms = new System.IO.MemoryStream(byteArray))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                ms.Dispose();
                return bitmapImage;
            }

        }

        #endregion

        #region Command
        private CommandHandler _IsClosed;
        public CommandHandler IsClosed
        {
            get
            {
                return (_IsClosed) ?? (_IsClosed = new CommandHandler(x =>
                {
                    _cts.Cancel();
                }));
            }
        }

        private CommandHandler _ChooseOne;
        public CommandHandler ChooseOne
        {
            get
            {
                return (_ChooseOne) ?? (_ChooseOne = new CommandHandler(x =>
                {
                    if (x == null) return;
                    VM_ShowComic temp = this;
                    foreach (var item in temp.ComicCollect)
                    {
                        VM_Comic tt = (VM_Comic)item;
                        tt.BackGround = Brushes.Transparent;
                    }

                    VM_Comic temp2 = x as VM_Comic;
                    temp.ImageLink = temp2.ImageLink;
                    temp2.BackGround = Brushes.CadetBlue;
                }));
            }
        }
        private CommandHandler _CmdDownload;
        public CommandHandler CmdDownload
        {
            get
            {
                return (_CmdDownload) ?? (_CmdDownload = new CommandHandler(x =>
                {
                    new W_ComicDownload(ComicCollect).ShowDialog();
                }));
            }
        }


        #endregion

    }
}
