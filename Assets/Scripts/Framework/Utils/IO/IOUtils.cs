using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;

namespace FrameWork
{
    public class IOUtils
    {
        public delegate void BytesAsyncCalback(byte[] data);

        public static readonly string localSaveRootPath = Application.persistentDataPath + "/root/";

        public static void DeleteFileIfExists(string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }

        public static void CreateDirectoryIfNotExist(string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        public static string ComputeMD5(byte[] data)
        {
            using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
            {
                byte[] md5 = md5Hash.ComputeHash(data);
                System.Text.StringBuilder md5Builder = new System.Text.StringBuilder();
                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < md5.Length; i++)
                {
                    md5Builder.Append(md5[i].ToString("x2"));
                }

                return md5Builder.ToString();
            }
        }

        public static string ComputeMD5(string path)
        {
            return ComputeMD5(File.ReadAllBytes(path));
        }

        #region Encrypt bytes & Decrypt bytes
        private const string abFuzzyKey = "TKABFASHIONSHOWXYYX";
        private const string abFuzzyIV = "AXIAKNDLAENVMDAKEQCMIOENC";
        public static byte[] EncryptBytes(byte[] bytes, string fuzzyKey = abFuzzyKey, string fuzzyIV = abFuzzyIV)
        {
            using (MemoryStream mstream = new MemoryStream())
            {
                using (AesManaged myAes = new AesManaged())
                {
                    byte[] salt = System.Text.Encoding.ASCII.GetBytes(fuzzyKey);
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(fuzzyIV, salt);
                    myAes.Key = key.GetBytes(myAes.KeySize / 8);
                    myAes.IV = key.GetBytes(myAes.BlockSize / 8);
                    using (CryptoStream cs = new CryptoStream(mstream, myAes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                    }
                }
                return mstream.ToArray();
            }
        }

        public static byte[] DecryptBytes(byte[] bytes, string fuzzyKey = abFuzzyKey, string fuzzyIV = abFuzzyIV)
        {
            using (MemoryStream mstream = new MemoryStream())
            {
                using (AesManaged myAes = new AesManaged())
                {
                    byte[] salt = System.Text.Encoding.ASCII.GetBytes(fuzzyKey);
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(fuzzyIV, salt);
                    myAes.Key = key.GetBytes(myAes.KeySize / 8);
                    myAes.IV = key.GetBytes(myAes.BlockSize / 8);
                    using (CryptoStream cs = new CryptoStream(mstream, myAes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                    }
                }
                return mstream.ToArray();
            }
        }
        #endregion

        public static byte[] LoadBytesFromFile(string path)
        {
            byte[] data = null;
#if UNITY_ANDROID || UNITY_IOS
            if (path.Contains("://"))
            {
                using (WWW www = new WWW(path))
                {
                    while (!www.isDone)
                    {
                        if (!string.IsNullOrEmpty(www.error))
                        {
                            Debug.LogError(www.error);
                            return null;
                        }
                        data = www.bytes;
                    }
                }
            }
            else
#endif
            if (File.Exists(path))
            {
                data = File.ReadAllBytes(path);
            }

            return data;
        }

        public static void LoadBytesFromFileAsync(string path, BytesAsyncCalback callback)
        {
            BytesState bstate = new BytesState();
            ThreadManager.Instance.DoThread(new ThreadJob(
                (object state) => 
                {
                    (state as BytesState).data = LoadBytesFromFile(path);
                }, bstate))
                .SetCallback((object state) => 
                {
                    BytesState cbState = state as BytesState;
                    if (cbState.data != null) callback(cbState.data);
                });
        }

        public static T DeserializeObjectFromFile<T>(string path) where T : class
        {
            T ret = DeserializeObjectFromBytes<T>(LoadBytesFromFile(path));
            return ret;
        }

        public static T DeserializeObjectFromBytes<T>(byte[] bytes) where T : class
        {
            if (bytes == null) return null;

            T ret = default(T);
            var memStream = new MemoryStream();
            var binFormatter = new BinaryFormatter();

            // Where 'objectBytes' is your byte array.
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Position = 0;

            binFormatter.Binder = AssemblyBinder.binder;
            ret = binFormatter.Deserialize(memStream) as T;
            return ret;
        }

        public static byte[] SerializeObjectToBytes<T>(T obj)
        {
            var binFormatter = new BinaryFormatter();
            var memStream = new MemoryStream();
            //binFormatter.Binder = AssemblyBinder.binder;
            binFormatter.Serialize(memStream, obj);

            //This gives you the byte array.
            return memStream.ToArray();
        }

        public static void SerializeObjectToFile<T>(string path, T obj)
        {
            SaveBytesToFile(path, SerializeObjectToBytes(obj));
        }

        public static void SaveBytesToFile(string path, byte[] bytes)
        {
            if (File.Exists(path)) File.Delete(path);
            CreateDirectoryIfNotExist(Path.GetDirectoryName(path));

            //This gives you the byte array.
            File.WriteAllBytes(path, bytes);
        }
    }

    public sealed class AssemblyBinder : SerializationBinder
    {
        public static AssemblyBinder binder = new AssemblyBinder();
        static string assembleFullName = Assembly.GetExecutingAssembly().FullName;
        public override Type BindToType(string assemblyName, string typeName)
        {
            //Debug.LogFormat("AssemblyName:[{0}], typeName:[{1}]", assemblyName, typeName);
            return Type.GetType(string.Format("{0}, {1}", typeName, assembleFullName));
        }
    }
}
