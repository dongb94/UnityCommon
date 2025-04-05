using System.Collections.Generic;
using UnityEngine;
using static LogHelper;

public class AnimationCursor : MonoBehaviour
{
	#region Variables

	private const string CURSOR_TEXTURE_PATH = "Cursor/";

	private CursorTexture _cursorTexture;
	public Texture2D Texture => _cursorTexture.texture;
	public CursorType Type => _cursorTexture.type;
	public Vector2 HotSpot => _cursorTexture.hotSpot;
	public float FrameDuration => _cursorTexture.frameDuration;

	private Texture2D[] _cursorTextureArray;

	public int FrameCount => _cursorTextureArray?.Length ?? 1;

	private bool _isActive = false;
	public bool IsActive => _isActive;

	// 자체 업데이트를 위한 필드
	private List<CursorTexture> _activeCursors = new List<CursorTexture>();
	private float _lastUpdateTime;
	private int _currentFrameIndex = 0;
	public int CurrentFrameIndex => _currentFrameIndex;

	#endregion Variables

	#region Unity Methods
	public void Awake()
	{
		_lastUpdateTime = 0f;
	}
	public void OnEnable()
	{
		_isActive = true;
		SetFrame(0);
	}

	public void LateUpdate()
	{
		if (!IsActive || !Application.isFocused) return;
		UpdateAnimation(Time.deltaTime);
	}
	private void OnDestroy()
	{
		Disable();

		if (_cursorTextureArray != null)
		{
			foreach (var tex in _cursorTextureArray)
			{
				if (tex != null)
				{
					UnityEngine.Object.Destroy(tex);
				}
			}
			_cursorTextureArray = null;
		}

		_lastUpdateTime = 0f;
		_currentFrameIndex = 0;
	}

	#endregion Unity Methods

	#region Public Methods
	public void Initialize(CursorTexture cursorTexture)
	{
		_cursorTexture = cursorTexture;
		InitializeFrames();
	}

	public bool SetFrame(int frameIndex)
	{
		if (_cursorTextureArray == null || _cursorTextureArray.Length == 0 ||
			frameIndex < 0 || frameIndex >= _cursorTextureArray.Length)
		{
			// 커서 이미지가 없는 경우 시스템 기본 커서 사용
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			return false;
		}

		var hotspot = new Vector2(
			_cursorTextureArray[frameIndex].width * HotSpot.x,
			_cursorTextureArray[frameIndex].height * HotSpot.y
		);

		Cursor.SetCursor(_cursorTextureArray[frameIndex], hotspot, CursorMode.Auto);
		return true;
	}

	public void Activate()
	{
		if (!_isActive)
		{
			_isActive = true;
			gameObject.SetActive(_isActive);
		}
	}

	public void Disable()
	{
		if (_isActive)
		{
			_isActive = false;
			gameObject.SetActive(_isActive);
		}
	}

	#endregion Public Methods

	#region InternalMethods

	// 에니메이션 기반이 되는 멀티플 스프라이트를 로드해, 각 스프라이트를 텍스쳐로 바꿔 저장
	private void InitializeFrames()
	{
		if (!Texture) return;

		Sprite[] sprites = Resources.LoadAll<Sprite>($"{CURSOR_TEXTURE_PATH}{Texture.name}");

		if (sprites == null || sprites.Length == 0)
		{
			LogWarning($"Failed to load sprites for {Texture.name}. Using system default cursor.");
			return;
		}

		_cursorTextureArray = new Texture2D[sprites.Length];

		for (int i = 0; i < sprites.Length; i++)
		{
			Sprite sprite = sprites[i];

			Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
			tex.filterMode = FilterMode.Point;

			Color[] pixels = sprite.texture.GetPixels(
				(int)sprite.textureRect.x,
				(int)sprite.textureRect.y,
				(int)sprite.textureRect.width,
				(int)sprite.textureRect.height
			);

			tex.SetPixels(pixels);
			tex.Apply();

			_cursorTextureArray[i] = tex;
		}
	}

	// 내부 애니메이션 업데이트 로직
	private bool UpdateAnimation(float deltaTime)
	{
		if (_cursorTextureArray == null || _cursorTextureArray.Length == 0 || FrameCount == 1)
		{
			Disable();
			return false;
		}

		_lastUpdateTime += deltaTime;
		if (_lastUpdateTime >= FrameDuration)
		{
			_lastUpdateTime = 0f;
			_currentFrameIndex = (_currentFrameIndex + 1) % FrameCount;
			return SetFrame(_currentFrameIndex);
		}

		return true;
	}
	#endregion InternalMethods
}