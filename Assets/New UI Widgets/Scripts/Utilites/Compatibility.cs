﻿namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;
#if UNITY_EDITOR
	using UnityEditor;
#endif

	/// <summary>
	/// Compatibility functions.
	/// </summary>
	public static class Compatibility
	{
#if UNITY_EDITOR
		/// <summary>
		/// Remove text previously added to force recomile.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <returns>True if text removed; otherwise false</returns>
		public static bool RemoveForceRecompileByLabel(string label)
		{
			var path = Utilites.GetAssetPath(label);
			if (path == null)
			{
				return false;
			}

			if (Directory.Exists(path))
			{
				RemoveForceRecompileFolder(path);
			}
			else
			{
				RemoveForceRecompileFile(path);
			}

			return true;
		}

		static void RemoveForceRecompileFolder(string path)
		{
			var dir = new DirectoryInfo(path);
			var files = dir.GetFiles("*.cs", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				RemoveForceRecompileFile(file.FullName);
			}

			AssetDatabase.Refresh();
		}

		static void RemoveForceRecompileFile(string filepath)
		{
			var lines = new List<string>(File.ReadAllLines(filepath));
			var prefix = "// Force script reload at ";

			if (lines[lines.Count - 1].StartsWith(prefix))
			{
				lines.RemoveAt(lines.Count - 1);

				File.WriteAllText(filepath, string.Join("\r\n", lines.ToArray()));
				File.SetLastWriteTimeUtc(filepath, DateTime.UtcNow);
			}
		}
#endif

#if UNITY_EDITOR
		/// <summary>
		/// Use this function to create a Prefab Asset at the given path from the given GameObject including any childen in the Scene without modifying the input objects.
		/// </summary>
		/// <param name="assetPath">The path to save the Prefab at.</param>
		/// <param name="instanceRoot">The GameObject to save as a Prefab.</param>
		/// <returns>Prefab instance.</returns>
		public static GameObject CreatePrefab(string assetPath, GameObject instanceRoot)
		{
#if UNITY_2018_3_OR_NEWER
			return PrefabUtility.SaveAsPrefabAsset(instanceRoot, assetPath);
#else
			return PrefabUtility.CreatePrefab(assetPath, instanceRoot);
#endif
		}

		static void ForceRecompileFolder(string path)
		{
			var dir = new DirectoryInfo(path);
			var files = dir.GetFiles("*.cs", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				ForceRecompileFile(file.FullName);
			}

			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Force recompile by changing specified script content.
		/// </summary>
		/// <param name="filepath">Path to file.</param>
		static void ForceRecompileFile(string filepath)
		{
			var lines = new List<string>(File.ReadAllLines(filepath));
			var prefix = "// Force script reload at ";

			if (lines[lines.Count - 1].StartsWith(prefix))
			{
				lines[lines.Count - 1] = prefix + DateTime.Now;
			}
			else
			{
				lines.Add(prefix + DateTime.Now);
			}

			File.WriteAllText(filepath, string.Join("\r\n", lines.ToArray()));
			File.SetLastWriteTimeUtc(filepath, DateTime.UtcNow);
		}

		/// <summary>
		/// Force recompile by changing specified script content.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <returns>True if recompilation done; otherwise false</returns>
		public static bool ForceRecompileByLabel(string label)
		{
			var path = Utilites.GetAssetPath(label);
			if (path == null)
			{
				return false;
			}

			if (Directory.Exists(path))
			{
				ForceRecompileFolder(path);
			}
			else
			{
				ForceRecompileFile(path);
			}

			return true;
		}

		/// <summary>
		/// Get TextMesh Pro version.
		/// </summary>
		/// <returns>TextMesh Pro version</returns>
		public static string GetTMProVersion()
		{
			Dictionary<string, string> guid2version = new Dictionary<string, string>()
			{
				{ "496f2e385b0c62542b5c739ccfafd8da", "V1Script" }, // first assetstore version
				{ "89f0137620f6af44b9ba852b4190e64e", "V2DLL" }, // unity assetstore version
				{ "f4688fdb7df04437aeb418b961361dc5", "V3Package" }, // unity package version
			};

			foreach (var gv in guid2version)
			{
				var path = AssetDatabase.GUIDToAssetPath(gv.Key);
				if (!string.IsNullOrEmpty(path))
				{
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1
					if (gv.Value == "V1Script")
					{
						return gv.Value + "NoInput";
					}
#endif
					return gv.Value;
				}
			}

			return null;
		}

		/// <summary>
		/// Imports package at packagePath into the current project.
		/// If interactive is true, an import package dialog will be opened, else all assets in the package will be imported into the current project.
		/// </summary>
		/// <param name="packagePath">Package path.</param>
		/// <param name="interactive">Show import package dialog.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "interactive", Justification = "Reviewed.")]
		public static void ImportPackage(string packagePath, bool interactive = false)
		{
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			AssetDatabase.ImportPackage(packagePath, interactive);
#else
			EditorUtility.DisplayDialog(
				"Warning",
				"Unity 4.6 and Unity 4.7 requires to manually import package \"" + packagePath + "\".",
				"OK");
#endif
		}

		/// <summary>
		/// Set ScrollRect viewport.
		/// </summary>
		/// <param name="scrollRect">ScrollRect.</param>
		/// <param name="viewport">Viewport.</param>
#if !(UNITY_5_3 || UNITY_5_3_OR_NEWER)
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "scrollRect", Justification = "Reviewed.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "viewport", Justification = "Reviewed.")]
#endif
		public static void SetViewport(ScrollRect scrollRect, RectTransform viewport)
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			scrollRect.viewport = viewport;
#endif
		}

		/// <summary>
		/// Save scene if user wants to.
		/// </summary>
		/// <returns>True if user saved scene; otherwise false.</returns>
		public static bool SceneSave()
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			return UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
#else
			return EditorApplication.SaveCurrentSceneIfUserWantsTo();
#endif
		}

		/// <summary>
		/// Save scene with the specified path.
		/// </summary>
		/// <param name="path">Path.</param>
		public static void SceneSave(string path)
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene(), path);
#else
			EditorApplication.SaveScene(path);
