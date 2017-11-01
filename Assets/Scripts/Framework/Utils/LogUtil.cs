using UnityEngine;

namespace FrameWork
{
    public class LogUtil
    {
        public enum Color
        {
            black,
            blue,
            gray,
            green,
            cyan,
            red,
            white,
            yellow
        }

        public static void ERROR(string msg)
        {
            Debug.LogError(string.Format("[Error] - {0}", msg));
        }

        public static void TODO(string msg)
        {
            Debug.Log(string.Format("[Todo] - {0}", msg));
        }

        public static void LogColor(Color color, object obj)
        {
            Debug.LogFormat(string.Format("<color={0}>{1}</color>", color.ToString(), obj));
        }

        public static void LogColor(Color color, string format, params object[] args)
        {
            Debug.LogFormat(string.Format("<color={0}>{1}</color>", color.ToString(), format), args);
        }
    }
}