using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ExHentaiDownloader.ViewModel;
using ExHentaiDownloader.Http;

namespace ExHentaiDownloader.Dialog
{
    /// <summary>
    /// W_Login.xaml 的互動邏輯
    /// </summary>
    public partial class W_Login : Window
    {
        public VM_Login _viewModel;
        public W_Login()
        {
            InitializeComponent();
            base.DataContext = _viewModel = new VM_Login();
            _viewModel.LoginWindowDelegate = LoginWindowsCommand;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((VM_Login)this.DataContext).Password = ((PasswordBox)sender).Password; }
        }

        private async Task LoginWindowsCommand(object sender, string ss)
        {
            switch (ss)
            {
                case VM_Login.LoginClickStr:
                    {

                        await OnLoaddingAsync();
                        break;
  
                    }
                default:
                    break;
            }
        }

        private async Task OnLoaddingAsync()
        {
            
            try
            {
                _viewModel.ProgressVisibility = Visibility.Visible;
                _viewModel.ProgressIndeterminate = true;
                _viewModel.ProgressStatus = Brushes.Green;
                await HttpHandler.TryLog(_viewModel.UserName,_viewModel.Password);
                new MainWindow().Show();
                _viewModel.ProgressVisibility = Visibility.Collapsed;
                this.Close();
            }
            catch (LoginException ee)
            {
                _viewModel.ProgressVisibility = Visibility.Visible;
                _viewModel.ProgressIndeterminate = false;
                _viewModel.ProgressStatus = Brushes.Red;
                new W_Message("Error", ee.Message).ShowDialog();
                _viewModel.ProgressVisibility = Visibility.Collapsed;
            }
            catch (LogAccessException ee)
            {
                _viewModel.ProgressVisibility = Visibility.Visible;
                _viewModel.ProgressIndeterminate = false;
                _viewModel.ProgressStatus = Brushes.Red;
                new W_Message("Error", ee.Message).ShowDialog();
                _viewModel.ProgressVisibility = Visibility.Collapsed;
            }
        }
    }
}
