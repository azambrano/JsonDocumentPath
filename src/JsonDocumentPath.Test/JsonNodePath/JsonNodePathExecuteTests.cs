using JDocument.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace JNodePath.Test;

public class JsonNodePathExecuteTests
{
    [Fact]
    public void GreaterThanIssue1518()
    {
        string statusJson = @"{""usingmem"": ""214376""}";//214,376
        var jObj = JsonNode.Parse(statusJson);

        var aa = jObj.SelectNode("$..[?(@.usingmem>10)]");//found,10
        Assert.Equal(jObj, aa);

        var bb = jObj.SelectNode("$..[?(@.usingmem>27000)]");//null, 27,000

        Assert.Equal(jObj, bb);

        var cc = jObj.SelectNode("$..[?(@.usingmem>21437)]");//found, 21,437
        Assert.Equal(jObj, cc);

        var dd = jObj.SelectNode("$..[?(@.usingmem>21438)]");//null,21,438
        Assert.Equal(jObj, dd);
    }

    [Fact]
    public void GreaterThanWithIntegerParameterAndStringValue()
    {
        string json = @"{
  ""persons"": [
    {
      ""name""  : ""John"",
      ""age"": ""26""
    },
    {
      ""name""  : ""Jane"",
      ""age"": ""2""
    }
  ]
}";

        var models = JsonNode.Parse(json);

        var results = models.SelectNodes("$.persons[?(@.age > 3)]").ToList();

