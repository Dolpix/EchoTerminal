using System;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
using UnityEngine;

namespace EchoTerminal.Scripts.TerminalCore.Token
{
public static class ParserRegistry
{
	private static readonly Dictionary<Type, Func<ITokenParser>> WithArgs = new();

	private static readonly Type[] TailOrder =
	{
		typeof(Vector3),
		typeof(Vector2),
		typeof(Color),
		typeof(Quaternion),
		typeof(Rect),
		typeof(Vector2Int),
		typeof(Vector3Int),
		typeof(int),
		typeof(long),
		typeof(float),
		typeof(double),
		typeof(char),
		typeof(string)
	};

	public static void Register<T>(Func<ITokenParser> factory) where T : ITokenParser
	{
		WithArgs[typeof(T)] = factory;
	}

	public static List<ITokenParser> CreateAllParsers()
	{
		var unordered = new List<ITokenParser>();

		IEnumerable<Type> types =
			AppDomain.CurrentDomain.GetAssemblies()
					 .SelectMany(a =>
					 {
						 try
						 {
							 return a.GetTypes();
						 }
						 catch
						 {
							 return Array.Empty<Type>();
						 }
					 })
					 .Where(t =>
						 typeof(ITokenParser).IsAssignableFrom(t) &&
						 !t.IsInterface &&
						 !t.IsAbstract &&
						 !t.IsGenericTypeDefinition &&
						 IsTerminalCoreParser(t)
					 );

		foreach (Type type in types)
		{
			ITokenParser parser = null;

			if (WithArgs.TryGetValue(type, out Func<ITokenParser> factory))
			{
				parser = factory();
			}
			else if (type.GetConstructor(Type.EmptyTypes) != null)
			{
				parser = (ITokenParser)Activator.CreateInstance(type);
			}

			if (parser != null)
			{
				unordered.Add(parser);
			}
		}

		List<ITokenParser> elementParsers = unordered.ToList();
		var listParser = new ListParser(elementParsers);
		elementParsers.Add(listParser);
		unordered.Add(listParser);
		List<ITokenParser> result = unordered.Where(p => !TailOrder.Contains(p.Type)).ToList();

		foreach (Type valueType in TailOrder)
		{
			result.AddRange(unordered.Where(p => p.Type == valueType));
		}

		var boundCommandParser = new BoundCommandParser(result);
		result.Add(boundCommandParser);
		elementParsers.Add(boundCommandParser);

		return result;
	}

	private static bool IsTerminalCoreParser(Type t)
	{
		return string.IsNullOrEmpty(t.Namespace) || t.Namespace == "EchoTerminal.TerminalCore";
	}
}
}