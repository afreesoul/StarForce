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
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;
using XLua;

namespace StarForce
{
    public class XLuaComponent : GameFrameworkComponent
    {
        const float GCInterval = 1;

        LuaEnv m_LuaEnv;
        float m_LastGCTime;
        string m_LuaRootPath;
        Action<float, float> m_UpdateFunc;
        Action<float, float> m_LateUpdateFunc;

        private void Start()
        {
            m_LuaEnv = new LuaEnv();
            m_LuaEnv.AddLoader(OnLoad);
            if (GameEntry.Base.EditorResourceMode)
            {
                m_LuaRootPath = Path.Combine(Application.dataPath, "GameMain/Lua/");
            }
            else
            {
                m_LuaRootPath = Path.Combine(Application.streamingAssetsPath, "GameMain/Lua");
            }
        }

        private void OnDestroy()
        {
            if (m_LuaEnv != null)
            {
                m_LuaEnv.Dispose();
                m_LuaEnv = null;
            }
        }

        private void Update()
        {
            if (m_LuaEnv == null)
            {
                return;
            }
            if (Time.time - m_LastGCTime > GCInterval)
            {
                m_LuaEnv.Tick();
                m_LastGCTime = Time.time;
            }
        }

        private byte[] OnLoad(ref string luaFileName)
        {
#if UNITY_EDITOR
            if (luaFileName == "emmy_core")
            {
                return null;
            }
#endif
            StringBuilder sb = ViStringBuilder.NewCache;
            sb.Append(m_LuaRootPath);
            sb.Append(luaFileName);
            sb.Append(".bytes");
            string luaFilePath = sb.ToString();
            return File.ReadAllBytes(luaFilePath);
        }


        public void FullGc()
        {
            m_LuaEnv?.FullGc();
        }

        public void StartMain()
        {
            DoString("require('Main')", "Main");
            Action startFunc = m_LuaEnv.Global.GetInPath<Action>("Main.Update");
            startFunc?.Invoke();
            m_UpdateFunc = m_LuaEnv.Global.GetInPath<Action<float, float>>("Main.Update");
            m_LateUpdateFunc = m_LuaEnv.Global.GetInPath<Action<float, float>>("Main.LateUpdate");
        }

        public void UpdateMain()
        {
            m_UpdateFunc?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        public void LateUpdateMain()
        {
            m_LateUpdateFunc?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }


        public void DoString(string scriptContent, string chunkName, LuaTable env = null)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(scriptContent);
            DoString(bytes, chunkName, env);
        }
        public void DoString(byte[] bytes, string chunkName, LuaTable env)
        {
            if (m_LuaEnv != null)
            {
                try
                {
                    m_LuaEnv.DoString(bytes, chunkName, env);
                }
                catch (System.Exception ex)
                {
                    Log.Error(Utility.Text.Format("xlua exception: {0}\n {1}", ex.Message, ex.StackTrace));
                }
            }

        }

    }
}
