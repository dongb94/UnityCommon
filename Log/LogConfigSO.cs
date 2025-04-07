using UnityEngine;

[CreateAssetMenu(fileName = "LogConfigSO", menuName = "Tools/LogConfigSo")]
public class LogConfigSO : ScriptableObject
{
	[SerializeField, Tooltip("txt 파일로그의 출력 여부")]
	private bool _fileLog = false;
	[SerializeField, Tooltip("유니티 콘솔에 로그 출력 여부")]
	private bool _consoleLog = true;

#if UNITY_EDITOR
	[SerializeField]
	private LogLevel CurrentLogLevel;
	[SerializeField]
	private LogTag LogTagDebug = LogTag.All;
#else
	public LogLevel CurrentLogLevel = LogLevel.Error;
#endif
	[SerializeField]
	private LogTag LogTagRelease = LogTag.All;

	public bool IsFileLoggingEnabled => _fileLog;
	public bool IsConsoleLoggingEnabled => _consoleLog;

#if UNITY_EDITOR
	public bool ShouldLog(LogLevel level, LogTag tag) => level >= CurrentLogLevel && (LogTagDebug.HasFlag(tag));
#else
	public bool ShouldLog(LogLevel level, LogTag tag) => level >= CurrentLogLevel && (LogTagRelease.HasFlag(tag));
#endif


	// Play 모드에서 변경사항 감지 및 적용
	private void OnValidate()
	{
		// 플레이 모드일 때만 이벤트 발생시킴
#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlaying)
		{
			// LogManager에게 설정 변경을 알림
			CustomUnityLogger.NotifySettingsChanged();
		}
#endif
	}

}
