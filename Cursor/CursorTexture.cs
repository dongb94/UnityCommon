
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
public struct CursorTexture
{
	[SerializeField]
	public Texture2D texture;
	[SerializeField]
	public CursorType type;
	[Tooltip("Range [0, 1]"), SerializeField]
	public Vector2 hotSpot;
	[SerializeField, Range(0.05f, 0.5f), Tooltip("프레임당 시간(sec)")]
	public float frameDuration;
}