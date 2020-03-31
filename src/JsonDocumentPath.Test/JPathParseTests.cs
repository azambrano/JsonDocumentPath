using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace JDocument.Test
{
    public class JsonDocumentPathParseTests
    {
        [Fact]
        public void BooleanQuery_TwoValues()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(1 > 2)]");
            Assert.Equal(1, path.Filters.Count);
            BooleanQueryExpression booleanExpression = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.Equal(1, ((JsonElement)booleanExpression.Left).GetInt32());
            Assert.Equal(2, ((JsonElement)booleanExpression.Right).GetInt32());
            Assert.Equal(QueryOperator.GreaterThan, booleanExpression.Operator);
        }

        [Fact]
        public void BooleanQuery_TwoPaths()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.price > @.max_price)]");
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
            JsonDocumentPath path = new JsonDocumentPath("Blah");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void SingleQuotedProperty()
        {
            JsonDocumentPath path = new JsonDocumentPath("['Blah']");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void SingleQuotedPropertyWithWhitespace()
        {
            JsonDocumentPath path = new JsonDocumentPath("[  'Blah'  ]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void SingleQuotedPropertyWithDots()
        {
            JsonDocumentPath path = new JsonDocumentPath("['Blah.Ha']");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal("Blah.Ha", ((FieldFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void SingleQuotedPropertyWithBrackets()
        {
            JsonDocumentPath path = new JsonDocumentPath("['[*]']");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal("[*]", ((FieldFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void SinglePropertyWithRoot()
        {
            JsonDocumentPath path = new JsonDocumentPath("$.Blah");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void SinglePropertyWithRootWithStartAndEndWhitespace()
        {
            JsonDocumentPath path = new JsonDocumentPath(" $.Blah ");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void RootWithBadWhitespace()
        {
            ExceptionAssert.Throws<JsonException>(() => { new JsonDocumentPath("$ .Blah"); }, @"Unexpected character while parsing path:  ");
        }

        [Fact]
        public void NoFieldNameAfterDot()
        {
            ExceptionAssert.Throws<JsonException>(() => { new JsonDocumentPath("$.Blah."); }, @"Unexpected end while parsing path.");
        }

        [Fact]
        public void RootWithBadWhitespace2()
        {
            ExceptionAssert.Throws<JsonException>(() => { new JsonDocumentPath("$. Blah"); }, @"Unexpected character while parsing path:  ");
        }

        [Fact]
        public void WildcardPropertyWithRoot()
        {
            JsonDocumentPath path = new JsonDocumentPath("$.*");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(null, ((FieldFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void WildcardArrayWithRoot()
        {
            JsonDocumentPath path = new JsonDocumentPath("$.[*]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(null, ((ArrayIndexFilter)path.Filters[0]).Index);
        }

        [Fact]
        public void RootArrayNoDot()
        {
            JsonDocumentPath path = new JsonDocumentPath("$[1]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(1, ((ArrayIndexFilter)path.Filters[0]).Index);
        }

        [Fact]
        public void WildcardArray()
        {
            JsonDocumentPath path = new JsonDocumentPath("[*]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(null, ((ArrayIndexFilter)path.Filters[0]).Index);
        }

        [Fact]
        public void WildcardArrayWithProperty()
        {
            JsonDocumentPath path = new JsonDocumentPath("[ * ].derp");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal(null, ((ArrayIndexFilter)path.Filters[0]).Index);
            Assert.Equal("derp", ((FieldFilter)path.Filters[1]).Name);
        }

        [Fact]
        public void QuotedWildcardPropertyWithRoot()
        {
            JsonDocumentPath path = new JsonDocumentPath("$.['*']");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal("*", ((FieldFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void SingleScanWithRoot()
        {
            JsonDocumentPath path = new JsonDocumentPath("$..Blah");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal("Blah", ((ScanFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void QueryTrue()
        {
            JsonDocumentPath path = new JsonDocumentPath("$.elements[?(true)]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("elements", ((FieldFilter)path.Filters[0]).Name);
            Assert.Equal(QueryOperator.Exists, ((QueryFilter)path.Filters[1]).Expression.Operator);
        }

        [Fact]
        public void ScanQuery()
        {
            JsonDocumentPath path = new JsonDocumentPath("$.elements..[?(@.id=='AAA')]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("elements", ((FieldFilter)path.Filters[0]).Name);

            BooleanQueryExpression expression = (BooleanQueryExpression)((QueryScanFilter)path.Filters[1]).Expression;

            List<PathFilter> paths = (List<PathFilter>)expression.Left;

            Assert.IsType(typeof(FieldFilter), paths[0]);
        }

        [Fact]
        public void WildcardScanWithRoot()
        {
            JsonDocumentPath path = new JsonDocumentPath("$..*");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(null, ((ScanFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void WildcardScanWithRootWithWhitespace()
        {
            JsonDocumentPath path = new JsonDocumentPath("$..* ");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(null, ((ScanFilter)path.Filters[0]).Name);
        }

        [Fact]
        public void TwoProperties()
        {
            JsonDocumentPath path = new JsonDocumentPath("Blah.Two");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            Assert.Equal("Two", ((FieldFilter)path.Filters[1]).Name);
        }

        [Fact]
        public void OnePropertyOneScan()
        {
            JsonDocumentPath path = new JsonDocumentPath("Blah..Two");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            Assert.Equal("Two", ((ScanFilter)path.Filters[1]).Name);
        }

        [Fact]
        public void SinglePropertyAndIndexer()
        {
            JsonDocumentPath path = new JsonDocumentPath("Blah[0]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            Assert.Equal(0, ((ArrayIndexFilter)path.Filters[1]).Index);
        }

        [Fact]
        public void SinglePropertyAndExistsQuery()
        {
            JsonDocumentPath path = new JsonDocumentPath("Blah[ ?( @..name ) ]");
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
            JsonDocumentPath path = new JsonDocumentPath("Blah[ ?( @.name=='hi' ) ]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.Equal(QueryOperator.Equals, expressions.Operator);
            Assert.Equal("hi", ((JsonElement)expressions.Right).GetString());
        }

        [Fact]
        public void SinglePropertyAndFilterWithEscapeQuote()
        {
            JsonDocumentPath path = new JsonDocumentPath(@"Blah[ ?( @.name=='h\'i' ) ]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.Equal(QueryOperator.Equals, expressions.Operator);
            Assert.Equal("h'i", ((JsonElement)expressions.Right).GetString());
        }

        [Fact]
        public void SinglePropertyAndFilterWithDoubleEscape()
        {
            JsonDocumentPath path = new JsonDocumentPath(@"Blah[ ?( @.name=='h\\i' ) ]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.Equal(QueryOperator.Equals, expressions.Operator);
            Assert.Equal("h\\i", ((JsonElement)expressions.Right).GetString());
        }

        [Fact]
        public void SinglePropertyAndFilterWithRegexAndOptions()
        {
            JsonDocumentPath path = new JsonDocumentPath("Blah[ ?( @.name=~/hi/i ) ]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.Equal(QueryOperator.RegexEquals, expressions.Operator);
            Assert.Equal("/hi/i", ((JsonElement)expressions.Right).GetString());
        }

        [Fact]
        public void SinglePropertyAndFilterWithRegex()
        {
            JsonDocumentPath path = new JsonDocumentPath("Blah[?(@.title =~ /^.*Sword.*$/)]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.Equal(QueryOperator.RegexEquals, expressions.Operator);
            Assert.Equal("/^.*Sword.*$/", ((JsonElement)expressions.Right).GetString());
        }

        [Fact]
        public void SinglePropertyAndFilterWithEscapedRegex()
        {
            JsonDocumentPath path = new JsonDocumentPath(@"Blah[?(@.title =~ /[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g)]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.Equal(QueryOperator.RegexEquals, expressions.Operator);
            Assert.Equal(@"/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g", ((JsonElement)expressions.Right).GetString());
        }

        [Fact]
        public void SinglePropertyAndFilterWithOpenRegex()
        {
            ExceptionAssert.Throws<JsonException>(() => { new JsonDocumentPath(@"Blah[?(@.title =~ /[\"); }, "Path ended with an open regex.");
        }

        [Fact]
        public void SinglePropertyAndFilterWithUnknownEscape()
        {
            ExceptionAssert.Throws<JsonException>(() => { new JsonDocumentPath(@"Blah[ ?( @.name=='h\i' ) ]"); }, @"Unknown escape character: \i");
        }

        [Fact]
        public void SinglePropertyAndFilterWithFalse()
        {
            JsonDocumentPath path = new JsonDocumentPath("Blah[ ?( @.name==false ) ]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.Equal(QueryOperator.Equals, expressions.Operator);
            Assert.Equal(false, ((JsonElement)expressions.Right).GetBoolean());
        }

        [Fact]
        public void SinglePropertyAndFilterWithTrue()
        {
            JsonDocumentPath path = new JsonDocumentPath("Blah[ ?( @.name==true ) ]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.Equal(QueryOperator.Equals, expressions.Operator);
            Assert.Equal(true, ((JsonElement)expressions.Right).GetBoolean());
        }

        [Fact]
        public void SinglePropertyAndFilterWithNull()
        {
            JsonDocumentPath path = new JsonDocumentPath("Blah[ ?( @.name==null ) ]");
            Assert.Equal(2, path.Filters.Count);
            Assert.Equal("Blah", ((FieldFilter)path.Filters[0]).Name);
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[1]).Expression;
            Assert.Equal(QueryOperator.Equals, expressions.Operator);
            Assert.Equal(null, ((JsonElement)expressions.Right).GetObjectValue());
        }

        [Fact]
        public void FilterWithScan()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@..name<>null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            List<PathFilter> paths = (List<PathFilter>)expressions.Left;
            Assert.Equal("name", ((ScanFilter)paths[0]).Name);
        }

        [Fact]
        public void FilterWithNotEquals()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name<>null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.Equal(QueryOperator.NotEquals, expressions.Operator);
        }

        [Fact]
        public void FilterWithNotEquals2()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name!=null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.Equal(QueryOperator.NotEquals, expressions.Operator);
        }

        [Fact]
        public void FilterWithLessThan()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name<null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.Equal(QueryOperator.LessThan, expressions.Operator);
        }

        [Fact]
        public void FilterWithLessThanOrEquals()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name<=null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.Equal(QueryOperator.LessThanOrEquals, expressions.Operator);
        }

        [Fact]
        public void FilterWithGreaterThan()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name>null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.Equal(QueryOperator.GreaterThan, expressions.Operator);
        }

        [Fact]
        public void FilterWithGreaterThanOrEquals()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name>=null)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.Equal(QueryOperator.GreaterThanOrEquals, expressions.Operator);
        }

        [Fact]
        public void FilterWithInteger()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name>=12)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.Equal(12, ((JsonElement)expressions.Right).GetInt32());
        }

        [Fact]
        public void FilterWithNegativeInteger()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name>=-12)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.Equal(-12, ((JsonElement)expressions.Right).GetInt32());
        }

        [Fact]
        public void FilterWithFloat()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name>=12.1)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.Equal(12.1d, ((JsonElement)expressions.Right).GetDouble());
        }

        [Fact]
        public void FilterExistWithAnd()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name&&@.title)]");
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
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name&&@.title||@.pie)]");
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
            JsonDocumentPath path = new JsonDocumentPath("[?($.name>=12.1)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            List<PathFilter> paths = (List<PathFilter>)expressions.Left;
            Assert.Equal(2, paths.Count);
            Assert.IsType(typeof(RootFilter), paths[0]);
            Assert.IsType(typeof(FieldFilter), paths[1]);
        }

        [Fact]
        public void BadOr1()
        {
            ExceptionAssert.Throws<JsonException>(() => new JsonDocumentPath("[?(@.name||)]"), "Unexpected character while parsing path query: )");
        }

        [Fact]
        public void BaddOr2()
        {
            ExceptionAssert.Throws<JsonException>(() => new JsonDocumentPath("[?(@.name|)]"), "Unexpected character while parsing path query: |");
        }

        [Fact]
        public void BaddOr3()
        {
            ExceptionAssert.Throws<JsonException>(() => new JsonDocumentPath("[?(@.name|"), "Unexpected character while parsing path query: |");
        }

        [Fact]
        public void BaddOr4()
        {
            ExceptionAssert.Throws<JsonException>(() => new JsonDocumentPath("[?(@.name||"), "Path ended with open query.");
        }

        [Fact]
        public void NoAtAfterOr()
        {
            ExceptionAssert.Throws<JsonException>(() => new JsonDocumentPath("[?(@.name||s"), "Unexpected character while parsing path query: s");
        }

        [Fact]
        public void NoPathAfterAt()
        {
            ExceptionAssert.Throws<JsonException>(() => new JsonDocumentPath("[?(@.name||@"), @"Path ended with open query.");
        }

        [Fact]
        public void NoPathAfterDot()
        {
            ExceptionAssert.Throws<JsonException>(() => new JsonDocumentPath("[?(@.name||@."), @"Unexpected end while parsing path.");
        }

        [Fact]
        public void NoPathAfterDot2()
        {
            ExceptionAssert.Throws<JsonException>(() => new JsonDocumentPath("[?(@.name||@.)]"), @"Unexpected end while parsing path.");
        }

        [Fact]
        public void FilterWithFloatExp()
        {
            JsonDocumentPath path = new JsonDocumentPath("[?(@.name>=5.56789e+0)]");
            BooleanQueryExpression expressions = (BooleanQueryExpression)((QueryFilter)path.Filters[0]).Expression;
            Assert.Equal(5.56789e+0, ((JsonElement)expressions.Right).GetDouble());
        }

        [Fact]
        public void MultiplePropertiesAndIndexers()
        {
            JsonDocumentPath path = new JsonDocumentPath("Blah[0]..Two.Three[1].Four");
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
            ExceptionAssert.Throws<JsonException>(() => { new JsonDocumentPath("Blah[[0]].Two.Three[1].Four"); }, @"Unexpected character while parsing path indexer: [");
        }

        [Fact]
        public void UnclosedIndexer()
        {
            ExceptionAssert.Throws<JsonException>(() => { new JsonDocumentPath("Blah[0"); }, @"Path ended with open indexer.");
        }

        [Fact]
        public void IndexerOnly()
        {
            JsonDocumentPath path = new JsonDocumentPath("[111119990]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(111119990, ((ArrayIndexFilter)path.Filters[0]).Index);
        }

        [Fact]
        public void IndexerOnlyWithWhitespace()
        {
            JsonDocumentPath path = new JsonDocumentPath("[  10  ]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(10, ((ArrayIndexFilter)path.Filters[0]).Index);
        }

        [Fact]
        public void MultipleIndexes()
        {
            JsonDocumentPath path = new JsonDocumentPath("[111119990,3]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(2, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes.Count);
            Assert.Equal(111119990, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes[0]);
            Assert.Equal(3, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes[1]);
        }

        [Fact]
        public void MultipleIndexesWithWhitespace()
        {
            JsonDocumentPath path = new JsonDocumentPath("[   111119990  ,   3   ]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(2, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes.Count);
            Assert.Equal(111119990, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes[0]);
            Assert.Equal(3, ((ArrayMultipleIndexFilter)path.Filters[0]).Indexes[1]);
        }

        [Fact]
        public void MultipleQuotedIndexes()
        {
            JsonDocumentPath path = new JsonDocumentPath("['111119990','3']");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(2, ((FieldMultipleFilter)path.Filters[0]).Names.Count);
            Assert.Equal("111119990", ((FieldMultipleFilter)path.Filters[0]).Names[0]);
            Assert.Equal("3", ((FieldMultipleFilter)path.Filters[0]).Names[1]);
        }

        [Fact]
        public void MultipleQuotedIndexesWithWhitespace()
        {
            JsonDocumentPath path = new JsonDocumentPath("[ '111119990' , '3' ]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(2, ((FieldMultipleFilter)path.Filters[0]).Names.Count);
            Assert.Equal("111119990", ((FieldMultipleFilter)path.Filters[0]).Names[0]);
            Assert.Equal("3", ((FieldMultipleFilter)path.Filters[0]).Names[1]);
        }

        [Fact]
        public void SlicingIndexAll()
        {
            JsonDocumentPath path = new JsonDocumentPath("[111119990:3:2]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(111119990, ((ArraySliceFilter)path.Filters[0]).Start);
            Assert.Equal(3, ((ArraySliceFilter)path.Filters[0]).End);
            Assert.Equal(2, ((ArraySliceFilter)path.Filters[0]).Step);
        }

        [Fact]
        public void SlicingIndex()
        {
            JsonDocumentPath path = new JsonDocumentPath("[111119990:3]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(111119990, ((ArraySliceFilter)path.Filters[0]).Start);
            Assert.Equal(3, ((ArraySliceFilter)path.Filters[0]).End);
            Assert.Equal(null, ((ArraySliceFilter)path.Filters[0]).Step);
        }

        [Fact]
        public void SlicingIndexNegative()
        {
            JsonDocumentPath path = new JsonDocumentPath("[-111119990:-3:-2]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(-111119990, ((ArraySliceFilter)path.Filters[0]).Start);
            Assert.Equal(-3, ((ArraySliceFilter)path.Filters[0]).End);
            Assert.Equal(-2, ((ArraySliceFilter)path.Filters[0]).Step);
        }

        [Fact]
        public void SlicingIndexEmptyStop()
        {
            JsonDocumentPath path = new JsonDocumentPath("[  -3  :  ]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(-3, ((ArraySliceFilter)path.Filters[0]).Start);
            Assert.Equal(null, ((ArraySliceFilter)path.Filters[0]).End);
            Assert.Equal(null, ((ArraySliceFilter)path.Filters[0]).Step);
        }

        [Fact]
        public void SlicingIndexEmptyStart()
        {
            JsonDocumentPath path = new JsonDocumentPath("[ : 1 : ]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(null, ((ArraySliceFilter)path.Filters[0]).Start);
            Assert.Equal(1, ((ArraySliceFilter)path.Filters[0]).End);
            Assert.Equal(null, ((ArraySliceFilter)path.Filters[0]).Step);
        }

        [Fact]
        public void SlicingIndexWhitespace()
        {
            JsonDocumentPath path = new JsonDocumentPath("[  -111119990  :  -3  :  -2  ]");
            Assert.Equal(1, path.Filters.Count);
            Assert.Equal(-111119990, ((ArraySliceFilter)path.Filters[0]).Start);
            Assert.Equal(-3, ((ArraySliceFilter)path.Filters[0]).End);
            Assert.Equal(-2, ((ArraySliceFilter)path.Filters[0]).Step);
        }

        [Fact]
        public void EmptyIndexer()
        {
            ExceptionAssert.Throws<JsonException>(() => { new JsonDocumentPath("[]"); }, "Array index expected.");
        }

        [Fact]
        public void IndexerCloseInProperty()
        {
            ExceptionAssert.Throws<JsonException>(() => { new JsonDocumentPath("]"); }, "Unexpected character while parsing path: ]");
        }

        [Fact]
        public void AdjacentIndexers()
        {
            JsonDocumentPath path = new JsonDocumentPath("[1][0][0][" + int.MaxValue + "]");
            Assert.Equal(4, path.Filters.Count);
            Assert.Equal(1, ((ArrayIndexFilter)path.Filters[0]).Index);
            Assert.Equal(0, ((ArrayIndexFilter)path.Filters[1]).Index);
            Assert.Equal(0, ((ArrayIndexFilter)path.Filters[2]).Index);
            Assert.Equal(int.MaxValue, ((ArrayIndexFilter)path.Filters[3]).Index);
        }

        [Fact]
        public void MissingDotAfterIndexer()
        {
            ExceptionAssert.Throws<JsonException>(() => { new JsonDocumentPath("[1]Blah"); }, "Unexpected character following indexer: B");
        }

        [Fact]
        public void PropertyFollowingEscapedPropertyName()
        {
            JsonDocumentPath path = new JsonDocumentPath("frameworks.dnxcore50.dependencies.['System.Xml.ReaderWriter'].source");
            Assert.Equal(5, path.Filters.Count);

            Assert.Equal("frameworks", ((FieldFilter)path.Filters[0]).Name);
            Assert.Equal("dnxcore50", ((FieldFilter)path.Filters[1]).Name);
            Assert.Equal("dependencies", ((FieldFilter)path.Filters[2]).Name);
            Assert.Equal("System.Xml.ReaderWriter", ((FieldFilter)path.Filters[3]).Name);
            Assert.Equal("source", ((FieldFilter)path.Filters[4]).Name);
        }
    }
}