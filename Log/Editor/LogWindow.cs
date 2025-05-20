using EnhancedHierarchy.Icons;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;
using static LogHelper;

public class CustomLogViewer : EditorWindow
{
	private static readonly float _repaintInterval = 1.0f; // 1초마다 갱신

	private Vector2 _scrollPosition;
	private List<LogEntry> _logs = new();
	private List<string> _searchingCash = new();
	private LogLevel _filterLevel = LogLevel.Debug;
	private LogTag _tag = LogTag.All;

	// 검색 관련 변수
	private string _searchQuery = "";
	private bool _isSearching = false;

	private string _logFilePath;
	private double _lastReadTime;

	[MenuItem("Window/Custom Log Viewer")]
	public static void ShowWindow()
	{
		GetWindow<CustomLogViewer>("Log Viewer");
	}

	private void OnEnable()
	{
		_logFilePath = Application.persistentDataPath + CustomUnityLogger.LOG_FILE_EDITOR_JSON_PATH;
		EditorApplication.update += PollLogFile;
	}

	private void OnDisable()
	{
		EditorApplication.update -= PollLogFile;
	}

	private void PollLogFile()
	{
		if (EditorApplication.timeSinceStartup - _lastReadTime < _repaintInterval)
			return;

		_lastReadTime = EditorApplication.timeSinceStartup;

		// Log(logFilePath, LogTag.Editor, this);
		if (!File.Exists(_logFilePath))
			return;

		string tempPath = _logFilePath + ".bak";
		try
		{
			File.Move(_logFilePath, tempPath);
		}
		catch(IOException) 
		{
			return;
		}

		var lines = File.ReadAllLines(tempPath);
		if (lines.Length == 0)
			return;
		File.Delete(tempPath);
		// Log($"Read Log File [Line{lines.Length}]", LogTag.Editor);

		foreach (var line in lines)
		{
			if (string.IsNullOrWhiteSpace(line))
				continue;

			var entry = JsonUtility.FromJson<LogEntry>(line);
			_logs.Add(entry);
			_searchingCash.Add(entry.message.ToLower());
		}

		Repaint();
	}

	private void OnGUI()
	{
	
		GUIStyle leftAlignedStyle;
		leftAlignedStyle = new GUIStyle(GUI.skin.label);
		leftAlignedStyle.alignment = TextAnchor.MiddleLeft;
		leftAlignedStyle.richText = true;

		GUILayout.Label("Custom Logs", EditorStyles.boldLabel);

		DrawFilterControls();
		DrawSearchBar();

		string lowerQuery = _searchQuery.ToLower();

		float lineHeight = 20f;
		int totalVisibleLogs = 0;

		// 필터 적용된 로그만 추출
		List<LogEntry> filtered = new();
		for (int i = 0; i < _logs.Count; i++)
		{
			var log = _logs[i];
			if (log.level < _filterLevel || !_tag.HasFlag(log.tag))
				continue;

			if (_isSearching && !_searchingCash[i].Contains(lowerQuery))
				continue;

			filtered.Add(_logs[i]);
		}

		totalVisibleLogs = filtered.Count;

		// 스크롤 시작
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

		// 보여질 인덱스 계산
		int totalCount = filtered.Count;
		int startIndex = Mathf.FloorToInt(_scrollPosition.y / lineHeight);
		int visibleCount = Mathf.CeilToInt(position.height / lineHeight);
		int endIndex = Mathf.Min(totalCount, startIndex + visibleCount);

		// 상단 건너뛰기
		GUILayout.Space(startIndex * lineHeight);

		// 실제 렌더링
		for (int i = startIndex; i < endIndex; i++)
		{
			DrawLogEntry(filtered[i], leftAlignedStyle);
		}

		// 하단 공간 확보 (나머지 줄 만큼)
		GUILayout.Space((totalCount - endIndex) * lineHeight);

		GUILayout.EndScrollView();
	}

	private void DrawSearchBar()
	{
		GUILayout.BeginHorizontal(EditorStyles.toolbar);

		// 검색 아이콘 또는 라벨
		GUILayout.Label("🔍Search:", GUILayout.Width(70));

		// 이전 검색어 저장
		string prevSearch = _searchQuery;

		// 검색창 표시
		_searchQuery = GUILayout.TextField(_searchQuery);

		// 검색어가 변경되었는지 확인
		if (_searchQuery != prevSearch)
		{
			_isSearching = !string.IsNullOrEmpty(_searchQuery);
			_scrollPosition = Vector2.zero;
		}

		// 검색창 클리어 버튼
		if (_isSearching && GUILayout.Button("X", GUILayout.Width(20)))
		{
			_searchQuery = "";
			_isSearching = false;
			GUI.FocusControl(null); // 포커스 해제
		}

		GUILayout.EndHorizontal();
	}

	private void DrawHighlightedText(LogLevel level, string message, string highlight, GUIStyle style)
	{
		string fullText = $"[{level}] {message}";
		string loweredText = fullText.ToLower();
		string loweredHighlight = highlight.ToLower();

		int index = loweredText.IndexOf(loweredHighlight);
		if (index >= 0)
		{
			// 재귀 호출을 위한 대안으로 간단하게 구현
			GUILayout.BeginHorizontal();

			string highlighted = fullText.Insert(index + highlight.Length, "</b></color>")
										 .Insert(index, "<color=yellow><b>");
			// Log(highlighted, LogTag.Editor, this);
			GUILayout.Label(highlighted, style);

			GUILayout.EndHorizontal();
		}
		else
		{
			// 검색어가 없으면 일반 텍스트로 표시
			GUILayout.Label(fullText, style);
		}
	}

	private void DrawFilterControls()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Log Level Filter", GUILayout.Width(120));
		_filterLevel = (LogLevel)EditorGUILayout.EnumPopup(_filterLevel);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Log Tag Filter", GUILayout.Width(120));
		_tag = (LogTag)EditorGUILayout.EnumFlagsField(_tag);
		GUILayout.EndHorizontal();
	}

	private void DrawLogEntry(LogEntry log, GUIStyle baseStyle)
	{
		GUIStyle tagStyle = new GUIStyle(GUI.skin.label);
		tagStyle.normal.textColor = ColorUtility.TryParseHtmlString(log.color, out var parsedColor) ? parsedColor : Color.white;
		tagStyle.alignment = TextAnchor.MiddleLeft;

		GUILayout.BeginHorizontal();
		GUILayout.Label($"[{log.time}]", baseStyle, GUILayout.Width(115));
		GUILayout.Label($"[{log.tag}]", tagStyle, GUILayout.Width(70));

		// 검색어 하이라이트 기능 추가
		if (_isSearching && !string.IsNullOrEmpty(_searchQuery))
		{
			DrawHighlightedText(log.level, log.message, _searchQuery, baseStyle);
		}
		else
		{
			GUILayout.Label($"[{log.level}] {log.message}", baseStyle);
		}

		GUILayout.EndHorizontal();
	}
}
