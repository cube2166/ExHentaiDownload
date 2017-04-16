using ExHentaiDownloader.ViewModel;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ExHentaiDownloader.Http
{
    public static class ParseHelper
    {
        public const string unconfig = ";uconfig=tl_m-uh_y-rc_0-cats_0-xns_0-ts_l-tr_2-prn_y-dm_t-ar_0-rx_0-ry_0-ms_n-mt_n-cs_a-to_a-pn_0-sc_0-sa_y-oi_n-qb_n-tf_n-hp_-hk_-xl_";

        private static int GetMaxImageCount(string Str)
        {
            var mates = Regex.Matches(Str, "[0-9][0-9]{0,}");
            int returnint;
            if (int.TryParse(mates[2].Value, out returnint))
            {
                return returnint;
            }
            else
            {
                return 0;
            }
        }
        public async static Task<ObservableCollection<VM_Comic>> GetMainListAsync(string uri, string cookie)
        {
            string htmlstring = await HttpHandler.GetStringWithCookie(uri, cookie + unconfig);
            return MainString2List(htmlstring);
        }
        private static ObservableCollection<VM_Comic> MainString2List(string htmlstring)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlstring);
            var htmlnode = doc.DocumentNode.GetNodebyClassName("itg");
            return MainHtmlNode2List(htmlnode);
        }
        private static ObservableCollection<VM_Comic> MainHtmlNode2List(HtmlNode htmlnode)
        {
            var result = from a in htmlnode.ChildNodes
                         where a.HasChildNodes
                         select new VM_Comic
                         {
                             ComicName = HtmlEntity.DeEntitize(a.GetNodebyClassName("id2").InnerText),
                             ThumbnailLink = (a.GetNodebyClassName("id3").Element("a").Element("img").Attributes["src"].Value),
                             ComicLink = (a.GetNodebyClassName("id2").Element("a").Attributes["href"].Value),
                             ComicNumber = (a.GetNodebyClassName("id42").InnerText),
                         };
            var dsa = new ObservableCollection<VM_Comic>(result);
            return new ObservableCollection<VM_Comic>(result);
        }

        #region Self
        public async static Task<ObservableCollection<VM_Comic>> ParseDeepSearch(VM_Comic vmc, string cookie, CancellationToken cts)
        {
            string url = vmc.ComicLink;
            //下載原始碼
            string htmlstring = await HttpHandler.GetStringWithCookie(url, cookie + unconfig);
            ObservableCollection<VM_Comic> tempList = new ObservableCollection<VM_Comic>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlstring);

            //找尋最多頁面到幾號
            int MaxPage = 0;
            HtmlNode pageNode = doc.DocumentNode.SelectSingleNode(@"//div[@class='gtb']");
            try
            {
                string pageString = pageNode.InnerText;
                string[] tempArray = pageString.Split(new char[] { ';', '&' });
                string MaxPages = tempArray[2];
                int threepoint = MaxPages.IndexOf("...");
                if (threepoint != -1)
                {
                    string temp = MaxPages.Substring(threepoint + 3, (MaxPages.Length - (threepoint + 3)));
                    MaxPage = int.Parse(temp);
                }
                else
                {
                    MaxPage = int.Parse(tempArray[2]) % 10;
                }

            }
            catch (Exception)
            {
            }

            Task[] TaskList = new Task[5];

            for (int iii = 0; iii < TaskList.Length; iii++)
            {
                //TaskList[iii] = new Task((number) =>
                //TaskList[iii] = Task.Factory.StartNew(async (number) =>
                //{
                //    for (int ll = (int)number; ll < MaxPage; ll += TaskList.Length)
                //    {

                //        int kk = ll;
                //        int jj = (kk * 40) + 1;
                //        //                       HtmlWeb webClient2 = new HtmlWeb();
                //        string tempStr = await HttpHandler.GetStringWithCookie(url + string.Format("?p={0}", kk), cookie + unconfig);
                //        HtmlDocument tempDoc = new HtmlDocument();
                //        tempDoc.LoadHtml(tempStr);
                //        //                        HtmlDocument doc2 = webClient.Load(url + string.Format("?p={0}", kk));

                //        HtmlNodeCollection evenDoc = tempDoc.DocumentNode.SelectNodes(@"//div[@class='gdtm']/div/a");
                //        if (evenDoc == null) continue;

                //        foreach (var item in evenDoc)
                //        {
                //            if (cts.IsCancellationRequested)
                //            {
                //                // 這裡撰寫取消工作的程式碼。
                //                return;
                //            }
                //            VM_Comic tempVM = new VM_Comic();
                //            string tempComicLink = item.GetAttributeValue("href", "");

                //            if (tempComicLink != null)
                //            {
                //                //HtmlDocument doc3 = webClient.Load(tempM.ComicLink);
                //                string tempStr2 = await HttpHandler.GetStringWithCookie(tempComicLink, cookie + unconfig);
                //                HtmlDocument tempDoc2 = new HtmlDocument();
                //                tempDoc2.LoadHtml(tempStr2);


                //                tempVM.ImageLink = tempDoc2.DocumentNode.SelectSingleNode(@"//img[@id='img']").GetAttributeValue("src", "");
                //            }
                //            tempVM.ComicName = vmc.ComicName;
                //            tempVM.ComicNumber = jj++.ToString();
                //            lock (tempList)
                //            {
                //                f_OnComicInsert(tempList, tempVM);
                //            }
                //        }
                //    }
                //}, iii);
                //                TaskList[iii].Start();

                TaskList[iii] = new Task(async (number) =>
              {
                  for (int ll = (int)number; ll < MaxPage; ll += TaskList.Length)
                  {

                      int kk = ll;
                      int jj = (kk * 40) + 1;
                       //                       HtmlWeb webClient2 = new HtmlWeb();
                       string tempStr = await HttpHandler.GetStringWithCookie(url + string.Format("?p={0}", kk), cookie + unconfig);
                      HtmlDocument tempDoc = new HtmlDocument();
                      tempDoc.LoadHtml(tempStr);
                       //                        HtmlDocument doc2 = webClient.Load(url + string.Format("?p={0}", kk));

                       HtmlNodeCollection evenDoc = tempDoc.DocumentNode.SelectNodes(@"//div[@class='gdtm']/div/a");
                      if (evenDoc == null) continue;

                      foreach (var item in evenDoc)
                      {

                          if (cts.IsCancellationRequested)
                          {
                               // 這裡撰寫取消工作的程式碼。
                               return;
                          }
                          VM_Comic tempVM = new VM_Comic();
                          string tempComicLink = item.GetAttributeValue("href", "");

                          if (tempComicLink != null)
                          {
                               //HtmlDocument doc3 = webClient.Load(tempM.ComicLink);
                               string tempStr2 = await HttpHandler.GetStringWithCookie(tempComicLink, cookie + unconfig);  //
                               HtmlDocument tempDoc2 = new HtmlDocument();
                              tempDoc2.LoadHtml(tempStr2);


                              tempVM.ImageLink = tempDoc2.DocumentNode.SelectSingleNode(@"//img[@id='img']").GetAttributeValue("src", "");
                          }
                          tempVM.ComicName = vmc.ComicName;
                          tempVM.ComicNumber = jj++.ToString();
                          lock (tempList)
                          {
                              f_OnComicInsert(tempList, tempVM);
                          }
                      }

                  }
              }, iii,cts ,TaskCreationOptions.LongRunning);
                TaskList[iii].Start();
            }
            Task.WaitAll(TaskList);
            return tempList;
        }
        private static ComicComp comp = new ComicComp();

        private static void f_OnComicInsert(ObservableCollection<VM_Comic> dist, VM_Comic src)
        {
            myClass.AddSorted(dist, src, comp);
            //App.Current.Dispatcher.Invoke(() =>
            //{
            //    //                dist.f_InsertComic(src,index);
            //    myClass.AddSorted(dist, new Comic_VM(src), comp);
            //    //               this.ParserStatus = status;
            //});
            ////           dist.f_AddComic(src);
        }


        #endregion


    }
    #region else

    public static class myClass
    {
        public static void AddSorted<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (comparer == null)
                    comparer = Comparer<T>.Default;

                int i = 0;
                while (i < list.Count && comparer.Compare(list[i], item) < 0)
                    i++;

                list.Insert(i, item);

            });
        }
    }

    class ComicComp : IComparer<VM_Comic>
    {
        // Compares by Height, Length, and Width.
        public int Compare(VM_Comic x, VM_Comic y)
        {
            if (x.ComicNumber.CompareTo(y.ComicNumber) != 0)
            {
                return int.Parse(x.ComicNumber).CompareTo(int.Parse(y.ComicNumber));
            }
            else
            {
                return 0;
            }
        }
    }
    #endregion
}
