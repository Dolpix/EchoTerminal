using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoTerminal
{
public class CommandRegistry
{
	private readonly Dictionary<string, List<CommandEntry>> _commands = new(StringComparer.OrdinalIgnoreCase);
	private readonly HashSet<string> _disabledCommands = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, HashSet<string>> _tagToCommands = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<Type, Component[]> _instanceCache = new();
	private bool _scanned;

	public IReadOnlyCollection<string> GetCommandNames()
	{
		return _commands.Keys.Where(k => !_disabledCommands.Contains(k)).ToList();
	}

	public IReadOnlyCollection<string> GetAllCommandNames()
	{
		return _commands.Keys;
	}

	public void Enable(string commandName)
	{
		_disabledCommands.Remove(commandName);
	}

	public void Disable(string commandName)
	{
		if (_commands.ContainsKey(commandName))
		{
			_disabledCommands.Add(commandName);
		}
	}

	public bool IsEnabled(string commandName)
	{
		return _commands.ContainsKey(commandName) && !_disabledCommands.Contains(commandName);
	}

	public IReadOnlyCollection<string> GetCommandNamesByTag(string tag)
	{
		return _tagToCommands.TryGetValue(tag, out HashSet<string> names)
			? names
			: Array.Empty<string>();
	}

	public void EnableByTag(string tag)
	{
		foreach (string name in GetCommandNamesByTag(tag))
		{
			Enable(name);
		}
	}

	public void DisableByTag(string tag)
	{
		foreach (string name in GetCommandNamesByTag(tag))
		{
			Disable(name);
		}
	}

	public bool TryGet(string name, out List<CommandEntry> entries)
	{
		if (_disabledCommands.Contains(name))
		{
			entries = null;
			return false;
		}

		return _commands.TryGetValue(name, out entries);
	}

	public Component[] GetInstances(Type monoType)
	{
		if (_instanceCache.TryGetValue(monoType, out Component[] cached))
		{
			return cached;
		}

		var found = (Component[])Object.FindObjectsByType(monoType, FindObjectsSortMode.None);
		_instanceCache[monoType] = found;
		return found;
	}

	public void InvalidateInstanceCache(Type monoType)
	{
		_instanceCache.Remove(monoType);
	}

	public void Scan()
	{
		if (_scanned)
		{
			return;
		}

		_scanned = true;

		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (assembly.IsDynamic || IsTestAssembly(assembly))
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
				if (type == null)
				{
					continue;
				}

				ScanMethods(
					type,
					BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
					null
				);

				if (typeof(MonoBehaviour).IsAssignableFrom(type) && !type.IsAbstract)
				{
					ScanMethods(
						type,
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
						type
					);
				}
			}
		}
	}

	public void RegisterType(Type type)
	{
		ScanMethods(type, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null);

		if (typeof(MonoBehaviour).IsAssignableFrom(type) && !type.IsAbstract)
		{
			ScanMethods(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, type);
		}
	}

	private static bool IsTestAssembly(Assembly assembly)
	{
		return assembly.GetReferencedAssemblies()
					   .Any(r => r.Name.Equals("nunit.framework", StringComparison.OrdinalIgnoreCase));
	}

	private void ScanMethods(Type type, BindingFlags flags, Type monoType)
	{
		foreach (MethodInfo method in type.GetMethods(flags))
		{
			var attr = method.GetCustomAttribute<TerminalCommandAttribute>();
			if (attr == null)
			{
				continue;
			}

			string name = string.IsNullOrEmpty(attr.Name)
				? method.Name
				: attr.Name;

			if (!_commands.TryGetValue(name, out List<CommandEntry> list))
			{
				list = new();
				_commands[name] = list;
			}

			if (!attr.Enabled)
			{
				_disabledCommands.Add(name);
			}

			var tagAttr = method.GetCustomAttribute<TerminalTagAttribute>();
			if (tagAttr != null)
			{
				foreach (string tag in tagAttr.Tags)
				{
					if (!_tagToCommands.TryGetValue(tag, out HashSet<string> tagSet))
					{
						tagSet = new(StringComparer.OrdinalIgnoreCase);
						_tagToCommands[tag] = tagSet;
					}

					tagSet.Add(name);
				}
			}

			bool hasTarget = method.GetCustomAttribute<TerminalTargetAttribute>() != null;
			if (hasTarget && monoType == null)
			{
				Debug.LogWarning(
					$"[TerminalTarget] on static method '{type.Name}.{method.Name}' has no effect — target filtering requires an instance command. Attribute ignored."
				);
				hasTarget = false;
			}

			list.Add(new(method, monoType, hasTarget));
		}
	}

	internal IEnumerable<Type> GetMonoTypes()
	{
		return _commands.Values
						.SelectMany(entries => entries)
						.Where(e => e.MonoType != null)
						.Select(e => e.MonoType)
						.Distinct();
	}
}
}