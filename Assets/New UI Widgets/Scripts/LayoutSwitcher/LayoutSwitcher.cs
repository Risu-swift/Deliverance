﻿namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// Layout switcher.
	/// </summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/New UI Widgets/Layout Switcher")]
	public class LayoutSwitcher : MonoBehaviour
	{
		/// <summary>
		/// The trackable objects.
		/// </summary>
		[SerializeField]
		protected List<RectTransform> Objects = new List<RectTransform>();

		/// <summary>
		/// The layouts.
		/// </summary>
		[SerializeField]
		public List<UILayout> Layouts = new List<UILayout>();

		/// <summary>
		/// Layout changed event.
		/// </summary>
		[SerializeField]
		public LayoutSwitcherEvent LayoutChanged = new LayoutSwitcherEvent();

		/// <summary>
		/// The default display size.
		/// </summary>
		[SerializeField]
		[Tooltip("Display size used when actual display size cannot be detected.")]
		public float DefaultDisplaySize;

		/// <summary>
		/// Window width.
		/// </summary>
		protected int WindowWidth = 0;

		/// <summary>
		/// Window height.
		/// </summary>
		protected int WindowHeight = 0;

		/// <summary>
		/// Function to select layout.
		/// </summary>
		protected Func<List<UILayout>, float, float, UILayout> layoutSelector = DefaultLayoutSelector;

		/// <summary>
		/// Function to select layout.
		/// </summary>
		public virtual Func<List<UILayout>, float, float, UILayout> LayoutSelector
		{
			get
			{
				return layoutSelector;
			}

			set
			{
				layoutSelector = value;

				ResolutionChanged();
			}
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		protected virtual void Update()
		{
			if (WindowWidth != Screen.width || WindowHeight != Screen.height)
			{
				WindowWidth = Screen.width;
				WindowHeight = Screen.height;
				ResolutionChanged();
			}
		}

		/// <summary>
		/// Saves the layout.
		/// </summary>
		/// <param name="layout">Layout.</param>
		public virtual void SaveLayout(UILayout layout)
		{
			layout.Save(Objects);
		}

		/// <summary>
		/// Load layout when resolution changed.
		/// </summary>
		public virtual void ResolutionChanged()
		{
			var currentLayout = GetCurrentLayout();
			if (currentLayout == null)
			{
				return;
			}

			currentLayout.Load();
			LayoutChanged.Invoke(currentLayout);
		}

		/// <summary>
		/// Gets the current layout.
		/// </summary>
		/// <returns>The current layout.</returns>
		public virtual UILayout GetCurrentLayout()
		{
			if (Layouts.Count == 0)
			{
				return null;
			}

			return LayoutSelector(Layouts, DisplaySize(), AspectRatio());
		}

		/// <summary>
		/// Default layout select.
		/// </summary>
		/// <param name="layouts">Available layouts.</param>
		/// <param name="displaySize">Display size in inches.</param>
		/// <param name="aspectRatio">Aspect ratio.</param>
		/// <returns>Layout to use.</returns>
		public static UILayout DefaultLayoutSelector(List<UILayout> layouts, float displaySize, float aspectRatio)
		{
			var layouts_ar = layouts.Where(x =>
			{
				var diff = Mathf.Abs(aspectRatio - (x.AspectRatio.x / x.AspectRatio.y));

				return diff < 0.05f;
			}).ToList();
			if (layouts_ar.Count == 0)
			{
				return null;
			}

			var layouts_ds = layouts_ar.Where(x => displaySize < x.MaxDisplaySize)
				.OrderBy(x => Mathf.Abs(x.MaxDisplaySize - displaySize)).ToList();
			if (layouts_ds.Count == 0)
			{
				layouts_ds = layouts_ar.OrderBy(x => Mathf.Abs(x.MaxDisplaySize - displaySize)).ToList();
			}

			return layouts_ds.First();
		}

		/// <summary>
		/// Current aspect ratio.
		/// </summary>
		/// <returns>The ratio.</returns>
		public virtual float AspectRatio()
		{
			return (float)WindowWidth / (float)WindowHeight;
		}

		/// <summary>
		/// Current display size.
		/// </summary>
		/// <returns>The size.</returns>
		public virtual float DisplaySize()
		{
			if (Screen.dpi == 0)
			{
				return DefaultDisplaySize;
			}

			return Mathf.Sqrt(WindowWidth ^ 2 + WindowHeight ^ 2) / Screen.dpi;
		}
	}
}