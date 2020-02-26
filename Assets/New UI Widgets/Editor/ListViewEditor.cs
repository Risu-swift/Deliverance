﻿namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// ListView editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ListView), true)]
	public class ListViewEditor : Editor
	{
		Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();

		string[] properties = new string[]
		{
			"interactable",
			"Source",
			"strings",
			"file",
			"CommentsStartWith",
			"sort",
			"Unique",
			"AllowEmptyItems",

			"multipleSelect",
			"selectedIndex",
			"direction",
			"Container",
			"defaultItem",
			"scrollRect",

			"backgroundColor",
			"textColor",
			"HighlightedBackgroundColor",
			"HighlightedTextColor",
			"selectedBackgroundColor",
			"selectedTextColor",
			"disabledColor",

			// other
			"FadeDuration",
			"LimitScrollValue",
			"loopedList",
			"setContentSizeFitter",
			"Navigation",
			"centerTheItems",

			"OnSelectString",
			"OnDeselectString",
		};

		HashSet<string> exclude = new HashSet<string>()
		{
			"interactable",
			"Source",
			"strings",
			"file",
			"CommentsStartWith",
			"AllowEmptyItems",
			"Unique",

			// obsolete
			"LimitScrollValue",
		};

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			Array.ForEach(properties, x =>
			{
				var p = serializedObject.FindProperty(x);
				if (p == null)
				{
					return;
				}

				if (serializedProperties.ContainsKey(x))
				{
					return;
				}

				serializedProperties.Add(x, p);
			});
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedProperties["interactable"]);
			EditorGUILayout.PropertyField(serializedProperties["Source"]);

			EditorGUI.indentLevel++;
			if (serializedProperties["Source"].enumValueIndex == 0)
			{
				var options = new GUILayoutOption[] { };
				EditorGUILayout.PropertyField(serializedProperties["strings"], new GUIContent("Data Source"), true, options);
			}
			else
			{
				EditorGUILayout.PropertyField(serializedProperties["file"]);
				EditorGUILayout.PropertyField(serializedProperties["CommentsStartWith"], true);
				EditorGUILayout.PropertyField(serializedProperties["AllowEmptyItems"]);
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.PropertyField(serializedProperties["Unique"], new GUIContent("Only unique items"));

			foreach (var sp in serializedProperties)
			{
				if (!exclude.Contains(sp.Key))
				{
					EditorGUILayout.PropertyField(sp.Value);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}