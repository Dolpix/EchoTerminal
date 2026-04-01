using System;
using System.Collections.Generic;
using System.Linq;

public static class ParserRegistry
{
    private static readonly Dictionary<Type, Func<ITokenParser>> WithArgs = new();

    public static void Register<T>(Func<ITokenParser> factory) where T : ITokenParser
    {
        WithArgs[typeof(T)] = factory;
    }

    public static Dictionary<Type, ITokenParser> CreateAll()
    {
        var result = new Dictionary<Type, ITokenParser>();

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

            if (WithArgs.TryGetValue(type, out var factory))
            {
                parser = factory();
            }
            else if (type.GetConstructor(Type.EmptyTypes) != null)
            {
                parser = (ITokenParser)Activator.CreateInstance(type);
            }

            if (parser != null)
            {
                result[parser.Type] = parser;
            }
        }

        return result;
    }

    private static bool IsTerminalCoreParser(Type t)
    {
        return string.IsNullOrEmpty(t.Namespace) || t.Namespace == "EchoTerminal.TerminalCore";
    }
}