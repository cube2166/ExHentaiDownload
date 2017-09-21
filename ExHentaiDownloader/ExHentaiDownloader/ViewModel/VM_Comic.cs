using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExHentaiDownloader.Model;
using System.ComponentModel;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Net;
using ExHentaiDownloader.Http;
using System.IO;
using System.Windows.Markup;
using ExHentaiDownloader.Guild;
using System.Windows.Threading;
using ExHentaiDownloader.Command;
using ExHentaiDownloader.Dialog;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ExHentaiDownloader.ViewModel
{
    public class VM_Comic : INotifyPropertyChanged
    {
        #region Private
        private M_Comic _comic;
        private SolidColorBrush _background;
        #endregion

        #region Create
        public VM_Comic()
        {
            _comic = new M_Comic();
        }

        public VM_Comic(VM_Comic src)
        {
            _comic = new M_Comic();
            _comic.ComicImage = src.ComicImage;
            _comic.ComicLink = src.ComicLink;
            _comic.ComicName = src.ComicName;
            _comic.ComicNumber = src.ComicNumber;
            _comic.ImageLink = src.ImageLink;
            //_comic.ThumbnailLink = src.ThumbnailLink;
        }
        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string ss)
        {
            if (PropertyChanged != null)
                 PropertyChanged(this, new PropertyChangedEventArgs(ss));            
        }

        public event Action<VM_Comic> finishEvent;
        public void OnFinished()
        {
            if (finishEvent != null)
                finishEvent(this);
        }

        #endregion

        #region Property
        public string ComicName
        {
            get { return _comic.ComicName; }
            set
            {
                if(_comic.ComicName!=value)
                {
                    _comic.ComicName = value;
                    OnPropertyChanged("ComicName");
                }
            }
        }
        public string ComicLink
        {
            get { return _comic.ComicLink; }
            set
            {
                if (_comic.ComicLink != value)
                {
                    _comic.ComicLink = value;
                    OnPropertyChanged("ComicLink");
                }
            }
        }
        public string ComicNumber
        {
            get { return _comic.ComicNumber; }
            set
            {
                if (_comic.ComicNumber != value)
                {
                    _comic.ComicNumber = value;
                    OnPropertyChanged("ComicNumber");
                }
            }
        }
        public string ImageLink
        {
            get { return _comic.ImageLink; }
            set
            {
                if (_comic.ImageLink != value)
                {
                    _comic.ImageLink = value;
                    OnPropertyChanged("ImageLing");
                }
            }
        }
        public string ThumbnailLink
        {
            get { return _comic.ThumbnailLink; }
            set
            {
                if (_comic.ThumbnailLink != value)
                {
                    _comic.ThumbnailLink = value;
                    OnPropertyChanged("ThumbnailLink");
                    Task.Run(() =>
                    {
                        Guild<VM_Comic>.PublishTask(_ShowImage, this);
                    });
                }
            }
        }
        public BitmapImage ComicImage
        {
            get { return _comic.ComicImage; }
            set
            {
                if (_comic.ComicImage != value)
                {
                    _comic.ComicImage = value;
                    OnPropertyChanged("ComicImage");
                }
            }
        }

        public SolidColorBrush BackGround
        {
            get { return _background; }
            set
            {
                if(_background != value)
                {
                    _background = value;
                    OnPropertyChanged("BackGround");
                }
            }
        }

        private int _ProgressPercentage;
        public int ProgressPercentage
        {
            get { return _ProgressPercentage; }
            set
            {
                if(_ProgressPercentage != value)
                {
                    _ProgressPercentage = value;
                    OnPropertyChanged("ProgressPercentage");
                }
            }
        }
        private string _ProgressStatus;
        public string ProgressStatus
        {
            get { return _ProgressStatus; }
            set
            {
                if (_ProgressStatus != value)
                {
                    _ProgressStatus = value;
                    OnPropertyChanged("ProgressStatus");
                }
            }
        }
        private SolidColorBrush _ProgressColor;
        public SolidColorBrush ProgressColor
        {
            get { return _ProgressColor; }
            set
            {
                if (_ProgressColor != value)
                {
                    _ProgressColor = value;
                    OnPropertyChanged("ProgressColor");
                }
            }
        }

        #endregion

        #region Method
        private void _ShowImage(object sender)
        {
            VM_Comic comic = (VM_Comic)sender;
            if (comic == null) return;
            using (WebClient client = new WebClient())
            {
                client.Proxy = null;

                client.Headers["Cookie"] = CookieHelper.GetCookie();
                byte[] bit = null;

                try
                {
                    var tt = client.DownloadDataTaskAsync(comic.ThumbnailLink);
                    tt.Wait(new TimeSpan(0, 0, 10));
                    if (client.IsBusy == true)
                    {
                        client.CancelAsync();
                        return;
                    }
                    else
                    {
                        bit = tt.Result;
                    }
                }
                catch (Exception e)
                {

                    return;
                }
                using (MemoryStream ms = new System.IO.MemoryStream(bit))
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    ms.Dispose();
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        comic.ComicImage = bitmapImage;
                    });
                }
            }
            GC.Collect();
        }
        #endregion

        #region Command
        private CommandHandler _OpenChrome;
        public CommandHandler OpenChrome
        {
            get
            {
                return (_OpenChrome) ?? (_OpenChrome = new CommandHandler(x =>
                  {
                      System.Diagnostics.Process.Start("chrome.exe", this.ComicLink);
                  }));
            }
        }

        #endregion
    }

    public class VM_Comic_Collect : ObservableCollection<VM_Comic>
    {
        public int MaxCount { get; set; }
    }

    //public class VM_Comic_Collect2 : VM_Comic_Collect
    //{
    //    //protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    //    //{
    //    //    if (e.Action == NotifyCollectionChangedAction.Add)
    //    //    {
    //    //        foreach (var item in e.NewItems)
    //    //        {
    //    //            VM_Comic temp = item as VM_Comic;
    //    //            if (temp == null) continue;

    //    //            temp.finishEvent += Temp_finishEvent;
    //    //        }
    //    //    }
    //    //    else if (e.Action == NotifyCollectionChangedAction.Remove)
    //    //    {
    //    //        foreach (var item in e.OldItems)
    //    //        {
    //    //            VM_Comic temp = item as VM_Comic;
    //    //            if (temp == null) continue;

    //    //            temp.finishEvent -= Temp_finishEvent;
    //    //        }
    //    //    }

    //    //    base.OnCollectionChanged(e);
    //    //}

    //    //private void Temp_finishEvent(VM_Comic obj)
    //    //{
    //    //    lock (this)
    //    //    {
    //    //        this.Remove(obj);
    //    //    }
    //    //}
    //}
}
