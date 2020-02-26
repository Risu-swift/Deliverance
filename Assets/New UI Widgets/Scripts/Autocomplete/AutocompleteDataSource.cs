﻿namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// AutocompleteDataSource.
	/// Set Autocomplete.DataSource with strings from file.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Autocomplete DataSource")]
	[RequireComponent(typeof(Autocomplete))]
	public class AutocompleteDataSource : MonoBehaviour
	{
		[SerializeField]
		TextAsset file;

		/// <summary>
		/// Gets or sets the file.
		/// </summary>
		/// <value>The file.</value>
		public TextAsset File
		{
			get
			{
				return file;
			}

			set
			{
				file = value;
				if (file != null)
				{
					SetDataSource(file);
				}
			}
		}

		/// <summary>
		/// The comments in file start with specified strings.
		/// </summary>
		[SerializeField]
		public List<string> CommentsStartWith = new List<string>() { "#", "//" };

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			File = file;
		}

		/// <summary>
		/// Gets the items from file.
		/// </summary>
		/// <param name="sourceFile">Source file.</param>
		public virtual void SetDataSource(TextAsset sourceFile)
		{
			if (file == null)
			{
				return;
			}

			var data = new List<string>();

			foreach (var item in sourceFile.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
			{
				var trimmed = item.TrimEnd();
				if (!string.IsNullOrEmpty(trimmed))
				{
					if (!IsComment(trimmed))
					{
						data.Add(trimmed);
					}
				}
			}

			GetComponent<Autocomplete>().DataSource = data;
		}

		/// <summary>
		/// Is comment?
		/// </summary>
		/// <param name="str">String to check.</param>
		/// <returns>true if input is comment; otherwise false.</returns>
		protected virtual bool IsComment(string str)
		{
			for (int i = 0; i < CommentsStartWith.Count; i++)
			{
				if (str.StartsWith(CommentsStartWith[i]))
				{
					return true;
				}
			}

			return false;
		}
	}
}