/*
 * Copyright (c) 2015. KinderGuardian Inc.
 */

#if UNITY_ANDROID && !UNITY_EDITOR
#define ITAVIO_ANDROID
#endif

#if UNITY_IOS && !UNITY_EDITOR
#define ITAVIO_IOS
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using itavio.Utilities;
using itavio.MiniJSON;

#if ITAVIO_IOS
using System.Runtime.InteropServices;
#endif

namespace itavio
{
    public class itavioManager : MonoBehaviour
    {
        private const string ITAVIO_CONFIG = "itavioConfig";
        private const string UNITY_CLASS = "ItavioManager";
        private const string INITIALIZE = "initialize";
        private const string GET_BALANCE = "getBalance";
        private const string START_DEBIT = "startDebit";
        private const string CANCEL_DEBIT = "cancelDebit";
        private const string COMPLETE_DEBIT = "completeDebit";
        private const string CHECK_FOR_PARENT = "checkForParent";

        private const int ERR_CODE_INSUFFICIENT_FUNDS = -1;

        public delegate void itavioErrorEventHandler(int code, string message);
        public delegate void itavioBoolEventHandler(bool result);
        public delegate void itavioDoubleEventHandler(double result);

        public static event itavioErrorEventHandler OnError;

        public static event itavioDoubleEventHandler OnGetBalance;
        public static event itavioBoolEventHandler OnStartDebit;
        public static event itavioBoolEventHandler OnCancelDebit;
        public static event itavioBoolEventHandler OnCompleteDebit;
        public static event itavioBoolEventHandler OnCheckForParent;
#if UNITY_IOS
        public static event itavioBoolEventHandler OnLinkWithParentApp;
#endif

        private static object s_startDebitCallback = null;
        private static object[] s_startDebitCallbackArgs = null;

        private static itavioManager s_instance;
        private static itavioManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    GameObject iM = new GameObject(UNITY_CLASS);
                    iM.transform.position = Vector3.zero;
                    DontDestroyOnLoad(iM);

                    s_instance = iM.AddComponent<itavioManager>();
                }

