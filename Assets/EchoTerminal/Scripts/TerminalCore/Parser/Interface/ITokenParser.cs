namespace EchoTerminal
{
public interface ITokenParser
{
	string TypeName { get; }
	TokenState Parse(string raw, bool isFinalized);
}
}