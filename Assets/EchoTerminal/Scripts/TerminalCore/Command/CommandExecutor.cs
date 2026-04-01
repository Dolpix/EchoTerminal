using System.Collections.Generic;
using System.Linq;
using EchoTerminal.TerminalCore;
using UnityEngine;

namespace EchoTerminal
{
    public class CommandExecutor
    {
        private readonly CommandRegistry _registry;
        private readonly Tokenizer _tokenizer;

        public CommandExecutor(CommandRegistry registry, Tokenizer tokenizer)
        {
            _registry = registry;
            _tokenizer = tokenizer;
        }

        public void Execute(string input)
        {
            var tokens = _tokenizer.Tokenize(input);

            if (tokens.Count == 0)
            {
                return;
            }

            if (tokens[0].State != TokenState.Resolved)
            {
                Debug.LogError($"Unknown command '{tokens[0].Raw}'.");
                return;
            }

            var commandName = tokens[0].Raw;
            if (!_registry.TryGet(commandName, out var entries))
            {
                Debug.LogError($"Unknown command '{commandName}'.");
                return;
            }

            var argInput = input.TrimStart();
            var spaceAfterCommand = argInput.IndexOf(' ');
            argInput = spaceAfterCommand >= 0 ? argInput[(spaceAfterCommand + 1)..] : string.Empty;

            foreach (var entry in entries)
            {
                var parameters = entry.Method.GetParameters();
                var expectedTypes = parameters.Select(p => p.ParameterType).ToList();

                var argTokens = string.IsNullOrWhiteSpace(argInput)
                    ? new List<Token>()
                    : _tokenizer.Tokenize(argInput, expectedTypes);

                if (argTokens.Count != parameters.Length)
                {
                    continue;
                }

                if (argTokens.Any(t => t.State != TokenState.Resolved))
                {
                    continue;
                }

                var args = argTokens.Select(t => _tokenizer.ParseValue(t)).ToArray();

                if (entry.IsStatic)
                {
                    entry.Method.Invoke(null, args);
                }
                else
                {
                    var instances = _registry.GetInstances(entry.MonoType);
                    if (instances.Length == 0)
                    {
                        Debug.LogError($"No active instance of '{entry.MonoType.Name}' found in the scene.");
                        return;
                    }

                    foreach (var instance in instances)
                    {
                        entry.Method.Invoke(instance, args);
                    }
                }

                return;
            }

            if (entries.Count == 1)
            {
                Debug.LogError($"Invalid arguments for '{commandName}'. Expected: {BuildSignatureHint(entries[0])}");
                return;
            }

            var signatures = string.Join(" | ", entries.Select(BuildSignatureHint));
            Debug.LogError($"Invalid arguments for '{commandName}'. Expected one of: {signatures}");
        }

        private static string BuildSignatureHint(CommandEntry entry)
        {
            var parameters = entry.Method.GetParameters();
            if (parameters.Length == 0)
            {
                return "(no arguments)";
            }

            var paramList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
            return $"({paramList})";
        }
    }
}