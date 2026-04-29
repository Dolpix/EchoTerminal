using System;
using System.Collections.Generic;

namespace EchoTerminal
{
public class InjectionContainer
{
	private readonly Dictionary<Type, Func<object>> _factories = new();

	public void Register<T>(Func<T> factory)
	{
		_factories[typeof(T)] = () => factory();
	}

	public bool TryResolve(Type type, out object value)
	{
		if (_factories.TryGetValue(type, out Func<object> factory))
		{
			value = factory();
			return true;
		}

		value = null;
		return false;
	}
}
}