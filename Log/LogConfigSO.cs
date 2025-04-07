using UnityEngine;

[CreateAssetMenu(fileName = "LogConfigSO", menuName = "Tools/LogConfigSo")]
public class LogConfigSO : ScriptableObject
{
	[SerializeField, Tooltip("txt ���Ϸα��� ��� ����")]
	private bool _fileLog = false;
	[SerializeField, Tooltip("����Ƽ �ֿܼ� �α� ��� ����")]
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


	// Play ��忡�� ������� ���� �� ����
	private void OnValidate()
	{
		// �÷��� ����� ���� �̺�Ʈ �߻���Ŵ
#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlaying)
		{
			// LogManager���� ���� ������ �˸�
			CustomUnityLogger.NotifySettingsChanged();
		}
#endif
	}

}
