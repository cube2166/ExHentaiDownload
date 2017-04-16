using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ExHentaiDownloader.Model
{
    public class M_Login
    {

        #region Struct

        public struct _UserData
        {
            public string UserName;
            public string Password;
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
        public _UserData UserData;
        public _ProgressData ProgressData;
        #endregion

    }
}
