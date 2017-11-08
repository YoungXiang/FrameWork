using System.Threading;
using System.Collections.Generic;

namespace FrameWork
{
    public class WorkerThread
    {
        public volatile bool isStarted;
        public volatile bool isRuning;

        private Queue<ThreadJob> executingJob;
        private Queue<ThreadJob> finishedJob;
        private Thread sysThread;
        private Mutex mutexExecuting;
        private Mutex mutexFinished;

        // Thread State Event
        ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        ManualResetEvent _pauseEvent = new ManualResetEvent(true);

        public WorkerThread(int threadStackSize)
        {
            isStarted = false;
            isRuning = false;
            executingJob = new Queue<ThreadJob>();
            finishedJob = new Queue<ThreadJob>();
            sysThread = new Thread(Run, threadStackSize);
            mutexExecuting = new Mutex(false);
            mutexFinished = new Mutex(false);
        }

        public int ExecutingJobCount()
        {
            return executingJob.Count;
        }

        public void QueueJob(ThreadJob job)
        {
            mutexExecuting.WaitOne();
            executingJob.Enqueue(job);
            if (!isStarted)
            {
                Start();
            }
            else if (!isRuning)
            {
                Resume();
            }
            mutexExecuting.ReleaseMutex();
        }

        public void Update()
        {
            while (finishedJob.Count > 0)
            {
                mutexFinished.WaitOne();
                ThreadJob job = finishedJob.Dequeue();
                if (job.Callback != null)
                {
                    job.Callback(job.state);
                }
                mutexFinished.ReleaseMutex();
            }
        }

        private void FinishJob(ThreadJob job)
        {
            mutexFinished.WaitOne();
            finishedJob.Enqueue(job);
            mutexFinished.ReleaseMutex();
        }

        public void Run()
        {
            while (true)
            {
                _pauseEvent.WaitOne(Timeout.Infinite);

                if (_shutdownEvent.WaitOne(0))
                    break;

                isRuning = true;

                // Do the work..
                while (executingJob.Count > 0)
                {
                    mutexExecuting.WaitOne();
                    ThreadJob job = executingJob.Dequeue();
                    job.Run();
                    mutexExecuting.ReleaseMutex();
                    FinishJob(job);
                }

                Pause();
            }
        }

        public void Start()
        {
            isStarted = true;
            // allocate thread
            sysThread.Start();
        }

        public void Pause()
        {
            isRuning = false;
            _pauseEvent.Reset();
        }

        public void Resume()
        {
            _pauseEvent.Set();
        }

        public void Stop()
        {
            // Signal the shutdown event
            _shutdownEvent.Set();
            // Make sure to resume any paused threads
            _pauseEvent.Set();
            // Wait for the thread to exit
            sysThread.Join();
        }

        public void Abort()
        {
            sysThread.Abort();
        }
    }
}