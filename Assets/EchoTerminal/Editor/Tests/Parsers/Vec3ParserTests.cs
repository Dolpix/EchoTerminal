using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class Vec3ParserTests
{
	private Vec3Parser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = new Vec3Parser();
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
	public void ValidVec3_ReturnResolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}

	[Test]
	public void IncompleteVec3_ReturnsPending()
	{
		Assert.AreEqual(TokenState.Pending, _parser.Parse("(10, 0"));
	}

	[Test]
	public void JustOpenParen_ReturnsPending()
	{
		Assert.AreEqual(TokenState.Pending, _parser.Parse("("));
	}

	[TestCase("(abc, 1, 2)", TokenState.Invalid)]
	[TestCase("(1, 2)", TokenState.Invalid)]
	[TestCase("(1, 2, 3, 4)", TokenState.Invalid)]
	[TestCase("(, ,)", TokenState.Invalid)]
	[TestCase("(10, 0, )", TokenState.Invalid)]
	[TestCase("(10, 0 )", TokenState.Invalid)]
	public void MalformedVec3_ReturnInvalid(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}

	[TestCase("10, 0, 5", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void NoLeadingParen_ReturnsUnresolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}
}