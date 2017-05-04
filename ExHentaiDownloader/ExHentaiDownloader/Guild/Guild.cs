using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExHentaiDownloader.Guild
{
    static public class Guild<T>
    {
        #region Private
        private const int Accommodate = 10;
        static private List<Adventurer> AdventurerList = new List<Adventurer>();
        static private Queue<Bulletin> BulletinBoard = new Queue<Bulletin>();
        static private ManualResetEvent mre = new ManualResetEvent(false);
        static private object LOCK = new object();
        private struct Bulletin
        {
            public Action<object> handle;
            public T target;
        };
        private const int restTime = 1000;
        private const int overTime = 10000;
        static private bool proAdventurer = false;
        #endregion

        #region Method
        static public void PublishTaskList(Action<object> handle, IList<T> targetList)
        {
            lock (LOCK)
            {
                foreach (var target in targetList)
                {
                    AddTaskToBulletinBoard(handle, target);
                }

                GeneralProcess();
            }
        }

        static public void PublishTask(Action<object> handle, T target)
        {
            lock (LOCK)
            {
                AddTaskToBulletinBoard(handle, target);

                GeneralProcess();
            }
        }

        static public void RequireGuildStop()
        {
            AllAdventurerStop();
            ClearBulletinBoard();
        }

        static private void AddTaskToBulletinBoard(Action<object> handle, T target)
        {
            Bulletin temp;
            temp.handle = handle;
            temp.target = target;

            BulletinBoard.Enqueue(temp);
        }

        static private void GeneralProcess()
        {
            //            RecruitProAdventurer();
            WakeUPAdventurer();
            FireAdventurer();
            RecruitAdventurer();
        }

        static private void RecruitProAdventurer()
        {
            if (proAdventurer == true) return;
            int tt = 60 * 60 * 1000;
            for (int i = 0; i < 2; i++)
            {
                Adventurer temp = new Adventurer(() => { }, BulletinBoard, tt, mre);
                AdventurerList.Add(temp);
            }
            proAdventurer = true;
        }

        static private void FireAdventurer()
        {
            int AdventurerList_Count = AdventurerList.Count;
            if (AdventurerList_Count > 0)
            {
                for (int ii = 0; ii < AdventurerList_Count; ii++)
                {
                    foreach (var Adventurer in AdventurerList)
                    {
                        if (Adventurer.WorkTime >= overTime || Adventurer.IsEnd == true)
                        {
                            Adventurer.GoAWay();   
                            AdventurerList.Remove(Adventurer);
                            break;
                        }
                    }
                }
            }
        }

        static private void WakeUPAdventurer()
        {
            int AdventurerList_Count = AdventurerList.Count;
            if (AdventurerList_Count > 0)
            {
                foreach (var Adventurer in AdventurerList)
                {
                    Adventurer.WakeUp();
                }
                mre.Set();
                mre.Reset();
            }
        }

        static private void RecruitAdventurer()
        {
            int AdventurerList_Count = AdventurerList.Count;
            if (AdventurerList_Count < Accommodate)
            {
                int preAddCount = Accommodate - AdventurerList_Count;
                for (int i = 0; i < preAddCount; i++)
                {
                    Adventurer temp = new Adventurer(() => { }, BulletinBoard, restTime, mre);
                    AdventurerList.Add(temp);
                }
            }
        }

        static private void AllAdventurerStop()
        {
            foreach (var Adventurer in AdventurerList)
            {
                Adventurer.Stop();
            }
        }

        static private void ClearBulletinBoard()
        {
            lock(BulletinBoard)
            {
                BulletinBoard.Clear();
            }
            GC.Collect();
        }
        #endregion

        class Adventurer
        {
            #region private
            private Queue<Bulletin> GuildBulletinBoard;
            private int RestTime;
            private bool _IsRest;
            private bool _IsWork;
            private bool _IsStop;
            private bool _IsEnd;
            private Stopwatch sw = new Stopwatch();
            private ManualResetEvent _mre;
            private Thread _thisTask;
            #endregion

            #region Create
            public Adventurer(Action work, Queue<Bulletin> BulletinBoard, int resttime, ManualResetEvent mre)
            {
                GuildBulletinBoard = BulletinBoard;
                RestTime = resttime;
                _mre = mre;
                GrabBulletin();
            }
            #endregion


            #region Property
            public double WorkTime
            {
                get { return sw.Elapsed.TotalMilliseconds; }
            }
            public bool IsRest
            {
                get { return _IsRest; }
                set { _IsRest = value; }
            }
            public bool IsEnd
            {
                get { return _IsEnd; }
                set { _IsEnd = value; }
            }
            #endregion

            #region Method
            private void GrabBulletin()
            {
                _IsWork = true;
                _IsStop = false;

                _thisTask = new Thread(() =>
                {
                    while (_IsEnd == false)
                    {
                        sw.Restart();
                        Bulletin temp;
                        temp.target = default(T);
                        temp.handle = null;

                        lock (GuildBulletinBoard)
                        {
                            if (GuildBulletinBoard.Count > 0)
                            {
                                temp = GuildBulletinBoard.Dequeue();
                            }
                        }
                        if (temp.target != null && temp.handle != null)
                        {
                            temp.handle(temp.target);
                            sw.Restart();
                        }
                        else
                        {
                            IsRest = true;
                            sw.Restart();
                            //                           _mre.WaitOne();
                            SpinWait.SpinUntil(() => !IsRest, restTime);
                            IsRest = false;
                            if (sw.Elapsed.TotalMilliseconds >= restTime)
                            {
                                break;
                            }
                        }
                        if (_IsStop == true)
                        {
                            //                           _mre.WaitOne();
                            _IsStop = false;
                            break;
                        }
                        SpinWait.SpinUntil(() => false, 10);
                    }
                    _IsWork = false;
                    IsRest = false;
                    _IsEnd = true;
                    sw.Stop();

                });
                _thisTask.IsBackground = true;
               _thisTask.Start();
            }

            public void WakeUp()
            {
                IsRest = false;
            }

            public void Stop()
            {
                _IsStop = true;
            }

            public void GoAWay()
            {
                _thisTask.Abort();
            }
            #endregion
        }
    }
}
