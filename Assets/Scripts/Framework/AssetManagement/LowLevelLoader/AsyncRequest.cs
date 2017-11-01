using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    /// <summary>
    /// Async request base class, use coroutine with thread or other async operation.
    /// </summary>
    public abstract class IAsyncRequestBase : CustomYieldInstruction
    {
        // id for this iterator
        public int id = -1;
        public VoidCallback beginRequest;
        public VoidCallback endRequest;

        public AsyncRequestListener listener;

        public override bool keepWaiting { get { return false; } }        
    }

    /// <summary>
    /// A generic async operation listener and wait.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class IAsyncRequest<T> : IAsyncRequestBase where T : AsyncOperation
    {
        public T asyncOp;

        public override bool keepWaiting
        {
            get
            {
                if (asyncOp != null)
                {
                    if (listener != null) listener(this);
                    return !asyncOp.isDone;
                }
                return false;
            }
        }
    }

    /// <summary>
    /// Asset bundle async operation request
    /// </summary>
    public class AssetBundleAsyncRequest : IAsyncRequest<AssetBundleCreateRequest>
    {
    }

    /// <summary>
    /// Asset async operation request
    /// </summary>
    public class AssetAsyncRequest : IAsyncRequest<AssetBundleRequest>
    {
    }

    /// <summary>
    /// Scene async operation request
    /// </summary>
    public class SceneAsyncRequest : IAsyncRequest<AsyncOperation>
    {
        public override bool keepWaiting
        {
            get
            {
                if (asyncOp != null)
                {
                    if (listener != null) listener(this);
                    return !asyncOp.isDone;
                }
                return false;
            }
        }
    }

    public class FakeAsyncOp : AsyncOperation
    {
        //
        // Summary:
        //     ///
        //     Has the operation finished? (Read Only)
        //     ///
        public new bool isDone { get { return true; } }
    }

    public class FakeAssetAsyncRequest : IAsyncRequest<FakeAsyncOp>
    {

    }
    
    public class WaitForAssetBundleComplete : IAsyncRequestBase
    {
        AssetBundleReference bundleReference;
        public WaitForAssetBundleComplete(AssetBundleReference bundleRef)
        {
            bundleReference = bundleRef;
        }

        public override bool keepWaiting { get { return bundleReference.bundle == null; } }
    }

    #region WWW 
    [Obsolete("This class uses unity's WWW class, which is slow and problematic.")]
    public class WWWRequest : IAsyncRequestBase
    {
        public WWW www;

        public override bool keepWaiting
        {
            get
            {
                if (!Caching.ready) return true;
                if (www != null)
                {
                    if (listener != null) listener(this);
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogFormat("WWWRequest error : ", www.error);
                        return false;     // quit if error detected.
                    }
                    return !www.isDone;
                }

                return false;
            }
        }
    }

    /// <summary>
    /// Advanced WWW request using WebClient.
    /// </summary>
    public class AWWWRequest : IAsyncRequestBase
    {
        public AWWW aw;

        public override bool keepWaiting
        {
            get
            {
                if (aw != null)
                {
                    if (listener != null) listener(this);
                    if (!string.IsNullOrEmpty(aw.error))
                    {
                        Debug.LogFormat("<color=yellow>[AssetsManagement]:</color>AWWWRequest error : {0}", aw.error);
                        return false;     // quit if error detected.
                    }
                    return !aw.isDone;
                }

                return false;
            }
        }
    }

    public class AdvancedABRequest : IAsyncRequestBase
    {
        public AWWW www;
        public AssetBundleCreateRequest cq;

        public override bool keepWaiting
        {
            get
            {
                if (www != null)
                {
                    if (listener != null) listener(this);
                    if (www.error.Length > 0)
                    {
                        LogUtil.LogColor(LogUtil.Color.red, "AdvancedABRequest error : {0}, {1}", www.error, www.url);
                        return false;     // quit if error detected.
                    }

                    if (www.isDone)
                    {
                        cq = AssetBundle.LoadFromMemoryAsync(www.bytes);
                        if (cq == null)
                        {
                            www.error = "LoadError";
                            LogUtil.LogColor(LogUtil.Color.red, "AdvancedABRequest error : {0}, {1}", www.error, www.url);
                            return false;
                        }
                        return cq.isDone;
                    }

                    return !www.isDone;
                }

                return false;
            }
        }
    }
    #endregion
}