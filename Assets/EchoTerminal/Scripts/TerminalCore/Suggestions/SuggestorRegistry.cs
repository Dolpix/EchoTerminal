using System;
using System.Collections.Generic;
using System.Reflection;
using EchoTerminal.TerminalCore;

namespace EchoTerminal
{
public class SuggestorRegistry
{
	private readonly Dictionary<Type, ISuggester> _suggestors = new();

	public void Scan(CommandRegistry commandRegistry)
	{
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
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

			foreach (Type type in types)
			{
				if (type == null || type.IsAbstract || type.IsInterface)
				{
					continue;
				}

				if (!typeof(ISuggester).IsAssignableFrom(type))
				{
					continue;
				}

				foreach (SuggestorForAttribute attr in type.GetCustomAttributes<SuggestorForAttribute>())
				{
					ISuggester instance;

					if (type == typeof(CommandNameSuggester))
					{
						instance = new CommandNameSuggester(commandRegistry);
					}
					else if (type.GetConstructor(Type.EmptyTypes) != null)
					{
						instance = (ISuggester)Activator.CreateInstance(type);
					}
					else
					{
						continue;
					}

					_suggestors[attr.TokenType] = instance;
				}
			}
		}
	}

	public bool TryGet(Type tokenType, out ISuggester suggester)
	{
		if (tokenType == null)
		{
			suggester = null;
			return false;
		}

		if (_suggestors.TryGetValue(tokenType, out suggester))
		{
			return true;
		}

		foreach ((Type registeredType, ISuggester s) in _suggestors)
		{
			if (!registeredType.IsAssignableFrom(tokenType))
			{
				continue;
			}

			suggester = s;
			return true;
		}

		return false;
	}
}
}