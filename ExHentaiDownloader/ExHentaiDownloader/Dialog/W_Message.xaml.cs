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

namespace ExHentaiDownloader.Dialog
{
    /// <summary>
    /// W_Message.xaml 的互動邏輯
    /// </summary>
    public partial class W_Message : Window
    {
        public W_Message(string title, string message)
        {
            InitializeComponent();
            this.MessageBoxTitle.Text = title;
            this.Message.Text = message;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
