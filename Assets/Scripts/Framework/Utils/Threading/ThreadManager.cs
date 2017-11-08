using UnityEngine;

namespace FrameWork
{
    /// <summary>
    /// 包含创建 Thread 和 Task
    /// </summary>
    public class ThreadManager : Singleton<ThreadManager>
    {
        // 256kb = 256 * 1024
        public const int MaxThreadStackSize = 262144;
        public const int MaxThreadNum = 10;

        public sealed class ThreadInstanceBehaviour : MonoBehaviour
        {
            void Update()
            {
                Instance.Update();
            }

            void OnDestroy()
            {
                Instance.Destroy();
            }
        }

        private ThreadInstanceBehaviour instBehaviour;
        
        private DictList<int, WorkerThread> pools;

        public void Initialize()
        {
            GameObject threadGO = new GameObject("ThreadInstance");
            instBehaviour = threadGO.AddComponent<ThreadInstanceBehaviour>();
            Object.DontDestroyOnLoad(threadGO);

            pools = new DictList<int, WorkerThread>();
        }

        public override void Destroy()
        {
            for (int i = 0; i < pools.Count; i++)
            {
                pools.Lists[i].Abort();
            }

            Object.Destroy(instBehaviour.gameObject);
        }

        public ThreadJob QueueThread(int fixedId, ThreadJob job)
        {
            AllocateThread(fixedId).QueueJob(job);
            return job;
        }

        public ThreadJob DoThread(ThreadJob job)
        {
            AllocateThread(GetFreeThreadID()).QueueJob(job);
            return job;
        }

        private int GetFreeThreadID()
        {
            int freeThread = 0;

            if (pools.Count < MaxThreadNum)
            {
                return pools.Count;
            }

            int maxJobCount = 10000;
            for (int i = 0; i < pools.Count; i++)
            {
                int executingJobCount = pools.Lists[i].ExecutingJobCount();
                if (executingJobCount < maxJobCount)
                {
                    maxJobCount = executingJobCount;
                    freeThread = i;
                }
                pools.Lists[i].Abort();
            }

            return freeThread;
        }

        private WorkerThread AllocateThread(int threadId)
        {
            if (!pools.ContainsKey(threadId))
            {
                WorkerThread wt = new WorkerThread(MaxThreadStackSize);
                pools.Add(threadId, wt);
                return wt;
            }

            return pools.Get(threadId);
        }

        public void Update()
        {
            for (int i = 0; i < pools.Count; i++)
            {
                pools.Lists[i].Update();
            }
        }
    }
}