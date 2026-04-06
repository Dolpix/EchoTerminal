using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class ListParserTests
{
	[SetUp]
	public void SetUp()
	{
		var parsers = new List<ITokenParser>
			{ new IntParser(), new FloatParser(), new BoolParser(), new StringParser() };
		_parser = new(parsers);
		parsers.Add(_parser);
	}

	private ListParser _parser;

	[Test]
	public void Type_IsIList()
	{
		Assert.AreEqual(typeof(IList), _parser.Type);
	}

	[TestCase("[]", TokenState.Resolved)]
	[TestCase("[1,2,3]", TokenState.Resolved)]
	[TestCase("[1.5,2.5]", TokenState.Resolved)]
	[TestCase("[true,false]", TokenState.Resolved)]
	[TestCase("[\"hello\",\"world\"]", TokenState.Resolved)]
	[TestCase("[abc]", TokenState.Resolved)]
	[TestCase("[[1,2],[3,4]]", TokenState.Resolved)]
	[TestCase("[", TokenState.Pending)]
	[TestCase("[1,2", TokenState.Pending)]
	[TestCase("[[1,2],[3,4]", TokenState.Pending)]
	[TestCase("", TokenState.Unresolved)]
	[TestCase("1,2,3", TokenState.Unresolved)]
	[TestCase("[??!]", TokenState.Invalid)]
	public void ParseTokenState_Dynamic_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("[]", TokenState.Resolved)]
	[TestCase("[1,2,3]", TokenState.Resolved)]
	[TestCase("[ 1 , 2 , 3 ]", TokenState.Resolved)]
	[TestCase("[-1,0,99]", TokenState.Resolved)]
	[TestCase("[1,2", TokenState.Pending)]
	[TestCase("[[1,2],[3,4]", TokenState.Pending)]
	[TestCase("", TokenState.Unresolved)]
	[TestCase("[abc]", TokenState.Invalid)]
	[TestCase("[1.5]", TokenState.Invalid)]
	public void ParseTokenState_IntExpected_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(List<int>)));
	}

	[TestCase("[]", TokenState.Resolved)]
	[TestCase("[1.5,2.0,3.14]", TokenState.Resolved)]
	[TestCase("[1,2,3]", TokenState.Resolved)]
	[TestCase("[abc]", TokenState.Invalid)]
	public void ParseTokenState_FloatExpected_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(List<float>)));
	}

	[TestCase("[]", TokenState.Resolved)]
	[TestCase("[true,false]", TokenState.Resolved)]
	[TestCase("[True]", TokenState.Resolved)]
	[TestCase("[yes]", TokenState.Invalid)]
	public void ParseTokenState_BoolExpected_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(List<bool>)));
	}

	[TestCase("[]", TokenState.Resolved)]
	[TestCase("[\"hello\",\"world\"]", TokenState.Resolved)]
	[TestCase("[\"hello\",\"world\"", TokenState.Pending)]
	public void ParseTokenState_StringExpected_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(List<string>)));
	}

	[TestCase("[[1,2],[3,4]]", TokenState.Resolved)]
	[TestCase("[[1],[2,3,4],[]]", TokenState.Resolved)]
	[TestCase("[[1,2],[3,4]", TokenState.Pending)]
	[TestCase("[[1,abc]]", TokenState.Invalid)]
	public void ParseTokenState_NestedIntExpected_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(List<List<int>>)));
	}

	// --- ParseValue (expected type) ---

	[Test]
	public void ParseValue_EmptyIntList()
	{
		var result = (List<int>)_parser.ParseValue("[]", typeof(List<int>));
		Assert.AreEqual(0, result.Count);
	}

	[Test]
	public void ParseValue_IntList_SingleElement()
	{
		var result = (List<int>)_parser.ParseValue("[42]", typeof(List<int>));
		Assert.AreEqual(new List<int> { 42 }, result);
	}

	[Test]
	public void ParseValue_IntList_MultipleElements()
	{
		var result = (List<int>)_parser.ParseValue("[1,2,3]", typeof(List<int>));
		Assert.AreEqual(new List<int> { 1, 2, 3 }, result);
	}

	[Test]
	public void ParseValue_IntList_ElementsWithSpaces()
	{
		var result = (List<int>)_parser.ParseValue("[ 1 , 2 , 3 ]", typeof(List<int>));
		Assert.AreEqual(new List<int> { 1, 2, 3 }, result);
	}

	[Test]
	public void ParseValue_FloatList()
	{
		var result = (List<float>)_parser.ParseValue("[1.5,2.0,3.14]", typeof(List<float>));
		Assert.AreEqual(new List<float> { 1.5f, 2.0f, 3.14f }, result);
	}

	[Test]
	public void ParseValue_BoolList()
	{
		var result = (List<bool>)_parser.ParseValue("[true,false,true]", typeof(List<bool>));
		Assert.AreEqual(new List<bool> { true, false, true }, result);
	}

	[Test]
	public void ParseValue_StringList_QuotedStrings()
	{
		var result = (List<string>)_parser.ParseValue("[\"hello\",\"world\"]", typeof(List<string>));
		Assert.AreEqual(new List<string> { "hello", "world" }, result);
	}

	// --- ParseValue (dynamic, no expected type) ---

	[Test]
	public void ParseValue_Dynamic_InferredAsIntList()
	{
		var result = _parser.ParseValue("[1,2,3]");
		Assert.IsInstanceOf<List<int>>(result);
		Assert.AreEqual(new List<int> { 1, 2, 3 }, result);
	}

	[Test]
	public void ParseValue_Dynamic_InferredAsFloatList()
	{
		var result = _parser.ParseValue("[1.5,2.5]");
		Assert.IsInstanceOf<List<float>>(result);
	}

	[Test]
	public void ParseValue_Dynamic_EmptyList()
	{
		var result = _parser.ParseValue("[]");
		Assert.IsInstanceOf<List<object>>(result);
		Assert.AreEqual(0, ((IList)result).Count);
	}

	// --- Nested list ---

	[Test]
	public void ParseValue_NestedIntList_WithExpectedType()
	{
		var result = (List<List<int>>)_parser.ParseValue("[[1,2],[3,4]]", typeof(List<List<int>>));
		Assert.AreEqual(2, result.Count);
		Assert.AreEqual(new List<int> { 1, 2 }, result[0]);
		Assert.AreEqual(new List<int> { 3, 4 }, result[1]);
	}

	[Test]
	public void ParseValue_NestedIntList_CommasInsideSublistsNotSplitEarly()
	{
		var result = (List<List<int>>)_parser.ParseValue("[[1,2,3],[4,5,6]]", typeof(List<List<int>>));
		Assert.AreEqual(2, result.Count);
		Assert.AreEqual(new List<int> { 1, 2, 3 }, result[0]);
		Assert.AreEqual(new List<int> { 4, 5, 6 }, result[1]);
	}
}
}