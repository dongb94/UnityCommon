using EnhancedHierarchy.Icons;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using static LogHelper;

public class CustomLogViewer : EditorWindow
{
	private Vector2 scrollPosition;
	private List<LogEntry> logs = new();
	private LogLevel filterLevel = LogLevel.Debug;
	private LogTag tag = LogTag.All;

	// 검색 관련 변수
	private string searchQuery = "";
	private bool isSearching = false;

	private string logFilePath;
	private double lastReadTime;

	[MenuItem("Window/Custom Log Viewer")]
	public static void ShowWindow()
	{
		GetWindow<CustomLogViewer>("Log Viewer");
	}

	private void OnEnable()
	{
		LogWarning("Test Log", LogTag.Editor, this);
		logFilePath = Application.persistentDataPath + CustomUnityLogger.LOG_FILE_EDITOR_JSON_PATH;
		EditorApplication.update += PollLogFile;
	}

	private void OnDisable()
	{
		EditorApplication.update -= PollLogFile;
	}

	private void PollLogFile()
	{
		if (EditorApplication.timeSinceStartup - lastReadTime < 1.0)
			return;

		lastReadTime = EditorApplication.timeSinceStartup;

		// Log(logFilePath, LogTag.Editor, this);
		if (!File.Exists(logFilePath))
			return;

		string tempPath = logFilePath + ".bak";
		try
		{
			File.Move(logFilePath, tempPath);
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
			logs.Add(entry);
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

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		foreach (var log in logs)
		{
			if (log.level < filterLevel || !tag.HasFlag(log.tag))
				continue;

			if (isSearching && !string.IsNullOrEmpty(searchQuery) &&
				!log.message.ToLower().Contains(searchQuery.ToLower()))
				continue;

			DrawLogEntry(log, leftAlignedStyle);
		}

		GUILayout.EndScrollView();
	}

	private void DrawSearchBar()
	{
		GUILayout.BeginHorizontal(EditorStyles.toolbar);

		// 검색 아이콘 또는 라벨
		GUILayout.Label("🔍Search:", GUILayout.Width(70));

		// 이전 검색어 저장
		string prevSearch = searchQuery;

		// 검색창 표시
		searchQuery = GUILayout.TextField(searchQuery);

		// 검색어가 변경되었는지 확인
		if (searchQuery != prevSearch)
		{
			isSearching = !string.IsNullOrEmpty(searchQuery);
			scrollPosition = Vector2.zero;
		}

		// 검색창 클리어 버튼
		if (isSearching && GUILayout.Button("X", GUILayout.Width(20)))
		{
			searchQuery = "";
			isSearching = false;
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
		filterLevel = (LogLevel)EditorGUILayout.EnumPopup(filterLevel);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Log Tag Filter", GUILayout.Width(120));
		tag = (LogTag)EditorGUILayout.EnumFlagsField(tag);
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
		if (isSearching && !string.IsNullOrEmpty(searchQuery))
		{
			DrawHighlightedText(log.level, log.message, searchQuery, baseStyle);
		}
		else
		{
			GUILayout.Label($"[{log.level}] {log.message}", baseStyle);
		}

		GUILayout.EndHorizontal();
	}
}