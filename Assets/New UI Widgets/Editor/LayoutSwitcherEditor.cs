﻿namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// LayoutSwitcher editor.
	/// </summary>
	[CustomEditor(typeof(LayoutSwitcher), true)]
	public class LayoutSwitcherEditor : Editor
	{
		Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();

		string[] properties = new string[]
		{
			"Objects",
			"Layouts",
			"DefaultDisplaySize",
			"LayoutChanged",
		};

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			Array.ForEach(properties, x =>
			{
				var p = serializedObject.FindProperty(x);
				serializedProperties.Add(x, p);
			});
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedProperties["Objects"], true);

			DisplayLayouts();

			EditorGUILayout.PropertyField(serializedProperties["DefaultDisplaySize"], true);
			EditorGUILayout.PropertyField(serializedProperties["LayoutChanged"], true);

			serializedObject.ApplyModifiedProperties();
		}

		void DisplayLayouts()
		{
			var switcher = target as LayoutSwitcher;
			var layouts = serializedProperties["Layouts"];

			layouts.isExpanded = EditorGUILayout.Foldout(layouts.isExpanded, new GUIContent("Layouts"));
			if (layouts.isExpanded)
			{
				EditorGUI.indentLevel++;
				layouts.arraySize = EditorGUILayout.IntField("Size", layouts.arraySize);
				for (int i = 0; i < layouts.arraySize; i++)
				{
					var property = layouts.GetArrayElementAtIndex(i);
					EditorGUILayout.PropertyField(property, true);
					if (GUILayout.Button("Save"))
					{
						switcher.SaveLayout(switcher.Layouts[i]);
					}
				}

				EditorGUI.indentLevel--;
			}
		}
	}
}