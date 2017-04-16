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
using ExHentaiDownloader.T;
using System.Windows.Threading;
using ExHentaiDownloader.Command;
using ExHentaiDownloader.Dialog;
using System.Windows.Media;
using System.Text.RegularExpressions;

namespace ExHentaiDownloader.ViewModel
{
    public class VM_Comic : INotifyPropertyChanged
    {
        #region Private
        private M_Comic _comic;
        private Task _task;
        private SolidColorBrush _background;
        #endregion

        #region Create
        public VM_Comic()
        {
            _comic = new M_Comic();
        } 
        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string ss)
        {
            if (PropertyChanged != null)
                 PropertyChanged(this, new PropertyChangedEventArgs(ss));            
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
                    //                   MyThreadPool<VM_Comic>.AddQueueUserWork(_ShowImage, this);
                    _task = new Task(async () =>
                    {
                        await App.Current.Dispatcher.BeginInvoke(new Action(async () =>
                        {
                            try
                            {
                                ComicImage = await GetImage(value);
                                OnPropertyChanged("ComicImage");
                            }
                            catch (Exception)
                            {
                            }

                        }));
                    }, TaskCreationOptions.LongRunning);
                    _task.Start();

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

        public void DownliadImage()
        {
            MyThreadPool<VM_Comic>.AddQueueUserWork(_DownloadImage, this);
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

        private async Task _ShowImage(object sender)
        {
            VM_Comic comic = (VM_Comic)sender;
            if (comic == null) return;
            WebClient client = new WebClient();

            client.Headers["Cookie"] = CookieHelper.GetCookie();
            var tt = client.DownloadDataTaskAsync(comic.ThumbnailLink);

            byte[] bit = await tt;
            using (MemoryStream ms = new System.IO.MemoryStream(bit))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                ms.Dispose();
 //               App.Current.Dispatcher.Invoke(() =>
 //               {
                    comic.ComicImage = bitmapImage;
 //               });
            }

        }

        private async Task _DownloadImage(object sender)
        {
            VM_Comic comic = (VM_Comic)sender;
            int number = int.Parse(comic.ComicNumber) - 1;
            if (comic == null) return;
            WebClient client = new WebClient();
            string SavePath = null;
            SavePath = comic.ComicName.Trim();
            SavePath = f_CleanInput(SavePath);
            if (SavePath.Length > 100) SavePath = SavePath.Substring(0, 100);
            Directory.CreateDirectory(SavePath);


            client.Headers["Cookie"] = CookieHelper.GetCookie();
            var tt = client.DownloadDataTaskAsync(comic.ImageLink);
            client.DownloadProgressChanged += (s, e) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    comic.ProgressPercentage = e.ProgressPercentage;
                    comic.ProgressStatus = "Downloading";
                });
            };
            client.DownloadDataCompleted += (s, e) =>
            {

                if (!e.Cancelled && e.Error == null)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        comic.ProgressPercentage = 100;
                        comic.ProgressStatus = "Success";
                        comic.ProgressColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF017C11"));
                    });
                }
                else
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        comic.ProgressPercentage = 0;
                        comic.ProgressStatus = "Fail";
                        comic.ProgressColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7A0000"));
                    });
                }

            };

            byte[] bit = await tt;
            using (MemoryStream ms = new System.IO.MemoryStream(bit))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                ms.Dispose();

                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                String photolocation = SavePath + "/" + comic.ComicNumber + ".jpg";
                encoder.Frames.Add(BitmapFrame.Create((BitmapImage)bitmapImage));
                using (var filestream = new FileStream(photolocation, FileMode.Create))
                    encoder.Save(filestream);

            }

        }
        private string f_CleanInput(string strIn)
        {
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-\[\]\(\)\-]", " ",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
        #endregion

        #region Command
        //private CommandHandler _IsSelected;
        //public CommandHandler IsSelected
        //{
        //    get
        //    {
        //        return (_IsSelected) ?? (_IsSelected = new CommandHandler(x =>
        //          {
        //              new W_ShowComic(this).ShowDialog();
        //          }));
        //    }
        //}

        #endregion




    }
}
