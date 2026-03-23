public interface ITokenParser
{
	string Name { get; }
	TokenState Parse(string raw, bool isFinalized);
}