/*
 * Copyright (c) 2015. KinderGuardian Inc.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using itavio.Utilities;

namespace itavio
{
    public class itavioConfig : ScriptableObject
    {

        [SerializeField]
        private List<itavioConfigPlatform> m_platforms = new List<itavioConfigPlatform>();

        public itavioConfigPlatform GetPlatform(string name)
        {
            for (int i = 0; i < m_platforms.Count; ++i)
            {
                if (name.ToLower() == m_platforms[i].Name.ToLower())
                {
                    return m_platforms[i];
                }
            }

            itavioDbg.LogError("itavioConfig could not be found for \"" + name + "\"");
            return null;
        }
    }

    [System.Serializable]
    public class itavioConfigPlatform
    {
        [SerializeField]
        private string m_name;

        [SerializeField]
        private itavioEnvironment m_environment;

        [SerializeField]
        [Multiline]
        private string m_secretKeyID;

        [SerializeField]
        [Multiline]
        private string m_secretKey;

        public string Name { get { return m_name; } }
        public itavioEnvironment Environment { get { return m_environment; } }
        public string SecretKeyID { get { return m_secretKeyID; } }
        public string SecretKey { get { return m_secretKey; } }
    }

    public enum itavioEnvironment
    {
        Production = 0,
        Staging = 1,
        Integration = 2
    }
}