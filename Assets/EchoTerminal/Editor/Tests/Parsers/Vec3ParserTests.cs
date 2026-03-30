using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class Vec3ParserTests
{
	private Vec3Parser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = TestParsers.CreateAll().OfType<Vec3Parser>().First();
	}

	[Test]
	public void Type_IsVector3()
	{
		Assert.AreEqual(typeof(Vector3), _parser.Type);
	}

	[TestCase("(10, 0, 5)", TokenState.Resolved)]
	[TestCase("(1.5, -2.0, 3.0)", TokenState.Resolved)]
	[TestCase("(0, 0, 0)", TokenState.Resolved)]
	[TestCase("(-1, -2, -3)", TokenState.Resolved)]
	[TestCase("(10, 0", TokenState.Pending)]
	[TestCase("(", TokenState.Pending)]
	[TestCase("(10, 0, )", TokenState.Invalid)]
	[TestCase("(10, 0 )", TokenState.Invalid)]
	[TestCase("(abc, 1, 2)", TokenState.Invalid)]
	[TestCase("(1, 2)", TokenState.Invalid)]
	[TestCase("(1, 2, 3, 4)", TokenState.Invalid)]
	[TestCase("(, ,)", TokenState.Invalid)]
	[TestCase("10, 0, 5", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void Vector3Parse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}
}
}