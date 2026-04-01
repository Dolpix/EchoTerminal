using System.Collections.Generic;
using System.Linq;
using EchoTerminal.TerminalCore;

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

            if (tokens.Count == 0 || tokens[0].State != TokenState.Resolved)
            {
                return;
            }

            var commandName = tokens[0].Raw;
            if (!_registry.TryGet(commandName, out var entries))
            {
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
                    foreach (var instance in _registry.GetInstances(entry.MonoType))
                    {
                        entry.Method.Invoke(instance, args);
                    }
                }

                return;
            }
        }
    }
}