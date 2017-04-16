using ExHentaiDownloader.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// W_ComicDownload.xaml 的互動邏輯
    /// </summary>
    public partial class W_ComicDownload : Window
    {
        private VM_DWComic _viewModel;
        public W_ComicDownload(ObservableCollection<VM_Comic> temp)
        {
            InitializeComponent();
            base.DataContext = _viewModel = new VM_DWComic(temp);
        }
    }
}
