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

namespace ExHentaiDownloader.Dialog
{
    /// <summary>
    /// W_ShowComic.xaml 的互動邏輯
    /// </summary>
    public partial class W_ShowComic : Window
    {
        private VM_ShowComic _viewModel;
        public W_ShowComic(VM_Comic temp)
        {
            base.DataContext = _viewModel = new VM_ShowComic(temp);
            InitializeComponent();
        }
    }
}
