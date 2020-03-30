using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace JDocument.Test
{
    public class QueryExpressionTests
    {
        [Fact]
        public void AndExpressionTest()
        {
            CompositeExpression compositeExpression = new CompositeExpression(QueryOperator.And)
            {
                Expressions = new List<QueryExpression>
                {
                    new BooleanQueryExpression(QueryOperator.Exists,new List<PathFilter>
                        {
                            new FieldFilter("FirstName")
                        },null),
                    new BooleanQueryExpression(QueryOperator.Exists,new List<PathFilter>
                        {
                            new FieldFilter("LastName")
                        },null)
                }
            };
            var o1 = JsonDocument.Parse("{\"Title\":\"Title!\",\"FirstName\":\"FirstName!\",\"LastName\":\"LastName!\"}").RootElement;

            Assert.True(compositeExpression.IsMatch(o1, o1));

            var o2 = JsonDocument.Parse("{\"Title\":\"Title!\",\"FirstName\":\"FirstName!\"}").RootElement;

            Assert.False(compositeExpression.IsMatch(o2, o2));

            var o3 = JsonDocument.Parse("{\"Title\":\"Title!\"}").RootElement;

            Assert.False(compositeExpression.IsMatch(o3, o3));
        }

        [Fact]
        public void OrExpressionTest()
        {
            CompositeExpression compositeExpression = new CompositeExpression(QueryOperator.Or)
            {
                Expressions = new List<QueryExpression>
                {
                    new BooleanQueryExpression(QueryOperator.Exists,new List<PathFilter>
                        {
                            new FieldFilter("FirstName")
                        },null),
                    new BooleanQueryExpression(QueryOperator.Exists,new List<PathFilter>
                        {
                            new FieldFilter("LastName")
                        },null)
                }
            };

            var o1 = JsonDocument.Parse("{\"Title\":\"Title!\",\"FirstName\":\"FirstName!\",\"LastName\":\"LastName!\"}").RootElement;

            Assert.True(compositeExpression.IsMatch(o1, o1));

            var o2 = JsonDocument.Parse("{\"Title\":\"Title!\",\"FirstName\":\"FirstName!\"}").RootElement;

            Assert.True(compositeExpression.IsMatch(o2, o2));

            var o3 = JsonDocument.Parse("{\"Title\":\"Title!\"}").RootElement;

            Assert.False(compositeExpression.IsMatch(o3, o3));
        }

        [Fact]
        public void BooleanExpressionTest_RegexEqualsOperator()
        {
            BooleanQueryExpression e1 = new BooleanQueryExpression(QueryOperator.RegexEquals, new List<PathFilter>
            {
                new ArrayIndexFilter()
            }, JsonDocument.Parse("\"/foo.*d/\"").RootElement);

            var oNull = JsonDocument.Parse("null").RootElement;

            Assert.True(e1.IsMatch(oNull, JsonDocument.Parse("[\"food\"]").RootElement));
            Assert.True(e1.IsMatch(oNull, JsonDocument.Parse("[\"fooood and drink\"]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[\"FOOD\"]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[\"foo\",\"foog\",\"good\"]").RootElement));

            BooleanQueryExpression e2 = new BooleanQueryExpression(QueryOperator.RegexEquals, new List<PathFilter>
            {
                new ArrayIndexFilter()
            }, JsonDocument.Parse("\"/Foo.*d/i\"").RootElement);

            Assert.True(e2.IsMatch(oNull, JsonDocument.Parse("[\"food\"]").RootElement));
            Assert.True(e2.IsMatch(oNull, JsonDocument.Parse("[\"fooood and drink\"]").RootElement));
            Assert.True(e2.IsMatch(oNull, JsonDocument.Parse("[\"FOOD\"]").RootElement));
            Assert.False(e2.IsMatch(oNull, JsonDocument.Parse("[\"foo\",\"foog\",\"good\"]").RootElement));
        }

        [Fact]
        public void BooleanExpressionTest_RegexEqualsOperator_CornerCase()
        {
            BooleanQueryExpression e1 = new BooleanQueryExpression(QueryOperator.RegexEquals, new List<PathFilter>
            {
                new ArrayIndexFilter()
            }, JsonDocument.Parse("\"/// comment/\"").RootElement);

            var oNull = JsonDocument.Parse("null").RootElement;

            Assert.True(e1.IsMatch(oNull, JsonDocument.Parse("[\"// comment\"]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[\"//comment\",\"/ comment\"]").RootElement));

            BooleanQueryExpression e2 = new BooleanQueryExpression(QueryOperator.RegexEquals, new List<PathFilter>
            {
                new ArrayIndexFilter()
            }, JsonDocument.Parse("\"/<tag>.*</tag>/i\"").RootElement);

            Assert.True(e2.IsMatch(oNull, JsonDocument.Parse("[\"<Tag>Test</Tag>\",\"\"]").RootElement));
            Assert.False(e2.IsMatch(oNull, JsonDocument.Parse("[\"<tag>Test<tag>\"]").RootElement));
        }

        [Fact]
        public void BooleanExpressionTest()
        {
            BooleanQueryExpression e1 = new BooleanQueryExpression(QueryOperator.LessThan, new List<PathFilter>
            {
                new ArrayIndexFilter()
            }, JsonDocument.Parse("3").RootElement);

            var oNull = JsonDocument.Parse("null").RootElement;

            Assert.True(e1.IsMatch(oNull, JsonDocument.Parse("[1,2,3,4,5]").RootElement));
            Assert.True(e1.IsMatch(oNull, JsonDocument.Parse("[2,3,4,5]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[3,4,5]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[4,5]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[\"11\",5]").RootElement));

            BooleanQueryExpression e2 = new BooleanQueryExpression(QueryOperator.LessThanOrEquals, new List<PathFilter>
            {
                new ArrayIndexFilter()
            }, JsonDocument.Parse("3").RootElement);

            Assert.True(e2.IsMatch(oNull, JsonDocument.Parse("[1,2,3,4,5]").RootElement));
            Assert.True(e2.IsMatch(oNull, JsonDocument.Parse("[2,3,4,5]").RootElement));
            Assert.True(e2.IsMatch(oNull, JsonDocument.Parse("[3,4,5]").RootElement));
            Assert.False(e2.IsMatch(oNull, JsonDocument.Parse("[4,5]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[\"11\",5]").RootElement));
        }

        [Fact]
        public void BooleanExpressionTest_GreaterThanOperator()
        {
            BooleanQueryExpression e1 = new BooleanQueryExpression(QueryOperator.GreaterThan, new List<PathFilter>
                {
                    new ArrayIndexFilter()
                }, JsonDocument.Parse("3").RootElement);

            var oNull = JsonDocument.Parse("null").RootElement;

            Assert.True(e1.IsMatch(oNull, JsonDocument.Parse("[\"2\",\"26\"]").RootElement));
            Assert.True(e1.IsMatch(oNull, JsonDocument.Parse("[2,26]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[2,3]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[\"2\",\"3\"]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[null,false,true,[],\"3\"]").RootElement));
        }

        [Fact]
        public void BooleanExpressionTest_GreaterThanOrEqualsOperator()
        {
            BooleanQueryExpression e1 = new BooleanQueryExpression(QueryOperator.GreaterThanOrEquals, new List<PathFilter>
                {
                    new ArrayIndexFilter()
                }, JsonDocument.Parse("3").RootElement);

            var oNull = JsonDocument.Parse("null").RootElement;

            Assert.True(e1.IsMatch(oNull, JsonDocument.Parse("[\"2\",\"26\"]").RootElement));
            Assert.True(e1.IsMatch(oNull, JsonDocument.Parse("[2,26]").RootElement));
            Assert.True(e1.IsMatch(oNull, JsonDocument.Parse("[2,3]").RootElement));
            Assert.True(e1.IsMatch(oNull, JsonDocument.Parse("[\"2\",\"3\"]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[2,1]").RootElement));
            Assert.False(e1.IsMatch(oNull, JsonDocument.Parse("[\"2\",\"1\"]").RootElement));
        }
    }
}