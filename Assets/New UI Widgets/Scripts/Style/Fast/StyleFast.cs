﻿namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Fast style definition.
	/// </summary>
	[Serializable]
	public class StyleFast : IStyleDefaultValues
	{
		/// <summary>
		/// Primary color.
		/// </summary>
		[Header("Colors")]
		[SerializeField]
		public Color ColorPrimary = new Color(1.000f, 0.843f, 0.451f, 1.000f);

		/// <summary>
		/// Secondary color.
		/// </summary>
		[SerializeField]
		public Color ColorSecondary = new Color(0.000f, 0.000f, 0.000f, 1.000f);

		/// <summary>
		/// Secondary color.
		/// </summary>
		[SerializeField]
		public Color ColorBackground = new Color(0.153f, 0.157f, 0.168f, 1.000f);

		/// <summary>
		/// Highlighted background color.
		/// </summary>
		[SerializeField]
		public Color ColorHighlightedBackground = new Color(0.710f, 0.478f, 0.141f, 1.000f);

		/// <summary>
		/// Selected background color.
		/// </summary>
		[SerializeField]
		public Color ColorSelectedBackground = new Color(0.769f, 0.612f, 0.153f, 1.000f);

		/// <summary>
		/// Disabled color.
		/// </summary>
		[SerializeField]
		public Color ColorDisabled = new Color(0.784f, 0.784f, 0.784f, 1.000f);

		/// <summary>
		/// Calendar weekend color.
		/// </summary>
		[SerializeField]
		public Color ColorCalendarWeekend = new Color(1.000f, 0.000f, 0.000f, 1.000f);

		/// <summary>
		/// Background.
		/// </summary>
		[Header("Common")]
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Hightlighted background.
		/// </summary>
		[SerializeField]
		public StyleImage BackgroundHightlighted;

		/// <summary>
		/// Background for collections item.
		/// </summary>
		[SerializeField]
		public StyleImage CollectionsItemBackground;

		/// <summary>
		/// Arrow.
		/// </summary>
		[SerializeField]
		public StyleImage Arrow;

		/// <summary>
		/// The font.
		/// </summary>
		[SerializeField]
		public Font Font;

		/// <summary>
		/// The TMPro font.
		/// </summary>
		[SerializeField]
#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
		public TMPro.TMP_FontAsset FontTMPro;
#elif UIWIDGETS_TMPRO_SUPPORT
		public TMPro.TextMeshProFont FontTMPro;
#else
		public ScriptableObject FontTMPro;
#endif

		/// <summary>
		/// Button.
		/// </summary>
		[Header("Buttons")]
		[SerializeField]
		public StyleFastButton Button;

		/// <summary>
		/// Close button.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonClose;

		/// <summary>
		/// Button pause.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonPause;

		/// <summary>
		/// Button down.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonArrowDown;

		/// <summary>
		/// Button up.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonArrowUp;

		/// <summary>
		/// Horizontal scrollbar.
		/// </summary>
		[Header("Scrollbars")]
		[SerializeField]
		public StyleFastScrollbar ScrollbarHorizontal;

		/// <summary>
		/// Vertical scrollbar.
		/// </summary>
		[SerializeField]
		public StyleFastScrollbar ScrollbarVertical;

		/// <summary>
		/// Determinate progressbar.
		/// </summary>
		[Header("Progressbars")]
		[SerializeField]
		public StyleFastButton ProgressbarDeterminate;

		/// <summary>
		/// Indeterminate progressbar.
		/// </summary>
		[SerializeField]
		public StyleFastProgressbarIndeterminate ProgressbarIndeterminate;

		/// <summary>
		/// Tab content background.
		/// </summary>
		[Header("Tabs")]
		[SerializeField]
		public StyleImage TabContentBackground;

		/// <summary>
		/// Inactive top tab.
		/// </summary>
		[SerializeField]
		public StyleFastButton TabTopInactive;

		/// <summary>
		/// Active top tab.
		/// </summary>
		[SerializeField]
		public StyleFastButton TabTopActive;

		/// <summary>
		/// Inactive left tab.
		/// </summary>
		[SerializeField]
		public StyleFastButton TabLeftInactive;

		/// <summary>
		/// Inactive left tab.
		/// </summary>
		[SerializeField]
		public StyleFastButton TabLeftActive;

		/// <summary>
		/// Dialog delimiter.
		/// </summary>
		[Header("Other")]
		[SerializeField]
		public StyleImage DialogDelimiter;

		/// <summary>
		/// Background for tooltip.
		/// </summary>
		[SerializeField]
		public StyleImage TooltipBackground;

		/// <summary>
		/// Checkmark.
		/// </summary>
		[SerializeField]
		public StyleImage Checkmark;

		/// <summary>
		/// Change primary style.
		/// </summary>
		/// <param name="style">Style.</param>
		public void ChangeStyle(Style style)
		{
			SetImages(style);
			SetRawImages(style);
			SetColors(style);
			SetSprites(style);
			SetFonts(style);
		}

		/// <summary>
		/// Set style images.
		/// </summary>
		/// <param name="style">Style.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed")]
		public void SetImages(Style style)
		{
			style.Collections.MainBackground = Background;
			style.Collections.Viewport = Background;
			style.Combobox.Background = Background;
			style.FileListView.PathItemBackground = Background;
			style.Accordion.ContentBackground = Background;
			style.Dialog.Background = Background;
			style.Dialog.Button.Mask = Background;
			style.ButtonSmall.Mask = Background;
			style.Calendar.DaysBackground = Background;
			style.Calendar.DayBackground = Background;
			style.CenteredSliderHorizontal.Background = Background;
			style.CenteredSliderVertical.Background = Background;
			style.ColorPicker.Background = Background;
			style.ColorPicker.PaletteToggle.Mask = Background;
			style.ColorPicker.InputToggle.Mask = Background;
			style.ColorPicker.InputSpinner.InputBackground = Background;
			style.ColorPicker.HexInputBackground = Background;
			style.ColorPickerRangeHorizontal.Background = Background;
			style.ColorPickerRangeVertical.Background = Background;
			style.RangeSliderHorizontal.Background = Background;
			style.RangeSliderVertical.Background = Background;
			style.Spinner.Background = Background;
			style.Time.InputBackground = Background;
			style.Time.AMPMBackground = Background;
			style.AudioPlayer.Progress.Background = Background;
			style.AudioPlayer.Play.Mask = Background;
			style.AudioPlayer.Pause.Mask = Background;
			style.AudioPlayer.Stop.Mask = Background;
			style.AudioPlayer.Toggle.Mask = Background;
			style.Button.Background = Background;
			style.Dropdown.Background = Background;
			style.Dropdown.OptionsBackground = Background;

			style.Collections.DefaultItemBackground = CollectionsItemBackground;

			style.TreeView.Toggle = Arrow;
			style.AudioPlayer.Play.Image = Arrow;

			style.Combobox.SingleInputBackground = Background;
			style.Accordion.ToggleBackground = Background;
			style.Autocomplete.Background = Background;
			style.CenteredSliderHorizontal.Handle = Background;
			style.CenteredSliderVertical.Handle = Background;
			style.ColorPicker.SliderVerticalHandle = Background;
			style.ColorPicker.SliderHorizontalHandle = Background;
			style.ColorPickerRangeHorizontal.Handle = Background;
			style.ColorPickerRangeVertical.Handle = Background;
			style.RangeSliderHorizontal.HandleMin = Background;
			style.RangeSliderHorizontal.HandleMax = Background;
			style.RangeSliderVertical.HandleMin = Background;
			style.RangeSliderVertical.HandleMax = Background;
			style.Spinner.InputBackground = Background;
			style.AudioPlayer.Progress.Handle = Background;
			style.Paginator.DefaultBackground = Background;
			style.InputField.Background = Background;
			style.Slider.Handle = Background;
			style.Toggle.Background = Background;

			style.Combobox.MultipleDefaultItemBackground = BackgroundHightlighted;
			style.Combobox.RemoveBackground = BackgroundHightlighted;
			style.Sidebar.Background = BackgroundHightlighted;

			style.Combobox.ToggleButton = ButtonArrowDown;
			style.FileListView.ButtonToggle = ButtonArrowDown;
			style.Calendar.NextMonth = ButtonArrowDown;
			style.ColorPicker.InputSpinner.ButtonMinus = ButtonArrowDown;
			style.Spinner.ButtonMinus = ButtonArrowDown;
			style.Time.ButtonDecrease = ButtonArrowDown;
			style.Dropdown.Arrow = ButtonArrowDown;

			style.FileListView.ButtonUp = ButtonArrowUp;
			style.Calendar.PrevMonth = ButtonArrowUp;
			style.ColorPicker.InputSpinner.ButtonPlus = ButtonArrowUp;
			style.Spinner.ButtonPlus = ButtonArrowUp;
			style.Time.ButtonIncrease = ButtonArrowUp;

			style.TabsTop.DefaultButton.Background = TabTopInactive.Background;

			style.TabsTop.DefaultButton.Mask = TabTopInactive.Mask;

			style.TabsTop.DefaultButton.Border = TabTopInactive.Border;

			style.TabsTop.ActiveButton.Background = TabTopActive.Background;

			style.TabsTop.ActiveButton.Mask = TabTopActive.Mask;

			style.TabsTop.ActiveButton.Border = TabTopActive.Border;

			style.TabsTop.ContentBackground = TabContentBackground;
			style.TabsLeft.ContentBackground = TabContentBackground;

			style.TabsLeft.DefaultButton.Background = TabLeftInactive.Background;

			style.TabsLeft.DefaultButton.Mask = TabLeftInactive.Mask;

			style.TabsLeft.DefaultButton.Border = TabLeftInactive.Border;

			style.TabsLeft.ActiveButton.Background = TabLeftActive.Background;

			style.TabsLeft.ActiveButton.Mask = TabLeftActive.Mask;

			style.TabsLeft.ActiveButton.Border = TabLeftActive.Border;

			style.Dialog.Delimiter = DialogDelimiter;

			style.Dialog.Button.Background = Button.Background;
			style.ButtonBig.Background = Button.Background;
			style.ColorPicker.PaletteToggle.Background = Button.Background;
			style.ColorPicker.InputToggle.Background = Button.Background;
			style.AudioPlayer.Play.Background = Button.Background;
			style.AudioPlayer.Pause.Background = Button.Background;
			style.AudioPlayer.Stop.Background = Button.Background;
			style.AudioPlayer.Toggle.Background = Button.Background;

			style.Dialog.Button.Border = Button.Border;
			style.ButtonSmall.Border = Button.Border;
			style.ColorPicker.PaletteToggle.Border = Button.Border;
			style.ColorPicker.InputToggle.Border = Button.Border;
			style.AudioPlayer.Play.Border = Button.Border;
			style.AudioPlayer.Pause.Border = Button.Border;
			style.AudioPlayer.Stop.Border = Button.Border;
			style.AudioPlayer.Toggle.Border = Button.Border;

			style.Notify.Background = BackgroundHightlighted;

			style.ButtonBig.Mask = Button.Mask;

			style.ButtonBig.Border = Button.Border;

			style.ButtonSmall.Background = Button.Background;

			style.ColorPicker.PaletteCursor = BackgroundHightlighted;
			style.Paginator.ActiveBackground = BackgroundHightlighted;

			style.AudioPlayer.Pause.Image = ButtonPause;

			style.ProgressbarDeterminate.FullbarImage = ProgressbarDeterminate.Background;

			style.ProgressbarDeterminate.FullbarMask = ProgressbarDeterminate.Mask;

			style.ProgressbarDeterminate.FullbarBorder = ProgressbarDeterminate.Border;

			style.ProgressbarDeterminate.EmptyBar = Background;

			style.ProgressbarDeterminate.Background = Background;

			style.ProgressbarIndeterminate.Mask = ProgressbarIndeterminate.Mask;

			style.ProgressbarIndeterminate.Border = ProgressbarIndeterminate.Border;

			style.Tooltip.Background = TooltipBackground;

			style.ButtonClose.Background = ButtonClose;

			style.HorizontalScrollbar.Background = ScrollbarHorizontal.Background;

			style.HorizontalScrollbar.Handle = ScrollbarHorizontal.Handle;

			style.VerticalScrollbar.Background = ScrollbarVertical.Background;

			style.VerticalScrollbar.Handle = ScrollbarVertical.Handle;

			style.Toggle.Checkmark = Checkmark;
			style.Dropdown.ItemCheckmark = Checkmark;
		}

		/// <summary>
		/// Set style raw images.
		/// </summary>
		/// <param name="style">Style.</param>
		public void SetRawImages(Style style)
		{
			style.ProgressbarIndeterminate.Texture = ProgressbarIndeterminate.Background;
		}

		/// <summary>
		/// Set style colors.
		/// </summary>
		/// <param name="style">Style.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed")]
		public void SetColors(Style style)
		{
			style.Fast.ColorPrimary = ColorPrimary;
			style.Fast.ColorSecondary = ColorSecondary;
			style.Fast.ColorHighlightedBackground = ColorHighlightedBackground;
			style.Fast.ColorSelectedBackground = ColorSelectedBackground;
			style.Fast.ColorDisabled = ColorDisabled;
			style.Fast.ColorCalendarWeekend = ColorCalendarWeekend;

			style.Collections.HighlightedColor = ColorSecondary;
			style.Collections.SelectedColor = ColorSecondary;
			style.TabsTop.DefaultButton.Text.Color = ColorSecondary;
			style.TabsTop.ActiveButton.Text.Color = ColorSecondary;
			style.TabsLeft.DefaultButton.Text.Color = ColorSecondary;
			style.TabsLeft.ActiveButton.Text.Color = ColorSecondary;
			style.Dialog.Button.Text.Color = ColorSecondary;
			style.Notify.Text.Color = ColorSecondary;
			style.ButtonBig.Text.Color = ColorSecondary;
			style.ButtonSmall.Text.Color = ColorSecondary;
			style.CenteredSliderHorizontal.UsableRange.Color = ColorSecondary;
			style.CenteredSliderVertical.UsableRange.Color = ColorSecondary;
			style.ColorPicker.PaletteToggle.Text.Color = ColorSecondary;
			style.ColorPicker.InputToggle.Text.Color = ColorSecondary;
			style.RangeSliderHorizontal.UsableRange.Color = ColorSecondary;
			style.RangeSliderVertical.UsableRange.Color = ColorSecondary;
			style.Switch.Border.Color = ColorSecondary;
			style.Switch.BackgroundDefault.Color = ColorSecondary;
			style.Switch.BackgroundOnColor = ColorSecondary;
			style.Switch.BackgroundOffColor = ColorSecondary;
			style.Slider.Background.Color = ColorSecondary;

			style.Calendar.ColorWeekend = ColorCalendarWeekend;

			style.Collections.DefaultColor = ColorPrimary;
			style.Collections.DefaultItemText.Color = ColorPrimary;
			style.Combobox.SingleDefaultItemText.Color = ColorPrimary;
			style.Combobox.MultipleDefaultItemText.Color = ColorPrimary;
			style.Combobox.Input.Color = ColorPrimary;
			style.Combobox.Placeholder.Color = ColorPrimary;
			style.Combobox.RemoveText.Color = ColorPrimary;
			style.Table.Border.Color = ColorPrimary;
			style.FileListView.PathItemText.Color = ColorPrimary;
			style.IOCollectionsErrors.Color = ColorPrimary;
			style.Dialog.Title.Color = ColorPrimary;
			style.Dialog.ContentText.Color = ColorPrimary;
			style.Calendar.CurrentDate.Color = ColorPrimary;
			style.Calendar.CurrentMonth.Color = ColorPrimary;
			style.Calendar.DayOfWeekText.Color = ColorPrimary;
			style.Calendar.DayText.Color = ColorPrimary;
			style.Calendar.ColorSelectedDay = ColorPrimary;
			style.Calendar.ColorCurrentMonth = ColorPrimary;
			style.CenteredSliderHorizontal.Fill.Color = ColorPrimary;
			style.CenteredSliderVertical.Fill.Color = ColorPrimary;
			style.ColorPicker.InputSpinner.InputText.Color = ColorPrimary;
			style.ColorPicker.InputSpinner.InputPlaceholder.Color = ColorPrimary;
			style.ColorPicker.HexInputText.Color = ColorPrimary;
			style.ColorPicker.HexInputPlaceholder.Color = ColorPrimary;
			style.RangeSliderHorizontal.Fill.Color = ColorPrimary;
			style.RangeSliderVertical.Fill.Color = ColorPrimary;
			style.Spinner.InputText.Color = ColorPrimary;
			style.Switch.MarkDefault.Color = ColorPrimary;
			style.Switch.MarkOnColor = ColorPrimary;
			style.Time.AMPMText.Color = ColorPrimary;
			style.AudioPlayer.Progress.Fill.Color = ColorPrimary;
			style.ProgressbarDeterminate.FullBarText.Color = ColorPrimary;
			style.Tooltip.Text.Color = ColorPrimary;
			style.ButtonClose.Text.Color = ColorPrimary;
			style.Text.Color = ColorSecondary;
			style.Button.Text.Color = ColorPrimary;
			style.Slider.Fill.Color = ColorPrimary;
			style.Toggle.Label.Color = ColorPrimary;

			style.Collections.HighlightedBackgroundColor = ColorHighlightedBackground;

			style.Collections.SelectedBackgroundColor = ColorSelectedBackground;

			style.Collections.DisabledColor = ColorDisabled;

			style.Table.Background.Color = ColorBackground;
			style.Canvas.Background.Color = ColorBackground;

			style.Table.HeaderText.Color = ColorSecondary;
			style.Accordion.ToggleText.Color = ColorSecondary;
			style.Accordion.ContentText.Color = ColorSecondary;
			style.Autocomplete.InputField.Color = ColorSecondary;
			style.Autocomplete.Placeholder.Color = ColorSecondary;
			style.ColorPicker.InputChannelLabel.Color = ColorSecondary;
			style.Time.InputText.Color = ColorSecondary;
			style.AudioPlayer.Play.Text.Color = ColorSecondary;
			style.AudioPlayer.Pause.Text.Color = ColorSecondary;
			style.AudioPlayer.Stop.Text.Color = ColorSecondary;
			style.AudioPlayer.Toggle.Text.Color = ColorSecondary;
			style.ProgressbarDeterminate.EmptyBarText.Color = ColorSecondary;
			style.Paginator.DefaultText.Color = ColorSecondary;
			style.Paginator.ActiveText.Color = ColorSecondary;
			style.InputField.Text.Color = ColorSecondary;
			style.Dropdown.Label.Color = ColorSecondary;
			style.Dropdown.ItemLabel.Color = ColorSecondary;

			style.Calendar.ColorOtherMonth = ColorDisabled;

			style.Spinner.InputPlaceholder.Color = ColorDisabled;

			style.Switch.MarkOffColor = ColorDisabled;
			style.InputField.Placeholder.Color = ColorDisabled;
		}

		/// <summary>
		/// Set style sprites.
		/// </summary>
		/// <param name="style">Style.</param>
		public void SetSprites(Style style)
		{
			style.Calendar.SelectedDayBackground = BackgroundHightlighted.Sprite;
		}

		/// <summary>
		/// Set style fonts.
		/// </summary>
		/// <param name="style">Style.</param>
		public void SetFonts(Style style)
		{
			style.Collections.DefaultItemText.Font = Font;
			style.Collections.DefaultItemText.FontTMPro = FontTMPro;
			style.Combobox.SingleDefaultItemText.Font = Font;
			style.Combobox.SingleDefaultItemText.FontTMPro = FontTMPro;
			style.Combobox.MultipleDefaultItemText.Font = Font;
			style.Combobox.MultipleDefaultItemText.FontTMPro = FontTMPro;
			style.Combobox.Input.Font = Font;
			style.Combobox.Input.FontTMPro = FontTMPro;
			style.Combobox.Placeholder.Font = Font;
			style.Combobox.Placeholder.FontTMPro = FontTMPro;
			style.Combobox.RemoveText.Font = Font;
			style.Combobox.RemoveText.FontTMPro = FontTMPro;
			style.Table.HeaderText.Font = Font;
			style.Table.HeaderText.FontTMPro = FontTMPro;
			style.FileListView.PathItemText.Font = Font;
			style.FileListView.PathItemText.FontTMPro = FontTMPro;
			style.IOCollectionsErrors.Font = Font;
			style.IOCollectionsErrors.FontTMPro = FontTMPro;
			style.Accordion.ToggleText.Font = Font;
			style.Accordion.ToggleText.FontTMPro = FontTMPro;
			style.Accordion.ContentText.Font = Font;
			style.Accordion.ContentText.FontTMPro = FontTMPro;
			style.TabsTop.DefaultButton.Text.Font = Font;
			style.TabsTop.DefaultButton.Text.FontTMPro = FontTMPro;
			style.TabsTop.ActiveButton.Text.Font = Font;
			style.TabsTop.ActiveButton.Text.FontTMPro = FontTMPro;
			style.TabsLeft.DefaultButton.Text.Font = Font;
			style.TabsLeft.DefaultButton.Text.FontTMPro = FontTMPro;
			style.TabsLeft.ActiveButton.Text.Font = Font;
			style.TabsLeft.ActiveButton.Text.FontTMPro = FontTMPro;
			style.Dialog.Title.Font = Font;
			style.Dialog.Title.FontTMPro = FontTMPro;
			style.Dialog.ContentText.Font = Font;
			style.Dialog.ContentText.FontTMPro = FontTMPro;
			style.Dialog.Button.Text.Font = Font;
			style.Dialog.Button.Text.FontTMPro = FontTMPro;
			style.Notify.Text.Font = Font;
			style.Notify.Text.FontTMPro = FontTMPro;
			style.Autocomplete.InputField.Font = Font;
			style.Autocomplete.InputField.FontTMPro = FontTMPro;
			style.Autocomplete.Placeholder.Font = Font;
			style.Autocomplete.Placeholder.FontTMPro = FontTMPro;
			style.ButtonBig.Text.Font = Font;
			style.ButtonBig.Text.FontTMPro = FontTMPro;
			style.ButtonSmall.Text.Font = Font;
			style.ButtonSmall.Text.FontTMPro = FontTMPro;
			style.Calendar.CurrentDate.Font = Font;
			style.Calendar.CurrentDate.FontTMPro = FontTMPro;
			style.Calendar.CurrentMonth.Font = Font;
			style.Calendar.CurrentMonth.FontTMPro = FontTMPro;
			style.Calendar.DayOfWeekText.Font = Font;
			style.Calendar.DayOfWeekText.FontTMPro = FontTMPro;
			style.Calendar.DayText.Font = Font;
			style.Calendar.DayText.FontTMPro = FontTMPro;
			style.ColorPicker.PaletteToggle.Text.Font = Font;
			style.ColorPicker.PaletteToggle.Text.FontTMPro = FontTMPro;
			style.ColorPicker.InputToggle.Text.Font = Font;
			style.ColorPicker.InputToggle.Text.FontTMPro = FontTMPro;
			style.ColorPicker.InputChannelLabel.Font = Font;
			style.ColorPicker.InputChannelLabel.FontTMPro = FontTMPro;
			style.ColorPicker.InputSpinner.InputText.Font = Font;
			style.ColorPicker.InputSpinner.InputText.FontTMPro = FontTMPro;
			style.ColorPicker.InputSpinner.InputPlaceholder.Font = Font;
			style.ColorPicker.InputSpinner.InputPlaceholder.FontTMPro = FontTMPro;
			style.ColorPicker.HexInputText.Font = Font;
			style.ColorPicker.HexInputText.FontTMPro = FontTMPro;
			style.ColorPicker.HexInputPlaceholder.Font = Font;
			style.ColorPicker.HexInputPlaceholder.FontTMPro = FontTMPro;
			style.Spinner.InputText.Font = Font;
			style.Spinner.InputText.FontTMPro = FontTMPro;
			style.Spinner.InputPlaceholder.Font = Font;
			style.Spinner.InputPlaceholder.FontTMPro = FontTMPro;
			style.Time.InputText.Font = Font;
			style.Time.InputText.FontTMPro = FontTMPro;
			style.Time.AMPMText.Font = Font;
			style.Time.AMPMText.FontTMPro = FontTMPro;
			style.AudioPlayer.Play.Text.Font = Font;
			style.AudioPlayer.Play.Text.FontTMPro = FontTMPro;
			style.AudioPlayer.Pause.Text.Font = Font;
			style.AudioPlayer.Pause.Text.FontTMPro = FontTMPro;
			style.AudioPlayer.Stop.Text.Font = Font;
			style.AudioPlayer.Stop.Text.FontTMPro = FontTMPro;
			style.AudioPlayer.Toggle.Text.Font = Font;
			style.AudioPlayer.Toggle.Text.FontTMPro = FontTMPro;
			style.ProgressbarDeterminate.EmptyBarText.Font = Font;
			style.ProgressbarDeterminate.EmptyBarText.FontTMPro = FontTMPro;
			style.ProgressbarDeterminate.FullBarText.Font = Font;
			style.ProgressbarDeterminate.FullBarText.FontTMPro = FontTMPro;
			style.Tooltip.Text.Font = Font;
			style.Tooltip.Text.FontTMPro = FontTMPro;
			style.Paginator.DefaultText.Font = Font;
			style.Paginator.DefaultText.FontTMPro = FontTMPro;
			style.Paginator.ActiveText.Font = Font;
			style.Paginator.ActiveText.FontTMPro = FontTMPro;
			style.ButtonClose.Text.Font = Font;
			style.ButtonClose.Text.FontTMPro = FontTMPro;
			style.Text.Font = Font;
			style.Text.FontTMPro = FontTMPro;
			style.InputField.Text.Font = Font;
			style.InputField.Text.FontTMPro = FontTMPro;
			style.InputField.Placeholder.Font = Font;
			style.InputField.Placeholder.FontTMPro = FontTMPro;
			style.Button.Text.Font = Font;
			style.Button.Text.FontTMPro = FontTMPro;
			style.Toggle.Label.Font = Font;
			style.Toggle.Label.FontTMPro = FontTMPro;
			style.Dropdown.Label.Font = Font;
			style.Dropdown.Label.FontTMPro = FontTMPro;
			style.Dropdown.ItemLabel.Font = Font;
			style.Dropdown.ItemLabel.FontTMPro = FontTMPro;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Sets the default values.
		/// </summary>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			BackgroundHightlighted.SetDefaultValues();

			CollectionsItemBackground.SetDefaultValues();
			Arrow.SetDefaultValues();

			Button.SetDefaultValues();
			ButtonClose.SetDefaultValues();
			ButtonPause.SetDefaultValues();
			ButtonArrowDown.SetDefaultValues();
			ButtonArrowUp.SetDefaultValues();

			ScrollbarHorizontal.SetDefaultValues();
			ScrollbarVertical.SetDefaultValues();

			ProgressbarDeterminate.SetDefaultValues();
			ProgressbarIndeterminate.SetDefaultValues();

			TabContentBackground.SetDefaultValues();
			TabTopInactive.SetDefaultValues();
			TabTopActive.SetDefaultValues();
			TabLeftInactive.SetDefaultValues();
			TabLeftActive.SetDefaultValues();

			DialogDelimiter.SetDefaultValues();
			TooltipBackground.SetDefaultValues();
			Checkmark.SetDefaultValues();

			if (Font == null)
			{
				Font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			}

			if (FontTMPro == null)
			{
#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
				FontTMPro = Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
#elif UIWIDGETS_TMPRO_SUPPORT
				FontTMPro = Resources.Load<TMPro.TextMeshProFont>("Fonts & Materials/ARIAL SDF");
#else
				FontTMPro = Resources.Load<ScriptableObject>("Fonts & Materials/ARIAL SDF");
#endif
			}
		}
#endif
	}
}