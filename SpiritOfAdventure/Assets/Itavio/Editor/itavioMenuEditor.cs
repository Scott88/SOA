/*
 * Copyright (c) 2015. KinderGuardian Inc.
 */

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace itavio.Editor
{
	public static class itavioMenuEditor
	{
		private const string k_resourcePath = "Assets/Plugins/Itavio/Resources/";

		[MenuItem("Assets/Create/Itavio Config")]
		private static void CreateConfig()
		{
			itavioConfig asset = ScriptableObject.CreateInstance<itavioConfig>();
			string assetPath = AssetDatabase.GenerateUniqueAssetPath(k_resourcePath + "New " + typeof(itavioConfig).ToString() + ".asset");

			AssetDatabase.CreateAsset(asset, assetPath);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
		}
	}
}