using System;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal.TerminalCore;
using UnityEngine;

public static class ParserRegistry
{
    private static readonly Dictionary<Type, Func<ITokenParser>> _withArgs = new();

    private static readonly Type[] _parseOrder =
    {
        typeof(CommandName),
        typeof(Vector3),
        typeof(int),
        typeof(float),
        typeof(bool),
        typeof(string)
    };

    public static void Register<T>(Func<ITokenParser> factory) where T : ITokenParser
    {
        _withArgs[typeof(T)] = factory;
    }

    public static Dictionary<Type, ITokenParser> CreateAll()
    {
        var result = new Dictionary<Type, ITokenParser>();
        foreach (var parser in CreateAllParsers())
        {
            result[parser.Type] = parser;
        }

        return result;
    }

    public static List<ITokenParser> CreateAllParsers()
    {
        var unordered = new List<ITokenParser>();

        var types = AppDomain.CurrentDomain.GetAssemblies()
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
                IsTerminalCoreParser(t)
            );

        foreach (var type in types)
        {
            ITokenParser parser = null;

            if (_withArgs.TryGetValue(type, out var factory))
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

        var result = new List<ITokenParser>();
        foreach (var valueType in _parseOrder)
        {
            result.AddRange(unordered.Where(p => p.Type == valueType));
        }

        foreach (var p in unordered.Where(p => !result.Contains(p)))
        {
            result.Add(p);
        }

        return result;
    }

    private static bool IsTerminalCoreParser(Type t)
    {
        return string.IsNullOrEmpty(t.Namespace) || t.Namespace == "EchoTerminal.TerminalCore";
    }
}