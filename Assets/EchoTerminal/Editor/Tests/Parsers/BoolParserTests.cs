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

        [TestCase("true", TokenState.Completed)]
        [TestCase("false", TokenState.Completed)]
        [TestCase("True", TokenState.Completed)]
        [TestCase("False", TokenState.Completed)]
        [TestCase("TRUE", TokenState.Completed)]
        [TestCase("FALSE", TokenState.Completed)]
        [TestCase("tru", TokenState.Partial)]
        [TestCase("fals", TokenState.Partial)]
        [TestCase("T", TokenState.Partial)]
        [TestCase("FA", TokenState.Partial)]
        [TestCase("1", TokenState.Failed)]
        [TestCase("0", TokenState.Failed)]
        [TestCase("yes", TokenState.Failed)]
        [TestCase("no", TokenState.Failed)]
        [TestCase("abc", TokenState.Failed)]
        [TestCase("", TokenState.Failed)]
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