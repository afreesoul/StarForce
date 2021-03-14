//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using StarForce;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityGameFramework.Runtime;
using XLua;


public class CSExport
{
    public static void CacheString(string str)
    {
        XLuaStringCache.CacheString(str);
    }

    public static void ClearCacheString()
    {
        XLuaStringCache.ClearCacheString();
    }
}