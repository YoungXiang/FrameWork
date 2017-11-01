
namespace FrameWork
{
    public class ThreadJob
    {
        public delegate void ThreadDelegate(object state);
        public ThreadDelegate Job;        // Called in SubThread
        public ThreadDelegate Callback;   // Called in MainThread

        public object state;                        // parameter

        public bool isJobDone;

        public ThreadJob(ThreadDelegate job_, object state_)
        {
            Job = job_;
            state = state_;
            isJobDone = false;
        }

        public void SetCallback(ThreadDelegate callback_)
        {
            Callback = callback_;
        }

        public void Run()
        {
            if (Job != null)
            {
                Job(state);
            }

            isJobDone = true;
        }
    }
}