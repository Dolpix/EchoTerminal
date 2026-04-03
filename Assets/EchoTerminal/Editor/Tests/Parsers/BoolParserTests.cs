using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
    [TestFixture]
    public class BoolParserTests
    {
        [SetUp]
        public void SetUp()
        {
            _parser = new BoolParser();
        }

        private BoolParser _parser;

        [Test]
        public void Type_IsBool()
        {
            Assert.AreEqual(typeof(bool), _parser.Type);
        }

        [TestCase("true", TokenState.Resolved)]
        [TestCase("false", TokenState.Resolved)]
        [TestCase("True", TokenState.Resolved)]
        [TestCase("False", TokenState.Resolved)]
        [TestCase("TRUE", TokenState.Resolved)]
        [TestCase("FALSE", TokenState.Resolved)]
        [TestCase("1", TokenState.Unresolved)]
        [TestCase("0", TokenState.Unresolved)]
        [TestCase("yes", TokenState.Unresolved)]
        [TestCase("no", TokenState.Unresolved)]
        [TestCase("abc", TokenState.Unresolved)]
        [TestCase("", TokenState.Unresolved)]
        public void BoolParse_ReturnsExpectedState(string raw, TokenState expected)
        {
            Assert.AreEqual(expected, _parser.ParseTokenState(raw));
        }

        [TestCase("true", true)]
        [TestCase("false", false)]
        [TestCase("True", true)]
        [TestCase("False", false)]
        public void BoolParseValue_ReturnsExpectedBool(string raw, bool expected)
        {
            Assert.AreEqual(expected, _parser.ParseValue(raw));
        }
    }
}