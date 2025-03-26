using JDocument.Test;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace JNodePath.Test;

public class JsonNodeParseTests
{
    [Fact]
    public void BooleanQuery_TwoValues()
    {
        JsonNodePath path = new JsonNodePath("[?(1 > 2)]");
        Assert.Equal(1, path.Filters.Count);
        BooleanQueryExpression booleanExpression = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(1, ((JsonNode)booleanExpression.Left).GetInt32());
        Assert.Equal(2, ((JsonNode)booleanExpression.Right).GetInt32());
        Assert.Equal(QueryOperator.GreaterThan, booleanExpression.Operator);
    }

    [Fact]
    public void BooleanQuery_TwoPaths()
    {
        JsonNodePath path = new JsonNodePath("[?(@.price > @.max_price)]");
        Assert.Equal(1, path.Filters.Count);
        BooleanQueryExpression booleanExpression = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        List<PathFilter> leftPaths = (List<PathFilter>)booleanExpression.Left;
        List<PathFilter> rightPaths = (List<PathFilter>)booleanExpression.Right;

        Assert.Equal("price", ((FieldFilter)leftPaths[0]).Name);
        Assert.Equal("max_price", ((FieldFilter)rightPaths[0]).Name);
        Assert.Equal(QueryOperator.GreaterThan, booleanExpression.Operator);
    }

