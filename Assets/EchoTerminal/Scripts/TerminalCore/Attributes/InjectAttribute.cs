using System;

namespace EchoTerminal
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class InjectAttribute : Attribute
    {
    }
}