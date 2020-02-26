﻿namespace UIWidgets.Styles
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the picker.
	/// </summary>
	public class StyleSupportPicker : MonoBehaviour, IStylable
	{
		/// <summary>
		/// The background.
		/// </summary>
		[SerializeField]
		public Image Background;

		/// <summary>
		/// The title text.
		/// </summary>
		[SerializeField]
		public GameObject TitleText;

		/// <summary>
		/// The content background.
		/// </summary>
		[SerializeField]
		public Image ContentBackground;

		/// <summary>
		/// The content text.
		/// </summary>
		[SerializeField]
		public GameObject ContentText;

		/// <summary>
		/// The delimiter.
		/// </summary>
		[SerializeField]
		public Image Delimiter;

		/// <summary>
		/// The buttons.
		/// </summary>
		[SerializeField]
		public List<StyleSupportButton> Buttons = new List<StyleSupportButton>();

		/// <summary>
		/// The stylables.
		/// </summary>
		[SerializeField]
		public List<GameObject> Stylables = new List<GameObject>();

		/// <summary>
		/// The close button at the header.
		/// </summary>
		[SerializeField]
		public StyleSupportButtonClose HeaderCloseButton;

		#region IStylable implementation

		/// <summary>
		/// Set the style.
		/// </summary>
		/// <returns><c>true</c>, if style was set, <c>false</c> otherwise.</returns>
		/// <param name="style">Style.</param>
		public virtual bool SetStyle(Style style)
		{
			style.Dialog.Background.ApplyTo(GetComponent<Image>());
			style.Dialog.Title.ApplyTo(TitleText);

			style.Dialog.ContentBackground.ApplyTo(ContentBackground);
			style.Dialog.ContentText.ApplyTo(ContentText);

			style.Dialog.Delimiter.ApplyTo(Delimiter);

			Buttons.ForEach(style.Dialog.Button.ApplyTo);

			Stylables.ForEach(x => style.ApplyTo(x));

			HeaderCloseButton.SetStyle(style);

			return true;
		}
		#endregion
	}
}