using UnityEngine;

public static class LogHelper
{
	public static void Log(LogLevel level, string message, LogTag tag = LogTag.General, Object context = null)
	{
		CustomUnityLogger.Log(level, message, tag, context);
	}

	public static void Log(string message, LogTag tag = LogTag.General, Object context = null)
	{
		CustomUnityLogger.Log(LogLevel.Debug, message, tag, context);
	}
	public static void LogInfo(string message, LogTag tag = LogTag.General, Object context = null)
	{
		CustomUnityLogger.Log(LogLevel.Info, message, tag, context);
	}

	public static void LogWarning(string message, LogTag tag = LogTag.General, Object context = null)
	{
		CustomUnityLogger.Log(LogLevel.Warning, message, tag, context);
	}

	public static void LogError(string message, LogTag tag = LogTag.General, Object context = null)
	{
		CustomUnityLogger.Log(LogLevel.Error, message, tag, context);
	}

	public static void LogCritical(string message, LogTag tag = LogTag.General, Object context = null)
	{
		CustomUnityLogger.Log(LogLevel.Critical, message, tag, context);
	}
}