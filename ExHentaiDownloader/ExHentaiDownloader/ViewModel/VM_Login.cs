using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExHentaiDownloader.Model;
using System.ComponentModel;
using System.Windows.Media;
using ExHentaiDownloader.Command;
using System.Windows;

namespace ExHentaiDownloader.ViewModel
{
    public class VM_Login :INotifyPropertyChanged
    {
        #region Private
        private M_Login _model;
        #endregion

        #region Create
        public VM_Login()
        {
            _model = new M_Login();
            _model.ProgressData.ProgressValue = 100;
            _model.ProgressData.ProgressIndeterminate = true;
            _model.ProgressData.ProgressVisibility = Visibility.Collapsed;
        }
        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange(string ss)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(ss));
        }
        #endregion

        #region Property
        #region UserData
        public string UserName
        {
            get { return _model.UserData.UserName; }
            set
            {
                if(_model.UserData.UserName != value)
                {
                    _model.UserData.UserName = value;
                    OnPropertyChange("UserName");
                }
            }
        }
        public string Password
        {
            get { return _model.UserData.Password; }
            set
            {
                if (_model.UserData.Password != value)
                {
                    _model.UserData.Password = value;
                    OnPropertyChange("Password");
                }
            }
        }
        #endregion
        #region ProgressData
        public int ProgressValue
        {
            get { return _model.ProgressData.ProgressValue; }
            set
            {
                if (_model.ProgressData.ProgressValue != value)
                {
                    _model.ProgressData.ProgressValue = value;
                    OnPropertyChange("ProgressValue");
                }
            }
        }
        public SolidColorBrush ProgressStatus
        {
            get { return _model.ProgressData.ProgressStatus; }
            set
            {
                if (_model.ProgressData.ProgressStatus != value)
                {
                    _model.ProgressData.ProgressStatus = value;
                    OnPropertyChange("ProgressStatus");
                }
            }
        }
        public bool ProgressIndeterminate
        {
            get { return _model.ProgressData.ProgressIndeterminate; }
            set
            {
                if (_model.ProgressData.ProgressIndeterminate != value)
                {
                    _model.ProgressData.ProgressIndeterminate = value;
                    OnPropertyChange("ProgressIndeterminate");
                }
            }
        }
        public Visibility ProgressVisibility
        {
            get { return _model.ProgressData.ProgressVisibility; }
            set
            {
                if (_model.ProgressData.ProgressVisibility != value)
                {
                    _model.ProgressData.ProgressVisibility = value;
                    OnPropertyChange("ProgressVisibility");
                }
            }
        }
        #endregion

        #endregion

        #region Command
        public const string LoginClickStr = "LoginClick";
        private CommandHandler _LoginClick;
        public CommandHandler LoginClick
        {
            get
            {
                return _LoginClick ?? (_LoginClick = new CommandHandler(x => ExecuteDelegate(LoginClickStr)));
            }
        }

        public Func<object, string, Task> LoginWindowDelegate;

        private void ExecuteDelegate(string ss)
        {
            if (LoginWindowDelegate != null)
                LoginWindowDelegate(this, ss);
        }
        #endregion

        #region Method

        public void testprogress()
        {
            this.ProgressIndeterminate = !this.ProgressIndeterminate;
        }
        #endregion
    }
}
