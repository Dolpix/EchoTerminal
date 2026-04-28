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
	private readonly Dictionary<Type, Component[]> _instanceCache = new();
	private bool _scanned;

	public IReadOnlyCollection<string> GetCommandNames()
	{
		return _commands.Keys;
	}

	internal IEnumerable<Type> GetMonoTypes()
	{
		return _commands.Values
			.SelectMany(entries => entries)
			.Where(e => e.MonoType != null)
			.Select(e => e.MonoType)
			.Distinct();
	}

	public bool TryGet(string name, out List<CommandEntry> entries)
	{
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

			bool hasTarget = method.GetCustomAttribute<TerminalTargetAttribute>() != null;
			if (hasTarget && monoType == null)
			{
				Debug.LogWarning($"[TerminalTarget] on static method '{type.Name}.{method.Name}' has no effect — target filtering requires an instance command. Attribute ignored.");
				hasTarget = false;
			}
			list.Add(new(method, monoType, hasTarget));
		}
	}
}
}