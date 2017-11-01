using System;
using System.IO;
using System.Net;

namespace FrameWork
{
    public class AWebClient : WebClient
    {
        public int timeOut { get; set; }
        public AWebClient()
        {
            timeOut = 1000 * 10;
        }

        public AWebClient(int timeOut_) { timeOut = timeOut_; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            request.Timeout = timeOut;
            return request;
        }
    }

    public class AWWW
    {
        public volatile string error = string.Empty;
        public volatile float progress = 0.0f;
        public volatile byte[] bytes = null;
        public string url;

        public bool isDone
        {
            get { return bytes != null && progress >= 1.0f; }
        }
        
        public static AWWW LoadFromCacheOrDownload(string url, int version)
        {
            AWWW aw = new AWWW();
            aw.url = url;
            if (ACaching.IsVersionCached(url, version))
            {
                string cachedPath = ACaching.GetCachedPath(url);
                UnityEngine.Debug.LogFormat("<color=yellow>[AssetsManagement]:</color>Loading From Cache: {0}-{1}-{2}", url, cachedPath, version);
                try
                {
                    FileStream fs = File.OpenRead(cachedPath);
                    {
                        int fileLength = (int)fs.Length;
                        aw.bytes = new byte[fileLength];
                        fs.BeginRead(aw.bytes, 0, fileLength, (System.IAsyncResult state) =>
                        {
                            FileStream endFs = (FileStream)state.AsyncState;
                            endFs.EndRead(state);
                            endFs.Close();

                            // now begin encrypt
                            aw.bytes = IOUtils.DecryptBytes(aw.bytes);
                            aw.progress = 1.0f;
                        }, fs);
                    }
                }
                catch(System.Exception ex)
                {
                    //LogUtil.LogColor(LogUtil.ColorLog.red, ex.Message);
                    aw.error = ex.Message;
                }
            }
            else
            {
                UnityEngine.Debug.LogFormat("<color=yellow>[AssetsManagement]:</color>Loading From WWW: {0}-{1}", url, version);
                try
                {
                    using (AWebClient webClient = new AWebClient())
                    {
                        webClient.DownloadDataAsync(new System.Uri(url));
                        webClient.DownloadDataCompleted += (object sender, DownloadDataCompletedEventArgs e) => 
                        {
                            aw.bytes = IOUtils.DecryptBytes(e.Result);
                            ACaching.SaveCachingData(e.Result, url);
                            //aw.bytes = e.Result;
                            aw.progress = 1.0f;
                        };
                        webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
                        {
                            if (aw.progress <= 1.0f)
                                aw.progress += (float)((double)e.BytesReceived / (double)e.TotalBytesToReceive);
                        };
                    }
                    ACaching.SaveCaching(url, version);
                }
                catch (System.Exception ex)
                {
                    //LogUtil.LogColor(LogUtil.ColorLog.red, ex.Message);
                    aw.error = ex.Message;
                }

            }
            return aw;
        }
    }
}
