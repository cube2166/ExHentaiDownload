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
            int ind = 0;
            ObservableCollection<VM_Comic> result = new ObservableCollection<VM_Comic>();
            foreach (var item in htmlnode.ChildNodes)
            {
                try
                {
                    VM_Comic temp = new VM_Comic();
                    temp.ComicName = item.GetNodebyClassName("it5").InnerText;
                    temp.ThumbnailLink = item.GetNodebyClassName("it2").InnerText;
                    string ThumbnailLink_temp = temp.ThumbnailLink.Split(new char[] { '~' })[2];
                    if (ThumbnailLink_temp == string.Empty)
                        continue;
                    ThumbnailLink_temp = "https://exhentai.org/" + ThumbnailLink_temp;
                    temp.ThumbnailLink = ThumbnailLink_temp;
                    temp.ComicLink = item.GetNodebyClassName("it5").Element("a").Attributes["href"].Value;
                    temp.ComicNumber = ind.ToString();
                    result.Add(temp);
                    ind++;
                }
                catch (Exception)
                {
                }
            }
            //VM_Comic temp = new VM_Comic();
            //temp.ComicName = htmlnode.ChildNodes[10].GetNodebyClassName("it5").InnerText;
            //temp.ThumbnailLink = htmlnode.ChildNodes[10].GetNodebyClassName("it2").InnerText;
            //string ThumbnailLink_temp = temp.ThumbnailLink.Split(new char[] { '~' })[2];
            ////if (ThumbnailLink_temp == string.Empty)
            ////    continue;
            //ThumbnailLink_temp = "exhentai.org/" + ThumbnailLink_temp;
            //temp.ThumbnailLink = ThumbnailLink_temp;
            //temp.ComicLink = htmlnode.ChildNodes[10].GetNodebyClassName("it5").Element("a").Attributes["href"].Value;

            //var result = from a in htmlnode.ChildNodes
            //             where a.HasChildNodes
            //             select new VM_Comic
            //             {
            //                 //ComicName = HtmlEntity.DeEntitize(a.GetNodebyClassName("id2").InnerText),
            //                 ComicName = HtmlEntity.DeEntitize(a.GetNodebyClassName("it5").InnerText),
            //                 ThumbnailLink = (a.GetNodebyClassName("id3").Element("a").Element("img").Attributes["src"].Value),
            //                 //ComicLink = (a.GetNodebyClassName("id2").Element("a").Attributes["href"].Value),
            //                 ComicLink = (a.GetNodebyClassName("it5").Element("a").Attributes["href"].Value),
            //                 ComicNumber = (a.GetNodebyClassName("id42").InnerText),
            //             };
            //return new ObservableCollection<VM_Comic>(result);
            return result;
        }

        #region Self
        public async static Task<ObservableCollection<VM_Comic>> ParseDeepSearch(VM_Comic vmc, string cookie, CancellationToken cts)
        {
            string url = vmc.ComicLink;
            //下載原始碼
            string htmlstring = await HttpHandler.GetStringWithCookie(url, cookie + unconfig);
            if (htmlstring == string.Empty) return null;
            //ObservableCollection<VM_Comic> tempList = new ObservableCollection<VM_Comic>();
            VM_Comic_Collect tempList = new VM_Comic_Collect();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlstring);

            //找尋最多頁面到幾號
            int MaxPage = 0;
            int MaxCount = 0;
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
                    MaxPage = (int)(long.Parse(tempArray[2]) % 10);
                    if (MaxPage == 0) MaxPage = 10;
                }

                string[] tempArray2 = tempArray[0].Split(new char[] { ' ' });
                MaxCount = int.Parse(tempArray2[5]);
                tempList.MaxCount = MaxCount;
            }
            catch (Exception e)
            {
            }

            //找名子 <h1 id="gj">[きゃろっと] 夫の同僚に過去の学生の頃の私と現在の人妻の私が種づけされちゃうお話 その後</h1>

            string thisName = null;
            HtmlNode nameNode = doc.DocumentNode.SelectSingleNode(@"//h1[@id='gj']");
            try
            {
                thisName = nameNode.InnerText;
                if (thisName == string.Empty) thisName = vmc.ComicName;
            }
            catch (Exception)
            {
            }


            Task[] TaskList = new Task[5];

            for (int iii = 0; iii < TaskList.Length; iii++)
            {
              TaskList[iii] = new Task(async (number) =>
              {
                  for (int ll = (int)number; ll < MaxPage; ll += TaskList.Length)
                  {

                      int kk = ll;
                      int jj = (kk * 40) + 1;
                       string tempStr = await HttpHandler.GetStringWithCookie(url + string.Format("?p={0}", kk), cookie + unconfig);
                      if (tempStr == string.Empty) continue;
                      HtmlDocument tempDoc = new HtmlDocument();
                      tempDoc.LoadHtml(tempStr);
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
                              if (tempStr2 == string.Empty) continue;
                              HtmlDocument tempDoc2 = new HtmlDocument();
                              tempDoc2.LoadHtml(tempStr2);


                              tempVM.ImageLink = tempDoc2.DocumentNode.SelectSingleNode(@"//img[@id='img']").GetAttributeValue("src", "");
                              tempVM.ComicLink = tempComicLink;
                          }
                          tempVM.ComicName = thisName;

                          tempVM.ComicNumber = jj.ToString();

                          //tempVM.ComicNumber = jj++.ToString();
                          lock (tempList)
                          {
                              f_OnComicInsert(tempList, tempVM);
                          }
                          jj++;
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
