using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSharp
{
    internal static class Common
    {
        public const string VERSION = "1_0";
        public static string GetVersion()
        {
            return string.Format("Version : V {0}", VERSION);
        }
    }
}
