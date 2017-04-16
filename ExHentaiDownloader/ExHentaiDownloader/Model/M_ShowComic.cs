using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExHentaiDownloader.Model
{
    public class M_ShowComic
    {
        #region Struct

        public struct _ShowComicData
        {
            public string ComicName;
            public string ComicLink;
            public string ImageLink;
            public BitmapImage ComicImage;
        }

        public struct _ProgressData
        {
            public SolidColorBrush ProgressStatus;
            public int ProgressValue;
            public bool ProgressIndeterminate;
            public Visibility ProgressVisibility;
        }

        #endregion

        #region Property
        public _ShowComicData ShowComicData;
        public _ProgressData ProgressData;
        #endregion
    }
}