    [Fact]
    public void SingleProperty()
    {
        JsonNodePath path = new JsonNodePath("Blah");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void SingleQuotedProperty()
    {
        JsonNodePath path = new JsonNodePath("['Blah']");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void SingleQuotedPropertyWithWhitespace()
    {
        JsonNodePath path = new JsonNodePath("[  'Blah'  ]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void SingleQuotedPropertyWithDots()
    {
        JsonNodePath path = new JsonNodePath("['Blah.Ha']");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal("Blah.Ha", ((FieldFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void SingleQuotedPropertyWithBrackets()
    {
        JsonNodePath path = new JsonNodePath("['[*]']");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal("[*]", ((FieldFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void SinglePropertyWithRoot()
    {
        JsonNodePath path = new JsonNodePath("$.Blah");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void SinglePropertyWithRootWithStartAndEndWhitespace()
    {
        JsonNodePath path = new JsonNodePath(" $.Blah ");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void RootWithBadWhitespace()
    {
        ExceptionAssert.Throws<JsonException>(() => { new JsonNodePath("$ .Blah"); }, @"Unexpected character while parsing path:  ");
    }

    [Fact]
    public void NoFieldNameAfterDot()
    {
        ExceptionAssert.Throws<JsonException>(() => { new JsonNodePath("$.Blah."); }, @"Unexpected end while parsing path.");
    }

    [Fact]
    public void RootWithBadWhitespace2()
    {
        ExceptionAssert.Throws<JsonException>(() => { new JsonNodePath("$. Blah"); }, @"Unexpected character while parsing path:  ");
    }

    [Fact]
    public void WildcardPropertyWithRoot()
    {
        JsonNodePath path = new JsonNodePath("$.*");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(null, ((FieldFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void WildcardArrayWithRoot()
    {
        JsonNodePath path = new JsonNodePath("$.[*]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(null, ((ArrayIndexFilter)path.Filters[0]).Index);
    }

    [Fact]
    public void RootArrayNoDot()
    {
        JsonNodePath path = new JsonNodePath("$[1]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(1, ((ArrayIndexFilter)path.Filters[0]).Index);
    }

    [Fact]
    public void WildcardArray()
    {
        JsonNodePath path = new JsonNodePath("[*]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(null, ((ArrayIndexFilter)path.Filters[0]).Index);
    }

    [Fact]
    public void WildcardArrayWithProperty()
    {
        JsonNodePath path = new JsonNodePath("[ * ].derp");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal(null, ((ArrayIndexFilter)path.Filters[0]).Index);
        Assert.Equal("derp", ((FieldFilter)path.Filters[1]).Name);
    }

    [Fact]
    public void QuotedWildcardPropertyWithRoot()
    {
        JsonNodePath path = new JsonNodePath("$.['*']");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal("*", ((FieldFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void SingleScanWithRoot()
    {
        JsonNodePath path = new JsonNodePath("$..Blah");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal("Blah", ((ScanFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void QueryTrue()
    {
        JsonNodePath path = new JsonNodePath("$.elements[?(true)]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("elements", ((FieldFilter)path.Filters[0]).Name);
        Assert.Equal(QueryOperator.Exists, ((QueryFilter)path.Filters[1]).Expression.Operator);
    }

    [Fact]
    public void ScanQuery()
    {
        JsonNodePath path = new JsonNodePath("$.elements..[?(@.id=='AAA')]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("elements", ((FieldFilter)path.Filters[0]).Name);

        BooleanQueryExpression expression = (BooleanQueryExpression)((QueryScanFilter)path.Filters[1]).Expression;

        List<PathFilter> paths = (List<PathFilter>)expression.Left;

        Assert.IsType(typeof(FieldFilter), paths[0]);
    }

    [Fact]
    public void WildcardScanWithRoot()
    {
        JsonNodePath path = new JsonNodePath("$..*");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(null, ((ScanFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void WildcardScanWithRootWithWhitespace()
    {
        JsonNodePath path = new JsonNodePath("$..* ");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(null, ((ScanFilter)path.Filters[0]).Name);
    }

    [Fact]
    public void TwoProperties()
    {
        JsonNodePath path = new JsonNodePath("Blah.Two");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        Assert.Equal("Two", ((FieldFilter)path.Filters[1]).Name);
    }

    [Fact]
    public void OnePropertyOneScan()
    {
        JsonNodePath path = new JsonNodePath("Blah..Two");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        Assert.Equal("Two", ((ScanFilter)path.Filters[1]).Name);
    }

    [Fact]
    public void SinglePropertyAndIndexer()
    {
        JsonNodePath path = new JsonNodePath("Blah[0]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        Assert.Equal(0, ((ArrayIndexFilter)path.Filters[1]).Index);
    }

    [Fact]
    public void SinglePropertyAndExistsQuery()
    {
        JsonNodePath path = new JsonNodePath("Blah[ ?( @..name ) ]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
        Assert.Equal(QueryOperator.Exists, expressions.Operator);
        List<PathFilter> paths = (List<PathFilter>)expressions.Left;
        Assert.Equal("name", ((ScanFilter)paths[0]).Name);
        Assert.Equal(1, paths.Count);
    }

    [Fact]
    public void SinglePropertyAndFilterWithWhitespace()
    {
        JsonNodePath path = new JsonNodePath("Blah[ ?( @.name=='hi' ) ]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
        Assert.Equal(QueryOperator.Equals, expressions.Operator);
        Assert.Equal("hi", ((JsonNode)expressions.Right).GetString());
    }

    [Fact]
    public void SinglePropertyAndFilterWithEscapeQuote()
    {
        JsonNodePath path = new JsonNodePath(@"Blah[ ?( @.name=='h\'i' ) ]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
        Assert.Equal(QueryOperator.Equals, expressions.Operator);
        Assert.Equal("h'i", ((JsonNode)expressions.Right).GetString());
    }

    [Fact]
    public void SinglePropertyAndFilterWithDoubleEscape()
    {
        JsonNodePath path = new JsonNodePath(@"Blah[ ?( @.name=='h\\i' ) ]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
        Assert.Equal(QueryOperator.Equals, expressions.Operator);
        var r = ((JsonNode)expressions.Right);
        var rStr = r.GetString();
        Assert.Equal("h\\i", ((JsonNode)expressions.Right).GetString());
    }

    [Fact]
    public void SinglePropertyAndFilterWithRegexAndOptions()
    {
        JsonNodePath path = new JsonNodePath("Blah[ ?( @.name=~/hi/i ) ]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
        Assert.Equal(QueryOperator.RegexEquals, expressions.Operator);
        Assert.Equal("/hi/i", ((JsonNode)expressions.Right).GetString());
    }

    [Fact]
    public void SinglePropertyAndFilterWithRegex()
    {
        JsonNodePath path = new JsonNodePath("Blah[?(@.title =~ /^.*Sword.*$/)]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
        Assert.Equal(QueryOperator.RegexEquals, expressions.Operator);
        Assert.Equal("/^.*Sword.*$/", ((JsonNode)expressions.Right).GetString());
    }

    [Fact]
    public void SinglePropertyAndFilterWithEscapedRegex()
    {
        JsonNodePath path = new JsonNodePath(@"Blah[?(@.title =~ /[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g)]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
        Assert.Equal(QueryOperator.RegexEquals, expressions.Operator);
        Assert.Equal(@"/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g", ((JsonNode)expressions.Right).GetString());
    }

    [Fact]
    public void SinglePropertyAndFilterWithOpenRegex()
    {
        ExceptionAssert.Throws<JsonException>(() => { new JsonNodePath(@"Blah[?(@.title =~ /[\"); }, "Path ended with an open regex.");
    }

    [Fact]
    public void SinglePropertyAndFilterWithUnknownEscape()
    {
        ExceptionAssert.Throws<JsonException>(() => { new JsonNodePath(@"Blah[ ?( @.name=='h\i' ) ]"); }, @"Unknown escape character: \i");
    }

    [Fact]
    public void SinglePropertyAndFilterWithFalse()
    {
        JsonNodePath path = new JsonNodePath("Blah[ ?( @.name==false ) ]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
        Assert.Equal(QueryOperator.Equals, expressions.Operator);
        Assert.Equal(false, ((JsonNode)expressions.Right).GetBoolean());
    }

    [Fact]
    public void SinglePropertyAndFilterWithTrue()
    {
        JsonNodePath path = new JsonNodePath("Blah[ ?( @.name==true ) ]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
        Assert.Equal(QueryOperator.Equals, expressions.Operator);
        Assert.Equal(true, ((JsonNode)expressions.Right).GetBoolean());
    }

    [Fact]
    public void SinglePropertyAndFilterWithNull()
    {
        JsonNodePath path = new JsonNodePath("Blah[ ?( @.name==null ) ]");
        Assert.Equal(2, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
        Assert.Equal(QueryOperator.Equals, expressions.Operator);
        Assert.Equal(null, ((JsonNode)expressions.Right).GetObjectValue());
    }

    [Fact]
    public void FilterWithScan()
    {
        JsonNodePath path = new JsonNodePath("[?(@..name<>null)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        List<PathFilter> paths = (List<PathFilter>)expressions.Left;
        Assert.Equal("name", ((ScanFilter)paths[0]).Name);
    }

    [Fact]
    public void FilterWithNotEquals()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name<>null)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(QueryOperator.NotEquals, expressions.Operator);
    }

    [Fact]
    public void FilterWithNotEquals2()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name!=null)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(QueryOperator.NotEquals, expressions.Operator);
    }

    [Fact]
    public void FilterWithLessThan()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name<null)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(QueryOperator.LessThan, expressions.Operator);
    }

    [Fact]
    public void FilterWithLessThanOrEquals()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name<=null)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(QueryOperator.LessThanOrEquals, expressions.Operator);
    }

    [Fact]
    public void FilterWithGreaterThan()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name>null)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(QueryOperator.GreaterThan, expressions.Operator);
    }

    [Fact]
    public void FilterWithGreaterThanOrEquals()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name>=null)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(QueryOperator.GreaterThanOrEquals, expressions.Operator);
    }

    [Fact]
    public void FilterWithInteger()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name>=12)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(12, ((JsonNode)expressions.Right).GetInt32());
    }

    [Fact]
    public void FilterWithNegativeInteger()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name>=-12)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(-12, ((JsonNode)expressions.Right).GetInt32());
    }

    [Fact]
    public void FilterWithFloat()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name>=12.1)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(12.1d, ((JsonNode)expressions.Right).GetDouble());
    }

    [Fact]
    public void FilterExistWithAnd()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name&&@.title)]");
        CompositeExpression expressions = (CompositeExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(QueryOperator.And, expressions.Operator);
        Assert.Equal(2, expressions.Expressions.Count);

        var first = (BooleanQueryExpression)expressions.Expressions[0];
        var firstPaths = (List<PathFilter>)first.Left;
        Assert.Equal("name", ((FieldFilter)firstPaths[0]).Name);
        Assert.Equal(QueryOperator.Exists, first.Operator);

        var second = (BooleanQueryExpression)expressions.Expressions[1];
        var secondPaths = (List<PathFilter>)second.Left;
        Assert.Equal("title", ((FieldFilter)secondPaths[0]).Name);
        Assert.Equal(QueryOperator.Exists, second.Operator);
    }

    [Fact]
    public void FilterExistWithAndOr()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name&&@.title||@.pie)]");
        CompositeExpression andExpression = (CompositeExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(QueryOperator.And, andExpression.Operator);
        Assert.Equal(2, andExpression.Expressions.Count);

        var first = (BooleanQueryExpression)andExpression.Expressions[0];
        var firstPaths = (List<PathFilter>)first.Left;
        Assert.Equal("name", ((FieldFilter)firstPaths[0]).Name);
        Assert.Equal(QueryOperator.Exists, first.Operator);

        CompositeExpression orExpression = (CompositeExpression)andExpression.Expressions[1];
        Assert.Equal(2, orExpression.Expressions.Count);

        var orFirst = (BooleanQueryExpression)orExpression.Expressions[0];
        var orFirstPaths = (List<PathFilter>)orFirst.Left;
        Assert.Equal("title", ((FieldFilter)orFirstPaths[0]).Name);
        Assert.Equal(QueryOperator.Exists, orFirst.Operator);

        var orSecond = (BooleanQueryExpression)orExpression.Expressions[1];
        var orSecondPaths = (List<PathFilter>)orSecond.Left;
        Assert.Equal("pie", ((FieldFilter)orSecondPaths[0]).Name);
        Assert.Equal(QueryOperator.Exists, orSecond.Operator);
    }

    [Fact]
    public void FilterWithRoot()
    {
        JsonNodePath path = new JsonNodePath("[?($.name>=12.1)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        List<PathFilter> paths = (List<PathFilter>)expressions.Left;
        Assert.Equal(2, paths.Count);
        Assert.IsType(typeof(RootFilter), paths[0]);
        Assert.IsType(typeof(FieldFilter), paths[1]);
    }

    [Fact]
    public void BadOr1()
    {
        ExceptionAssert.Throws<JsonException>(() => new JsonNodePath("[?(@.name||)]"), "Unexpected character while parsing path query: )");
    }

    [Fact]
    public void BaddOr2()
    {
        ExceptionAssert.Throws<JsonException>(() => new JsonNodePath("[?(@.name|)]"), "Unexpected character while parsing path query: |");
    }

    [Fact]
    public void BaddOr3()
    {
        ExceptionAssert.Throws<JsonException>(() => new JsonNodePath("[?(@.name|"), "Unexpected character while parsing path query: |");
    }

    [Fact]
    public void BaddOr4()
    {
        ExceptionAssert.Throws<JsonException>(() => new JsonNodePath("[?(@.name||"), "Path ended with open query.");
    }

    [Fact]
    public void NoAtAfterOr()
    {
        ExceptionAssert.Throws<JsonException>(() => new JsonNodePath("[?(@.name||s"), "Unexpected character while parsing path query: s");
    }

    [Fact]
    public void NoPathAfterAt()
    {
        ExceptionAssert.Throws<JsonException>(() => new JsonNodePath("[?(@.name||@"), @"Path ended with open query.");
    }

    [Fact]
    public void NoPathAfterDot()
    {
        ExceptionAssert.Throws<JsonException>(() => new JsonNodePath("[?(@.name||@."), @"Unexpected end while parsing path.");
    }

    [Fact]
    public void NoPathAfterDot2()
    {
        ExceptionAssert.Throws<JsonException>(() => new JsonNodePath("[?(@.name||@.)]"), @"Unexpected end while parsing path.");
    }

    [Fact]
    public void FilterWithFloatExp()
    {
        JsonNodePath path = new JsonNodePath("[?(@.name>=5.56789e+0)]");
        BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
        Assert.Equal(5.56789e+0, ((JsonNode)expressions.Right).GetDouble());
    }

    [Fact]
    public void MultiplePropertiesAndIndexers()
    {
        JsonNodePath path = new JsonNodePath("Blah[0]..Two.Three[1].Four");
        Assert.Equal(6, path.Filters.Count);
        Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        Assert.Equal(0, ((ArrayIndexFilter)path.Filters[1]).Index);
        Assert.Equal("Two", ((ScanFilter)path.Filters[2]).Name);
        Assert.Equal("Three", ((FieldFilter)path.Filters[3]).Name);
        Assert.Equal(1, ((ArrayIndexFilter)path.Filters[4]).Index);
        Assert.Equal("Four", ((FieldFilter)path.Filters[5]).Name);
    }

    [Fact]
    public void BadCharactersInIndexer()
    {
        ExceptionAssert.Throws<JsonException>(() => { new JsonNodePath("Blah[[0]].Two.Three[1].Four"); }, @"Unexpected character while parsing path indexer: [");
    }

    [Fact]
    public void UnclosedIndexer()
    {
        ExceptionAssert.Throws<JsonException>(() => { new JsonNodePath("Blah[0"); }, @"Path ended with open indexer.");
    }

    [Fact]
    public void IndexerOnly()
    {
        JsonNodePath path = new JsonNodePath("[111119990]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(111119990, ((ArrayIndexFilter)path.Filters[0]).Index);
    }

    [Fact]
    public void IndexerOnlyWithWhitespace()
    {
        JsonNodePath path = new JsonNodePath("[  10  ]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(10, ((ArrayIndexFilter)path.Filters[0]).Index);
    }

    [Fact]
    public void MultipleIndexes()
    {
        JsonNodePath path = new JsonNodePath("[111119990,3]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(2, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes.Count);
        Assert.Equal(111119990, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes[0]);
        Assert.Equal(3, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes[1]);
    }

    [Fact]
    public void MultipleIndexesWithWhitespace()
    {
        JsonNodePath path = new JsonNodePath("[   111119990  ,   3   ]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(2, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes.Count);
        Assert.Equal(111119990, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes[0]);
        Assert.Equal(3, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes[1]);
    }

    [Fact]
    public void MultipleQuotedIndexes()
    {
        JsonNodePath path = new JsonNodePath("['111119990','3']");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(2, ((FieldMultipleFilter)path.Filters[0]).Names.Count);
        Assert.Equal("111119990", ((FieldMultipleFilter)path.Filters[0]).Names[0]);
        Assert.Equal("3", ((FieldMultipleFilter)path.Filters[0]).Names[1]);
    }

    [Fact]
    public void MultipleQuotedIndexesWithWhitespace()
    {
        JsonNodePath path = new JsonNodePath("[ '111119990' , '3' ]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(2, ((FieldMultipleFilter)path.Filters[0]).Names.Count);
        Assert.Equal("111119990", ((FieldMultipleFilter)path.Filters[0]).Names[0]);
        Assert.Equal("3", ((FieldMultipleFilter)path.Filters[0]).Names[1]);
    }

    [Fact]
    public void SlicingIndexAll()
    {
        JsonNodePath path = new JsonNodePath("[111119990:3:2]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(111119990, ((ArraySliceFilter)path.Filters[0]).Start);
        Assert.Equal(3, ((ArraySliceFilter)path.Filters[0]).End);
        Assert.Equal(2, ((ArraySliceFilter)path.Filters[0]).Step);
    }

    [Fact]
    public void SlicingIndex()
    {
        JsonNodePath path = new JsonNodePath("[111119990:3]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(111119990, ((ArraySliceFilter)path.Filters[0]).Start);
        Assert.Equal(3, ((ArraySliceFilter)path.Filters[0]).End);
        Assert.Equal(null, ((ArraySliceFilter)path.Filters[0]).Step);
    }

    [Fact]
    public void SlicingIndexNegative()
    {
        JsonNodePath path = new JsonNodePath("[-111119990:-3:-2]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(-111119990, ((ArraySliceFilter)path.Filters[0]).Start);
        Assert.Equal(-3, ((ArraySliceFilter)path.Filters[0]).End);
        Assert.Equal(-2, ((ArraySliceFilter)path.Filters[0]).Step);
    }

    [Fact]
    public void SlicingIndexEmptyStop()
    {
        JsonNodePath path = new JsonNodePath("[  -3  :  ]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(-3, ((ArraySliceFilter)path.Filters[0]).Start);
        Assert.Equal(null, ((ArraySliceFilter)path.Filters[0]).End);
        Assert.Equal(null, ((ArraySliceFilter)path.Filters[0]).Step);
    }

    [Fact]
    public void SlicingIndexEmptyStart()
    {
        JsonNodePath path = new JsonNodePath("[ : 1 : ]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(null, ((ArraySliceFilter)path.Filters[0]).Start);
        Assert.Equal(1, ((ArraySliceFilter)path.Filters[0]).End);
        Assert.Equal(null, ((ArraySliceFilter)path.Filters[0]).Step);
    }

    [Fact]
    public void SlicingIndexWhitespace()
    {
        JsonNodePath path = new JsonNodePath("[  -111119990  :  -3  :  -2  ]");
        Assert.Equal(1, path.Filters.Count);
        Assert.Equal(-111119990, ((ArraySliceFilter)path.Filters[0]).Start);
        Assert.Equal(-3, ((ArraySliceFilter)path.Filters[0]).End);
        Assert.Equal(-2, ((ArraySliceFilter)path.Filters[0]).Step);
    }

    [Fact]
    public void EmptyIndexer()
    {
        ExceptionAssert.Throws<JsonException>(() => { new JsonNodePath("[]"); }, "Array index expected.");
    }

    [Fact]
    public void IndexerCloseInProperty()
    {
        ExceptionAssert.Throws<JsonException>(() => { new JsonNodePath("]"); }, "Unexpected character while parsing path: ]");
    }

    [Fact]
    public void AdjacentIndexers()
    {
        JsonNodePath path = new JsonNodePath("[1][0][0][" + int.MaxValue + "]");
        Assert.Equal(4, path.Filters.Count);
        Assert.Equal(1, ((ArrayIndexFilter)path.Filters[0]).Index);
        Assert.Equal(0, ((ArrayIndexFilter)path.Filters[1]).Index);
        Assert.Equal(0, ((ArrayIndexFilter)path.Filters[2]).Index);
        Assert.Equal(int.MaxValue, ((ArrayIndexFilter)path.Filters[3]).Index);
    }

    [Fact]
    public void MissingDotAfterIndexer()
    {
        ExceptionAssert.Throws<JsonException>(() => { new JsonNodePath("[1]Blah"); }, "Unexpected character following indexer: B");
    }

    [Fact]
    public void PropertyFollowingEscapedPropertyName()
    {
        JsonNodePath path = new JsonNodePath("frameworks.dnxcore50.dependencies.['System.Xml.ReaderWriter'].source");
        Assert.Equal(5, path.Filters.Count);

        Assert.Equal("frameworks", ((FieldFilter)path.Filters[0]).Name);
        Assert.Equal("dnxcore50", ((FieldFilter)path.Filters[1]).Name);
        Assert.Equal("dependencies", ((FieldFilter)path.Filters[2]).Name);
        Assert.Equal("System.Xml.ReaderWriter", ((FieldFilter)path.Filters[3]).Name);
        Assert.Equal("source", ((FieldFilter)path.Filters[4]).Name);
    }
}