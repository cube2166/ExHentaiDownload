using ExHentaiDownloader.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ExHentaiDownloader.T;
using ExHentaiDownloader.Command;
using System.Windows.Media;

namespace ExHentaiDownloader.ViewModel
{
    public class VM_DWComic : INotifyPropertyChanged
    {
        #region Private
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
        public VM_DWComic(ObservableCollection<VM_Comic> clist)
        {
            ComicCollect = clist;
            MyThreadPool<VM_Comic>.AddQueueUserWork(DownloadImage, ComicCollect);
        }
        #endregion

        #region Property
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

        private async Task DownloadImage(object sender)
        {
            VM_Comic comic = (VM_Comic)sender;
            //ObservableCollection<VM_Comic> ComicCollect = (ObservableCollection < VM_Comic > )List;
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
                    //ComicCollect[number].ProgressPercentage = e.ProgressPercentage;
                    //ComicCollect[number].ProgressStatus = "Downloading";
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
                            //ComicCollect[number].ProgressPercentage = 100;
                            //ComicCollect[number].ProgressStatus = "Success";
                            //ComicCollect[number].ProgressColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF017C11"));
                            comic.ProgressPercentage = 100;
                            comic.ProgressStatus = "Success";
                            comic.ProgressColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF017C11"));
                        });
                }
                else
                {
                    App.Current.Dispatcher.Invoke(() =>
                        {
                            //ComicCollect[number].ProgressPercentage = 0;
                            //ComicCollect[number].ProgressStatus = "Fail";
                            //ComicCollect[number].ProgressColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7A0000"));
                            comic.ProgressPercentage = 0;
                            comic.ProgressStatus = "Fail";
                            comic.ProgressColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7A0000"));
                        });
                }

            };

            byte[] bit = await tt;
            if (bit == null || bit.Length < 5) return;
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
        private CommandHandler _IsClosed;
        public CommandHandler IsClosed
        {
            get
            {
                return (_IsClosed) ?? (_IsClosed = new CommandHandler(x =>
                {
                    MyThreadPool<VM_Comic>.AllTaskStop();
                }));
            }
        }
        #endregion


    }
}
