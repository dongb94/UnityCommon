using System.Collections.Generic;
using UnityEngine;

//----------------------------------
//         CursorManager
//----------------------------------
public class CursorManager : Singleton<CursorManager>
{
	#region Variables

	[SerializeField]
	private CursorTexturesSO CursorTexturesSO;

	[Header("Default Cursor")]
	[SerializeField]
	private CursorTexture DefaultCursor;

	private Dictionary<CursorType, CursorTexture> _cursors;
	private Dictionary<CursorType, CursorTexture> Cursors
	{
		get
		{
			if (_cursors == null)
			{
				if (CursorTexturesSO == null) return null;

				var length = CursorTexturesSO.Cursors.Length;
				_cursors = new Dictionary<CursorType, CursorTexture>();
				for (var i = 0; i < length; i++)
				{
					_cursors.Add(CursorTexturesSO.Cursors[i].Type, CursorTexturesSO.Cursors[i]);
				}

				if (DefaultCursor == null && _cursors.TryGetValue(CursorType.Default, out var defaultCursor))
				{
					DefaultCursor = defaultCursor;
				}
			}

			return _cursors;
		}
	}

	#endregion Variables

	#region Unity Methods

	private void OnEnable() => SetCursorToDefault();

	#endregion Unity Methods

	#region Static Methods

	public static void ShowCursor() => Cursor.visible = true;
	public static void HideCursor() => Cursor.visible = false;

	public static void SetCursor(CursorType type, CursorMode mode = CursorMode.Auto) =>
		Instance?.SetCursorTexture(type, mode);
	public static void SetCursorToDefault(CursorMode mode = CursorMode.Auto) =>
		Instance?.SetCursorTexture(CursorType.Default, mode);

	#endregion Static Methods

	#region Methods

	public void SetCursorTexture(CursorType type, CursorMode mode = CursorMode.Auto)
	{
		if (Cursors.TryGetValue(type, out var cursor))
		{
			cursor.SetCursor();
		}
		else
		{
			ResetCursor();
		}
	}

	private void ResetCursor() => DefaultCursor?.SetCursor();

	#endregion Methods

}