                return s_instance;
            }
        }

        /// <summary>
        /// Initialize the ItavioSDK for a target platform
        /// </summary>
        /// <param name="platformName">Name of the target platform</param>
        public static void initialize(string platformName)
        {
            itavioConfigPlatform itavioPlatform = (Resources.Load(ITAVIO_CONFIG) as itavioConfig).GetPlatform(platformName);
            initialize(itavioPlatform);
        }

        /// <summary>
        /// Initialize the ItavioSDK for a target platform
        /// </summary>
        /// <param name="platform">Target platform</param>
        public static void initialize(itavioConfigPlatform platform)
        {
            itavioDbg.Log("Initializing Platform: " + platform.Name);
#if ITAVIO_ANDROID
            if (Instance.Plugin != null)
            {
                object[] args = new object[] { platform.SecretKeyID, platform.SecretKey, (int)platform.Environment };
                Instance.Plugin.Call<int>(INITIALIZE, args);
            }
#elif ITAVIO_IOS
            if (PluginExists)
            {
                itavioInitialize(platform.SecretKeyID, platform.SecretKey, (int)platform.Environment);
            }
#else   // Unsupported Platform
            if (Instance != null)
            {
                itavioDbg.LogWarning("Initialized Stub");
            }
#endif
        }

        /// <summary>
        /// Request the users balance
        /// </summary>
        public static void getBalance()
        {
            itavioDbg.Log("Get Balance");
#if ITAVIO_ANDROID
            if (Instance.Plugin != null)
            {
                Instance.Plugin.Call<int>(GET_BALANCE);
            }

#elif ITAVIO_IOS
            if (PluginExists)
            {
                itavioGetBalance();
            }
#else   // Unsupported Platform
            itavioDbg.LogWarning("Get Balance Stub");
            Instance.onGetBalanceSuccess(Json.Serialize(new Dictionary<string, double>() { { "balance", 5.0 } }));            
#endif
        }

        /// <summary>
        /// Start a debit transaction
        /// </summary>
        /// <typeparam name="T">Purchase delegate type</typeparam>
        /// <param name="amount">Amount to debit</param>
        /// <param name="currencyCode">ISO 4217 currency codes</param>
        /// <param name="callback">Purchase delegate</param>
        /// <param name="callbackArgs">Purchase delegate arguments</param>
        public static void startDebit<T>(double amount, string currencyCode, T callback, params object[] callbackArgs) where T : class
        {
            if (!typeof(T).IsSubclassOf(typeof(Delegate)))
            {
                throw new InvalidOperationException(typeof(T).Name + " is not a delegate type");
            }

            s_startDebitCallback = callback;
            s_startDebitCallbackArgs = callbackArgs;

            itavioDbg.Log("Start Debit: " + amount + "/" + currencyCode);
#if ITAVIO_ANDROID
            if (Instance.Plugin != null)
            {
                object[] args = new object[] { amount, currencyCode };
                Instance.Plugin.Call<int>(START_DEBIT, args);
            }
#elif ITAVIO_IOS
            if (PluginExists)
            {
                itavioStartDebit(amount, currencyCode);
            }
#else   // Unsupported Platform
            itavioDbg.LogWarning("Start Debit Stub");
            Instance.onStartDebitSuccess(Json.Serialize(new Dictionary<string, object>() { { "enabled", true }, { "hasFunds", true } }));
#endif
        }

        /// <summary>
        /// Finalize a debit transaction
        /// </summary>
        /// <param name="success">True if completed, false if cancelled</param>
        public static void finalizeDebit(bool success)
        {
            s_startDebitCallback = null;
            s_startDebitCallbackArgs = null;
            switch (success)
            {
                case true:
                    itavioDbg.Log("Complete Debit");
#if ITAVIO_ANDROID
                    if (Instance.Plugin != null)
                    {
                        Instance.Plugin.Call<int>(COMPLETE_DEBIT);
                    }
#elif ITAVIO_IOS
                    if (PluginExists)
                    {
                        itavioCompleteDebit();
                    }
#else   // Unsupported Platform
                    itavioDbg.LogWarning("Complete Debit Stub");
                    Instance.onCompleteDebitSuccess("");
#endif
                    break;
                case false:
                    itavioDbg.Log("Cancel Debit");
#if ITAVIO_ANDROID
                    if (Instance.Plugin != null)
                    {
                        Instance.Plugin.Call<int>(CANCEL_DEBIT);
                    }
#elif ITAVIO_IOS
                    if (PluginExists)
                    {
                        itavioCancelDebit();
                    }
#else   // Unsupported Platform
                    itavioDbg.LogWarning("Cancel Debit Stub");
                    Instance.onCancelDebitSuccess("");
#endif
                    break;
            }
        }

        /// <summary>
        /// Check for Itavio parent app
        /// </summary>
        /// <param name="showGetAppDialog">Show the user a dialog prompt to get the parent app if it is not present</param>
        public static void checkForParent(bool showGetAppDialog = false, bool ignoreSuppression = false)
        {
            itavioDbg.Log("Check For Parent - Show Dialog: " + showGetAppDialog);
#if ITAVIO_ANDROID
            if (Instance.Plugin != null)
            {
                object[] args = new object[] { showGetAppDialog };
                Instance.Plugin.Call(CHECK_FOR_PARENT, args);
            }
#elif ITAVIO_IOS
            if (PluginExists)
            {
                itavioCheckForParent(showGetAppDialog, ignoreSuppression);
            }
#else   // Unsupported Platform
            itavioDbg.LogWarning("Check For Parent Stub");
            Instance.onCheckForParentSuccess(Json.Serialize(new Dictionary<string, object>() { { "hasParentApp", true } }));
#endif
        }

#if UNITY_IOS

        public static bool IsLinked {
            get
            {
#if ITAVIO_IOS
                return itavioHasLink();
#else // Unity Editor
                itavioDbg.LogWarning("IsLinked Stub");
                return true;
#endif
            }
        }

        /// <summary>
        /// Link the game to the active account in the parent app (triggers an app switch)
        /// </summary>
        /// <param name="showGetAppDialog">If the parent app is missing, show the user a prompt to get the parent app</param>
        public static void linkWithParentApp(bool showGetAppDialog, bool relink = false)
        {
#if ITAVIO_IOS
            itavioLinkWithParentApp(showGetAppDialog, relink);
#else // Unity Editor
            itavioDbg.LogWarning("Link with Parent Stub");
            Instance.onLinkSuccess("");
#endif
        }

        private void onLinkSuccess(string message)
        {
            if (OnLinkWithParentApp != null)
            {
                OnLinkWithParentApp(true);
            }
        }

        private void onLinkError(string message)
        {
            dispatchErrorMessage(message);
        }
