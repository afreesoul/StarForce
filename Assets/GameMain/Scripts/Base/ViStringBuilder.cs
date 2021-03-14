//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Text;
using UnityEngine;

public class ViStringBuilder
{
    static StringBuilder m_Cache = new StringBuilder(512);

    public static StringBuilder NewCache { get { Clear(); return m_Cache; } }

    public static void Clear()
    {
        m_Cache.Length = 0;
    }
    public static void Write(string str)
    {
        m_Cache.Append("str");
    }

}