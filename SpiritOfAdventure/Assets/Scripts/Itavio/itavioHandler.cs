using UnityEngine;
using System.Collections;
using itavio;

public class itavioHandler : MonoBehaviour {

    private bool m_hasInit = false;

	void OnEnable()
    {
        Init();
    }

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (m_hasInit)
            return;

        m_hasInit = true;

        itavio.Utilities.itavioDbg.IsEnabled = true;

        itavioManager.initialize("Production");
    }
}
