using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExHentaiDownloader.T
{
    static public class MyThreadPool<T>
    {
        #region Private
        static private int _t_maxcount;
        static private List<MyTask<T>> MyTaskList;
        static private Queue<T> _QueueData;
        static private Action<object> _thisAction;
        static private Func<object,Task> _thisAction2;
        static private CancellationTokenSource cts;
        static private ManualResetEvent mre;
        #endregion

        #region Method
        static public void AddQueueUserWork(Action<object> aa, IList<T> data)
        {
            _t_maxcount = 5;
            if (_QueueData == null) _QueueData = new Queue<T>();
            if (cts == null) cts = new CancellationTokenSource();
            if (MyTaskList == null) MyTaskList = new List<MyTask<T>>();
            if (mre == null) mre = new ManualResetEvent(false);

            foreach (var item in data)
            {
                _QueueData.Enqueue(item);
            }
            if (MyTaskList != null && MyTaskList.Count != 5)
            {
                if (_thisAction == null) _thisAction = aa;
                for (int ii = 0; ii < _t_maxcount; ii++)
                {
                    MyTaskList.Add(new MyTask<T>(_thisAction, _QueueData, cts.Token, TaskCreationOptions.LongRunning, mre));
                    MyTaskList[ii].TaskRun();
                }
            }

            if (_thisAction != aa)
            {
                foreach (var item in MyTaskList)
                {
                    item.TaskChange(aa);
                }
            }

            if (_QueueData.Count > 0)
            {
                AllTaskRun();
            }
        }

        static public void AddQueueUserWork(Func<object, Task> aa, IList<T> data)
        {
            _t_maxcount = 5;
            if (_QueueData == null) _QueueData = new Queue<T>();
            if (cts == null) cts = new CancellationTokenSource();
            if (MyTaskList == null) MyTaskList = new List<MyTask<T>>();
            if (mre == null) mre = new ManualResetEvent(false);

            foreach (var item in data)
            {
                _QueueData.Enqueue(item);
            }
            if (MyTaskList != null && MyTaskList.Count != 5)
            {
                if (_thisAction2 == null) _thisAction2 = aa;
                for (int ii = 0; ii < _t_maxcount; ii++)
                {
                    MyTaskList.Add(new MyTask<T>(_thisAction2, _QueueData, cts.Token, TaskCreationOptions.LongRunning, mre, oo => { }));
                    MyTaskList[ii].TaskRun();
                }
            }

            if (MyTaskList != null)
            {
                foreach (var item in MyTaskList)
                {
                    if (item.runningTime >= (10 * 1000))
                    {
                        MyTaskList.Remove(item);
                        MyTaskList.Add(new MyTask<T>(_thisAction2, _QueueData, cts.Token, TaskCreationOptions.LongRunning, mre, oo => { }));
                        MyTaskList[4].TaskRun();
                    }
                }
            }


            if (_thisAction2 != aa)
            {
                foreach (var item in MyTaskList)
                {
                    item.TaskChange(aa);
                }
            }

            if (_QueueData.Count > 0)
            {
                AllTaskRun();
            }
        }

        static public void AddQueueUserWork(Func<object, Task> aa, T data)
        {
            _t_maxcount = 5;
            if (_QueueData == null) _QueueData = new Queue<T>();
            if (cts == null) cts = new CancellationTokenSource();
            if (MyTaskList == null) MyTaskList = new List<MyTask<T>>();
            if (mre == null) mre = new ManualResetEvent(false);

            _QueueData.Enqueue(data);
            if (MyTaskList != null && MyTaskList.Count != 5)
            {
                if (_thisAction2 == null) _thisAction2 = aa;
                for (int ii = 0; ii < _t_maxcount; ii++)
                {
                    MyTaskList.Add(new MyTask<T>(_thisAction2, _QueueData, cts.Token, TaskCreationOptions.LongRunning, mre, oo => { }));
                    MyTaskList[ii].TaskRun();
                }
            }

            if (MyTaskList != null)
            {
                foreach (var item in MyTaskList)
                {
                    if (item.runningTime >= (10 * 1000))
                    {
                        MyTaskList.Remove(item);
                        MyTaskList.Add(new MyTask<T>(_thisAction2, _QueueData, cts.Token, TaskCreationOptions.LongRunning, mre, oo => { }));
                        MyTaskList[4].TaskRun();
                    }
                }
            }


            if (_thisAction2 != aa)
            {
                foreach (var item in MyTaskList)
                {
                    item.TaskChange(aa);
                }
            }

            if (_QueueData.Count > 0)
            {
                AllTaskRun();
            }
        }

        static private void AllTaskRun()
        {
            if (MyTaskList == null) return;
            if (mre == null) return;

            foreach (var item in MyTaskList)
            {
                item.isWork = true;
            }
            mre.Set();
            mre.Reset();

        }

        static public void AllTaskStop()
        {
            if (cts == null) return;
            //cts.Cancel();
            lock(_QueueData)
            {
                _QueueData.Clear();
            }
            foreach (var item in MyTaskList)
            {
                item.isWork = false;
 //               item.thisList = null;
            }
            GC.Collect();
        }
        #endregion

    }

    class MyTask<T> : Task
    {
        #region private
        private Action _work;
        private Action<object> _work2;
        private Func<object,Task> _work3;
        private Thread _thisThread;
        private bool _isWork;
        private bool _isAlive;
        private bool _isWait;
        private bool _isAbort;
        private const int timeout_ms = 2000;
        private CancellationToken _ct;
        private ManualResetEvent _mre;
        private System.Diagnostics.Stopwatch _sw = new System.Diagnostics.Stopwatch();
        #endregion

        #region Create
        public MyTask(Action work) : base(work)
        {
            _work = work;
        }
        public MyTask(Action<object> work, object state) : base(work, state)
        {
            _work2 = work;
        }
        public MyTask(Action<object> work, object state, TaskCreationOptions opt) : base(work, state, opt)
        {
            _work2 = work;
        }
        public MyTask(Action<object> work, object state, CancellationToken ct, TaskCreationOptions opt) : base(work, state, ct, opt)
        {
            _work2 = work;
            _ct = ct;
        }
        public MyTask(Action<object> work, object state, CancellationToken ct, TaskCreationOptions opt, ManualResetEvent mre) : base(work, state, ct, opt)
        {
            _work2 = work;
            _ct = ct;
            _mre = mre;
        }
        public MyTask(Func<object,Task> work, object state, CancellationToken ct, TaskCreationOptions opt, ManualResetEvent mre, Action<object> temp) : base(temp, state, ct, opt)
        {
            _work3 = work;
            _ct = ct;
            _mre = mre;
        }
        #endregion

        #region Property 
        public bool isWork
        {
            get { return _isWork; }
            set
            {
                if (_isWork != value)
                    _isWork = value;
            }
        }
        public bool isWait
        {
            get { return _isWait; }
            set
            {
                if (_isWait != value)
                    _isWait = value;
            }
        }
        public bool isAlive
        {
            get { return _isAlive; }
            set
            {
                if (_isAlive != value)
                    _isAlive = value;
            }
        }
        public bool isAbort
        {
            get { return _isAbort; }
            set
            {
                if (_isAbort != value)
                    _isAbort = value;
            }
        }
        public double runningTime
        {
            get { return _sw.Elapsed.TotalMilliseconds; }
        }

        #endregion

        #region Method
        public void TaskRun()
        {
            if (isAlive == false)
            {
                isWait = false;
                isAlive = true;
                Run(async () =>
                {
 //                   _thisThread = Thread.CurrentThread;
                    while (isAlive)
                    {
                        ////等待任務
                        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                        //sw.Start();
                        //while (isWait)
                        //{
                        //    _mre.WaitOne();
                        //    break;
                        //    //                            Thread.Sleep(1);
                        //    //                            SpinWait.SpinUntil(() => false, 1);
                        //    //if (isAlive == false || sw.Elapsed.TotalMilliseconds >= timeout_ms)
                        //    //{
                        //    //    sw.Stop();

                        //    //    //isAlive = false;
                        //    //    Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " End!");
                        //    //    _mre.WaitOne();
                        //    //    break;
                        //    //}
                        //    //if (_ct != null)
                        //    //{
                        //    //    if (_ct.IsCancellationRequested == true)
                        //    //    {
                        //    //        Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " End!");
                        //    //        _mre.WaitOne();
                        //    //        break;
                        //    //    }
                        //    //}

                        //}
                        if (isWait == true) _mre.WaitOne();
                        if (isWork == false) _mre.WaitOne();

                        //執行任務
                        T ss = default(T);
                        lock (AsyncState)
                        {
                            Queue<T> list = (Queue<T>)AsyncState;
                            if (list.Count != 0)
                            {
                                ss = list.Dequeue();
                            }
                        }
                        if (ss != null)
                        {
                            _sw.Restart();
                            isWait = false;
                            if (_work != null) _work();
                            else if (_work2 != null) _work2(ss);
                            else if (_work3 != null) await _work3(ss);
                            _sw.Restart();
                        }
                        else
                        {
                            _sw.Stop();
                            isWait = true;
                        }
                    }
                    isAlive = false;
                }, _ct);
            }

        }

        public void TaskChange(Action<object> work)
        {
            _work2 = work;
        }

        public void TaskChange(Func<object,Task> work)
        {
            _work3 = work;
//            _thisList = list;
        }

        public void TaskAbort()
        {
//            _thisThread.Abort();
            isAbort = true;
        }

        #endregion
    }
}
