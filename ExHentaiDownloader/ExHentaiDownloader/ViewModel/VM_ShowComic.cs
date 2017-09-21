using ExHentaiDownloader.Command;
using ExHentaiDownloader.Dialog;
using ExHentaiDownloader.Guild;
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
    public class VM_ShowComic : VM_Comic
    {
        #region Private
        private CancellationTokenSource _cts;
        private ObservableCollection<VM_Comic> _comicCollect;
        #endregion

        #region Create
        public VM_ShowComic(VM_Comic temp)
        {
            _cts = new CancellationTokenSource();
            base.ComicLink= temp.ComicLink;
            base.ComicName = temp.ComicName;

            ProgressColor = Brushes.Green;
            ProgressVisibility = Visibility.Collapsed;

            OnLoading(ComicLink, CookieHelper.GetCookie());
        }

        #endregion

        #region Property
        #region ShowComic
        private string _ImageLink;
        new public string ImageLink
        {
            get { return _ImageLink; }
            set
            {
                if (_ImageLink != value)
                {
                    _ImageLink = value;
                                     
                    OnPropertyChanged("ImageLink");
                    ThumbnailLink = _ImageLink;
                }
            }
        }
        #endregion

        #region Progress
        private Visibility _ProgressVisibility;
        public Visibility ProgressVisibility
        {
            get { return _ProgressVisibility; }
            set
            {
                if (_ProgressVisibility != value)
                {
                    _ProgressVisibility = value;
                    OnPropertyChanged("ProgressVisibility");
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
                    _comicCollect.CollectionChanged += _comicCollect_CollectionChanged;

                    OnPropertyChanged("ComicCollect");
                }
            }
        }

        private void _comicCollect_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {           
            VM_Comic_Collect temp = _comicCollect as VM_Comic_Collect;
            if (temp.MaxCount == temp.Count)
                canDownload = true;

            //if (temp.Count > 1)
            //    temp.RemoveAt(0);
        }

        private bool _canDownload;
        public bool canDownload
        {
            get
            {
                return _canDownload;
            }
            set
            {
                if (_canDownload != value)
                {
                    _canDownload = value;
                    OnPropertyChanged("canDownload");
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
                    ComicCollect.Clear();
                    GC.Collect();
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
