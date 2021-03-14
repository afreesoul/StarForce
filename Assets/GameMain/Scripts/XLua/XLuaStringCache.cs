//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityGameFramework.Runtime;
using XLua;

namespace StarForce
{
    public class XLuaStringCache
    {
        const int LimitCacheStringLen = 40;

        public static Dictionary<int, string> sStringPool = new Dictionary<int, string>();
        public static GameFrameworkMultiDictionary<int, string> sStringListPool = new GameFrameworkMultiDictionary<int, string>();

        public static void CacheString(string str)
        {
            int len = str.Length;
            if (len > LimitCacheStringLen)
            {
                Log.Warning("cache string len is large than 40:", str, len.ToString());
                return;
            }
            int hashCode = 0;
            int i = 0;
            for (; i < len; ++i)
            {
                hashCode = unchecked(hashCode * 31 + str[i]);
            }
            string ret;
            if (sStringPool.TryGetValue(hashCode, out ret))
            {
                if (ret.Equals(str, StringComparison.Ordinal))
                {
                    Log.Warning("cache string repeate:", str);
                    return;
                }
                sStringListPool.Add(hashCode, str);
            }
            else
            {
                sStringPool.Add(hashCode, str);
            }
        }

        public static void ClearCacheString()
        {
            sStringPool.Clear();
            sStringListPool.Clear();
        }

        unsafe public static string GetStringNoGC(IntPtr str, int len)
        {
            string ret;
            if (len > LimitCacheStringLen)
            {
                ret = Marshal.PtrToStringAnsi(str, len);
                return ret;
            }
            Span<byte> buff = new Span<byte>(str.ToPointer(), len);
            int hashCode = 0;
            int i = 0;
            for (; i < len; ++i)
            {
                hashCode = unchecked(hashCode * 31 + buff[i]);
            }
            if (!sStringPool.TryGetValue(hashCode, out ret))
            {
                ret = Marshal.PtrToStringAnsi(str, len);
                return ret;
            }
            for (i = 0; i < len; ++i)
            {
                if (ret[i] != buff[i])
                {
                    break;
                }
            }
            if (i == len)
            {
                return ret;
            }
            GameFrameworkLinkedListRange<string> range;
            if (sStringListPool.TryGetValue(hashCode, out range))
            {
                LinkedListNode<string> current = range.First;
                while (current != null && current != range.Terminal)
                {
                    string iterStr = current.Value;
                    for (i = 0; i < len; ++i)
                    {
                        if (iterStr[i] != buff[i])
                        {
                            break;
                        }
                    }
                    if (i == len)
                    {
                        ret = iterStr;
                        return ret;
                    }
                }
            }
            ret = Marshal.PtrToStringAnsi(str, len);
            return ret;
        }
    }
}
