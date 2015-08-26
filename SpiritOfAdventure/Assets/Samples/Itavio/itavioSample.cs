/*
 * Copyright (c) 2015. KinderGuardian Inc.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using itavio;
using itavio.Utilities;

public class itavioSample : MonoBehaviour
{
    private bool m_hasParentApp = false;
    private bool m_showGetAppDialog = false;
    private bool m_canCompleteDebit = false;
    private double m_balance = -1.0;
    private string m_purchaseValue = "1";
    private List<string> m_messages = new List<string>() { "", "", "" };

    private string Currency { get { return (m_currencyIsCAD ? "CAD" : "USD"); } }
    private bool m_currencyIsCAD = false;

    public delegate void SampleTransactionCompleted();
    public event SampleTransactionCompleted OnSampleTransactionCompleted;

    // Use this for initialization
    void Start()
    {
        itavioDbg.IsEnabled = true;	// Enable Itavio Debug Messages
        //itavioManager.initialize("itavioSample");
        itavioManager.initialize("itavioSampleStaging");

        OnSampleTransactionCompleted += itavioSample_OnSampleTransactionCompleted;

        itavioManager.OnGetBalance += itavioManager_OnGetBalance;
        itavioManager.OnStartDebit += itavioManager_OnStartDebit;
        itavioManager.OnCancelDebit += itavioManager_OnCancelDebit;
        itavioManager.OnCompleteDebit += itavioManager_OnCompleteDebit;
        itavioManager.OnCheckForParent += itavioManager_OnCheckForParent;
        itavioManager.OnError += itavioManager_OnError;

#if UNITY_IOS
        itavioManager.OnLinkWithParentApp += itavioManager_OnLinkWithParentApp;
#endif
    }

    private void Log(string message)
    {
        m_messages.Add(message);
        itavioDbg.Log(message);
    }

    private void SampleTransaction(string item, int cost)
    {
        Log("Sample Transaction Started");
        if (OnSampleTransactionCompleted != null)
        {
            OnSampleTransactionCompleted();
        }
    }

    private void itavioSample_OnSampleTransactionCompleted()
    {
        Log("Sample Transaction Completed");
        m_canCompleteDebit = true;
    }

    void itavioManager_OnGetBalance(double result)
    {
        Log("Sample Retrieved Balance");
        m_balance = result;
    }

    void itavioManager_OnStartDebit(bool result)
    {
        if (result)
        {
            Log("Sample Started Debit");
        }
    }

    void itavioManager_OnCancelDebit(bool result)
    {
        if (result)
        {
            Log("Sample Debit Cancelled");
            m_canCompleteDebit = false;
        }
    }

    void itavioManager_OnCompleteDebit(bool result)
    {
        if (result)
        {
            Log("Sample Debit Completed");
            m_canCompleteDebit = false;
        }
    }

    void itavioManager_OnCheckForParent(bool result)
    {
        Log("Has Parent App: " + result);
        m_hasParentApp = result;
    }

    void itavioManager_OnError(int code, string message)
    {
        m_messages.Add("OnError " + code + ": " + message);
        itavioDbg.LogError("Error " + code + ": " + message);
    }

#if UNITY_IOS
    void itavioManager_OnLinkWithParentApp(bool result)
    {
        Log("Linked Parent: " + result);
    }
#endif

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));

        if (m_messages.Count > 3)
        {
            m_messages.RemoveRange(0, m_messages.Count - 3);
        }

        for (int i = 0; i < m_messages.Count; ++i)
        {
            GUILayout.Label(m_messages[i]);
        }

        GUI.enabled = true;

        m_showGetAppDialog = GUILayout.Toggle(m_showGetAppDialog, "Show Get App Dialog");

        if (GUILayout.Button("Check For Parent", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            itavioManager.checkForParent(m_showGetAppDialog);
        }

        GUI.enabled = m_hasParentApp;

#if UNITY_IOS
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Link with Parent", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            itavioManager.linkWithParentApp(m_showGetAppDialog);
        }

        if (GUILayout.Button("Re-Link with Parent", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            itavioManager.linkWithParentApp(m_showGetAppDialog, true);
        }
        GUILayout.EndHorizontal();
#endif

        GUILayout.Label("Balance: " + m_balance);

        if (GUILayout.Button("Get Balance", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            itavioManager.getBalance();
        }

        if (GUILayout.Button("Toggle Currency (Currently " + Currency + ")", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            m_currencyIsCAD = !m_currencyIsCAD;
            Log("Currency is " + Currency);
        }

        m_purchaseValue = GUILayout.TextField(m_purchaseValue);
        if (!m_canCompleteDebit)
        {
            if (GUILayout.Button("Start Debit", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                itavioManager.startDebit<Action<string, int>>(Convert.ToDouble(m_purchaseValue), Currency, SampleTransaction, "item_id", 1);
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Complete Debit", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                itavioManager.finalizeDebit(true);
            }

            if (GUILayout.Button("Cancel Debit", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                itavioManager.finalizeDebit(false);
            }
            GUILayout.EndHorizontal();
        }

        GUI.enabled = true;
        GUILayout.EndArea();
    }

}