        Assert.Equal(1, results.Count);
    }

    [Fact]
    public void GreaterThanWithStringParameterAndIntegerValue()
    {
        string json = @"{
          ""persons"": [
            {
              ""name""  : ""John"",
              ""age"": 26
            },
            {
              ""name""  : ""Jane"",
              ""age"": 2
            }
          ]
        }";

        var models = JsonNode.Parse(json);

        var results = models.SelectNodes("$.persons[?(@.age > '3')]").ToList();

        Assert.Equal(1, results.Count);
    }

    [Fact]
    public void RecursiveWildcard()
    {
        string json = @"{
                ""a"": [
                    {
                        ""id"": 1
                    }
                ],
                ""b"": [
                    {
                        ""id"": 2
                    },
                    {
                        ""id"": 3,
                        ""c"": {
                            ""id"": 4
                        }
                    }
                ],
                ""d"": [
                    {
                        ""id"": 5
                    }
                ]
            }";

        var models = JsonNode.Parse(json);
        var results = models.SelectNodes("$.b..*.id").ToList();

        Assert.Equal(3, results.Count);
        Assert.Equal(2, results[0].GetValue<int>());
        Assert.Equal(3, results[1].GetValue<int>());
        Assert.Equal(4, results[2].GetValue<int>());
    }

    [Fact]
    public void ScanFilter()
    {
        string json = @"{
          ""elements"": [
            {
              ""id"": ""A"",
              ""children"": [
                {
                  ""id"": ""AA"",
                  ""children"": [
                    {
                      ""id"": ""AAA""
                    },
                    {
                      ""id"": ""AAB""
                    }
                  ]
                },
                {
                  ""id"": ""AB""
                }
              ]
            },
            {
              ""id"": ""B"",
              ""children"": []
            }
          ]
        }";

        var models = JsonNode.Parse(json);
        var results = models.SelectNodes("$.elements..[?(@.id=='AAA')]").ToList();
        Assert.Equal(1, results.Count);

        var oModels = (JsonObject)models;

        var tryGetResult = oModels
            ["elements"][0]
            ["children"][0]
            ["children"][0];

        Assert.Equal(tryGetResult, results[0]);
    }

    [Fact]
    public void FilterTrue()
    {
        string json = @"{
              ""elements"": [
                {
                  ""id"": ""A"",
                  ""children"": [
                    {
                      ""id"": ""AA"",
                      ""children"": [
                        {
                          ""id"": ""AAA""
                        },
                        {
                          ""id"": ""AAB""
                        }
                      ]
                    },
                    {
                      ""id"": ""AB""
                    }
                  ]
                },
                {
                  ""id"": ""B"",
                  ""children"": []
                }
              ]
            }";

        var models = JsonNode.Parse(json);

        var results = models.SelectNodes("$.elements[?(true)]").ToList();

        Assert.Equal(2, results.Count);
        Assert.Equal(results[0], models["elements"][0]);
        Assert.Equal(results[1], models["elements"][1]);
    }

    [Fact]
    public void ScanFilterTrue()
    {
        string json = @"{
                  ""elements"": [
                    {
                      ""id"": ""A"",
                      ""children"": [
                        {
                          ""id"": ""AA"",
                          ""children"": [
                            {
                              ""id"": ""AAA""
                            },
                            {
                              ""id"": ""AAB""
                            }
                          ]
                        },
                        {
                          ""id"": ""AB""
                        }
                      ]
                    },
                    {
                      ""id"": ""B"",
                      ""children"": []
                    }
                  ]
                }";

        var models = JsonNode.Parse(json);

        var results = models.SelectNodes("$.elements..[?(true)]").ToList();

        Assert.Equal(25, results.Count);
    }

    [Fact]
    public void ScanFilterDeepTrue()
    {
        string json = @"{
                  ""elements"": [
                    {
                      ""id"": ""A"",
                      ""children"": [
                        {
                          ""id"": ""AA"",
                          ""children"": [
                            {
                              ""id"": ""AAA""
                            },
                            {
                              ""id"": ""AAB""
                            }
                          ]
                        },
                        {
                          ""id"": ""AB""
                        }
                      ]
                    },
                    {
                      ""id"": ""B"",
                      ""children"": []
                    }
                  ]
                }";

        var models = JsonNode.Parse(json);
        var results = models.SelectNodes("$.elements..[?(@.id=='AA')]").ToList();

        Assert.Single(results);
    }

    [Fact]
    public void ScanQuoted()
    {
        string json = @"{
                    ""Node1"": {
                        ""Child1"": {
                            ""Name"": ""IsMe"",
                            ""TargetNode"": {
                                ""Prop1"": ""Val1"",
                                ""Prop2"": ""Val2""
                            }
                        },
                        ""My.Child.Node"": {
                            ""TargetNode"": {
                                ""Prop1"": ""Val1"",
                                ""Prop2"": ""Val2""
                            }
                        }
                    },
                    ""Node2"": {
                        ""TargetNode"": {
                            ""Prop1"": ""Val1"",
                            ""Prop2"": ""Val2""
                        }
                    }
                }";

        var models = JsonNode.Parse(json);

        int result = models.SelectNodes("$..['My.Child.Node']").Count();
        Assert.Equal(1, result);

        result = models.SelectNodes("..['My.Child.Node']").Count();
        Assert.Equal(1, result);
    }

    [Fact]
    public void ScanMultipleQuoted()
    {
        string json = @"{
                    ""Node1"": {
                        ""Child1"": {
                            ""Name"": ""IsMe"",
                            ""TargetNode"": {
                                ""Prop1"": ""Val1"",
                                ""Prop2"": ""Val2""
                            }
                        },
                        ""My.Child.Node"": {
                            ""TargetNode"": {
                                ""Prop1"": ""Val3"",
                                ""Prop2"": ""Val4""
                            }
                        }
                    },
                    ""Node2"": {
                        ""TargetNode"": {
                            ""Prop1"": ""Val5"",
                            ""Prop2"": ""Val6""
                        }
                    }
                }";

        var models = JsonNode.Parse(json);

        var results = models.SelectNodes("$..['My.Child.Node','Prop1','Prop2']").ToList();
        Assert.Equal("Val1", results[0].GetValue<string>());
        Assert.Equal("Val2", results[1].GetValue<string>());
        Assert.Equal(JsonValueKind.Object, results[2].GetValueKind());
        Assert.Equal("Val3", results[3].GetValue<string>());
        Assert.Equal("Val4", results[4].GetValue<string>());
        Assert.Equal("Val5", results[5].GetValue<string>());
        Assert.Equal("Val6", results[6].GetValue<string>());
    }

    [Fact]
    public void ParseWithEmptyArrayContent()
    {
        var json = @"{
                    ""controls"": [
                        {
                            ""messages"": {
                                ""addSuggestion"": {
                                    ""en-US"": ""Add""
                                }
                            }
                        },
                        {
                            ""header"": {
                                ""controls"": []
                            },
                            ""controls"": [
                                {
                                    ""controls"": [
                                        {
                                            ""defaultCaption"": {
                                                ""en-US"": ""Sort by""
                                            },
                                            ""sortOptions"": [
                                                {
                                                    ""label"": {
                                                        ""en-US"": ""Name""
                                                    }
                                                }
                                            ]
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }";
        var node = JsonNode.Parse(json);
        var elements = node.SelectNodes("$..en-US").ToList();

        Assert.Equal(3, elements.Count);
        Assert.Equal("Add", elements[0].GetValue<string>());
        Assert.Equal("Sort by", elements[1].GetValue<string>());
        Assert.Equal("Name", elements[2].GetValue<string>());
    }

    [Fact]
    public void SelectElementAfterEmptyContainer()
    {
        string json = @"{
                    ""cont"": [],
                    ""test"": ""no one will find me""
                }";

        var node = JsonNode.Parse(json);

        var results = node.SelectNodes("$..test").ToList();

        Assert.Equal(1, results.Count);
        Assert.Equal("no one will find me", results[0].GetValue<string>());
    }

    [Fact]
    public void EvaluatePropertyWithRequired()
    {
        string json = "{\"bookId\":\"1000\"}";
        var node = JsonNode.Parse(json);

        string bookId = (string)node.SelectNode("bookId", true).GetValue<string>();

        Assert.Equal("1000", bookId);
    }

    [Fact]
    public void EvaluateEmptyPropertyIndexer()
    {
        string json = @"{
                    """": 1
                }";

        var node = JsonNode.Parse(json);

        var t = node.SelectNode("['']");
        Assert.Equal(1, t.GetValue<int>());
    }

    [Fact]
    public void EvaluateEmptyString()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);
        var t = node.SelectNode("");
        Assert.Equal(node, t);

        t = node.SelectNode("['']");
        Assert.Equal(null, t);
    }

    [Fact]
    public void EvaluateEmptyStringWithMatchingEmptyProperty()
    {
        string json = @"{
                    "" "": 1
                }";
        var node = JsonNode.Parse(json);

        var t = node.SelectNode("[' ']");
        Assert.Equal(1, t.GetValue<int>());
    }

    [Fact]
    public void EvaluateWhitespaceString()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        var t = node.SelectNode(" ");
        Assert.Equal(node, t);
    }

    [Fact]
    public void EvaluateDollarString()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        var t = node.SelectNode("$");
        Assert.Equal(node, t);
    }

    [Fact]
    public void EvaluateDollarTypeString()
    {
        string json = @"{
                    ""$values"": [1,2,3]
                }";
        var node = JsonNode.Parse(json);

        var t = node.SelectNode("$values[1]");
        Assert.Equal(2, t.GetValue<int>());
    }

    [Fact]
    public void EvaluateSingleProperty()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        var t = node.SelectNode("Blah");
        Assert.NotNull(t);
        Assert.Equal(JsonValueKind.Number, t.GetValueKind());
        Assert.Equal(1, t.GetValue<int>());
    }

    [Fact]
    public void EvaluateWildcardProperty()
    {
        string json = @"{
                    ""Blah"": 1,
                    ""Blah2"": 2
                }";
        var node = JsonNode.Parse(json);

        var t = node.SelectNodes("$.*").ToList();
        Assert.NotNull(t);
        Assert.Equal(2, t.Count);
        Assert.Equal(1, t[0].GetValue<int>());
        Assert.Equal(2, t[1].GetValue<int>());
    }

    [Fact]
    public void QuoteName()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        var t = node.SelectNode("['Blah']");
        Assert.NotNull(t);
        Assert.Equal(JsonValueKind.Number, t.GetValueKind());
        Assert.Equal(1, t.GetValue<int>());
    }

    [Fact]
    public void EvaluateMissingProperty()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        var t = node.SelectNode("Missing[1]");
        Assert.Null(t);
    }

    [Fact]
    public void EvaluateIndexerOnObject()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        var t = node.SelectNode("[1]");
        Assert.Null(t);
    }

    [Fact]
    public void EvaluateIndexerOnObjectWithError()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        ExceptionAssert.Throws<JsonException>(() =>
        {
            node.SelectNode("[1]", true);
        }, @"Index 1 not valid on JsonObject.");
    }

    [Fact]
    public void EvaluateWildcardIndexOnObjectWithError()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        ExceptionAssert.Throws<JsonException>(() =>
        {
            node.SelectNode("[*]", true);
        }, @"Index * not valid on JsonObject.");
    }

    [Fact]
    public void EvaluateSliceOnObjectWithError()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        ExceptionAssert.Throws<JsonException>(() =>
        {
            node.SelectNode("[:]", true);
        }, @"Array slice is not valid on JsonObject.");
    }

    [Fact]
    public void EvaluatePropertyOnArray()
    {
        string json = @"[1,2,3,4,5]";
        var node = JsonNode.Parse(json);

        var t = node.SelectNode("BlahBlah");
        Assert.Null(t);
    }

    [Fact]
    public void EvaluateMultipleResultsError()
    {
        string json = @"[1,2,3,4,5]";
        var node = JsonNode.Parse(json);
        ExceptionAssert.Throws<JsonException>(() => { node.SelectNode("[0, 1]"); }, @"Path returned multiple tokens.");
    }

    [Fact]
    public void EvaluatePropertyOnArrayWithError()
    {
        string json = @"[1,2,3,4,5]";
        var node = JsonNode.Parse(json);

        ExceptionAssert.Throws<JsonException>(() =>
        {
            node.SelectNode("BlahBlah", true);
        }, @"Property 'BlahBlah' not valid on JsonArray.");
    }

    [Fact]
    public void EvaluateNoResultsWithMultipleArrayIndexes()
    {
        string json = @"[1,2,3,4,5]";
        var node = JsonNode.Parse(json);

        ExceptionAssert.Throws<JsonException>(() => { node.SelectNode("[9,10]", true); }, @"Index 9 outside the bounds of JArray.");
    }

    [Fact]
    public void EvaluateMissingPropertyWithError()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        ExceptionAssert.Throws<JsonException>(() => { node.SelectNode("Missing", true); }, "Property 'Missing' does not exist on JsonElement.");
    }

    [Fact]
    public void EvaluatePropertyWithoutError()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        var v = node.SelectNode("Blah", true).GetValue<int>();
        Assert.Equal(1, v);
    }

    [Fact]
    public void EvaluateMissingPropertyIndexWithError()
    {
        string json = @"{
                    ""Blah"": 1
                }";
        var node = JsonNode.Parse(json);

        ExceptionAssert.Throws<JsonException>(() =>
        {
            node.SelectNode("['Missing','Missing2']", true);
        }, "Property 'Missing' does not exist on JsonObject.");
    }

    [Fact]
    public void EvaluateMultiPropertyIndexOnArrayWithError()
    {
        var a = JsonNode.Parse("[1,2,3,4,5]");

        ExceptionAssert.Throws<JsonException>(() =>
        {
            a.SelectNode("['Missing','Missing2']", true);
        }, "Properties 'Missing', 'Missing2' not valid on JsonArray.");
    }

    [Fact]
    public void EvaluateArraySliceWithError()
    {
        var a = JsonNode.Parse("[1,2,3,4,5]");

        ExceptionAssert.Throws<JsonException>(() => { a.SelectNode("[99:]", true); }, "Array slice of 99 to * returned no results.");

        ExceptionAssert.Throws<JsonException>(() => { a.SelectNode("[1:-19]", true); }, "Array slice of 1 to -19 returned no results.");

        ExceptionAssert.Throws<JsonException>(() => { a.SelectNode("[:-19]", true); }, "Array slice of * to -19 returned no results.");

        a = JsonNode.Parse("[]");

        ExceptionAssert.Throws<JsonException>(() => { a.SelectNode("[:]", true); }, "Array slice of * to * returned no results.");
    }

    [Fact]
    public void EvaluateOutOfBoundsIndxer()
    {
        var a = JsonNode.Parse("[1,2,3,4,5]");

        var t = a.SelectNode("[1000].Ha");
        Assert.Null(t);
    }

    [Fact]
    public void EvaluateArrayOutOfBoundsIndxerWithError()
    {
        var a = JsonNode.Parse("[1,2,3,4,5]");

        ExceptionAssert.Throws<JsonException>(() => { a.SelectNode("[1000].Ha", true); }, "Index 1000 outside the bounds of JArray.");
    }

    [Fact]
    public void EvaluateArray()
    {
        var a = JsonNode.Parse("[1,2,3,4]");

        var t = a.SelectNode("[1]");
        Assert.NotNull(t);
        Assert.Equal(JsonValueKind.Number, t.GetValueKind());
        Assert.Equal(2, t.GetValue<int>());
    }

    [Fact]
    public void EvaluateArraySlice()
    {
        var a = JsonNode.Parse(@"[1, 2, 3, 4, 5, 6, 7, 8, 9]");
        List<JsonNode?> t = null;

        t = a.SelectNodes("[-3:]").ToList();
        Assert.Equal(3, t.Count);
        Assert.Equal(7, t[0].GetValue<int>());
        Assert.Equal(8, t[1].GetValue<int>());
        Assert.Equal(9, t[2].GetValue<int>());

        t = a.SelectNodes("[-1:-2:-1]").ToList();
        Assert.Equal(1, t.Count);
        Assert.Equal(9, t[0].GetValue<int>());

        t = a.SelectNodes("[-2:-1]").ToList();
        Assert.Equal(1, t.Count);
        Assert.Equal(8, t[0].GetValue<int>());

        t = a.SelectNodes("[1:1]").ToList();
        Assert.Equal(0, t.Count);

        t = a.SelectNodes("[1:2]").ToList();
        Assert.Equal(1, t.Count);
        Assert.Equal(2, t[0].GetValue<int>());

        t = a.SelectNodes("[::-1]").ToList();
        Assert.Equal(9, t.Count);
        Assert.Equal(9, t[0].GetValue<int>());
        Assert.Equal(8, t[1].GetValue<int>());
        Assert.Equal(7, t[2].GetValue<int>());
        Assert.Equal(6, t[3].GetValue<int>());
        Assert.Equal(5, t[4].GetValue<int>());
        Assert.Equal(4, t[5].GetValue<int>());
        Assert.Equal(3, t[6].GetValue<int>());
        Assert.Equal(2, t[7].GetValue<int>());
        Assert.Equal(1, t[8].GetValue<int>());

        t = a.SelectNodes("[::-2]").ToList();
        Assert.Equal(5, t.Count);
        Assert.Equal(9, t[0].GetValue<int>());
        Assert.Equal(7, t[1].GetValue<int>());
        Assert.Equal(5, t[2].GetValue<int>());
        Assert.Equal(3, t[3].GetValue<int>());
        Assert.Equal(1, t[4].GetValue<int>());
    }

    [Fact]
    public void EvaluateWildcardArray()
    {
        var a = JsonNode.Parse(@"[1, 2, 3, 4]");

        List<JsonNode?> t = a.SelectNodes("[*]").ToList();
        Assert.NotNull(t);
        Assert.Equal(4, t.Count);
        Assert.Equal(1, t[0].GetValue<int>());
        Assert.Equal(2, t[1].GetValue<int>());
        Assert.Equal(3, t[2].GetValue<int>());
        Assert.Equal(4, t[3].GetValue<int>());
    }

    [Fact]
    public void EvaluateArrayMultipleIndexes()
    {
        var a = JsonNode.Parse(@"[1, 2, 3, 4]");

        IEnumerable<JsonNode?> t = a.SelectNodes("[1,2,0]").ToList();
        Assert.NotNull(t);
        Assert.Equal(3, t.Count());
        Assert.Equal(2, t.ElementAt(0).GetValue<int>());
        Assert.Equal(3, t.ElementAt(1).GetValue<int>());
        Assert.Equal(1, t.ElementAt(2).GetValue<int>());
    }

    [Fact]
    public void EvaluateScan()
    {
        JsonNode o1 = JsonNode.Parse(@"{ ""Name"": 1 }");
        JsonNode o2 = JsonNode.Parse(@"{ ""Name"": 2 }");
        var a = JsonNode.Parse(@"[{ ""Name"": 1 }, { ""Name"": 2 }]");

        var t = a.SelectNodes("$..Name").ToList();
        Assert.NotNull(t);
        Assert.Equal(2, t.Count);
        Assert.Equal(1, t[0].GetValue<int>());
        Assert.Equal(2, t[1].GetValue<int>());
    }

    [Fact]
    public void EvaluateWildcardScan()
    {
        JsonNode o1 = JsonNode.Parse(@"{ ""Name"": 1 }");
        JsonNode o2 = JsonNode.Parse(@"{ ""Name"": 2 }");
        var a = JsonNode.Parse(@"[{ ""Name"": 1 }, { ""Name"": 2 }]");

        var t = a.SelectNodes("$..*").ToList();
        Assert.NotNull(t);
        Assert.Equal(5, t.Count);

        Assert.True(a.DeepEquals(t[0]));

        Assert.True(o1.DeepEquals(t[1]!));

        Assert.Equal(1, t[2].GetValue<int>());
        Assert.True(o2.DeepEquals(t[3]));
        Assert.Equal(2, t[4].GetValue<int>());
    }

    [Fact]
    public void EvaluateScanNestResults()
    {
        JsonNode o1 = JsonNode.Parse(@"{ ""Name"": 1 }");
        JsonNode o2 = JsonNode.Parse(@"{ ""Name"": 2 }");
        JsonNode o3 = JsonNode.Parse(@"{ ""Name"": { ""Name"": [ 3 ] } }");
        var a = JsonNode.Parse(@"[
                { ""Name"": 1 },
                { ""Name"": 2 },
                { ""Name"": { ""Name"": [3] } }
            ]");

        var t = a.SelectNodes("$..Name").ToList();
        Assert.NotNull(t);
        Assert.Equal(4, t.Count);
        Assert.Equal(1, t[0].GetValue<int>());
        Assert.Equal(2, t[1].GetValue<int>());
        Assert.True(JsonNode.Parse(@"{ ""Name"": [3] }").DeepEquals(t[2]));
        Assert.True(JsonNode.Parse("[3]").DeepEquals(t[3]));
    }

    [Fact]
    public void EvaluateWildcardScanNestResults()
    {
        JsonNode o1 = JsonNode.Parse(@"{ ""Name"": 1 }");
        JsonNode o2 = JsonNode.Parse(@"{ ""Name"": 2 }");
        JsonNode o3 = JsonNode.Parse(@"{ ""Name"": { ""Name"": [3] } }");
        var a = JsonNode.Parse(@"[
                { ""Name"": 1 },
                { ""Name"": 2 },
                { ""Name"": { ""Name"": [3] } }
            ]");

        var t = a.SelectNodes("$..*").ToList();
        Assert.NotNull(t);
        Assert.Equal(9, t.Count);

        Assert.True(a.DeepEquals(t[0]));
        Assert.True(o1.DeepEquals(t[1]));
        Assert.Equal(1, t[2].GetValue<int>());
        Assert.True(o2.DeepEquals(t[3]));
        Assert.Equal(2, t[4].GetValue<int>());
        Assert.True(o3.DeepEquals(t[5]));
        Assert.True(JsonNode.Parse(@"{ ""Name"": [3] }").DeepEquals(t[6]));
        Assert.True(JsonNode.Parse("[3]").DeepEquals(t[7]));
        Assert.Equal(3, t[8].GetValue<int>());
        Assert.True(JsonNode.Parse("[3]").DeepEquals(t[7]));
    }

    [Fact]
    public void EvaluateSinglePropertyReturningArray()
    {
        var o = JsonNode.Parse(@"{ ""Blah"": [ 1, 2, 3 ] }");

        var t = o.SelectNode("Blah");
        Assert.NotNull(t);
        Assert.Equal(JsonValueKind.Array, t?.GetValueKind());

        t = o.SelectNode("Blah[2]");
        Assert.Equal(JsonValueKind.Number, t?.GetValueKind());
        Assert.Equal(3, t?.GetValue<int>());
    }

    [Fact]
    public void EvaluateLastSingleCharacterProperty()
    {
        JsonNode o2 = JsonNode.Parse(@"{""People"":[{""N"":""Jeff""}]}");
        var a2 = o2.SelectNode("People[0].N").GetValue<string>();

        Assert.Equal("Jeff", a2);
    }

    [Fact]
    public void ExistsQuery()
    {
        var a = JsonNode.Parse(@"[
                { ""hi"": ""ho"" },
                { ""hi2"": ""ha"" }
            ]");

        var t = a.SelectNodes("[ ?( @.hi ) ]").ToList();
        Assert.NotNull(t);
        Assert.Equal(1, t.Count);
        Assert.True(JsonNode.Parse(@"{ ""hi"": ""ho"" }").DeepEquals(t[0]));
    }

    [Fact]
    public void EqualsQuery()
    {
        var a = JsonNode.Parse(@"[
                { ""hi"": ""ho"" },
                { ""hi"": ""ha"" }
            ]");

        var t = a.SelectNodes("[ ?( @.['hi'] == 'ha' ) ]").ToList();
        Assert.NotNull(t);
        Assert.Equal(1, t.Count);
        Assert.True(JsonNode.Parse(@"{ ""hi"": ""ha"" }").DeepEquals(t[0]));
    }

    [Fact]
    public void NotEqualsQuery()
    {
        var a = JsonNode.Parse(@"[
                { ""hi"": ""ho"" },
                { ""hi"": ""ha"" }
            ]");

        var t = a.SelectNodes("[ ?( @..hi <> 'ha' ) ]").ToList();
        Assert.NotNull(t);
        Assert.Equal(1, t.Count);
        Assert.True(JsonNode.Parse(@"{ ""hi"": ""ho"" }").DeepEquals(t[0]));
    }

    [Fact]
    public void NoPathQuery()
    {
        var a = JsonNode.Parse("[1, 2, 3]");

        var t = a.SelectNodes("[ ?( @ > 1 ) ]").ToList();
        Assert.NotNull(t);
        Assert.Equal(2, t.Count);
        Assert.Equal(2, t[0].GetValue<int>());
        Assert.Equal(3, t[1].GetValue<int>());
    }

    [Fact]
    public void MultipleQueries()
    {
        var a = JsonNode.Parse("[1, 2, 3, 4, 5, 6, 7, 8, 9]");

        // json path does item based evaluation - http://www.sitepen.com/blog/2008/03/17/jsonpath-support/
        // first query resolves array to ints
        // int has no children to query
        var t = a.SelectNodes("[?(@ <> 1)][?(@ <> 4)][?(@ < 7)]").ToList();
        Assert.NotNull(t);
        Assert.Equal(0, t.Count);
    }

    [Fact]
    public void GreaterQuery()
    {
        var a = JsonNode.Parse(@"
            [
                { ""hi"": 1 },
                { ""hi"": 2 },
                { ""hi"": 3 }
            ]");

        var t = a.SelectNodes("[ ?( @.hi > 1 ) ]").ToList();
        Assert.NotNull(t);
        Assert.Equal(2, t.Count);
        Assert.True(JsonNode.Parse(@"{ ""hi"": 2 }").DeepEquals(t[0]));
        Assert.True(JsonNode.Parse(@"{ ""hi"": 3 }").DeepEquals(t[1]));
    }

    [Fact]
    public void LesserQuery_ValueFirst()
    {
        var a = JsonNode.Parse(@"
            [
                { ""hi"": 1 },
                { ""hi"": 2 },
                { ""hi"": 3 }
            ]");

        var t = a.SelectNodes("[ ?( 1 < @.hi ) ]").ToList();
        Assert.NotNull(t);
        Assert.Equal(2, t.Count);
        Assert.True(JsonNode.Parse(@"{ ""hi"": 2 }").DeepEquals(t[0]));
        Assert.True(JsonNode.Parse(@"{ ""hi"": 3 }").DeepEquals(t[1]));
    }

    [Fact]
    public void GreaterOrEqualQuery()
    {
        var a = JsonNode.Parse(@"
            [
                { ""hi"": 1 },
                { ""hi"": 2 },
                { ""hi"": 2.0 },
                { ""hi"": 3 }
            ]");

        var t = a.SelectNodes("[ ?( @.hi >= 1 ) ]").ToList();
        Assert.NotNull(t);
        Assert.Equal(4, t.Count);
        Assert.True(JsonNode.Parse(@"{ ""hi"": 1 }").DeepEquals(t[0]));
        Assert.True(JsonNode.Parse(@"{ ""hi"": 2 }").DeepEquals(t[1]));
        Assert.True(JsonNode.Parse(@"{ ""hi"": 2.0 }").DeepEquals(t[2]));
        Assert.True(JsonNode.Parse(@"{ ""hi"": 3 }").DeepEquals(t[3]));
    }

    [Fact]
    public void NestedQuery()
    {
        var a = JsonNode.Parse(@"
            [
                {
                    ""name"": ""Bad Boys"",
                    ""cast"": [ { ""name"": ""Will Smith"" } ]
                },
                {
                    ""name"": ""Independence Day"",
                    ""cast"": [ { ""name"": ""Will Smith"" } ]
                },
                {
                    ""name"": ""The Rock"",
                    ""cast"": [ { ""name"": ""Nick Cage"" } ]
                }
            ]");

        var t = a.SelectNodes("[?(@.cast[?(@.name=='Will Smith')])].name").ToList();
        Assert.NotNull(t);
        Assert.Equal(2, t.Count);
        Assert.Equal("Bad Boys", t[0].GetValue<string>());
        Assert.Equal("Independence Day", t[1].GetValue<string>());
    }

    [Fact]
    public void MultiplePaths()
    {
        var a = JsonNode.Parse(@"[
              {
                ""price"": 199,
                ""max_price"": 200
              },
              {
                ""price"": 200,
                ""max_price"": 200
              },
              {
                ""price"": 201,
                ""max_price"": 200
              }
            ]");

        var results = a.SelectNodes("[?(@.price > @.max_price)]").ToList();
        Assert.Equal(1, results.Count);
        Assert.True(a[2].DeepEquals(results[0]));
    }

    [Fact]
    public void Exists_True()
    {
        var a = JsonNode.Parse(@"[
              {
                ""price"": 199,
                ""max_price"": 200
              },
              {
                ""price"": 200,
                ""max_price"": 200
              },
              {
                ""price"": 201,
                ""max_price"": 200
              }
            ]");

        var results = a.SelectNodes("[?(true)]").ToList();
        Assert.Equal(3, results.Count);
        Assert.True(a[0].DeepEquals(results[0]));
        Assert.True(a[1].DeepEquals(results[1]));
        Assert.True(a[2].DeepEquals(results[2]));
    }

    [Fact]
    public void Exists_Null()
    {
        var a = JsonNode.Parse(@"[
              {
                ""price"": 199,
                ""max_price"": 200
              },
              {
                ""price"": 200,
                ""max_price"": 200
              },
              {
                ""price"": 201,
                ""max_price"": 200
              }
            ]");

        var results = a.SelectNodes("[?(true)]").ToList();
        Assert.Equal(3, results.Count);
        Assert.True(a[0].DeepEquals(results[0]));
        Assert.True(a[1].DeepEquals(results[1]));
        Assert.True(a[2].DeepEquals(results[2]));
    }

    [Fact]
    public void WildcardWithProperty()
    {
        var o = JsonNode.Parse(@"{
            ""station"": 92000041000001,
            ""containers"": [
                {
                    ""id"": 1,
                    ""text"": ""Sort system"",
                    ""containers"": [
                        {
                            ""id"": ""2"",
                            ""text"": ""Yard 11""
                        },
                        {
                            ""id"": ""92000020100006"",
                            ""text"": ""Sort yard 12""
                        },
                        {
                            ""id"": ""92000020100005"",
                            ""text"": ""Yard 13""
                        }
                    ]
                },
                {
                    ""id"": ""92000020100011"",
                    ""text"": ""TSP-1""
                },
                {
                    ""id"":""92000020100007"",
                    ""text"": ""Passenger 15""
                }
            ]
        }");

        var tokens = o.SelectNodes("$..*[?(@.text)]").ToList();
        int i = 0;
        Assert.Equal("Sort system", tokens[i++]["text"].GetString());
        Assert.Equal("TSP-1", tokens[i++]["text"].GetString());
        Assert.Equal("Passenger 15", tokens[i++]["text"].GetString());
        Assert.Equal("Yard 11", tokens[i++]["text"].GetString());
        Assert.Equal("Sort yard 12", tokens[i++]["text"].GetString());
        Assert.Equal("Yard 13", tokens[i++]["text"].GetString());
        Assert.Equal(6, tokens.Count);
    }

    [Fact]
    public void QueryAgainstNonStringValues()
    {
        IList<object> values = new List<object>
                        {
                            "ff2dc672-6e15-4aa2-afb0-18f4f69596ad",
                            new Guid("ff2dc672-6e15-4aa2-afb0-18f4f69596ad"),
                            "http://localhost",
                            new Uri("http://localhost"),
                            "2000-12-05T05:07:59Z",
                            new DateTime(2000, 12, 5, 5, 7, 59, DateTimeKind.Utc),
            #if !NET20
                            "2000-12-05T05:07:59-10:00",
                            new DateTimeOffset(2000, 12, 5, 5, 7, 59, -TimeSpan.FromHours(10)),
            #endif
                            "SGVsbG8gd29ybGQ=",
                            Encoding.UTF8.GetBytes("Hello world"),
                            "365.23:59:59",
                            new TimeSpan(365, 23, 59, 59)
                        };
        var json = @"{
                ""prop"": [ " +
             String.Join(", ", values.Select(v => $"{{\"childProp\": {JsonSerializer.Serialize(v)}}}")) +
         @"]
            }";
        var o = JsonNode.Parse(json);

        var t = o.SelectNodes("$.prop[?(@.childProp =='ff2dc672-6e15-4aa2-afb0-18f4f69596ad')]").ToList();
        Assert.Equal(2, t.Count);

        t = o.SelectNodes("$.prop[?(@.childProp =='http://localhost')]").ToList();
        Assert.Equal(2, t.Count);

        t = o.SelectNodes("$.prop[?(@.childProp =='2000-12-05T05:07:59Z')]").ToList();
        Assert.Equal(2, t.Count);

#if !NET20
        t = o.SelectNodes("$.prop[?(@.childProp =='2000-12-05T05:07:59-10:00')]").ToList();
        Assert.Equal(2, t.Count);
#endif

        t = o.SelectNodes("$.prop[?(@.childProp =='SGVsbG8gd29ybGQ=')]").ToList();
        Assert.Equal(2, t.Count);

        t = o.SelectNodes("$.prop[?(@.childProp =='365.23:59:59')]").ToList();

        /*
           Dotnet 6.0 JsonNode Parse the TimeSpan as string '365.23:59:59'
         */
#if NET6_0_OR_GREATER

        Assert.Equal(2, t.Count);
#else
        Assert.Equal(1, t.Count);
#endif
    }

    [Fact]
    public void Example()
    {
        var o = JsonNode.Parse(@"{
            ""Stores"": [
              ""Lambton Quay"",
              ""Willis Street""
            ],
            ""Manufacturers"": [
              {
                ""Name"": ""Acme Co"",
                ""Products"": [
                  {
                    ""Name"": ""Anvil"",
                    ""Price"": 50
                  }
                ]
              },
              {
                ""Name"": ""Contoso"",
                ""Products"": [
                  {
                    ""Name"": ""Elbow Grease"",
                    ""Price"": 99.95
                  },
                  {
                    ""Name"": ""Headlight Fluid"",
                    ""Price"": 4
                  }
                ]
              }
            ]
          }");

        string? name = o.SelectNode("Manufacturers[0].Name").GetValue<string>();
        // Acme Co

        decimal? productPrice = o.SelectNode("Manufacturers[0].Products[0].Price").GetValue<decimal>();
        // 50

        string? productName = o.SelectNode("Manufacturers[1].Products[0].Name").GetValue<string>();
        // Elbow Grease

        Assert.Equal("Acme Co", name);
        Assert.Equal(50m, productPrice);
        Assert.Equal("Elbow Grease", productName);

        IList<string> storeNames = ((JsonArray)o.SelectNode("Stores"))!.Select(s => s.GetString()).ToList();
        // Lambton Quay
        // Willis Street

        IList<string?> firstProductNames = ((JsonArray)o["Manufacturers"])!.Select(
            m => m.SelectNode("Products[1].Name")?.GetString()).ToList();
        // null
        // Headlight Fluid

        decimal totalPrice = ((JsonArray)o["Manufacturers"])!.Aggregate(
            0M, (sum, m) => sum + m.SelectNode("Products[0].Price").GetValue<decimal>());
        // 149.95

        Assert.Equal(2, storeNames.Count);
        Assert.Equal("Lambton Quay", storeNames[0]);
        Assert.Equal("Willis Street", storeNames[1]);
        Assert.Equal(2, firstProductNames.Count);
        Assert.Equal(null, firstProductNames[0]);
        Assert.Equal("Headlight Fluid", firstProductNames[1]);
        Assert.Equal(149.95m, totalPrice);
    }

    [Fact]
    public void NotEqualsAndNonPrimativeValues()
    {
        string json = @"[
              {
                ""name"": ""string"",
                ""value"": ""aString""
              },
              {
                ""name"": ""number"",
                ""value"": 123
              },
              {
                ""name"": ""array"",
                ""value"": [
                  1,
                  2,
                  3,
                  4
                ]
              },
              {
                ""name"": ""object"",
                ""value"": {
                  ""1"": 1
                }
              }
            ]";

        var a = JsonNode.Parse(json);

        var result = a.SelectNodes("$.[?(@.value!=1)]").ToList();
        Assert.Equal(4, result.Count);

        result = a.SelectNodes("$.[?(@.value!='2000-12-05T05:07:59-10:00')]").ToList();
        Assert.Equal(4, result.Count);

        result = a.SelectNodes("$.[?(@.value!=null)]").ToList();
        Assert.Equal(4, result.Count);

        result = a.SelectNodes("$.[?(@.value!=123)]").ToList();
        Assert.Equal(3, result.Count);

        result = a.SelectNodes("$.[?(@.value)]").ToList();
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void RootInFilter()
    {
        string json = @"[
               {
                  ""store"" : {
                     ""book"" : [
                        {
                           ""category"" : ""reference"",
                           ""author"" : ""Nigel Rees"",
                           ""title"" : ""Sayings of the Century"",
                           ""price"" : 8.95
                        },
                        {
                           ""category"" : ""fiction"",
                           ""author"" : ""Evelyn Waugh"",
                           ""title"" : ""Sword of Honour"",
                           ""price"" : 12.99
                        },
                        {
                           ""category"" : ""fiction"",
                           ""author"" : ""Herman Melville"",
                           ""title"" : ""Moby Dick"",
                           ""isbn"" : ""0-553-21311-3"",
                           ""price"" : 8.99
                        },
                        {
                           ""category"" : ""fiction"",
                           ""author"" : ""J. R. R. Tolkien"",
                           ""title"" : ""The Lord of the Rings"",
                           ""isbn"" : ""0-395-19395-8"",
                           ""price"" : 22.99
                        }
                     ],
                     ""bicycle"" : {
                        ""color"" : ""red"",
                        ""price"" : 19.95
                     }
                  },
                  ""expensive"" : 10
               }
            ]";

        var a = JsonNode.Parse(json);

        var result = a.SelectNodes("$.[?($.[0].store.bicycle.price < 20)]").ToList();
        Assert.Equal(1, result.Count);

        result = a.SelectNodes("$.[?($.[0].store.bicycle.price < 10)]").ToList();
        Assert.Equal(0, result.Count);
    }

    [Fact]
    public void RootInFilterWithRootObject()
    {
        string json = @"{
                ""store"" : {
                    ""book"" : [
                        {
                            ""category"" : ""reference"",
                            ""author"" : ""Nigel Rees"",
                            ""title"" : ""Sayings of the Century"",
                            ""price"" : 8.95
                        },
                        {
                            ""category"" : ""fiction"",
                            ""author"" : ""Evelyn Waugh"",
                            ""title"" : ""Sword of Honour"",
                            ""price"" : 12.99
                        },
                        {
                            ""category"" : ""fiction"",
                            ""author"" : ""Herman Melville"",
                            ""title"" : ""Moby Dick"",
                            ""isbn"" : ""0-553-21311-3"",
                            ""price"" : 8.99
                        },
                        {
                            ""category"" : ""fiction"",
                            ""author"" : ""J. R. R. Tolkien"",
                            ""title"" : ""The Lord of the Rings"",
                            ""isbn"" : ""0-395-19395-8"",
                            ""price"" : 22.99
                        }
                    ],
                    ""bicycle"" : [
                        {
                            ""color"" : ""red"",
                            ""price"" : 19.95
                        }
                    ]
                },
                ""expensive"" : 10
            }";

        JsonNode a = JsonNode.Parse(json);

        var result = a.SelectNodes("$..book[?(@.price <= $['expensive'])]").ToList();
        Assert.Equal(2, result.Count);

        result = a.SelectNodes("$.store..[?(@.price > $.expensive)]").ToList();
        Assert.Equal(3, result.Count);
    }

    public const string IsoDateFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";

    [Fact]
    public void RootInFilterWithInitializers()
    {
        var minDate = DateTime.MinValue.ToString(IsoDateFormat);

        JsonNode rootObject = JsonNode.Parse(@"
            {
                ""referenceDate"": """ + minDate + @""",
                ""dateObjectsArray"": [
                    { ""date"": """ + minDate + @""" },
                    { ""date"": """ + DateTime.MaxValue.ToString(IsoDateFormat) + @""" },
                    { ""date"": """ + DateTime.Now.ToString(IsoDateFormat) + @""" },
                    { ""date"": """ + minDate + @""" }
                ]
            }");

        var result = rootObject.SelectNodes("$.dateObjectsArray[?(@.date == $.referenceDate)]").ToList();
        Assert.Equal(2, result.Count);
    }
}