using ExHentaiDownloader.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExHentaiDownloader.Tool
{
    /// <summary>
    /// T_ComicWordList.xaml 的互動邏輯
    /// </summary>
    public partial class T_ComicWordList : UserControl
    {
        public T_ComicWordList()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            //VM_ShowComic temp = base.DataContext as VM_ShowComic;
            //foreach (var item in e.RemovedItems)
            //{
            //    VM_Comic tt = (VM_Comic)item;
            //    tt.BackGround = Brushes.Transparent;
            //}

            //VM_Comic temp2 = ((sender as ListBox).SelectedItem as VM_Comic);
            //temp.ImageLink = temp2.ImageLink;
            //temp2.BackGround = Brushes.CadetBlue;
        }

    }
}
