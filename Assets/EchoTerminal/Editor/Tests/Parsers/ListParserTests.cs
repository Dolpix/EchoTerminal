using System;
using System.Collections;
using System.Collections.Generic;
using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
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
		_parser = new(parsers);
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
	[TestCase("[(1,2,3),(4,wrong,6)]", TokenState.Failed)]

	[TestCase("[]", TokenState.Completed, typeof(List<int>))]
	[TestCase("[1,2,3]", TokenState.Completed, typeof(List<int>))]
	[TestCase("[ 1 , 2 , 3 ]", TokenState.Completed, typeof(List<int>))]
	[TestCase("[-1,0,99]", TokenState.Completed, typeof(List<int>))]
	[TestCase("[1,2", TokenState.Partial, typeof(List<int>))]
	[TestCase("[abc]", TokenState.Failed, typeof(List<int>))]
	[TestCase("[1.5]", TokenState.Failed, typeof(List<int>))]

	[TestCase("[]", TokenState.Completed, typeof(List<float>))]
	[TestCase("[1.5,2.0,3.14]", TokenState.Completed, typeof(List<float>))]
	[TestCase("[1,2,3]", TokenState.Completed, typeof(List<float>))]
	[TestCase("[abc]", TokenState.Failed, typeof(List<float>))]
	[TestCase("[1,2,ab", TokenState.Failed, typeof(List<float>))]

	[TestCase("[]", TokenState.Completed, typeof(List<bool>))]
	[TestCase("[true,false]", TokenState.Completed, typeof(List<bool>))]
	[TestCase("[True]", TokenState.Completed, typeof(List<bool>))]
	[TestCase("[yes]", TokenState.Failed, typeof(List<bool>))]

	[TestCase("[]", TokenState.Completed, typeof(List<string>))]
	[TestCase("[\"hello\",\"world\"]", TokenState.Completed, typeof(List<string>))]
	[TestCase("[\"hello\",\"world\"", TokenState.Partial, typeof(List<string>))]

	[TestCase("[[1,2],[3,4]]", TokenState.Completed, typeof(List<List<int>>))]
	[TestCase("[[1],[2,3,4],[]]", TokenState.Completed, typeof(List<List<int>>))]
	[TestCase("[[1,2],[3,4]", TokenState.Partial, typeof(List<List<int>>))]
	[TestCase("[[1,abc]]", TokenState.Failed, typeof(List<List<int>>))]
	[TestCase("[1,4]", TokenState.Failed, typeof(List<List<int>>))]
	[TestCase("[42,", TokenState.Failed, typeof(List<List<int>>))]

	[TestCase("[(1,2,3),(4,5,6)]", TokenState.Completed, typeof(List<Vector3>))]
	[TestCase("[(1,2,3),(4,5,6)", TokenState.Partial, typeof(List<Vector3>))]
	[TestCase("[(1,2,3),(4,5,6),(", TokenState.Partial, typeof(List<Vector3>))]
	[TestCase("[(1,2,3),(4,wrong,6)]", TokenState.Failed, typeof(List<Vector3>))]
	[TestCase("[(1,2,3),(4,wrong,6)", TokenState.Failed, typeof(List<Vector3>))]
	[TestCase("[(1,2,3),(4,wro", TokenState.Failed, typeof(List<Vector3>))]
	public void ParseTokenState_ReturnsExpectedState(string raw, TokenState expected, Type targetType = null)
	{
		var result = targetType == null
			? _parser.ParseTokenState(raw)
			: _parser.ParseTokenState(raw, targetType);

		Assert.AreEqual(expected, result);
	}
}
}