#endif

        private void onGetBalanceSuccess(string message)
        {
            var dict = Json.Deserialize(message) as Dictionary<string, object>;
            double balance = Convert.ToDouble(dict["balance"]);

            itavioDbg.Log("onGetBalanceSuccess - Balance: " + balance);
            if (OnGetBalance != null)
            {
                OnGetBalance(balance);
            }
        }

        private void onGetBalanceError(string message)
        {
            dispatchErrorMessage(message);
        }

        private void onStartDebitSuccess(string message)
        {
            itavioDbg.Log("onStartDebitSuccess");
            var dict = Json.Deserialize(message) as Dictionary<string, object>;

            bool isEnabled = (bool)dict["enabled"];
            
            if (OnStartDebit != null)
            {
                OnStartDebit(isEnabled);
            }

            if (s_startDebitCallback != null && s_startDebitCallbackArgs != null)
            {
                Delegate a = s_startDebitCallback as Delegate;
                a.DynamicInvoke(s_startDebitCallbackArgs);
            }
        }

        private void onStartDebitError(string message)
        {
            dispatchErrorMessage(message);
        }

        private void onCancelDebitSuccess(string message)
        {
            itavioDbg.Log("onCancelDebitSuccess");
            if (OnCancelDebit != null)
            {
                OnCancelDebit(true);
            }
        }

        private void onCancelDebitError(string message)
        {
            dispatchErrorMessage(message);
        }

        private void onCompleteDebitSuccess(string message)
        {
            itavioDbg.Log("onCompleteDebitSuccess");
            if (OnCompleteDebit != null)
            {
                OnCompleteDebit(true);
            }
        }

        private void onCompleteDebitError(string message)
        {
            dispatchErrorMessage(message);
        }

        private void onCheckForParentSuccess(string message)
        {
            var dict = Json.Deserialize(message) as Dictionary<string, object>;
            bool hasParent = Convert.ToBoolean(dict["hasParentApp"]);

            itavioDbg.Log("onCheckForParentSuccess - Has Parent: " + hasParent);
            if (OnCheckForParent != null)
            {
                OnCheckForParent(hasParent);
            }
        }

        private void onCheckForParentError(string message)
        {
            dispatchErrorMessage(message);
        }

        private void dispatchErrorMessage(string message)
        {
            int errCode = -1;
            string errMessage = "";

            if (!string.IsNullOrEmpty(message))
            {
                Dictionary<string, object> dict = Json.Deserialize(message) as Dictionary<string, object>;
                errCode = Convert.ToInt32(dict["code"]);// as int;
                errMessage = dict["message"] as string;
            }

            itavioDbg.Log("Error " + errCode + ": " + errMessage);
            if (OnError != null)
            {
                OnError(errCode, errMessage);
            }
        }

#if ITAVIO_ANDROID
        
        private AndroidJavaObject activityContext = null;
        private AndroidJavaClass androidClass;

        private AndroidJavaObject Plugin
        {
            get
            {
                AndroidJavaObject pluginInstance = null;

                using (AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject currentActivity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                    activityContext = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                }
                if (androidClass == null)
                {
                    AndroidJNI.AttachCurrentThread();
                    androidClass = new AndroidJavaClass("com.itavio.android.plugin.unity.UnityBridge");
                    object[] args = new[] { activityContext };
                    pluginInstance = androidClass.CallStatic<AndroidJavaObject>("getInstance");
                }
                else
                {
                    object[] args = new[] { activityContext };
                    pluginInstance = androidClass.CallStatic<AndroidJavaObject>("getInstance");
                }
                return pluginInstance;
            }
        }

#elif ITAVIO_IOS

        [DllImport("__Internal")]
        private static extern int itavioInitialize(string secretKeyId, string secretKey, int environment);

        [DllImport("__Internal")]
        private static extern void itavioGetBalance();

        [DllImport("__Internal")]
        private static extern void itavioStartDebit(double amount, string currencyCode);

        [DllImport("__Internal")]
        private static extern void itavioCancelDebit();

        [DllImport("__Internal")]
        private static extern void itavioCompleteDebit();

        [DllImport("__Internal")]
        private static extern void itavioCheckForParent(bool showGetAppDialog, bool ignoreSuppression);
        
        [DllImport("__Internal")]
        private static extern bool itavioHasLink();

        [DllImport("__Internal")]
        private static extern void itavioLinkWithParentApp(bool showGetAppDialog, bool relink);

        private static bool PluginExists { get { return Instance != null; } }

#endif
    }
}