#endif
		}

		/// <summary>
		/// Create new empty scene.
		/// </summary>
		public static void SceneNew()
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#else
			EditorApplication.NewScene();

			var camera = GameObject.Find("Main Camera");
			if (camera != null)
			{
				UnityEngine.Object.DestroyImmediate(camera);
			}
#endif
		}

		/// <summary>
		/// Returns the first asset object of type T at given path assetPath.
		/// </summary>
		/// <returns>The asset at path.</returns>
		/// <param name="path">Path.</param>
		/// <typeparam name="T">Asset type.</typeparam>
		public static T LoadAssetAtPath<T>(string path)
			where T : UnityEngine.Object
		{
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			return AssetDatabase.LoadAssetAtPath<T>(path);
#else
			return Resources.LoadAssetAtPath<T>(path);
#endif
		}

		/// <summary>
		/// Create EventSystem.
		/// </summary>
		public static void CreateEventSystem()
		{
			var eventSystemGO = new GameObject("EventSystem");
			eventSystemGO.AddComponent<EventSystem>();
			eventSystemGO.AddComponent<StandaloneInputModule>();
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			eventSystemGO.AddComponent<TouchInputModule>();
#endif

			Undo.RegisterCreatedObjectUndo(eventSystemGO, "Create " + eventSystemGO.name);
		}
#endif

		/// <summary>
		/// Clones the object original and returns the clone.
		/// </summary>
		/// <typeparam name="T">Type of the original object.</typeparam>
		/// <param name="original">	An existing object that you want to make a copy of.</param>
		/// <returns>The instantiated clone</returns>
		public static T Instantiate<T>(T original)
			where T : UnityEngine.Object
		{
#if UNITY_4_6 || UNITY_4_7
			return UnityEngine.Object.Instantiate(original) as T;
#else
			return UnityEngine.Object.Instantiate(original);
#endif
		}

		/// <summary>
		/// Set layout group child size settings.
		/// </summary>
		/// <param name="lg">Layout group.</param>
		/// <param name="width">Control width.</param>
		/// <param name="height">Control height.</param>
#if !UNITY_2017_1_OR_NEWER
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "width", Justification = "Reviewed.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "lg", Justification = "Reviewed.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "height", Justification = "Reviewed.")]
#endif
		public static void SetLayoutChildControlsSize(HorizontalOrVerticalLayoutGroup lg, bool width, bool height)
		{
#if UNITY_2017_1_OR_NEWER
			lg.childControlWidth = width;
			lg.childControlHeight = height;
#endif
		}

		/// <summary>
		/// Get layout group childControlWidth setting.
		/// </summary>
		/// <param name="lg">Layout group.</param>
		/// <returns>childControlWidth value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "lg", Justification = "Conditional compilation.")]
		public static bool GetLayoutChildControlWidth(HorizontalOrVerticalLayoutGroup lg)
		{
#if UNITY_2017_1_OR_NEWER
			return lg.childControlWidth;
#else
			return true;
#endif
		}

		/// <summary>
		/// Get layout group childControlHeight setting.
		/// </summary>
		/// <param name="lg">Layout group.</param>
		/// <returns>childControlHeight value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "lg", Justification = "Conditional compilation.")]
		public static bool GetLayoutChildControlHeight(HorizontalOrVerticalLayoutGroup lg)
		{
#if UNITY_2017_1_OR_NEWER
			return lg.childControlHeight;
#else
			return true;
#endif
		}

		/// <summary>
		/// Gets the cursor mode.
		/// </summary>
		/// <returns>The cursor mode.</returns>
		public static CursorMode GetCursorMode()
		{
#if UNITY_WEBGL
			return CursorMode.ForceSoftware;
#else
			return CursorMode.Auto;
#endif
		}

		/// <summary>
		/// Gets the components.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="components">Components.</param>
		/// <typeparam name="T">Component type.</typeparam>
		public static void GetComponents<T>(GameObject source, List<T> components)
			where T : class
		{
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			source.GetComponents<T>(components);
#else
			foreach (var component in source.GetComponents(typeof(T)))
			{
				components.Add(component as T);
			}
#endif
		}

		/// <summary>
		/// Gets the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="source">Source.</param>
		/// <typeparam name="T">Component type.</typeparam>
		public static T GetComponent<T>(Component source)
			where T : class
		{
			return GetComponent<T>(source.gameObject);
		}

		/// <summary>
		/// Gets the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="source">Source.</param>
		/// <typeparam name="T">Component type.</typeparam>
		public static T GetComponent<T>(GameObject source)
			where T : class
		{
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			return source.GetComponent<T>();
#else
			return source.GetComponent(typeof(T)) as T;
#endif
		}

		/// <summary>
		/// Disable and enable gameobject back to apply shader variables changes.
		/// </summary>
		/// <param name="go">Target gameobject.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "go", Justification = "Conditional compilation.")]
		public static void ToggleGameObject(GameObject go)
		{
			#if (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER) && !UNITY_2017_1_OR_NEWER
			go.SetActive(false);
			go.SetActive(true);
			#endif
		}

		/// <summary>
		/// Disable and enable gameobject back to apply shader variables changes.
		/// </summary>
		/// <param name="component">Target component.</param>
		public static void ToggleGameObject(Component component)
		{
			ToggleGameObject(component.gameObject);
		}
	}
}