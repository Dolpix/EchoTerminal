using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EchoTerminal.Scripts.TerminalCore.Attributes;
using EchoTerminal.Scripts.TerminalCore.Command;
using EchoTerminal.Scripts.TerminalCore.Targets;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;

namespace EchoTerminal.Scripts.TerminalCore.Suggestions
{
public class SuggestorRegistry
{
	private readonly Dictionary<Type, ISuggester> _suggestors = new();

	public void Scan(CommandRegistry commandRegistry, ITargetProvider targetProvider = null)
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
					else if (type == typeof(TargetSuggester))
					{
						if (targetProvider == null)
						{
							continue;
						}

						instance = new TargetSuggester(targetProvider);
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

	public void InitComplexSuggesters(CommandRegistry commandRegistry, CommandParser commandParser)
	{
		if (_suggestors.TryGetValue(typeof(BoundCommand), out ISuggester bcs) &&
			bcs is BoundCommandSuggester bSuggester)
		{
			bSuggester.Init(commandRegistry, this, commandParser);
		}

		if (_suggestors.TryGetValue(typeof(IList), out ISuggester ls) && ls is ListSuggester listSuggester)
		{
			listSuggester.Init(this);
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