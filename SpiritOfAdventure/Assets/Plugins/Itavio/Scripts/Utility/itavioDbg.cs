/*
 * Copyright (c) 2015. KinderGuardian Inc.
 */

using UnityEngine;
using System.Collections;

namespace itavio.Utilities
{
    public class itavioDbg
    {

#if UNITY_EDITOR
        public static bool IsEnabled = true;
#else
	public static bool IsEnabled = false;
#endif

        private const string preface = "<color=#00BCD4>(Itavio)</color> ";

        public static void Log(string message)
        {
            if (IsEnabled)
                Debug.Log(preface + message);
        }

        public static void LogWarning(string message)
        {
            if (IsEnabled)
                Debug.LogWarning(preface + message);

        }

        public static void LogError(string message)
        {
            if (IsEnabled)
                Debug.LogError(preface + message);
        }
    }
}