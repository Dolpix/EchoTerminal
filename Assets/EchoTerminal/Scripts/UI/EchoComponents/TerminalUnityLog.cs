using UnityEngine;

namespace EchoTerminal.Components
{
public class TerminalUnityLog : IEchoComponent
{
	private readonly Terminal _terminal;

	public TerminalUnityLog(Terminal terminal)
	{
		_terminal = terminal;
		Application.logMessageReceived += OnLogMessageReceived;
	}

	~TerminalUnityLog()
	{
		Application.logMessageReceived -= OnLogMessageReceived;
	}

	private void OnLogMessageReceived(string message, string stackTrace, LogType type)
	{
		(Color color, LogKind kind) = type switch
		{
			LogType.Error     => (new(1f, 0.3f, 0.3f), LogKind.Error),
			LogType.Exception => (new(1f, 0.3f, 0.3f), LogKind.Error),
			LogType.Warning   => (new(1f, 0.9f, 0.3f), LogKind.Warning),
			LogType.Assert    => (new Color(1f, 0.5f, 0.2f), LogKind.Warning),
			_                 => (new Color(0.7f, 0.7f, 0.7f), LogKind.Log)
		};

		string text = type == LogType.Log
			? message
			: BuildDetailedMessage(message, stackTrace);

		_terminal.Log(text, color, kind);
	}

	private static string BuildDetailedMessage(string message, string stackTrace)
	{
		string caller = ParseFirstUserFrame(stackTrace);
		return caller != null ? $"{message}\n  at {caller}" : message;
	}

	private static string ParseFirstUserFrame(string stackTrace)
	{
		if (string.IsNullOrEmpty(stackTrace))
		{
			return null;
		}

		foreach (string rawLine in stackTrace.Split('\n'))
		{
			string line = rawLine.Trim();
			if (string.IsNullOrEmpty(line))
			{
				continue;
			}

			if (line.StartsWith("UnityEngine.") || line.StartsWith("UnityEditor."))
			{
				continue;
			}

			if (!line.Contains('('))
			{
				continue;
			}

			return line;
		}

		return null;
	}
}
}