
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public enum LogLevel
{
	Debug,
	Info,
	Warning,
	Error,
	Critical
}

[System.Flags]
public enum LogTag
{
	None	= 0, // 실제 사용 X, 모든 요소 해제
	General = 1 << 0,
	Network = 1 << 1,
	UI		= 1 << 2,
	Input	= 1 << 3,
	Audio	= 1 << 4,
	GamePlay= 1 << 5,
	Editor	= 1 << 30,
	All		= ~0 // 실제 사용 X, 모든 요소 선택
}

[System.Serializable]
public struct LogEntry
{
	public LogLevel level;
	public LogTag tag;
	public string time;
	public string message;
	public string color;
}

/// <summary>
/// 유니티엔진 및 C# 스크립트에서 발생하는 로그를 파일, 콘솔, UI콘솔에 출력한다.
/// </summary>
public static class CustomUnityLogger
{
	#region Variables
	public const string LOG_FILE_TXT_PATH = "/Ine_Unity_log.log";
	public const string LOG_FILE_EDITOR_JSON_PATH = "/Ine_Unity_log.json";
	public const string LOG_CONFIG_SO_PATH = "Log/LogConfigSO";
	private static string LogFilePath => Application.persistentDataPath + LOG_FILE_TXT_PATH;
	private static string LogFilePathEditorJsonPath => Application.persistentDataPath + LOG_FILE_EDITOR_JSON_PATH;

	private static bool IsInitialized = false;
	private static LogConfigSO logConfig;
	#endregion Variables

	public static void Initialize()
	{
		logConfig = Resources.Load<LogConfigSO>(LOG_CONFIG_SO_PATH);
		if (logConfig == null)
		{
			Debug.LogWarning("LogSettings not found. Using default settings.");
			logConfig = ScriptableObject.CreateInstance<LogConfigSO>();
		}
		IsInitialized = true;
	}

#if UNITY_EDITOR
	// Play Mode에서 에셋 변화시 다시 로드. 빌드 파일에서는 SO가 동적으로 바뀌지 않으므로 불필요.
	public static void NotifySettingsChanged()
	{
		logConfig = ScriptableObject.CreateInstance<LogConfigSO>();
	}
#endif

	/// <summary>
	/// 로그 출력을 위한 함수
	/// </summary>
	/// <param name="level">로그 레벨</param>
	/// <param name="message">로그 내용</param>
	/// <param name="tag">검색 및 분류를 위한 로그 태그</param>
	/// <param name="context">Editor에서 추적할 Unity Object</param>
	public static void Log(LogLevel level, object message, LogTag tag = LogTag.General, UnityEngine.Object context = null)
	{
		if (!IsInitialized) Initialize();

		if(tag == LogTag.All || tag	== LogTag.None)
			return;

		string time = $"{DateTime.Now:yy.MM.dd HH:mm:ss}";
		string msg = message != null ? message.ToString() : "";
#if UNITY_EDITOR
		string color = GetTagColor(tag);

		// Editor Window용 json 파일 저장
		{   // save json for editor viewer
			var log = new LogEntry
			{
				level = level,
				tag = tag,
				time = time,
				message = msg,
				color = color,
			};

			var json = JsonUtility.ToJson(log) + "\n";
			try
			{
				File.AppendAllText(LogFilePathEditorJsonPath, json);
			}
			catch(IOException) 
			{
				Debug.LogWarning($"Editor Log not appended : {msg}");
			}
		}
#endif

		if (!logConfig.ShouldLog(level, tag))
			return;

#if UNITY_EDITOR
		// 유니티 콘솔 로그 출력
		if(logConfig.IsConsoleLoggingEnabled)
		{
			string tagStrEditor = $"<color={color}><b>[{tag.ToString().PadRight(8)}\t]</b></color>";
			Debug.unityLogger.Log(ToUnityLogType(level), message: $"{tagStrEditor} {msg}", context);
		}
#endif

		string formatted = $"[{time}][{level.ToString().PadRight(8)}\t][{tag.ToString().PadRight(8)}\t] {msg}\n";

		// 포메팅 후 txt파일로 출력
		if (logConfig.IsFileLoggingEnabled)
		{
			File.AppendAllText(LogFilePath, formatted);
		}

		// TODO: 서버 전송
	}


	public static LogType ToUnityLogType(LogLevel level)
	{
		return level switch
		{
			LogLevel.Debug		=> LogType.Log,
			LogLevel.Info		=> LogType.Log,
			LogLevel.Warning	=> LogType.Warning,
			LogLevel.Error		=> LogType.Error,
			LogLevel.Critical	=> LogType.Exception,
			_					=> LogType.Log
		};
	}

	public static string GetTagColor(LogTag tag)
	{
		return tag switch
		{
			LogTag.General	=> "white",
			LogTag.UI		=> "magenta",
			LogTag.Input	=> "green",
			LogTag.Network	=> "cyan",
			LogTag.Editor	=> "teal",
			_				=> "grey"
		};
	}
}
