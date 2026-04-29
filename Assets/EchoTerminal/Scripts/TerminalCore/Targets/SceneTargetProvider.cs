using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoTerminal
{
public class SceneTargetProvider : ITargetProvider
{
	private readonly CommandRegistry _registry;
	private IReadOnlyList<string> _cache;
	private int _lastFrameCount = -1;

	public SceneTargetProvider(CommandRegistry registry)
	{
		_registry = registry;
	}

	public IReadOnlyList<string> GetTargets()
	{
		int frame = Time.frameCount;
		if (_cache != null && frame == _lastFrameCount)
		{
			return _cache;
		}

		var targets = new List<string> { "@all" };
		var seen = new HashSet<string>();

		foreach (Type monoType in _registry.GetMonoTypes())
		{
			var stale = false;
			foreach (Component instance in _registry.GetInstances(monoType))
			{
				if (instance == null)
				{
					stale = true;
					continue;
				}

				string name = instance.gameObject.name;
				if (seen.Add(name))
				{
					targets.Add("@" + name);
				}
			}

			if (stale)
			{
				_registry.InvalidateInstanceCache(monoType);
			}
		}

		_lastFrameCount = frame;
		_cache = targets;
		return _cache;
	}
}
}