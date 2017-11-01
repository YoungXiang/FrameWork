using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public class ACaching
    {
        internal static readonly string cachePath = Path.Combine(Application.persistentDataPath, "acache");
        internal static readonly long cacheMaxSize = 1024 * 1024 * 1024;    // 1 GB
        #region Privates
        static ACaching()
        {
            IOUtils.CreateDirectoryIfNotExist(cachePath);
        }

        static string UrlToShortPath(string url)
        {
            //string fileExt = Path.GetExtension(url);
            return string.Format("{0}.cached", url.GetHashCode().ToString());
        }

        static long cachedSizeTotal = 0;
        static void EvaluateCacheSize()
        {
            ThreadManager.Instance.DoThread(new ThreadJob((object state_)=>
            {
                cachedSizeTotal = 0;
                DirectoryInfo info = new DirectoryInfo(cachePath);
                Queue<FileInfo> cachedFiles = new Queue<FileInfo>();
                FileInfo[] files = info.GetFiles("*.cached", SearchOption.TopDirectoryOnly).OrderBy(p => p.CreationTime).ToArray();
                foreach (FileInfo file in files)
                {
                    cachedSizeTotal += file.Length;
                    cachedFiles.Enqueue(file);
                }

                Debug.LogFormat("cachedSizeTotal({0}) : {1}mb", cachedSizeTotal, cachedSizeTotal / 1024 / 1024);

                while (cachedSizeTotal > cacheMaxSize && cachedFiles.Count > 0)
                {
                    FileInfo fileInfo = cachedFiles.Dequeue();
                    cachedSizeTotal -= fileInfo.Length;
                    fileInfo.Delete();
                }
            }, null));
        }
        #endregion

        #region Publics
        public static string GetCachedPath(string url)
        {
            return Path.Combine(cachePath, UrlToShortPath(url));
        }

        public static bool IsVersionCached(string url, int version)
        {
            if (!File.Exists(GetCachedPath(url)))
            {
                PlayerPrefs.DeleteKey(url);
                return false;
            }

            int cachedVersion = PlayerPrefs.GetInt(url, -1);
            return cachedVersion == version;
        }
        
        public static void SaveCaching(string url, int version)
        {
            PlayerPrefs.SetInt(url, version);
        }

        public static void SaveCachingData(byte[] bytes, string url)
        {
            File.WriteAllBytes(GetCachedPath(url), bytes);
            //PlayerPrefs.SetInt(url, version);
            EvaluateCacheSize();
        }

        /// <summary>
        /// Clear cache by url;
        /// </summary>
        /// <param name="url"></param>
        public static void CleanCaching(string url)
        {
            string cachedPath = GetCachedPath(url);
            if (File.Exists(cachedPath))
            {
                File.Delete(cachedPath);
            }

            PlayerPrefs.DeleteKey(url);
        }

        /// <summary>
        /// Clear all cached
        /// </summary>
        public static void CleanCaching()
        {
            DirectoryInfo info = new DirectoryInfo(cachePath);
            Queue<FileInfo> cachedFiles = new Queue<FileInfo>();
            FileInfo[] files = info.GetFiles("*.cached", SearchOption.TopDirectoryOnly).OrderBy(p => p.CreationTime).ToArray();
            foreach (FileInfo file in files)
            {
                file.Delete();
            }
        }
        #endregion
    }
}

