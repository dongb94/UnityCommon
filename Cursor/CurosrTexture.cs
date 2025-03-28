using System;
using UnityEngine;

public enum CursorType
{
	Default,
	Click,
	Pointer,
	Open,
	Grab,
	Text,
	Loading,
	Move,
	NorthSouth,
	EastWest,
	NorthWestSouthEast,
	NorthEastSouthWest,
	Pin,
}

[Serializable]
public class CursorTexture
{
	public Texture2D Texture;
	public CursorType Type;
	[Tooltip("Range [0, 1]")]
	public Vector2 HotSpot;

	public bool SetCursor(CursorMode mode = CursorMode.Auto)
	{
		if (Texture == null) return false;

		var hotspot = new Vector2(Texture.width * HotSpot.x, Texture.height * HotSpot.y);
		Cursor.SetCursor(Texture, hotspot, mode);

		return true;
	}
}