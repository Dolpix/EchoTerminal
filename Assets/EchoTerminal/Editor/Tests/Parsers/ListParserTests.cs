using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class ListParserTests
{
	[SetUp]
	public void SetUp()
	{
		var parsers = new List<ITokenParser>
		{
			new IntParser(), new FloatParser(), new BoolParser(), new StringParser(), new Vec3Parser()
		};
		_parser = new ListParser(parsers);
		parsers.Add(_parser);
	}

	private ListParser _parser;

	[Test]
	public void Type_IsIList()
	{
		Assert.AreEqual(typeof(IList), _parser.Type);
	}

	[TestCase("[]", TokenState.Completed)]
	[TestCase("[1,2,3]", TokenState.Completed)]
	[TestCase("[1.5,2.5]", TokenState.Completed)]
	[TestCase("[true,false]", TokenState.Completed)]
	[TestCase("[\"hello\",\"world\"]", TokenState.Completed)]
	[TestCase("[abc]", TokenState.Completed)]
	[TestCase("[[1,2],[3,4]]", TokenState.Completed)]
	[TestCase("[", TokenState.Partial)]
	[TestCase("[1,2", TokenState.Partial)]
	[TestCase("[[1,2],[3,4]", TokenState.Partial)]
	[TestCase("", TokenState.Failed)]
	[TestCase("1,2,3", TokenState.Failed)]
	[TestCase("[??!]", TokenState.Failed)]
	public void ParseTokenState_Dynamic_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("[]", TokenState.Completed)]
	[TestCase("[1,2,3]", TokenState.Completed)]
	[TestCase("[ 1 , 2 , 3 ]", TokenState.Completed)]
	[TestCase("[-1,0,99]", TokenState.Completed)]
	[TestCase("[1,2", TokenState.Partial)]
	[TestCase("[[1,2],[3,4]", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
	[TestCase("[abc]", TokenState.Failed)]
	[TestCase("[1.5]", TokenState.Failed)]
	public void ParseTokenState_IntExpected_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(List<int>)));
	}

	[TestCase("[]", TokenState.Completed)]
	[TestCase("[1.5,2.0,3.14]", TokenState.Completed)]
	[TestCase("[1,2,3]", TokenState.Completed)]
	[TestCase("[abc]", TokenState.Failed)]
	[TestCase("[1,2,ab", TokenState.Failed)]
	public void ParseTokenState_FloatExpected_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(List<float>)));
	}

	[TestCase("[]", TokenState.Completed)]
	[TestCase("[true,false]", TokenState.Completed)]
	[TestCase("[True]", TokenState.Completed)]
	[TestCase("[yes]", TokenState.Failed)]
	public void ParseTokenState_BoolExpected_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(List<bool>)));
	}

	[TestCase("[]", TokenState.Completed)]
	[TestCase("[\"hello\",\"world\"]", TokenState.Completed)]
	[TestCase("[\"hello\",\"world\"", TokenState.Partial)]
	public void ParseTokenState_StringExpected_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(List<string>)));
	}

	[TestCase("[[1,2],[3,4]]", TokenState.Completed)]
	[TestCase("[[1],[2,3,4],[]]", TokenState.Completed)]
	[TestCase("[[1,2],[3,4]", TokenState.Partial)]
	[TestCase("[[1,abc]]", TokenState.Failed)]
	[TestCase("[1,4]", TokenState.Failed)]
	[TestCase("[42,", TokenState.Failed)]
	public void ParseTokenState_NestedIntExpected_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(List<List<int>>)));
	}

	[TestCase("[(1,2,3),(4,5,6)]", TokenState.Completed)]
	[TestCase("[(1,2,3),(4,5,6)", TokenState.Partial)]
	[TestCase("[(1,2,3),(4,5,6),(", TokenState.Partial)]
	[TestCase("[(1,2,3),(4,wrong,6)]", TokenState.Failed)]
	[TestCase("[(1,2,3),(4,wrong,6)", TokenState.Failed)]
	[TestCase("[(1,2,3),(4,wro", TokenState.Failed)]
	public void ParseTokenState_Vec3Expected_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(List<Vector3>)));
	}

	[TestCase("[(1,2,3),(4,wrong,6)]", TokenState.Failed)]
	[TestCase("[(1,2,3),(4,wrong,6)", TokenState.Failed)]
	public void ParseTokenState_Vec3Dynamic_FailsOnBadChild(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}
}
}