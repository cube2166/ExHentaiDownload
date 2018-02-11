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
using ExHentaiDownloader.Guild;
using ExHentaiDownloader.Command;
using System.Windows.Media;
using System.Threading;
using System.Collections.Specialized;

namespace ExHentaiDownloader.ViewModel
{
    public class VM_DWComic : INotifyPropertyChanged
    {
        #region Private
        private VM_Comic_Collect _comicCollect;
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
            //ComicCollect = clist;
            ComicCollect = new VM_Comic_Collect();
            foreach (var item in clist)
            {
                VM_Comic temp = new VM_Comic(item);
                ComicCollect.Add(temp);
            }
            Task.Run(() =>
            {
                SpinWait.SpinUntil(() => false, 2000);
                Guild<VM_Comic>.PublishTaskList(DownloadImage, ComicCollect);
            });           
        }
        #endregion

        #region Property
        #region ComicCollect
        public VM_Comic_Collect ComicCollect
        {
            get { return _comicCollect; }
            set
            {
                if (_comicCollect != value)
                {
                    _comicCollect = value;
                    _comicCollect.CollectionChanged += _comicCollect_CollectionChanged;

                    OnPropertyChange("ComicCollect");
                }
            }
        }

        private void _comicCollect_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    VM_Comic temp = item as VM_Comic;
                    if (temp == null) continue;

                    temp.finishEvent += Temp_finishEvent;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    VM_Comic temp = item as VM_Comic;
                    if (temp == null) continue;

                    temp.finishEvent -= Temp_finishEvent;
                }
            }
        }

        private void Temp_finishEvent(VM_Comic obj)
        {
            lock (this)
            {
                ComicCollect.Remove(obj);
            }
        }

        #endregion
        #endregion

        #region Method

        private void DownloadImage(object sender)
        {
            VM_Comic comic = (VM_Comic)sender;
            if (comic == null) return;
            WebClient client = new WebClient();
            client.Proxy = null;
            string SavePath = null;
            SavePath = comic.ComicName.Trim();
            SavePath = f_CleanInput(SavePath);
            if (SavePath.Length > 100) SavePath = SavePath.Substring(0, 100);
            Directory.CreateDirectory(SavePath);


            client.Headers["Cookie"] = CookieHelper.GetCookie();
            //            var tt = client.DownloadDataTaskAsync(comic.ImageLink);
            String photolocation = SavePath + "/" + comic.ComicNumber + ".jpg";
            int thread = 0;
            client.DownloadProgressChanged += (s, e) =>
            {
                int temp = e.ProgressPercentage;
                if (temp > thread)
                {
                    thread += 10;
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        comic.ProgressPercentage = thread;
                        comic.ProgressStatus = "Downloading";
                    });
                }
                //if (temp % 10 == 0)
                //{
                //    App.Current.Dispatcher.Invoke(() =>
                //    {
                //        comic.ProgressPercentage = temp;
                //        comic.ProgressStatus = "Downloading";
                //    });
                //}

            };
            client.DownloadFileCompleted += (s, e) =>
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
                    File.Delete(photolocation);
                }

            };
            try
            {
                var tt =  client.DownloadFileTaskAsync(new Uri(comic.ImageLink), photolocation);
                tt.Wait(new TimeSpan(0, 0, 30));

                if (client.IsBusy == true)
                {
                    client.CancelAsync();
                    return;
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    comic.OnFinished();
                });
                    
            }
            catch (Exception e)
            {
                return;
            }
            GC.Collect();
        }
        private string f_CleanInput(string strIn)
        {
            try
            {
                string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                return r.Replace(strIn, "");
                //return Regex.Replace(strIn, @"[^\w\.@-\[\]\(\)\-]", " ",
                //                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
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
                    Guild<VM_Comic>.RequireGuildStop();
                }));
            }
        }
        #endregion


    }
}
