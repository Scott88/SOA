using UnityEngine;
using UnityEditor;

public class AndroidTools : MonoBehaviour {

    [MenuItem("Android Tools/Set Keystore Values")]
    private static void SetKeystore()
    {
        PlayerSettings.Android.keystoreName = Application.dataPath + "/Editor/SOAKeystore.keystore";
        PlayerSettings.Android.keystorePass = "R38245809";

        PlayerSettings.Android.keyaliasName = "soa01";
        PlayerSettings.Android.keyaliasPass = "Sc0ut2453";
    }
	
}
