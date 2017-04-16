using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ExHentaiDownloader.Dialog;
using ExHentaiDownloader.Http;

namespace ExHentaiDownloader
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        //new W_Login().show();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                LogInHelper.LogCookieCheck(CookieHelper.GetCookie());
                new MainWindow().Show();
            }
            catch (Exception)
            {
                new W_Login().Show();
            }
        }
    }

}
