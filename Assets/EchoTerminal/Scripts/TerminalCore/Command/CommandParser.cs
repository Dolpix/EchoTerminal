using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EchoTerminal.Scripts.Test;
using UnityEngine;

namespace EchoTerminal
{
public class CommandParser
{
	private static Dictionary<Type, IParser> _parsers;
	private readonly List<ITokenParser> _tokenParsers;

	public CommandParser(CommandRegistry registry)
	{
		Registry = registry;

		_tokenParsers = new();
		foreach (var kv in Parsers)
		{
			if (kv.Value is ITokenParser tp)
			{
				_tokenParsers.Add(tp);
			}
		}
	}

	public CommandRegistry Registry { get; }

	internal static IReadOnlyDictionary<Type, IParser> Parsers => GetParsers();

	public CommandParseResult Parse(string input)
	{
		var trimmed = input.TrimStart();
		if (trimmed.Length == 0) return CommandParseResult.Empty;

		var tokens = Tokenizer.Tokenize(trimmed, _tokenParsers);
		if (tokens.Count == 0) return CommandParseResult.Empty;

		var commandName = tokens[0].Raw;
		var argTokens = tokens.Count > 1 ? tokens.GetRange(1, tokens.Count - 1) : new List<Token>();

		if (!Registry.TryGet(commandName, out var entries))
		{
			return new(commandName, false, Array.Empty<OverloadResult>());
		}

		var overloads = new List<OverloadResult>(entries.Count);
		foreach (var entry in entries)
		{
			overloads.Add(ParseOverload(entry, argTokens));
		}

		return new(commandName, true, overloads);
	}

	private static OverloadResult ParseOverload(CommandEntry entry, List<Token> argTokens)
	{
		var expectedParams = GetParams(entry);
		var results = new List<ParamResult>(expectedParams.Count);
		var allValid = true;
		var listConsumedAll = false;

		for (var i = 0; i < expectedParams.Count; i++)
		{
			var param = expectedParams[i];

			if (param.Type.IsGenericType && param.Type.GetGenericTypeDefinition() == typeof(List<>))
			{
				var remaining = i < argTokens.Count ? BuildRemaining(argTokens, i) : string.Empty;
				var listResult = ParseListParam(param, remaining);
				results.Add(listResult);
				if (!listResult.IsValid) allValid = false;
				listConsumedAll = true;
				break;
			}

			if (i >= argTokens.Count)
			{
				results.Add(new(param, null, null, false));
				allValid = false;
				continue;
			}

			var token = argTokens[i];
			ParseTokenValue(param, token.Raw, out var value, out var isValid);
			results.Add(new(param, token.Raw, value, isValid));
			if (!isValid) allValid = false;
		}

		var hasOverflow = !listConsumedAll && argTokens.Count > expectedParams.Count;
		return new(entry, results, allValid && !hasOverflow);
	}

	private static string BuildRemaining(List<Token> tokens, int from)
	{
		var parts = new List<string>(tokens.Count - from);
		for (var i = from; i < tokens.Count; i++) parts.Add(tokens[i].Raw);
		return string.Join(" ", parts);
	}

	private static ParamResult ParseListParam(CommandParam param, string remaining)
	{
		if (string.IsNullOrEmpty(remaining)) return new(param, null, null, false);

		var elementType = param.Type.GetGenericArguments()[0];
		var list = (IList)Activator.CreateInstance(param.Type);
		var valid = true;
		var pos = 0;

		while (pos < remaining.Length)
		{
			string slice;
			if (remaining[pos] == '(')
			{
				var close = remaining.IndexOf(')', pos);
				if (close == -1) { valid = false; break; }
				slice = remaining[pos..(close + 1)];
			}
			else
			{
				var comma = remaining.IndexOf(',', pos);
				slice = comma == -1 ? remaining[pos..] : remaining[pos..comma];
			}

			if (!TryParseToken(slice.Trim(), elementType, out var el)) { valid = false; break; }
			list.Add(el);
			pos += slice.Length;
			if (pos < remaining.Length && remaining[pos] == ',') pos++;
			else break;
		}

		return new(param, remaining, valid ? list : null, valid);
	}

	private static void ParseTokenValue(CommandParam param, string raw, out object value, out bool isValid)
	{
		value = null;
		isValid = false;

		if (param.IsTarget)
		{
			if (Parsers.TryGetValue(typeof(GameObject), out var goParser) &&
				goParser.TryParse(raw, out var go, out _))
			{
				value = go;
				isValid = go != null;
			}

			return;
		}

		if (Parsers.TryGetValue(param.Type, out var parser))
		{
			if (parser.TryParse(raw, out value, out _)) isValid = true;
			return;
		}

		if (param.Type.IsEnum) isValid = Enum.TryParse(param.Type, raw, true, out value);
	}

	internal static List<CommandParam> GetParams(CommandEntry entry)
	{
		var result = new List<CommandParam>();

		if (!entry.IsStatic) result.Add(new("gameObject", typeof(GameObject), true));

		foreach (var p in entry.Method.GetParameters())
		{
			if (p.ParameterType != typeof(Terminal)) result.Add(new(p.Name, p.ParameterType));
		}

		return result;
	}

	internal static bool TryParseToken(string token, Type type, out object result)
	{
		result = null;
		if (Parsers.TryGetValue(type, out var parser)) return parser.TryParse(token, out result, out _);
		if (type.IsEnum) return Enum.TryParse(type, token, true, out result);
		return false;
	}

	private static Dictionary<Type, IParser> GetParsers()
	{
		if (_parsers != null) return _parsers;

		_parsers = new();
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (assembly.IsDynamic) continue;

			Type[] types;
			try { types = assembly.GetTypes(); }
			catch (ReflectionTypeLoadException e) { types = e.Types; }
			if (types == null) continue;

			foreach (var type in types)
			{
				if (type == null || type.IsAbstract || type.IsInterface) continue;
				if (!typeof(IParser).IsAssignableFrom(type)) continue;
				if (type.GetConstructor(Type.EmptyTypes) == null) continue;
				var parser = (IParser)Activator.CreateInstance(type);
				_parsers[parser.TargetType] = parser;
			}
		}

		return _parsers;
	}
}
}
