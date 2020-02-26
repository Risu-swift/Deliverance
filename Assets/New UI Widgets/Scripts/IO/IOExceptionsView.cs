﻿namespace UIWidgets
{
	using System;
	using System.IO;
	using System.Security;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// IOExceptionsView.
	/// Handle IO exceptions - catch exception and display following errors.
	/// </summary>
	public class IOExceptionsView : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Invalid argument error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorArgument;

		/// <summary>
		/// Long path error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorLongPath;

		/// <summary>
		/// Unauthorized access error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorUnauthorizedAccess;

		/// <summary>
		/// Security error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorSecurity;

		/// <summary>
		/// Directory not found error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorDirectoryNotFound;

		/// <summary>
		/// IO error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorIO;

		/// <summary>
		/// Current error message.
		/// </summary>
		protected GameObject currentError;

		/// <summary>
		/// Current error message.
		/// </summary>
		public virtual GameObject CurrentError
		{
			get
			{
				return currentError;
			}

			set
			{
				if (currentError != null)
				{
					currentError.gameObject.SetActive(false);
				}

				currentError = value;

				if (currentError != null)
				{
					currentError.gameObject.SetActive(true);
				}
			}
		}

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

			ErrorArgument.SetActive(false);
			ErrorLongPath.SetActive(false);
			ErrorUnauthorizedAccess.SetActive(false);
			ErrorSecurity.SetActive(false);
			ErrorDirectoryNotFound.SetActive(false);
			ErrorIO.SetActive(false);
		}

		/// <summary>
		/// Execute action and handle catched exceptions if raised.
		/// </summary>
		/// <param name="action">Action.</param>
		public virtual void Execute(Action action)
		{
			Init();
			CurrentError = null;

			try
			{
				action();
			}
			catch (UnauthorizedAccessException)
			{
				// The caller does not have the required permission.
				CurrentError = ErrorUnauthorizedAccess;
			}
			catch (SecurityException)
			{
				// The caller does not have the required permission.
				CurrentError = ErrorSecurity;
			}
			catch (DirectoryNotFoundException)
			{
				// The path encapsulated in the DirectoryInfo object is invalid, such as being on an unmapped drive.
				CurrentError = ErrorDirectoryNotFound;
			}
			catch (PathTooLongException)
			{
				// The specified path, file name, or both exceed the system-defined maximum length.
				// For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
				CurrentError = ErrorLongPath;
			}
			catch (IOException)
			{
				// Path is a file name -or- network error has occurred.
				CurrentError = ErrorIO;
			}
			catch (ArgumentNullException)
			{
				// Path is null.
				CurrentError = ErrorArgument;
			}
			catch (ArgumentException)
			{
				// Path is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the GetInvalidPathChars method.
				CurrentError = ErrorArgument;
			}
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(Style style)
		{
			style.IOCollectionsErrors.ApplyTo(ErrorArgument);
			style.IOCollectionsErrors.ApplyTo(ErrorLongPath);
			style.IOCollectionsErrors.ApplyTo(ErrorUnauthorizedAccess);
			style.IOCollectionsErrors.ApplyTo(ErrorSecurity);
			style.IOCollectionsErrors.ApplyTo(ErrorDirectoryNotFound);
			style.IOCollectionsErrors.ApplyTo(ErrorIO);

			return true;
		}
		#endregion
	}
}