using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace JNodePath.Test;

public class JsonNodeQueryExpressionTests
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
        var o1 = JsonNode.Parse("{\"Title\":\"Title!\",\"FirstName\":\"FirstName!\",\"LastName\":\"LastName!\"}");

        Assert.True(compositeExpression.IsMatch(o1, o1));

        var o2 = JsonNode.Parse("{\"Title\":\"Title!\",\"FirstName\":\"FirstName!\"}");

        Assert.False(compositeExpression.IsMatch(o2, o2));

        var o3 = JsonNode.Parse("{\"Title\":\"Title!\"}");

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

        var o1 = JsonNode.Parse("{\"Title\":\"Title!\",\"FirstName\":\"FirstName!\",\"LastName\":\"LastName!\"}");

        Assert.True(compositeExpression.IsMatch(o1, o1));

        var o2 = JsonNode.Parse("{\"Title\":\"Title!\",\"FirstName\":\"FirstName!\"}");

        Assert.True(compositeExpression.IsMatch(o2, o2));

        var o3 = JsonNode.Parse("{\"Title\":\"Title!\"}");

        Assert.False(compositeExpression.IsMatch(o3, o3));
    }

    [Fact]
    public void BooleanExpressionTest_RegexEqualsOperator()
    {
        BooleanQueryExpression e1 = new BooleanQueryExpression(QueryOperator.RegexEquals, new List<PathFilter>
        {
            new ArrayIndexFilter()
        }, JsonNode.Parse("\"/foo.*d/\""));

        var oNull = JsonNode.Parse("null");

        Assert.True(e1.IsMatch(oNull, JsonNode.Parse("[\"food\"]")));
        Assert.True(e1.IsMatch(oNull, JsonNode.Parse("[\"fooood and drink\"]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[\"FOOD\"]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[\"foo\",\"foog\",\"good\"]")));

        BooleanQueryExpression e2 = new BooleanQueryExpression(QueryOperator.RegexEquals, new List<PathFilter>
        {
            new ArrayIndexFilter()
        }, JsonNode.Parse("\"/Foo.*d/i\""));

        Assert.True(e2.IsMatch(oNull, JsonNode.Parse("[\"food\"]")));
        Assert.True(e2.IsMatch(oNull, JsonNode.Parse("[\"fooood and drink\"]")));
        Assert.True(e2.IsMatch(oNull, JsonNode.Parse("[\"FOOD\"]")));
        Assert.False(e2.IsMatch(oNull, JsonNode.Parse("[\"foo\",\"foog\",\"good\"]")));
    }

    [Fact]
    public void BooleanExpressionTest_RegexEqualsOperator_CornerCase()
    {
        BooleanQueryExpression e1 = new BooleanQueryExpression(QueryOperator.RegexEquals, new List<PathFilter>
        {
            new ArrayIndexFilter()
        }, JsonNode.Parse("\"/// comment/\""));

        var oNull = JsonNode.Parse("null");

        Assert.True(e1.IsMatch(oNull, JsonNode.Parse("[\"// comment\"]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[\"//comment\",\"/ comment\"]")));

        BooleanQueryExpression e2 = new BooleanQueryExpression(QueryOperator.RegexEquals, new List<PathFilter>
        {
            new ArrayIndexFilter()
        }, JsonNode.Parse("\"/<tag>.*</tag>/i\""));

        Assert.True(e2.IsMatch(oNull, JsonNode.Parse("[\"<Tag>Test</Tag>\",\"\"]")));
        Assert.False(e2.IsMatch(oNull, JsonNode.Parse("[\"<tag>Test<tag>\"]")));
    }

    [Fact]
    public void BooleanExpressionTest()
    {
        BooleanQueryExpression e1 = new BooleanQueryExpression(QueryOperator.LessThan, new List<PathFilter>
        {
            new ArrayIndexFilter()
        }, JsonNode.Parse("3"));

        var oNull = JsonNode.Parse("null");

        Assert.True(e1.IsMatch(oNull, JsonNode.Parse("[1,2,3,4,5]")));
        Assert.True(e1.IsMatch(oNull, JsonNode.Parse("[2,3,4,5]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[3,4,5]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[4,5]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[\"11\",5]")));

        BooleanQueryExpression e2 = new BooleanQueryExpression(QueryOperator.LessThanOrEquals, new List<PathFilter>
        {
            new ArrayIndexFilter()
        }, JsonNode.Parse("3"));

        Assert.True(e2.IsMatch(oNull, JsonNode.Parse("[1,2,3,4,5]")));
        Assert.True(e2.IsMatch(oNull, JsonNode.Parse("[2,3,4,5]")));
        Assert.True(e2.IsMatch(oNull, JsonNode.Parse("[3,4,5]")));
        Assert.False(e2.IsMatch(oNull, JsonNode.Parse("[4,5]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[\"11\",5]")));
    }

    [Fact]
    public void BooleanExpressionTest_GreaterThanOperator()
    {
        BooleanQueryExpression e1 = new BooleanQueryExpression(QueryOperator.GreaterThan, new List<PathFilter>
            {
                new ArrayIndexFilter()
            }, JsonNode.Parse("3"));

        var oNull = JsonNode.Parse("null");

        Assert.True(e1.IsMatch(oNull, JsonNode.Parse("[\"2\",\"26\"]")));
        Assert.True(e1.IsMatch(oNull, JsonNode.Parse("[2,26]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[2,3]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[\"2\",\"3\"]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[null,false,true,[],\"3\"]")));
    }

    [Fact]
    public void BooleanExpressionTest_GreaterThanOrEqualsOperator()
    {
        BooleanQueryExpression e1 = new BooleanQueryExpression(QueryOperator.GreaterThanOrEquals, new List<PathFilter>
            {
                new ArrayIndexFilter()
            }, JsonNode.Parse("3"));

        var oNull = JsonNode.Parse("null");

        Assert.True(e1.IsMatch(oNull, JsonNode.Parse("[\"2\",\"26\"]")));
        Assert.True(e1.IsMatch(oNull, JsonNode.Parse("[2,26]")));
        Assert.True(e1.IsMatch(oNull, JsonNode.Parse("[2,3]")));
        Assert.True(e1.IsMatch(oNull, JsonNode.Parse("[\"2\",\"3\"]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[2,1]")));
        Assert.False(e1.IsMatch(oNull, JsonNode.Parse("[\"2\",\"1\"]")));
    }
}