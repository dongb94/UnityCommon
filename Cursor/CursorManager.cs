using System.Collections.Generic;
using UnityEngine;

//----------------------------------
//         CursorManager
//----------------------------------
public class CursorManager : Singleton<CursorManager>
{
	#region Variables

	[SerializeField]
	private CursorTexturesSO cursorTexturesSO;

	private Dictionary<CursorType, AnimationCursor> _cursors;
	private Dictionary<CursorType, AnimationCursor> Cursors
	{
		get
		{
			if (_cursors == null)
			{
				if (cursorTexturesSO == null) return null;

				var length = cursorTexturesSO.cursors.Length;
				_cursors = new Dictionary<CursorType, AnimationCursor>();
				for (var i = 0; i < length; i++)
				{
					var cursor = cursorTexturesSO.cursors[i];
					var animationCursor = new GameObject(cursor.type.ToString()).AddComponent<AnimationCursor>();
					animationCursor.transform.SetParent(this.transform);
					animationCursor.Initialize(cursor);
					animationCursor.Disable();
					_cursors.Add(cursorTexturesSO.cursors[i].type, animationCursor);
				}
			}

			return _cursors;
		}
	}

	private bool _isInitialized = false;
	private AnimationCursor _defaultCursor;
	private AnimationCursor _currentCursor;

	#endregion Variables

	#region Unity Methods

	private void OnEnable()
	{
		if (!_isInitialized)
		{
			Initialize();
		}
		ResetCursor();
	}

	private void OnDisable()
	{
		ResetCursor();
	}

	private void OnDestroy()
	{
		CleanupResources();
		ResetCursor();
	}

	#endregion Unity Methods

	#region Static Methods

	public static void ShowCursor() => Cursor.visible = true;
	public static void HideCursor() => Cursor.visible = false;

	#endregion Static Methods

	#region Methods
	private void Initialize()
	{
		_defaultCursor = Cursors.GetValueOrDefault(CursorType.Default);
		_isInitialized = true;
		SetCursor(CursorType.Default);
	}

	public void SetCursor(CursorType type)
	{
		if (!_isInitialized) return;

		if (_currentCursor) _currentCursor.Disable();
		if (Cursors != null && Cursors.TryGetValue(type, out var cursor))
		{
			_currentCursor = cursor;
			cursor.Activate();
		}
		else
		{
			ResetCursor();
		}
	}

	private void ResetCursor()
	{
		if (_defaultCursor != null)
		{
			_defaultCursor.Activate();
		}
		else
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}

	private void CleanupResources()
	{
		// 활성 커서 비활성화
		if (Cursors != null)
		{
			foreach (var cursor in Cursors.Values)
			{
				Destroy(cursor);
			}
		}
		_isInitialized = false;
		_defaultCursor = null;
		_cursors = null;
	}

	#endregion Methods

}