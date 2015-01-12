using System;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;


#endif

[Serializable]
public class LevelKey
{
	public int sceneNumber;
	public string key;
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(LevelKey))]
public class LevelKeyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);
		Rect contentPosition = EditorGUI.PrefixLabel(position, label);

		float originalWidth = contentPosition.width;

		EditorGUI.indentLevel = 0;

		contentPosition.width = originalWidth * 0.30f;
		EditorGUIUtility.labelWidth = 42f;

		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("sceneNumber"), new GUIContent("Scene"));

		contentPosition.x += contentPosition.width;
		contentPosition.width = originalWidth * 0.70f;
		EditorGUIUtility.labelWidth = 25f;

		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("key"), new GUIContent("Key"));

		EditorGUI.EndProperty();
	}
}

#endif