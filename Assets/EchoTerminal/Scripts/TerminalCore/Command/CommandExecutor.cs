using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EchoTerminal
{
public static class CommandExecutor
{
	public static bool Execute(string input, CommandRegistry registry, IList<ITokenParser> parsers)
	{
		var tokens = Tokenizer.Tokenize(input, parsers);
		if (tokens.Count == 0)
		{
			Debug.LogWarning("[EchoTerminal] Submit ignored: input was empty or whitespace.");
			return false;
		}

		var commandName = tokens[0].Raw;
		if (!registry.TryGet(commandName, out var entries))
		{
			Debug.LogWarning($"[EchoTerminal] Unknown command: \"{commandName}\". Type a registered command name.");
			return false;
		}

		foreach (var entry in entries)
		{
			var parameters = entry.Method.GetParameters();
			if (!AllTokensResolve(tokens, parameters, parsers))
			{
				continue;
			}

			var args = BuildArgs(tokens, parameters, parsers, commandName);
			if (args == null)
			{
				continue;
			}

			if (entry.IsStatic)
			{
				InvokeSafe(entry, null, args, commandName);
			}
			else
			{
				var instances = registry.GetInstances(entry.MonoType);
				if (instances.Length == 0)
				{
					Debug.LogWarning(
						$"[EchoTerminal] Command \"{commandName}\" is defined on {entry.MonoType.Name} but no active instances were found in the scene."
					);
					continue;
				}

				foreach (var instance in instances)
				{
					InvokeSafe(entry, instance, args, commandName);
				}
			}

			return true;
		}

		Debug.LogWarning($"[EchoTerminal] No matching overload found for \"{commandName}\" with the given arguments.");
		return false;
	}

	private static bool AllTokensResolve(List<Token> tokens, ParameterInfo[] parameters, IList<ITokenParser> parsers)
	{
		if (tokens.Count - 1 != parameters.Length)
		{
			return false;
		}

		for (var i = 0; i < parameters.Length; i++)
		{
			var expectedType = parameters[i].ParameterType;
			var matched = parsers.FirstOrDefault(parser => parser.Type == expectedType);

			if (matched == null)
			{
				return false;
			}

			if (matched.ParseTokenState(tokens[i + 1].Raw) != TokenState.Resolved)
			{
				return false;
			}
		}

		return true;
	}

	private static object[] BuildArgs(
		List<Token> tokens,
		ParameterInfo[] parameters,
		IList<ITokenParser> parsers,
		string commandName)
	{
		var args = new object[tokens.Count - 1];
		for (var i = 0; i < args.Length; i++)
		{
			if (i >= parameters.Length)
			{
				break;
			}

			var expectedType = parameters[i].ParameterType;
			var matched = parsers.FirstOrDefault(parser => parser.Type == expectedType);

			if (matched == null)
			{
				Debug.LogError(
					$"[EchoTerminal] No parser found for type \"{expectedType.Name}\" (parameter {i + 1} of \"{commandName}\").");
				return null;
			}

			args[i] = matched.ParseValue(tokens[i + 1].Raw);
		}

		return args;
	}

	private static void InvokeSafe(CommandEntry entry, object target, object[] args, string commandName)
	{
		try
		{
			entry.Method.Invoke(target, args);
		}
		catch (TargetParameterCountException)
		{
			Debug.LogError(
				$"[EchoTerminal] Wrong number of arguments for \"{commandName}\". Expected {entry.Method.GetParameters().Length}, got {args.Length}."
			);
		}
		catch (TargetInvocationException e)
		{
			Debug.LogError(
				$"[EchoTerminal] Command \"{commandName}\" threw an exception: {e.InnerException?.Message}"
			);
		}
		catch (Exception e)
		{
			Debug.LogError(
				$"[EchoTerminal] Failed to invoke \"{commandName}\": {e.Message}"
			);
		}
	}
}
}