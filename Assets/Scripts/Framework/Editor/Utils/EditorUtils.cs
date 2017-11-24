using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EditorUtils
{
    public static string KeyFromProduct(string key)
    {
        return string.Format("[{0}]_{1}", Application.productName, key);
    }
}
