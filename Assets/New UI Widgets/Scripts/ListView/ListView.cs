namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using EasyLayoutNS;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// ListView sources.
	/// </summary>
	public enum ListViewSources
	{
		/// <summary>
		/// Use strings as source for list.
		/// </summary>
		List = 0,

		/// <summary>
		/// Get strings from file, one line per string.
		/// </summary>
		File = 1,
	}

	/// <summary>
	/// List view.
	/// http://ilih.ru/images/unity-assets/UIWidgets/ListView.png
	/// </summary>
	[DataBindSupport]
	public class ListView : ListViewBase, IStylable
	{
		[SerializeField]
		[Obsolete("Use DataSource instead.")]
		List<string> strings = new List<string>();

		/// <summary>
		/// Data source.
		/// </summary>
		protected ObservableList<string> dataSource;

		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		/// <value>The data source.</value>
		[DataBindField]
		public virtual ObservableList<string> DataSource
		{
			get
			{
				if (dataSource == null)
				{
					#pragma warning disable 0618
					dataSource = new ObservableList<string>(strings);
					dataSource.OnChange += UpdateItems;
					strings = null;
					#pragma warning restore 0618
				}

				if (!isListViewInited)
				{
					Init();
				}

				return dataSource;
			}

			set
			{
				Init();

				SetNewItems(value, IsMainThread);

				if (IsMainThread)
				{
					SetScrollValue(0f);
				}
				else
				{
					DataSourceSetted = true;
				}
			}
		}

		/// <summary>
		/// If data source setted?
		/// </summary>
		protected bool DataSourceSetted = false;

		/// <summary>
		/// Is data source changed?
		/// </summary>
		protected bool IsDataSourceChanged = false;

		/// <summary>
		/// Gets the strings.
		/// </summary>
		/// <value>The strings.</value>
		[Obsolete("Use DataSource instead.")]
		public List<string> Strings
		{
			get
			{
				return new List<string>(DataSource);
			}

			set
			{
				DataSource = new ObservableList<string>(value);
			}
		}

		/// <summary>
		/// Gets the strings.
		/// </summary>
		/// <value>The strings.</value>
		[Obsolete("Use DataSource instead.")]
		public new List<string> Items
		{
			get
			{
				return new List<string>(DataSource);
			}

			set
			{
				DataSource = new ObservableList<string>(value);
			}
		}

		[SerializeField]
		TextAsset file;

		/// <summary>
		/// Gets or sets the file with strings for ListView. One string per line.
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
					GetItemsFromFile(file);
					SetScrollValue(0f);
				}
			}
		}

		/// <summary>
		/// The comments in file start with specified strings.
		/// </summary>
		[SerializeField]
		public List<string> CommentsStartWith = new List<string>() { "#", "//" };

		/// <summary>
		/// The source.
		/// </summary>
		[SerializeField]
		public ListViewSources Source = ListViewSources.List;

		/// <summary>
		/// Allow only unique strings.
		/// </summary>
		[SerializeField]
		public bool Unique = true;

		/// <summary>
		/// Allow empty strings.
		/// </summary>
		[SerializeField]
		public bool AllowEmptyItems;

		[SerializeField]
		Color backgroundColor = Color.white;

		[SerializeField]
		Color textColor = Color.black;

		/// <summary>
		/// Default background color.
		/// </summary>
		public Color BackgroundColor
		{
			get
			{
				return backgroundColor;
			}

			set
			{
				backgroundColor = value;
				ComponentsColoring(true);
			}
		}

		/// <summary>
		/// Default text color.
		/// </summary>
		public Color TextColor
		{
			get
			{
				return textColor;
			}

			set
			{
				textColor = value;
				ComponentsColoring(true);
			}
		}

		/// <summary>
		/// Color of background on pointer over.
		/// </summary>
		[SerializeField]
		public Color HighlightedBackgroundColor = new Color(203, 230, 244, 255);

		/// <summary>
		/// Color of text on pointer text.
		/// </summary>
		[SerializeField]
		public Color HighlightedTextColor = Color.black;

		[SerializeField]
		Color selectedBackgroundColor = new Color(53, 83, 227, 255);

		[SerializeField]
		Color selectedTextColor = Color.black;

		/// <summary>
		/// Background color of selected item.
		/// </summary>
		public Color SelectedBackgroundColor
		{
			get
			{
				return selectedBackgroundColor;
			}

			set
			{
				selectedBackgroundColor = value;
				ComponentsColoring(true);
			}
		}

		/// <summary>
		/// Text color of selected item.
		/// </summary>
		public Color SelectedTextColor
		{
			get
			{
				return selectedTextColor;
			}

			set
			{
				selectedTextColor = value;
				ComponentsColoring(true);
			}
		}

		/// <summary>
		/// How long a color transition should take.
		/// </summary>
		[SerializeField]
		public float FadeDuration = 0f;

		[SerializeField]
		[FormerlySerializedAs("defaultItem")]
		Image defaultItem;

		/// <summary>
		/// The default item template.
		/// </summary>
		public Image DefaultItem
		{
			get
			{
				return defaultItem;
			}

			set
			{
				SetDefaultItem(value);
			}
		}

		/// <summary>
		/// The components list.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected readonly List<ListViewStringComponent> Components = new List<ListViewStringComponent>();

		/// <summary>
		/// The components cache list.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected readonly List<ListViewStringComponent> ComponentsCache = new List<ListViewStringComponent>();

		/// <summary>
		/// The components displayed indices.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected readonly List<int> ComponentsDisplayedIndices = new List<int>();

		ListViewComponentPool<ListViewStringComponent, string> componentsPool;

		/// <summary>
		/// The components pool.
		/// Constructor with lists needed to avoid lost connections when instantiated copy of the inited ListView.
		/// </summary>
		protected ListViewComponentPool<ListViewStringComponent, string> ComponentsPool
		{
			get
			{
				if (componentsPool == null)
				{
					componentsPool = new ListViewComponentPool<ListViewStringComponent, string>(Components, ComponentsCache, ComponentsDisplayedIndices)
					{
						Owner = this,
						Container = Container,
						CallbackAdd = AddCallback,
						CallbackRemove = RemoveCallback,
						Template = ConvertDefaultItem(),
					};
				}

				return componentsPool;
			}
		}

		/// <summary>
		/// The displayed indices.
		/// </summary>
		[SerializeField]
		protected List<int> DisplayedIndices = new List<int>();

		/// <summary>
		/// Gets the first displayed index.
		/// </summary>
		/// <value>The first displayed index.</value>
		protected int DisplayedIndicesFirst
		{
			get
			{
				return DisplayedIndices.Count > 0 ? DisplayedIndices[0] : -1;
			}
		}

		/// <summary>
		/// Gets the last displayed index.
		/// </summary>
		/// <value>The last displayed index.</value>
		protected int DisplayedIndicesLast
		{
			get
			{
				return DisplayedIndices.Count > 0 ? DisplayedIndices[DisplayedIndices.Count - 1] : -1;
			}
		}

		/// <summary>
		/// Gets the selected item.
		/// </summary>
		/// <value>The selected item.</value>
		[DataBindField]
		public string SelectedItem
		{
			get
			{
				if (SelectedIndex == -1)
				{
					return null;
				}

				return DataSource[SelectedIndex];
			}
		}

		/// <summary>
		/// Gets the selected items.
		/// </summary>
		/// <value>The selected items.</value>
		[DataBindField]
		public List<string> SelectedItems
		{
			get
			{
				return SelectedIndices.Convert<int, string>(x => DataSource[x]);
			}
		}

		/// <summary>
		/// If enabled scroll limited to last item.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ScrollRect.MovementType = Clamped instead.")]
		public bool LimitScrollValue = false;

		/// <summary>
		/// The sort.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Sort")]
		protected bool sort = true;

		/// <summary>
		/// Sort items.
		/// Advice to use DataSource.Comparison instead Sort and SortFunc.
		/// </summary>
		public bool Sort
		{
			get
			{
				return sort;
			}

			set
			{
				sort = value;
				if (Sort && isListViewInited && sortFunc != null)
				{
					UpdateItems();
				}
			}
		}

		/// <summary>
		/// The sort function.
		/// </summary>
		protected Func<IEnumerable<string>, IEnumerable<string>> sortFunc = items => items.OrderBy(x => x);

		/// <summary>
		/// Sort function.
		/// Advice to use DataSource.Comparison instead Sort and SortFunc.
		/// </summary>
		public Func<IEnumerable<string>, IEnumerable<string>> SortFunc
		{
			get
			{
				return sortFunc;
			}

			set
			{
				sortFunc = value;
				if (Sort && isListViewInited && sortFunc != null)
				{
					UpdateItems();
				}
			}
		}

		/// <summary>
		/// OnSelect event.
		/// </summary>
		[DataBindEvent("SelectedItem", "SelectedItems")]
		public ListViewEvent OnSelectString = new ListViewEvent();

		/// <summary>
		/// OnDeselect event.
		/// </summary>
		[DataBindEvent("SelectedItem", "SelectedItems")]
		public ListViewEvent OnDeselectString = new ListViewEvent();

		[SerializeField]
		ScrollRect scrollRect;

		/// <summary>
		/// Gets or sets the ScrollRect.
		/// </summary>
		/// <value>The ScrollRect.</value>
		public ScrollRect ScrollRect
		{
			get
			{
				return scrollRect;
			}

			set
			{
				if (scrollRect != null)
				{
					var r = scrollRect.GetComponent<ResizeListener>();
					if (r != null)
					{
						r.OnResize.RemoveListener(SetNeedResize);
					}

					scrollRect.onValueChanged.RemoveListener(OnScrollUpdate);
				}

				scrollRect = value;

				if (scrollRect != null)
				{
					var resizeListener = Utilites.GetOrAddComponent<ResizeListener>(scrollRect);
					resizeListener.OnResize.AddListener(SetNeedResize);

					scrollRect.onValueChanged.AddListener(OnScrollUpdate);

					UpdateScrollRectSize();
				}
			}
		}

		/// <summary>
		/// The size of the DefaultItem.
		/// </summary>
		protected Vector2 ItemSize;

		/// <summary>
		/// The size of the ScrollRect.
		/// </summary>
		protected Vector2 ScrollRectSize;

		/// <summary>
		/// Count of visible items.
		/// </summary>
		protected int maxVisibleItems;

		/// <summary>
		/// Count of visible items.
		/// </summary>
		protected int visibleItems;

		/// <summary>
		/// Count of hidden items by top filler.
		/// </summary>
		protected int topHiddenItems;

		/// <summary>
		/// Count of hidden items by bottom filler.
		/// </summary>
		protected int bottomHiddenItems;

		/// <summary>
		/// Set content size fitter settings?
		/// </summary>
		[SerializeField]
		protected bool setContentSizeFitter = true;

		/// <summary>
		/// The set ContentSizeFitter parametres according direction.
		/// </summary>
		public bool SetContentSizeFitter
		{
			get
			{
				return setContentSizeFitter;
			}

			set
			{
				setContentSizeFitter = value;
				if (LayoutBridge != null)
				{
					LayoutBridge.UpdateContentSizeFitter = value;
				}
			}
		}

		/// <summary>
		/// The direction.
		/// </summary>
		[SerializeField]
		protected ListViewDirection direction = ListViewDirection.Vertical;

		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		/// <value>The direction.</value>
		public ListViewDirection Direction
		{
			get
			{
				return direction;
			}

			set
			{
				direction = value;

				(Container as RectTransform).anchoredPosition = Vector2.zero;
				if (scrollRect)
				{
					scrollRect.horizontal = IsHorizontal();
					scrollRect.vertical = !IsHorizontal();
				}

				if (CanOptimize() && (layout is EasyLayout))
				{
					LayoutBridge.IsHorizontal = IsHorizontal();

					CalculateMaxVisibleItems();
				}

				UpdateView();
			}
		}

		[NonSerialized]
		bool isListViewInited = false;

		LayoutGroup layout;

		/// <summary>
		/// Gets the layout.
		/// </summary>
		/// <value>The layout.</value>
		public EasyLayout Layout
		{
			get
			{
				return layout as EasyLayout;
			}
		}

		/// <summary>
		/// LayoutBridge.
		/// </summary>
		ILayoutBridge layoutBridge;

		/// <summary>
		/// LayoutBridge.
		/// </summary>
		protected ILayoutBridge LayoutBridge
		{
			get
			{
				if ((layoutBridge == null) && CanOptimize())
				{
					if (layout == null)
					{
						layout = Container.GetComponent<LayoutGroup>();
					}

					if (layout is EasyLayout)
					{
						layoutBridge = new EasyLayoutBridge(layout as EasyLayout, DefaultItem.transform as RectTransform, setContentSizeFitter);
						layoutBridge.IsHorizontal = IsHorizontal();
					}
					else if (layout is HorizontalOrVerticalLayoutGroup)
					{
						layoutBridge = new StandardLayoutBridge(layout as HorizontalOrVerticalLayoutGroup, DefaultItem.transform as RectTransform, setContentSizeFitter);
					}
				}

				return layoutBridge;
			}
		}

		HashSet<string> SelectedItemsCache = new HashSet<string>();

		/// <summary>
		/// The main thread.
		/// </summary>
		protected Thread MainThread;

		/// <summary>
		/// Gets a value indicating whether this instance is executed in main thread.
		/// </summary>
		/// <value><c>true</c> if this instance is executed in main thread; otherwise, <c>false</c>.</value>
		protected bool IsMainThread
		{
			get
			{
				return MainThread != null && MainThread.Equals(Thread.CurrentThread);
			}
		}

		/// <summary>
		/// Center the list items if all items visible.
		/// </summary>
		[SerializeField]
		[Tooltip("Center the items if all items visible.")]
		protected bool centerTheItems;

		/// <summary>
		/// Center the list items if all items visible.
		/// </summary>
		public virtual bool CenterTheItems
		{
			get
			{
				return centerTheItems;
			}

			set
			{
				centerTheItems = value;
				if (isListViewInited)
				{
					UpdateView();
				}
			}
		}

		/// <summary>
		/// List should be looped.
		/// </summary>
		[SerializeField]
		protected bool loopedList;

		/// <summary>
		/// List can be looped.
		/// </summary>
		/// <value><c>true</c> if list can be looped; otherwise, <c>false</c>.</value>
		public virtual bool LoopedList
		{
			get
			{
				return loopedList;
			}

			set
			{
				loopedList = value;
			}
		}

		/// <summary>
		/// List can be looped and items is enough to make looped list.
		/// </summary>
		/// <value><c>true</c> if looped list available; otherwise, <c>false</c>.</value>
		public virtual bool LoopedListAvailable
		{
			get
			{
				return LoopedList && CanOptimize() && (GetScrollSize() < ListSize());
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isListViewInited)
			{
				return;
			}

			isListViewInited = true;

			MainThread = Thread.CurrentThread;

			base.Init();
			base.Items = new List<ListViewItem>();

			SelectedItemsCache.Clear();
			SelectedIndices.Convert<int, string>(GetDataItem).ForEach(x => SelectedItemsCache.Add(x));

			SetItemIndices = false;

			DestroyGameObjects = false;

			DefaultItem.gameObject.SetActive(true);

			if (CanOptimize())
			{
				ScrollRect = scrollRect;

				CalculateItemSize();
				CalculateMaxVisibleItems();
			}

			SetContentSizeFitter = setContentSizeFitter;

			DefaultItem.gameObject.SetActive(false);

			UpdateItems();
		}

		/// <summary>
		/// Converts the default item.
		/// </summary>
		/// <returns>The default item.</returns>
		protected ListViewStringComponent ConvertDefaultItem()
		{
			var component = Utilites.GetOrAddComponent<ListViewStringComponent>(defaultItem);
			if (component.Text == null)
			{
				component.Text = defaultItem.GetComponentInChildren<Text>();
			}

			return component;
		}

		/// <summary>
		/// Sets the default item.
		/// </summary>
		/// <param name="newDefaultItem">New default item.</param>
		protected virtual void SetDefaultItem(Image newDefaultItem)
		{
			if (newDefaultItem == null)
			{
				throw new ArgumentNullException("newDefaultItem");
			}

			defaultItem = newDefaultItem;

			if (!isListViewInited)
			{
				return;
			}

			defaultItem.gameObject.SetActive(true);
			CalculateItemSize();

			ComponentsPool.Template = ConvertDefaultItem();

			CalculateMaxVisibleItems();

			UpdateView();

			if (scrollRect != null)
			{
				var resizeListener = scrollRect.GetComponent<ResizeListener>();
				if (resizeListener != null)
				{
					resizeListener.OnResize.Invoke();
				}
			}
		}

		/// <summary>
		/// Destroy the component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected static void DestroyComponent(UnityEngine.Component component)
		{
			Destroy(component.gameObject);
		}

		/// <summary>
		/// Calculates the size of the item.
		/// </summary>
		protected virtual void CalculateItemSize()
		{
			if (LayoutBridge == null)
			{
				return;
			}

			ItemSize = LayoutBridge.GetItemSize();
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="index">Index.</param>
		protected string GetDataItem(int index)
		{
			return DataSource[index];
		}

		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		public override bool IsHorizontal()
		{
			return direction == ListViewDirection.Horizontal;
		}

		/// <summary>
		/// Gets the default height of the item.
		/// </summary>
		/// <returns>The default item height.</returns>
		public override float GetDefaultItemHeight()
		{
			return ItemSize.y;
		}

		/// <summary>
		/// Gets the default width of the item.
		/// </summary>
		/// <returns>The default item width.</returns>
		public override float GetDefaultItemWidth()
		{
			return ItemSize.x;
		}

		/// <summary>
		/// Gets the spacing between items. Not implemented for ListViewBase.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public override float GetItemSpacing()
		{
			return LayoutBridge.GetSpacing();
		}

		/// <summary>
		/// Calculates the max count of visible items.
		/// </summary>
		protected virtual void CalculateMaxVisibleItems()
		{
			if (IsHorizontal())
			{
				maxVisibleItems = Mathf.CeilToInt(ScrollRectSize.x / ItemSize.x) + 1;
			}
			else
			{
				maxVisibleItems = Mathf.CeilToInt(ScrollRectSize.y / ItemSize.y) + 1;
			}
		}

		/// <summary>
		/// Handle instance resize.
		/// </summary>
		public virtual void Resize()
		{
			NeedResize = false;

			UpdateScrollRectSize();

			CalculateMaxVisibleItems();
			UpdateView();
		}

		/// <summary>
		/// Update ScrollRect size.
		/// </summary>
		protected void UpdateScrollRectSize()
		{
			ScrollRectSize = (scrollRect.transform as RectTransform).rect.size;
			ScrollRectSize.x = float.IsNaN(ScrollRectSize.x) ? 1f : Mathf.Max(ScrollRectSize.x, 1f);
			ScrollRectSize.y = float.IsNaN(ScrollRectSize.y) ? 1f : Mathf.Max(ScrollRectSize.y, 1f);
		}

		/// <summary>
		/// Determines whether this instance can be optimized.
		/// </summary>
		/// <returns><c>true</c> if this instance can be optimized; otherwise, <c>false</c>.</returns>
		protected bool CanOptimize()
		{
			return scrollRect != null && (layout != null || Container.GetComponent<EasyLayout>() != null);
		}

		/// <summary>
		/// Invokes the select event.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void InvokeSelect(int index)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning("Incorrect index: " + index, this);
			}

			var component = GetComponent(index);
			var item = DataSource[index];

			base.InvokeSelect(index);

			SelectedItemsCache.Add(item);
			OnSelectString.Invoke(index, item);

			SelectColoring(component);
		}

		/// <summary>
		/// Invokes the deselect event.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void InvokeDeselect(int index)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning("Incorrect index: " + index, this);
			}

			var component = GetComponent(index);
			var item = DataSource[index];

			base.InvokeDeselect(index);

			SelectedItemsCache.Remove(item);
			OnDeselectString.Invoke(index, item);

			DefaultColoring(component);
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		public override void UpdateItems()
		{
			if (Source == ListViewSources.List)
			{
				SetNewItems(DataSource, IsMainThread);
				IsDataSourceChanged = !IsMainThread;
			}
			else
			{
				Source = ListViewSources.List;

				GetItemsFromFile(File);
			}
		}

		/// <summary>
		/// Clear strings list.
		/// </summary>
		public override void Clear()
		{
			DataSource.Clear();
		}

		/// <summary>
		/// Gets the items from file.
		/// </summary>
		public void GetItemsFromFile()
		{
			GetItemsFromFile(File);
		}

		/// <summary>
		/// Is comment?
		/// </summary>
		/// <param name="item">Item to check.</param>
		/// <returns>true if item is comment; otherwise false.</returns>
		protected bool IsComment(string item)
		{
			foreach (var comment in CommentsStartWith)
			{
				if (item.StartsWith(comment))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the items from file.
		/// </summary>
		/// <param name="sourceFile">Source file.</param>
		public void GetItemsFromFile(TextAsset sourceFile)
		{
			if (file == null)
			{
				return;
			}

			var data = new ObservableList<string>();

			foreach (var str in sourceFile.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
			{
				var item = str.TrimEnd();

				if (Unique && data.Contains(item))
				{
					continue;
				}

				if (!AllowEmptyItems && string.IsNullOrEmpty(item))
				{
					continue;
				}

				if (IsComment(item))
				{
					continue;
				}

				data.Add(item);
			}

			SetNewItems(data, IsMainThread);
			IsDataSourceChanged = !IsMainThread;
		}

		/// <summary>
		/// Finds the indices of specified item.
		/// </summary>
		/// <returns>The indices.</returns>
		/// <param name="item">Item.</param>
		[Obsolete("Use FindIndices()")]
		public virtual List<int> FindIndicies(string item)
		{
			return FindIndices(item);
		}

		/// <summary>
		/// Finds the indices of specified item.
		/// </summary>
		/// <returns>The indices.</returns>
		/// <param name="item">Item.</param>
		public virtual List<int> FindIndices(string item)
		{
			var result = new List<int>();

			for (int i = 0; i < DataSource.Count; i++)
			{
				if (DataSource[i] == item)
				{
					result.Add(i);
				}
			}

			return result;
		}

		/// <summary>
		/// Finds the index of specified item.
		/// </summary>
		/// <returns>The index.</returns>
		/// <param name="item">Item.</param>
		public virtual int FindIndex(string item)
		{
			return DataSource.IndexOf(item);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(string item)
		{
			var old_indices = (Sort && SortFunc != null) ? FindIndices(item) : null;

			DataSource.Add(item);

			if (Sort && SortFunc != null)
			{
				var new_indices = FindIndices(item);

				foreach (var new_index in new_indices)
				{
					if (!old_indices.Contains(new_index))
					{
						return new_index;
					}
				}

				if (new_indices.Count > 0)
				{
					return new_indices[0];
				}

				return -1;
			}
			else
			{
				return DataSource.Count - 1;
			}
		}

		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed item.</returns>
		public virtual int Remove(string item)
		{
			var index = FindIndex(item);
			if (index == -1)
			{
				return index;
			}

			DataSource.Remove(item);

			return index;
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="component">Component.</param>
		void RemoveCallback(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			component.onPointerEnterItem.RemoveListener(OnPointerEnterCallback);
			component.onPointerExitItem.RemoveListener(OnPointerExitCallback);
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="component">Component.</param>
		void AddCallback(ListViewItem component)
		{
			component.onPointerEnterItem.AddListener(OnPointerEnterCallback);
			component.onPointerExitItem.AddListener(OnPointerExitCallback);
		}

		/// <summary>
		/// Determines if item exists with the specified index.
		/// </summary>
		/// <returns><c>true</c> if item exists with the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public override bool IsValid(int index)
		{
			return (index >= 0) && (index < DataSource.Count);
		}

		/// <summary>
		/// Process the pointer enter callback event.
		/// </summary>
		/// <param name="component">Component.</param>
		void OnPointerEnterCallback(ListViewItem component)
		{
			OnPointerEnterCallback(component as ListViewStringComponent);
		}

		/// <summary>
		/// Process the pointer enter callback event.
		/// </summary>
		/// <param name="component">Component.</param>
		void OnPointerEnterCallback(ListViewStringComponent component)
		{
			if (!IsValid(component.Index))
			{
				var message = string.Format("Index must be between 0 and Items.Count ({0})", DataSource.Count);
				throw new IndexOutOfRangeException(message);
			}

			if (IsSelected(component.Index))
			{
				return;
			}

			HighlightColoring(component);
		}

		/// <summary>
		/// Process the pointer exit callback event.
		/// </summary>
		/// <param name="component">Component.</param>
		void OnPointerExitCallback(ListViewItem component)
		{
			OnPointerExitCallback(component as ListViewStringComponent);
		}

		/// <summary>
		/// Process the pointer exit callback event.
		/// </summary>
		/// <param name="component">Component.</param>
		void OnPointerExitCallback(ListViewStringComponent component)
		{
			if (!IsValid(component.Index))
			{
				var message = string.Format("Index must be between 0 and Items.Count ({0})", DataSource.Count);
				throw new IndexOutOfRangeException(message);
			}

			if (IsSelected(component.Index))
			{
				return;
			}

			DefaultColoring(component);
		}

		/// <summary>
		/// Sets the scroll value.
		/// </summary>
		/// <param name="value">Value.</param>
		protected void SetScrollValue(float value)
		{
			if (scrollRect.content == null)
			{
				return;
			}

			var current_position = scrollRect.content.anchoredPosition;
			var new_position = new Vector2(current_position.x, value);

			const float delta = 0.1f;
			var diff = (IsHorizontal() && Mathf.Abs(current_position.x - new_position.x) != delta)
				|| (!IsHorizontal() && Mathf.Abs(current_position.y - new_position.y) != delta);

			scrollRect.StopMovement();
			if (diff)
			{
				scrollRect.content.anchoredPosition = new_position;
				ScrollUpdate();
			}
		}

		/// <summary>
		/// Gets the scroll value.
		/// </summary>
		/// <returns>The scroll value.</returns>
		protected float GetScrollValue()
		{
			var pos = scrollRect.content.anchoredPosition;
			var result = IsHorizontal() ? -pos.x : pos.y;
			if (LoopedListAvailable)
			{
				return result;
			}

			result = Mathf.Max(0f, result);
			return float.IsNaN(result) ? 0f : result;
		}

		/// <summary>
		/// Gets the size of the item.
		/// </summary>
		/// <returns>The item size.</returns>
		protected float GetItemSize()
		{
			return IsHorizontal()
				? ItemSize.x + LayoutBridge.GetSpacing()
				: ItemSize.y + LayoutBridge.GetSpacing();
		}

		/// <summary>
		/// Gets the size of the scroll.
		/// </summary>
		/// <returns>The scroll size.</returns>
		protected float GetScrollSize()
		{
			return IsHorizontal() ? ScrollRectSize.x : ScrollRectSize.y;
		}

		/// <summary>
		/// Scrolls to item with specifid index.
		/// </summary>
		/// <param name="index">Index.</param>
		public override void ScrollTo(int index)
		{
			if (!CanOptimize())
			{
				return;
			}

			var first_visible = GetFirstVisibleIndex(true);
			var last_visible = GetLastVisibleIndex(true);

			if (first_visible > index)
			{
				SetScrollValue(GetItemPosition(index));
			}
			else if (last_visible < index)
			{
				SetScrollValue(GetItemPositionBottom(index));
			}
		}

		/// <summary>
		/// Gets the item bottom position by index.
		/// </summary>
		/// <returns>The item bottom position.</returns>
		/// <param name="index">Index.</param>
		public virtual float GetItemPositionBottom(int index)
		{
			return GetItemPosition(index) + GetItemSize() + LayoutBridge.GetFullMargin() - GetScrollSize();
		}

		/// <summary>
		/// Gets the last index of the visible.
		/// </summary>
		/// <returns>The last visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected virtual int GetLastVisibleIndex(bool strict = false)
		{
			var window = GetScrollValue() + GetScrollSize();
			var last_visible_index = strict
				? Mathf.FloorToInt(window / GetItemSize())
				: Mathf.CeilToInt(window / GetItemSize());

			return last_visible_index - 1;
		}

		/// <summary>
		/// Gets the first index of the visible.
		/// </summary>
		/// <returns>The first visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected virtual int GetFirstVisibleIndex(bool strict = false)
		{
			var first_visible_index = strict
				? Mathf.CeilToInt(GetScrollValue() / GetItemSize())
				: Mathf.FloorToInt(GetScrollValue() / GetItemSize());
			if (LoopedListAvailable)
			{
				return first_visible_index;
			}

			first_visible_index = Mathf.Max(0, first_visible_index);
			if (strict)
			{
				return first_visible_index;
			}

			return Mathf.Min(first_visible_index, Mathf.Max(0, DataSource.Count - visibleItems));
		}

		/// <summary>
		/// Raises the scroll update event.
		/// </summary>
		/// <param name="position">Position.</param>
		protected virtual void OnScrollUpdate(Vector2 position)
		{
			ScrollUpdate();
		}

		/// <summary>
		/// Refresh the scroll position.
		/// </summary>
		protected virtual void RefreshScrollPosition()
		{
			var scroll = GetScrollValue();
			var list_size = ListSize();
			if (scroll > list_size)
			{
				SetScrollValue(scroll - list_size);
			}
			else if (scroll < 0)
			{
				SetScrollValue(scroll + list_size);
			}
		}

		/// <summary>
		/// Update ListView according scroll position.
		/// </summary>
		protected virtual void ScrollUpdate()
		{
			if (LoopedListAvailable)
			{
				RefreshScrollPosition();
			}

			var oldTopHiddenItems = topHiddenItems;
			CalculateVisibilityData();

			if (oldTopHiddenItems == topHiddenItems)
			{
				return;
			}

			SetDisplayedIndices(topHiddenItems, topHiddenItems + visibleItems, false);
		}

		/// <summary>
		/// Determines whether is sort enabled.
		/// </summary>
		/// <returns><c>true</c> if is sort enabled; otherwise, <c>false</c>.</returns>
		public bool IsSortEnabled()
		{
			if (DataSource.Comparison != null)
			{
				return true;
			}

			return Sort && SortFunc != null;
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest index.</returns>
		/// <param name="eventData">Event data.</param>
		public virtual int GetNearestIndex(PointerEventData eventData)
		{
			Vector2 point;
			var rectTransform = Container as RectTransform;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out point))
			{
				return -1;
			}

			var rect = rectTransform.rect;
			if (!rect.Contains(point))
			{
				return -1;
			}

			return GetNearestIndex(point);
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		/// <param name="point">Point.</param>
		public virtual int GetNearestIndex(Vector2 point)
		{
			if (IsSortEnabled())
			{
				return -1;
			}

			var pos = IsHorizontal() ? point.x : -point.y;
			var index = Mathf.RoundToInt(pos / GetItemSize());

			return Mathf.Min(index, DataSource.Count - 1);
		}

		/// <summary>
		/// Calculates the visibility data.
		/// </summary>
		protected virtual void CalculateVisibilityData()
		{
			if (CanOptimize() && (DataSource.Count > 0))
			{
				visibleItems = Mathf.Min(maxVisibleItems, DataSource.Count);

				topHiddenItems = GetFirstVisibleIndex();
				if (topHiddenItems > (DataSource.Count - 1))
				{
					topHiddenItems = Mathf.Max(0, DataSource.Count - 2);
				}

				bottomHiddenItems = Mathf.Max(0, DataSource.Count - visibleItems - Mathf.Abs(topHiddenItems));
			}
			else
			{
				topHiddenItems = 0;
				bottomHiddenItems = 0;
				visibleItems = DataSource.Count;
			}
		}

		/// <summary>
		/// Convert visible index to item index.
		/// </summary>
		/// <returns>The item index.</returns>
		/// <param name="index">Visible index.</param>
		protected virtual int VisibleIndex2ItemIndex(int index)
		{
			return index % DataSource.Count;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected void UpdateView()
		{
			CalculateVisibilityData();

			SetDisplayedIndices(topHiddenItems, topHiddenItems + visibleItems);
		}

		/// <summary>
		/// Set data to component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void ComponentSetData(ListViewStringComponent component)
		{
			component.SetData(DataSource[component.Index]);
			Coloring(component as ListViewItem);
		}

		/// <summary>
		/// Sets the displayed indices.
		/// </summary>
		/// <param name="startIndex">Start index.</param>
		/// <param name="endIndex">End index.</param>
		/// <param name="isNewData">Is new data?</param>
		protected virtual void SetDisplayedIndices(int startIndex, int endIndex, bool isNewData = true)
		{
			DisplayedIndices.Clear();

			for (int i = startIndex; i < endIndex; i++)
			{
				DisplayedIndices.Add(VisibleIndex2ItemIndex(i));
			}

			if (isNewData)
			{
				ComponentsPool.DisplayedIndicesSet(DisplayedIndices, ComponentSetData);
			}
			else
			{
				ComponentsPool.DisplayedIndicesUpdate(DisplayedIndices, ComponentSetData);
			}

			UpdateLayoutBridge();
		}

		/// <summary>
		/// Updates the layout bridge.
		/// </summary>
		protected virtual void UpdateLayoutBridge()
		{
			if (LayoutBridge == null)
			{
				return;
			}

			if (IsRequiredCenterTheItems())
			{
				var filler = (GetScrollSize() - ListSize() - LayoutBridge.GetFullMargin()) / 2;
				LayoutBridge.SetFiller(filler, 0f);
			}
			else
			{
				LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
			}

			LayoutBridge.UpdateLayout();
		}

		/// <summary>
		/// Validate scroll value.
		/// </summary>
		[Obsolete("Not more used since LimitScrollValue is obsolete.")]
		protected virtual void ValidateScrollValue()
		{
		}

		/// <summary>
		/// Determines whether is required center the list items.
		/// </summary>
		/// <returns><c>true</c> if required center the list items; otherwise, <c>false</c>.</returns>
		protected virtual bool IsRequiredCenterTheItems()
		{
			if (!CenterTheItems)
			{
				return false;
			}

			return GetScrollSize() > ListSize();
		}

		/// <summary>
		/// Get the list size.
		/// </summary>
		/// <returns>The size.</returns>
		protected virtual float ListSize()
		{
			return Mathf.Max(0f, (Mathf.CeilToInt((float)DataSource.Count / (float)GetItemsPerBlock()) * GetItemSize()) - LayoutBridge.GetSpacing());
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		/// <param name="updateView">Update view?</param>
		protected virtual void SetNewItems(ObservableList<string> newItems, bool updateView = true)
		{
			lock (DataSource)
			{
				DataSource.OnChange -= UpdateItems;

				if (Unique)
				{
					newItems.BeginUpdate();

					var unique = newItems.Distinct().ToArray();

					if (unique.Length != newItems.Count)
					{
						newItems.Clear();
						newItems.AddRange(unique);
					}

					newItems.EndUpdate();
				}

				if (Sort && SortFunc != null)
				{
					newItems.BeginUpdate();

					var sorted = new List<string>(SortFunc(newItems));

					newItems.Clear();
					newItems.AddRange(sorted);

					newItems.EndUpdate();
				}

				SilentDeselect(SelectedIndices);

				var new_selected_indices = new List<int>();
				foreach (var selected_item in SelectedItemsCache)
				{
					var new_index = newItems.IndexOf(selected_item);
					if (new_index != -1)
					{
						new_selected_indices.Add(new_index);
					}
				}

				dataSource = newItems;

				SilentSelect(new_selected_indices);
				SelectedItemsCache.Clear();
				SelectedIndices.Convert<int, string>(GetDataItem).ForEach(x => SelectedItemsCache.Add(x));

				if (updateView)
				{
					UpdateView();
				}

				DataSource.OnChange += UpdateItems;
			}
		}

		/// <summary>
		/// Calculates the size of the bottom filler.
		/// </summary>
		/// <returns>The bottom filler size.</returns>
		protected virtual float CalculateBottomFillerSize()
		{
			return Mathf.Max(0, (DataSource.Count - DisplayedIndicesLast - 1) * GetItemSize());
		}

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		protected virtual float CalculateTopFillerSize()
		{
			if (topHiddenItems == 0)
			{
				return 0f;
			}

			var size = topHiddenItems * GetItemSize();
			return LoopedListAvailable ? size : Mathf.Max(0, size);
		}

		/// <summary>
		/// Coloring the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void Coloring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			if (SelectedIndices.Contains(component.Index))
			{
				SelectColoring(component);
			}
			else
			{
				DefaultColoring(component);
			}
		}

		/// <summary>
		/// Updates the colors.
		/// </summary>
		/// <param name="instant">Is should be instant color update?</param>
		public override void ComponentsColoring(bool instant = false)
		{
			if (instant)
			{
				var old_duration = FadeDuration;
				FadeDuration = 0f;
				ComponentsPool.ForEach(x => Coloring(x as ListViewItem));
				FadeDuration = old_duration;
			}
			else
			{
				ComponentsPool.ForEach(x => Coloring(x as ListViewItem));
			}
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public int Set(string item, bool allowDuplicate = true)
		{
			int index;

			if (!allowDuplicate)
			{
				index = DataSource.IndexOf(item);
				if (index == -1)
				{
					index = Add(item);
				}
			}
			else
			{
				index = Add(item);
			}

			Select(index);

			return index;
		}

		/// <summary>
		/// Called when item selected.
		/// Use it for change visible style of selected item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void SelectItem(int index)
		{
			SelectColoring(GetComponent(index));
		}

		/// <summary>
		/// Called when item deselected.
		/// Use it for change visible style of deselected item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void DeselectItem(int index)
		{
			DefaultColoring(GetComponent(index));
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			if (IsSelected(component.Index))
			{
				return;
			}

			HighlightColoring(component as ListViewStringComponent);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void HighlightColoring(ListViewStringComponent component)
		{
			if (component == null)
			{
				return;
			}

			if (IsSelected(component.Index))
			{
				return;
			}

			component.GraphicsColoring(HighlightedTextColor, HighlightedBackgroundColor, FadeDuration);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			SelectColoring(component as ListViewStringComponent);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(ListViewStringComponent component)
		{
			if (component == null)
			{
				return;
			}

			component.GraphicsColoring(selectedTextColor, selectedBackgroundColor, FadeDuration);
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			DefaultColoring(component as ListViewStringComponent);
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(ListViewStringComponent component)
		{
			if (component == null)
			{
				return;
			}

			component.GraphicsColoring(textColor, backgroundColor, FadeDuration);
		}

		/// <summary>
		/// Determines whether item visible.
		/// </summary>
		/// <returns><c>true</c> if item visible; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public bool IsItemVisible(int index)
		{
			return (topHiddenItems <= index) && (index <= (topHiddenItems + visibleItems - 1));
		}

		/// <summary>
		/// Gets the visible indices.
		/// </summary>
		/// <returns>The visible indices.</returns>
		[Obsolete("Use GetVisibleIndices()")]
		public List<int> GetVisibleIndicies()
		{
			return GetVisibleIndices();
		}

		/// <summary>
		/// Gets the visible indices.
		/// </summary>
		/// <returns>The visible indices.</returns>
		public List<int> GetVisibleIndices()
		{
			var result = new List<int>(visibleItems);

			for (int i = topHiddenItems; i < topHiddenItems + visibleItems; i++)
			{
				result.Add(i);
			}

			return result;
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			layout = null;
			layoutBridge = null;

			ScrollRect = null;

			ComponentsPool.Template = null;

			base.OnDestroy();
		}

		/// <summary>
		/// Is need to handle resize event?
		/// </summary>
		protected bool NeedResize;

		/// <summary>
		/// Update this instance.
		/// </summary>
		protected virtual void Update()
		{
			if (DataSourceSetted || IsDataSourceChanged)
			{
				var reset_scroll = DataSourceSetted;

				DataSourceSetted = false;
				IsDataSourceChanged = false;

				lock (DataSource)
				{
					UpdateView();
				}

				if (reset_scroll)
				{
					SetScrollValue(0f);
				}
			}

			if (NeedResize)
			{
				Resize();
			}
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			StartCoroutine(ForceRebuild());

			var old = FadeDuration;
			FadeDuration = 0f;
			ComponentsPool.ForEach(Coloring);
			FadeDuration = old;
		}

		System.Collections.IEnumerator ForceRebuild()
		{
			yield return null;
			ForEachComponent(MarkLayoutForRebuild);
		}

		void MarkLayoutForRebuild(ListViewItem item)
		{
			if (item != null)
			{
				LayoutRebuilder.MarkLayoutForRebuild(item.transform as RectTransform);
			}
		}

		void SetNeedResize()
		{
			if (!CanOptimize())
			{
				return;
			}

			NeedResize = true;
		}

		#region ListViewPaginator support

		/// <summary>
		/// Gets the ScrollRect.
		/// </summary>
		/// <returns>The ScrollRect.</returns>
		public override ScrollRect GetScrollRect()
		{
			return ScrollRect;
		}

		/// <summary>
		/// Gets the items count.
		/// </summary>
		/// <returns>The items count.</returns>
		public override int GetItemsCount()
		{
			return DataSource.Count;
		}

		/// <summary>
		/// Gets the items per block count.
		/// </summary>
		/// <returns>The items per block.</returns>
		public override int GetItemsPerBlock()
		{
			return 1;
		}

		/// <summary>
		/// Gets the item position by index.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPosition(int index)
		{
			return (index * GetItemSize()) - GetItemSpacing();
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		public override int GetNearestItemIndex()
		{
			return Mathf.Clamp(Mathf.RoundToInt(GetScrollValue() / GetItemSize()), 0, DataSource.Count - 1);
		}
		#endregion

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="style">Style data.</param>
		protected virtual void SetStyleDefaultItem(Style style)
		{
			if (defaultItem != null)
			{
				ConvertDefaultItem().SetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);
			}

			if (ComponentsPool != null)
			{
				ComponentsPool.ForEachAll(x => x.SetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style));
			}
		}

		/// <summary>
		/// Sets the style for the colors.
		/// </summary>
		/// <param name="style">Style.</param>
		protected virtual void SetStyleColors(Style style)
		{
			backgroundColor = style.Collections.DefaultBackgroundColor;
			textColor = style.Collections.DefaultColor;
			HighlightedBackgroundColor = style.Collections.HighlightedBackgroundColor;
			HighlightedTextColor = style.Collections.HighlightedColor;
			selectedBackgroundColor = style.Collections.SelectedBackgroundColor;
			selectedTextColor = style.Collections.SelectedColor;

			#if !UNITY_EDITOR
			ComponentsColoring(true);
			#endif
		}

		/// <summary>
		/// Sets the style for the scroll rect.
		/// </summary>
		/// <param name="style">Style.</param>
		protected virtual void SetStyleScrollRect(Style style)
		{
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			var viewport = scrollRect.viewport != null ? scrollRect.viewport : Container.parent;
			#else
			var viewport = Container.parent;
			#endif
			style.Collections.Viewport.ApplyTo(viewport.GetComponent<Image>());

			style.HorizontalScrollbar.ApplyTo(scrollRect.horizontalScrollbar);
			style.VerticalScrollbar.ApplyTo(scrollRect.verticalScrollbar);
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(Style style)
		{
			SetStyleDefaultItem(style);

			SetStyleColors(style);

			SetStyleScrollRect(style);

			style.Collections.MainBackground.ApplyTo(GetComponent<Image>());

			return true;
		}
		#endregion
	}
}