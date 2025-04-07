using UnityEngine;
using static LogHelper;

public class TestLogCreator : MonoBehaviour
{
	private void Start()
	{
		RunAllLogTests(this);
	}

	public static void RunAllLogTests(UnityEngine.Object context = null)
	{
		foreach (LogLevel level in System.Enum.GetValues(typeof(LogLevel)))
		{
			foreach (LogTag tag in System.Enum.GetValues(typeof(LogTag)))
			{
				string message = $"🔍 Level: {level}, Tag: {tag}";
				Log(level, message, tag, context);
			}
		}
	}
}
