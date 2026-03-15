using System;
using System.Collections.Generic;
using System.Reflection;
using EchoTerminal.Scripts.Test;
using UnityEngine;

namespace EchoTerminal
{
public static class CommandProcessor
{
	private static Dictionary<Type, IParser> _parsers;

	public static IReadOnlyDictionary<Type, IParser> Parsers => GetParsers();

	public static List<CommandParam> GetParams(CommandEntry entry)
	{
		var result = new List<CommandParam>();

		if (!entry.IsStatic)
		{
			result.Add(new("gameObject", typeof(GameObject), true));
		}

		foreach (var p in entry.Method.GetParameters())
		{
			if (p.ParameterType == typeof(Terminal))
			{
				continue;
			}

			result.Add(new(p.Name, p.ParameterType));
		}

		return result;
	}

	public static bool TryParseInput(string input, out string commandName, out string args, out int leadingSpaces)
	{
		var trimmed = input.TrimStart();
		leadingSpaces = input.Length - trimmed.Length;
		commandName = string.Empty;
		args = null;

		if (trimmed.Length == 0)
		{
			return false;
		}

		var space = trimmed.IndexOf(' ');
		commandName = space == -1 ? trimmed : trimmed[..space];
		args = space == -1 ? null : trimmed[(space + 1)..];
		return true;
	}

	internal static bool TryParseToken(string token, Type type, out object result)
	{
		result = null;
		var parsers = GetParsers();

		if (parsers.TryGetValue(type, out var parser))
		{
			return parser.TryParse(token, out result, out _);
		}

		if (type.IsEnum)
		{
			return Enum.TryParse(type, token, true, out result);
		}

		return false;
	}

	private static Dictionary<Type, IParser> GetParsers()
	{
		if (_parsers != null)
		{
			return _parsers;
		}

		_parsers = new();
		Debug.Log("[Command Processor] Loading parsers...]");

		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (assembly.IsDynamic)
			{
				continue;
			}

			Type[] types;
			try
			{
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				types = e.Types;
			}

			if (types == null)
			{
				continue;
			}

			foreach (var type in types)
			{
				if (type == null || type.IsAbstract || type.IsInterface)
				{
					continue;
				}

				if (!typeof(IParser).IsAssignableFrom(type))
				{
					continue;
				}

				if (type.GetConstructor(Type.EmptyTypes) == null)
				{
					continue;
				}

				var parser = (IParser)Activator.CreateInstance(type);
				_parsers[parser.TargetType] = parser;
			}
		}

		return _parsers;
	}
}
}