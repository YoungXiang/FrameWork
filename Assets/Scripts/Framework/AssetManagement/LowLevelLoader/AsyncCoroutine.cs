using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameWork
{
    public delegate void CoroutinerJobFinished(CoroutinerJob job);
    public class CoroutinerJob : IEnumerable
    {
        public Queue<IAsyncRequestBase> queueJobs;
        public bool isRunning;

        public CoroutinerJob()
        {
            isRunning = false;
            queueJobs = new Queue<IAsyncRequestBase>();
        }
        
        public IEnumerator Run(CoroutinerJobFinished finishedCallback)
        {
            isRunning = true;
            while (queueJobs.Count > 0)
            {
                IAsyncRequestBase it = queueJobs.Dequeue();
                if (it.beginRequest != null)
                {
                    it.beginRequest(); it.beginRequest = null;
                }
                yield return it;
                if (it.endRequest != null)
                {
                    it.endRequest(); it.endRequest = null;
                }
            }
            isRunning = false;

            finishedCallback(this);
        }

        public void Enqueue(IAsyncRequestBase request)
        {
            queueJobs.Enqueue(request);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (IAsyncRequestBase job in queueJobs)
            {
                yield return job;
            }
        }
    }

    public class AsyncCoroutine : MonoBehaviour
    {   
        protected List<CoroutinerJob> coQueue = new List<CoroutinerJob>();
        protected Dictionary<int, AsyncRequestListener> listeners = new Dictionary<int, AsyncRequestListener>();

#if UNITY_EDITOR
        public int queueLength = 0;
#endif

        public void NewLoadingQueue()
        {
            coQueue.Add(new CoroutinerJob());
        }
        
        protected void FinishedCallback(CoroutinerJob job)
        {
            coQueue.Remove(job);
#if UNITY_EDITOR
            queueLength = coQueue.Count;
#endif
        }

        protected CoroutinerJob GetLast()
        {
            if (coQueue.Count <= 0)
            {
                NewLoadingQueue();
            }
            
#if UNITY_EDITOR
            queueLength = coQueue.Count;
#endif

            CoroutinerJob last = coQueue[coQueue.Count - 1];
            return last;
        }

        public void WaitForLoad(IAsyncRequestBase routine)
        {
            CoroutinerJob last = GetLast();
            last.Enqueue(routine);

            TryFlushListener(routine.id);

            if (!last.isRunning)
            {
                StartCoroutine(last.Run(FinishedCallback));
            }
        }

        public void Load(IAsyncRequestBase routine)
        {
            StartCoroutine(RunInstance(routine));
        }

        public void AddListener(int id, AsyncRequestListener listener)
        {
            if (!listeners.ContainsKey(id))
            {
                listeners.Add(id, listener);
            }
            else
            {
                listeners[id] += listener;
            }

            TryFlushListener(id);
        }

        void TryFlushListener(int id)
        {
            if (!listeners.ContainsKey(id)) return;

            IAsyncRequestBase request = GetRunningRequest(id);
            if (request != null)
            {
                request.listener += listeners[id];
                Debug.Log("Listener Flushed : " + id);
                listeners.Remove(id);
            }
        }
        
        public IEnumerator RunInstance(IAsyncRequestBase request)
        {
            if (request.beginRequest != null) request.beginRequest();
            yield return request;
            if (request.endRequest != null) request.endRequest();
        }

        public IAsyncRequestBase GetRunningRequest(int id)
        {
            for (int i = 0; i < coQueue.Count; i++)
            {
                foreach (IAsyncRequestBase request in coQueue[i])
                {
                    if (request.id == id)
                    {
                        return request;
                    }
                }
            }

            return null;
        }

        public bool IsDone(int id)
        {
            for (int i = 0; i < coQueue.Count; i++)
            {
                foreach (IAsyncRequestBase request in coQueue[i])
                {
                    if (request.id == id)
                    {
                        return !request.keepWaiting;
                    }
                }
            }

            return false;
        }

        public static AsyncCoroutine Create()
        {
            GameObject go = new GameObject("AssetCoroutiner");
            DontDestroyOnLoad(go);
            return go.AddComponent<AsyncCoroutine>();
        }        
    }
}
