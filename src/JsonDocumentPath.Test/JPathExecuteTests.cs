using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xunit;

namespace JDocument.Test
{
    public class JPathExecuteTests
    {
        [Fact]
        public void GreaterThanIssue1518()
        {
            string statusJson = @"{""usingmem"": ""214376""}";//214,376
            var jObj = JsonDocument.Parse(statusJson).RootElement;

            var aa = jObj.SelectElement("$..[?(@.usingmem>10)]");//found,10
            Assert.Equal(jObj, aa);

            var bb = jObj.SelectElement("$..[?(@.usingmem>27000)]");//null, 27,000
            Assert.Equal(jObj, bb);

            var cc = jObj.SelectElement("$..[?(@.usingmem>21437)]");//found, 21,437
            Assert.Equal(jObj, cc);

            var dd = jObj.SelectElement("$..[?(@.usingmem>21438)]");//null,21,438
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

            var models = JsonDocument.Parse(json).RootElement;

            var results = models.SelectElements("$.persons[?(@.age > 3)]").ToList();

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

            var models = JsonDocument.Parse(json).RootElement;

            var results = models.SelectElements("$.persons[?(@.age > '3')]").ToList();

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

            var models = JsonDocument.Parse(json).RootElement;
            var results = models.SelectElements("$.b..*.id").ToList();

            Assert.Equal(3, results.Count);
            Assert.Equal(2, results[0].Value.GetInt32());
            Assert.Equal(3, results[1].Value.GetInt32());
            Assert.Equal(4, results[2].Value.GetInt32());
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

            var models = JsonDocument.Parse(json).RootElement;
            var results = models.SelectElements("$.elements..[?(@.id=='AAA')]").ToList();
            Assert.Equal(1, results.Count);
            Assert.Equal(models.GetProperty("elements")[0].GetProperty("children")[0].GetProperty("children")[0], results[0]);
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

            var models = JsonDocument.Parse(json).RootElement;

            var results = models.SelectElements("$.elements[?(true)]").ToList();

            Assert.Equal(2, results.Count);
            Assert.Equal(results[0], models.GetProperty("elements")[0]);
            Assert.Equal(results[1], models.GetProperty("elements")[1]);
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

            var models = JsonDocument.Parse(json).RootElement;

            var results = models.SelectElements("$.elements..[?(true)]").ToList();

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

            var models = JsonDocument.Parse(json).RootElement;
            var results = models.SelectElements("$.elements..[?(@.id=='AA')]").ToList();

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

            var models = JsonDocument.Parse(json).RootElement;

            int result = models.SelectElements("$..['My.Child.Node']").Count();
            Assert.Equal(1, result);

            result = models.SelectElements("..['My.Child.Node']").Count();
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

            var models = JsonDocument.Parse(json).RootElement;

            var results = models.SelectElements("$..['My.Child.Node','Prop1','Prop2']").ToList();
            Assert.Equal("Val1", results[0].Value.GetString());
            Assert.Equal("Val2", results[1].Value.GetString());
            Assert.Equal(JsonValueKind.Object, results[2].Value.ValueKind);
            Assert.Equal("Val3", results[3].Value.GetString());
            Assert.Equal("Val4", results[4].Value.GetString());
            Assert.Equal("Val5", results[5].Value.GetString());
            Assert.Equal("Val6", results[6].Value.GetString());
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
            var document = JsonDocument.Parse(json).RootElement;
            var elements = document.SelectElements("$..en-US").ToList();

            Assert.Equal(3, elements.Count);
            Assert.Equal("Add", elements[0].Value.GetString());
            Assert.Equal("Sort by", elements[1].Value.GetString());
            Assert.Equal("Name", elements[2].Value.GetString());
        }

        [Fact]
        public void SelectElementAfterEmptyContainer()
        {
            string json = @"{
                    ""cont"": [],
                    ""test"": ""no one will find me""
                }";

            var document = JsonDocument.Parse(json).RootElement;

            var results = document.SelectElements("$..test").ToList();

            Assert.Equal(1, results.Count);
            Assert.Equal("no one will find me", results[0].Value.GetString());
        }

        [Fact]
        public void EvaluatePropertyWithRequired()
        {
            string json = "{\"bookId\":\"1000\"}";
            var document = JsonDocument.Parse(json).RootElement;

            string bookId = (string)document.SelectElement("bookId", true).Value.GetString();

            Assert.Equal("1000", bookId);
        }

        [Fact]
        public void EvaluateEmptyPropertyIndexer()
        {
            string json = @"{
                    """": 1
                }";

            var document = JsonDocument.Parse(json).RootElement;

            var t = document.SelectElement("['']");
            Assert.Equal(1, t.Value.GetInt32());
        }

        [Fact]
        public void EvaluateEmptyString()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;
            var t = document.SelectElement("");
            Assert.Equal(document, t);

            t = document.SelectElement("['']");
            Assert.Equal(null, t);
        }

        [Fact]
        public void EvaluateEmptyStringWithMatchingEmptyProperty()
        {
            string json = @"{
                    "" "": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            var t = document.SelectElement("[' ']");
            Assert.Equal(1, t.Value.GetInt32());
        }

        [Fact]
        public void EvaluateWhitespaceString()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            var t = document.SelectElement(" ");
            Assert.Equal(document, t);
        }

        [Fact]
        public void EvaluateDollarString()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            var t = document.SelectElement("$");
            Assert.Equal(document, t);
        }

        [Fact]
        public void EvaluateDollarTypeString()
        {
            string json = @"{
                    ""$values"": [1,2,3]
                }";
            var document = JsonDocument.Parse(json).RootElement;

            var t = document.SelectElement("$values[1]");
            Assert.Equal(2, t.Value.GetInt32());
        }

        [Fact]
        public void EvaluateSingleProperty()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            var t = document.SelectElement("Blah");
            Assert.NotNull(t);
            Assert.Equal(JsonValueKind.Number, t.Value.ValueKind);
            Assert.Equal(1, t.Value.GetInt32());
        }

        [Fact]
        public void EvaluateWildcardProperty()
        {
            string json = @"{
                    ""Blah"": 1,
                    ""Blah2"": 2
                }";
            var document = JsonDocument.Parse(json).RootElement;

            var t = document.SelectElements("$.*").ToList();
            Assert.NotNull(t);
            Assert.Equal(2, t.Count);
            Assert.Equal(1, t[0].Value.GetInt32());
            Assert.Equal(2, t[1].Value.GetInt32());
        }

        [Fact]
        public void QuoteName()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            var t = document.SelectElement("['Blah']");
            Assert.NotNull(t);
            Assert.Equal(JsonValueKind.Number, t.Value.ValueKind);
            Assert.Equal(1, t.Value.GetInt32());
        }

        [Fact]
        public void EvaluateMissingProperty()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            var t = document.SelectElement("Missing[1]");
            Assert.Null(t);
        }

        [Fact]
        public void EvaluateIndexerOnObject()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            var t = document.SelectElement("[1]");
            Assert.Null(t);
        }

        [Fact]
        public void EvaluateIndexerOnObjectWithError()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            ExceptionAssert.Throws<JsonException>(() => { document.SelectElement("[1]", true); }, @"Index 1 not valid on JsonElement.");
        }

        [Fact]
        public void EvaluateWildcardIndexOnObjectWithError()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            ExceptionAssert.Throws<JsonException>(() => { document.SelectElement("[*]", true); }, @"Index * not valid on JsonElement.");
        }

        [Fact]
        public void EvaluateSliceOnObjectWithError()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            ExceptionAssert.Throws<JsonException>(() => { document.SelectElement("[:]", true); }, @"Array slice is not valid on JsonElement.");
        }

        [Fact]
        public void EvaluatePropertyOnArray()
        {
            string json = @"[1,2,3,4,5]";
            var document = JsonDocument.Parse(json).RootElement;

            var t = document.SelectElement("BlahBlah");
            Assert.Null(t);
        }

        [Fact]
        public void EvaluateMultipleResultsError()
        {
            string json = @"[1,2,3,4,5]";
            var document = JsonDocument.Parse(json).RootElement;
            ExceptionAssert.Throws<JsonException>(() => { document.SelectElement("[0, 1]"); }, @"Path returned multiple tokens.");
        }

        [Fact]
        public void EvaluatePropertyOnArrayWithError()
        {
            string json = @"[1,2,3,4,5]";
            var document = JsonDocument.Parse(json).RootElement;

            ExceptionAssert.Throws<JsonException>(() => { document.SelectElement("BlahBlah", true); }, @"Property 'BlahBlah' not valid on JsonElement.");
        }

        [Fact]
        public void EvaluateNoResultsWithMultipleArrayIndexes()
        {
            string json = @"[1,2,3,4,5]";
            var document = JsonDocument.Parse(json).RootElement;

            ExceptionAssert.Throws<JsonException>(() => { document.SelectElement("[9,10]", true); }, @"Index 9 outside the bounds of JArray.");
        }

        [Fact]
        public void EvaluateMissingPropertyWithError()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            ExceptionAssert.Throws<JsonException>(() => { document.SelectElement("Missing", true); }, "Property 'Missing' does not exist on JsonElement.");
        }

        [Fact]
        public void EvaluatePropertyWithoutError()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            var v = document.SelectElement("Blah", true).Value.GetInt32();
            Assert.Equal(1, v);
        }

        [Fact]
        public void EvaluateMissingPropertyIndexWithError()
        {
            string json = @"{
                    ""Blah"": 1
                }";
            var document = JsonDocument.Parse(json).RootElement;

            ExceptionAssert.Throws<JsonException>(() => { document.SelectElement("['Missing','Missing2']", true); }, "Property 'Missing' does not exist on JObject.");
        }

        [Fact]
        public void EvaluateMultiPropertyIndexOnArrayWithError()
        {
            var a = JsonDocument.Parse("[1,2,3,4,5]").RootElement;

            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("['Missing','Missing2']", true); }, "Properties 'Missing', 'Missing2' not valid on JsonElement.");
        }

        [Fact]
        public void EvaluateArraySliceWithError()
        {
            var a = JsonDocument.Parse("[1,2,3,4,5]").RootElement;

            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[99:]", true); }, "Array slice of 99 to * returned no results.");

            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[1:-19]", true); }, "Array slice of 1 to -19 returned no results.");

            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[:-19]", true); }, "Array slice of * to -19 returned no results.");

            a = JsonDocument.Parse("[]").RootElement;

            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[:]", true); }, "Array slice of * to * returned no results.");
        }

        [Fact]
        public void EvaluateOutOfBoundsIndxer()
        {
            var a = JsonDocument.Parse("[1,2,3,4,5]").RootElement;

            var t = a.SelectElement("[1000].Ha");
            Assert.Null(t);
        }

        [Fact]
        public void EvaluateArrayOutOfBoundsIndxerWithError()
        {
            var a = JsonDocument.Parse("[1,2,3,4,5]").RootElement;

            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[1000].Ha", true); }, "Index 1000 outside the bounds of JArray.");
        }

        [Fact]
        public void EvaluateArray()
        {
            var a = JsonDocument.Parse("[1,2,3,4]").RootElement;

            var t = a.SelectElement("[1]");
            Assert.NotNull(t);
            Assert.Equal(JsonValueKind.Number, t.Value.ValueKind);
            Assert.Equal(2, t.Value.GetInt32());
        }

        [Fact]
        public void EvaluateArraySlice()
        {
            var a = JsonDocument.Parse(@"[1, 2, 3, 4, 5, 6, 7, 8, 9]").RootElement;
            List<JsonElement?> t = null;

            t = a.SelectElements("[-3:]").ToList();
            Assert.Equal(3, t.Count);
            Assert.Equal(7, t[0].Value.GetInt32());
            Assert.Equal(8, t[1].Value.GetInt32());
            Assert.Equal(9, t[2].Value.GetInt32());

            t = a.SelectElements("[-1:-2:-1]").ToList();
            Assert.Equal(1, t.Count);
            Assert.Equal(9, t[0].Value.GetInt32());

            t = a.SelectElements("[-2:-1]").ToList();
            Assert.Equal(1, t.Count);
            Assert.Equal(8, t[0].Value.GetInt32());

            t = a.SelectElements("[1:1]").ToList();
            Assert.Equal(0, t.Count);

            t = a.SelectElements("[1:2]").ToList();
            Assert.Equal(1, t.Count);
            Assert.Equal(2, t[0].Value.GetInt32());

            t = a.SelectElements("[::-1]").ToList();
            Assert.Equal(9, t.Count);
            Assert.Equal(9, t[0].Value.GetInt32());
            Assert.Equal(8, t[1].Value.GetInt32());
            Assert.Equal(7, t[2].Value.GetInt32());
            Assert.Equal(6, t[3].Value.GetInt32());
            Assert.Equal(5, t[4].Value.GetInt32());
            Assert.Equal(4, t[5].Value.GetInt32());
            Assert.Equal(3, t[6].Value.GetInt32());
            Assert.Equal(2, t[7].Value.GetInt32());
            Assert.Equal(1, t[8].Value.GetInt32());

            t = a.SelectElements("[::-2]").ToList();
            Assert.Equal(5, t.Count);
            Assert.Equal(9, t[0].Value.GetInt32());
            Assert.Equal(7, t[1].Value.GetInt32());
            Assert.Equal(5, t[2].Value.GetInt32());
            Assert.Equal(3, t[3].Value.GetInt32());
            Assert.Equal(1, t[4].Value.GetInt32());
        }

        [Fact]
        public void EvaluateWildcardArray()
        {
            var a = JsonDocument.Parse(@"[1, 2, 3, 4]").RootElement;

            List<JsonElement?> t = a.SelectElements("[*]").ToList();
            Assert.NotNull(t);
            Assert.Equal(4, t.Count);
            Assert.Equal(1, t[0].Value.GetInt32());
            Assert.Equal(2, t[1].Value.GetInt32());
            Assert.Equal(3, t[2].Value.GetInt32());
            Assert.Equal(4, t[3].Value.GetInt32());
        }

        [Fact]
        public void EvaluateArrayMultipleIndexes()
        {
            var a = JsonDocument.Parse(@"[1, 2, 3, 4]");

            IEnumerable<JsonElement?> t = a.SelectElements("[1,2,0]").ToList();
            Assert.NotNull(t);
            Assert.Equal(3, t.Count());
            Assert.Equal(2, t.ElementAt(0).Value.GetInt32());
            Assert.Equal(3, t.ElementAt(1).Value.GetInt32());
            Assert.Equal(1, t.ElementAt(2).Value.GetInt32());
        }

        [Fact]
        public void EvaluateScan()
        {
            JsonDocument o1 = JsonDocument.Parse(@"{ ""Name"": 1 }");
            JsonDocument o2 = JsonDocument.Parse(@"{ ""Name"": 2 }");
            var a = JsonDocument.Parse(@"[{ ""Name"": 1 }, { ""Name"": 2 }]");

            var t = a.SelectElements("$..Name").ToList();
            Assert.NotNull(t);
            Assert.Equal(2, t.Count);
            Assert.Equal(1, t[0].Value.GetInt32());
            Assert.Equal(2, t[1].Value.GetInt32());
        }

        [Fact]
        public void EvaluateWildcardScan()
        {
            JsonDocument o1 = JsonDocument.Parse(@"{ ""Name"": 1 }");
            JsonDocument o2 = JsonDocument.Parse(@"{ ""Name"": 2 }");
            var a = JsonDocument.Parse(@"[{ ""Name"": 1 }, { ""Name"": 2 }]");

            var t = a.SelectElements("$..*").ToList();
            Assert.NotNull(t);
            Assert.Equal(5, t.Count);
            Assert.True(a.DeepEquals(t[0].Value));
            Assert.True(o1.DeepEquals(t[1].Value));
            Assert.Equal(1, t[2].Value.GetInt32());
            Assert.True(o2.DeepEquals(t[3].Value));
            Assert.Equal(2, t[4].Value.GetInt32());
        }

        [Fact]
        public void EvaluateScanNestResults()
        {
            JsonDocument o1 = JsonDocument.Parse(@"{ ""Name"": 1 }");
            JsonDocument o2 = JsonDocument.Parse(@"{ ""Name"": 2 }");
            JsonDocument o3 = JsonDocument.Parse(@"{ ""Name"": { ""Name"": [ 3 ] } }");
            var a = JsonDocument.Parse(@"[
                { ""Name"": 1 },
                { ""Name"": 2 },
                { ""Name"": { ""Name"": [3] } }
            ]");

            var t = a.SelectElements("$..Name").ToList();
            Assert.NotNull(t);
            Assert.Equal(4, t.Count);
            Assert.Equal(1, t[0].Value.GetInt32());
            Assert.Equal(2, t[1].Value.GetInt32());
            Assert.True(JsonDocument.Parse(@"{ ""Name"": [3] }").DeepEquals(t[2].Value));
            Assert.True(JsonDocument.Parse("[3]").DeepEquals(t[3].Value));
        }

        [Fact]
        public void EvaluateWildcardScanNestResults()
        {
            JsonDocument o1 = JsonDocument.Parse(@"{ ""Name"": 1 }");
            JsonDocument o2 = JsonDocument.Parse(@"{ ""Name"": 2 }");
            JsonDocument o3 = JsonDocument.Parse(@"{ ""Name"": { ""Name"": [3] } }");
            var a = JsonDocument.Parse(@"[
                { ""Name"": 1 },
                { ""Name"": 2 },
                { ""Name"": { ""Name"": [3] } }
            ]");

            var t = a.SelectElements("$..*").ToList();
            Assert.NotNull(t);
            Assert.Equal(9, t.Count);

            Assert.True(a.DeepEquals(t[0].Value));
            Assert.True(o1.DeepEquals(t[1].Value));
            Assert.Equal(1, t[2].Value.GetInt32());
            Assert.True(o2.DeepEquals(t[3]));
            Assert.Equal(2, t[4].Value.GetInt32());
            Assert.True(o3.DeepEquals(t[5]));
            Assert.True(JsonDocument.Parse(@"{ ""Name"": [3] }").DeepEquals(t[6].Value));
            Assert.True(JsonDocument.Parse("[3]").DeepEquals(t[7].Value));
            Assert.Equal(3, t[8].Value.GetInt32());
            Assert.True(JsonDocument.Parse("[3]").DeepEquals(t[7].Value));
        }

        [Fact]
        public void EvaluateSinglePropertyReturningArray()
        {
            var o = JsonDocument.Parse(@"{ ""Blah"": [ 1, 2, 3 ] }");

            var t = o.SelectElement("Blah");
            Assert.NotNull(t);
            Assert.Equal(JsonValueKind.Array, t?.ValueKind);

            t = o.SelectElement("Blah[2]");
            Assert.Equal(JsonValueKind.Number, t?.ValueKind);
            Assert.Equal(3, t?.GetInt32());
        }

        [Fact]
        public void EvaluateLastSingleCharacterProperty()
        {
            JsonDocument o2 = JsonDocument.Parse(@"{""People"":[{""N"":""Jeff""}]}");
            var a2 = o2.SelectElement("People[0].N").Value.GetString();

            Assert.Equal("Jeff", a2);
        }

        [Fact]
        public void ExistsQuery()
        {
            var a = JsonDocument.Parse(@"[
                { ""hi"": ""ho"" },
                { ""hi2"": ""ha"" }
            ]");

            var t = a.SelectElements("[ ?( @.hi ) ]").ToList();
            Assert.NotNull(t);
            Assert.Equal(1, t.Count);
            Assert.True(JsonDocument.Parse(@"{ ""hi"": ""ho"" }").DeepEquals(t[0].Value));
        }

        [Fact]
        public void EqualsQuery()
        {
            var a = JsonDocument.Parse(@"[
                { ""hi"": ""ho"" },
                { ""hi"": ""ha"" }
            ]");

            var t = a.SelectElements("[ ?( @.['hi'] == 'ha' ) ]").ToList();
            Assert.NotNull(t);
            Assert.Equal(1, t.Count);
            Assert.True(JsonDocument.Parse(@"{ ""hi"": ""ha"" }").DeepEquals(t[0].Value));
        }

        [Fact]
        public void NotEqualsQuery()
        {
            var a = JsonDocument.Parse(@"[
                { ""hi"": ""ho"" },
                { ""hi"": ""ha"" }
            ]");

            var t = a.SelectElements("[ ?( @..hi <> 'ha' ) ]").ToList();
            Assert.NotNull(t);
            Assert.Equal(1, t.Count);
            Assert.True(JsonDocument.Parse(@"{ ""hi"": ""ho"" }").DeepEquals(t[0].Value));
        }

        [Fact]
        public void NoPathQuery()
        {
            var a = JsonDocument.Parse("[1, 2, 3]");

            var t = a.SelectElements("[ ?( @ > 1 ) ]").ToList();
            Assert.NotNull(t);
            Assert.Equal(2, t.Count);
            Assert.Equal(2, t[0].Value.GetInt32());
            Assert.Equal(3, t[1].Value.GetInt32());
        }

        [Fact]
        public void MultipleQueries()
        {
            var a = JsonDocument.Parse("[1, 2, 3, 4, 5, 6, 7, 8, 9]");

            // json path does item based evaluation - http://www.sitepen.com/blog/2008/03/17/jsonpath-support/
            // first query resolves array to ints
            // int has no children to query
            var t = a.SelectElements("[?(@ <> 1)][?(@ <> 4)][?(@ < 7)]").ToList();
            Assert.NotNull(t);
            Assert.Equal(0, t.Count);
        }

        [Fact]
        public void GreaterQuery()
        {
            var a = JsonDocument.Parse(@"
            [
                { ""hi"": 1 },
                { ""hi"": 2 },
                { ""hi"": 3 }
            ]");

            var t = a.SelectElements("[ ?( @.hi > 1 ) ]").ToList();
            Assert.NotNull(t);
            Assert.Equal(2, t.Count);
            Assert.True(JsonDocument.Parse(@"{ ""hi"": 2 }").DeepEquals(t[0].Value));
            Assert.True(JsonDocument.Parse(@"{ ""hi"": 3 }").DeepEquals(t[1].Value));
        }

        [Fact]
        public void LesserQuery_ValueFirst()
        {
            var a = JsonDocument.Parse(@"
            [
                { ""hi"": 1 },
                { ""hi"": 2 },
                { ""hi"": 3 }
            ]");

            var t = a.SelectElements("[ ?( 1 < @.hi ) ]").ToList();
            Assert.NotNull(t);
            Assert.Equal(2, t.Count);
            Assert.True(JsonDocument.Parse(@"{ ""hi"": 2 }").DeepEquals(t[0].Value));
            Assert.True(JsonDocument.Parse(@"{ ""hi"": 3 }").DeepEquals(t[1].Value));
        }

        [Fact]
        public void GreaterOrEqualQuery()
        {
            var a = JsonDocument.Parse(@"
            [
                { ""hi"": 1 },
                { ""hi"": 2 },
                { ""hi"": 2.0 },
                { ""hi"": 3 }
            ]");

            var t = a.SelectElements("[ ?( @.hi >= 1 ) ]").ToList();
            Assert.NotNull(t);
            Assert.Equal(4, t.Count);
            Assert.True(JsonDocument.Parse(@"{ ""hi"": 1 }").DeepEquals(t[0].Value));
            Assert.True(JsonDocument.Parse(@"{ ""hi"": 2 }").DeepEquals(t[1].Value));
            Assert.True(JsonDocument.Parse(@"{ ""hi"": 2.0 }").DeepEquals(t[2].Value));
            Assert.True(JsonDocument.Parse(@"{ ""hi"": 3 }").DeepEquals(t[3].Value));
        }

        [Fact]
        public void NestedQuery()
        {
            var a = JsonDocument.Parse(@"
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

            var t = a.SelectElements("[?(@.cast[?(@.name=='Will Smith')])].name").ToList();
            Assert.NotNull(t);
            Assert.Equal(2, t.Count);
            Assert.Equal("Bad Boys", t[0].Value.GetString());
            Assert.Equal("Independence Day", t[1].Value.GetString());
        }

        [Fact]
        public void MultiplePaths()
        {
            var a = JsonDocument.Parse(@"[
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

            var results = a.SelectElements("[?(@.price > @.max_price)]").ToList();
            Assert.Equal(1, results.Count);
            Assert.True(a.RootElement[2].DeepEquals(results[0].Value));
        }

        [Fact]
        public void Exists_True()
        {
            var a = JsonDocument.Parse(@"[
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

            var results = a.SelectElements("[?(true)]").ToList();
            Assert.Equal(3, results.Count);
            Assert.True(a.RootElement[0].DeepEquals(results[0].Value));
            Assert.True(a.RootElement[1].DeepEquals(results[1].Value));
            Assert.True(a.RootElement[2].DeepEquals(results[2].Value));
        }

        [Fact]
        public void Exists_Null()
        {
            var a = JsonDocument.Parse(@"[
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

            var results = a.SelectElements("[?(true)]").ToList();
            Assert.Equal(3, results.Count);
            Assert.True(a.RootElement[0].DeepEquals(results[0].Value));
            Assert.True(a.RootElement[1].DeepEquals(results[1].Value));
            Assert.True(a.RootElement[2].DeepEquals(results[2].Value));
        }

        [Fact]
        public void WildcardWithProperty()
        {
            var o = JsonDocument.Parse(@"{
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

            var tokens = o.SelectElements("$..*[?(@.text)]").ToList();
            int i = 0;
            Assert.Equal("Sort system", tokens[i++].Value.GetProperty("text").GetString());
            Assert.Equal("TSP-1", tokens[i++].Value.GetProperty("text").GetString());
            Assert.Equal("Passenger 15", tokens[i++].Value.GetProperty("text").GetString());
            Assert.Equal("Yard 11", tokens[i++].Value.GetProperty("text").GetString());
            Assert.Equal("Sort yard 12", tokens[i++].Value.GetProperty("text").GetString());
            Assert.Equal("Yard 13", tokens[i++].Value.GetProperty("text").GetString());
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
            var o = JsonDocument.Parse(json);

            var t = o.SelectElements("$.prop[?(@.childProp =='ff2dc672-6e15-4aa2-afb0-18f4f69596ad')]").ToList();
            Assert.Equal(2, t.Count);

            t = o.SelectElements("$.prop[?(@.childProp =='http://localhost')]").ToList();
            Assert.Equal(2, t.Count);

            t = o.SelectElements("$.prop[?(@.childProp =='2000-12-05T05:07:59Z')]").ToList();
            Assert.Equal(2, t.Count);

#if !NET20
            t = o.SelectElements("$.prop[?(@.childProp =='2000-12-05T05:07:59-10:00')]").ToList();
            Assert.Equal(2, t.Count);
#endif

            t = o.SelectElements("$.prop[?(@.childProp =='SGVsbG8gd29ybGQ=')]").ToList();
            Assert.Equal(2, t.Count);

            t = o.SelectElements("$.prop[?(@.childProp =='365.23:59:59')]").ToList();

            /*
               Dotnet 6.0 JsonDocument Parse the TimeSpan as string '365.23:59:59'
             */
#if NET6_0
            
            Assert.Equal(2, t.Count);
#else
            Assert.Equal(1, t.Count);
#endif

        }

        [Fact]
        public void Example()
        {
            var o = JsonDocument.Parse(@"{
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

            string? name = o.SelectElement("Manufacturers[0].Name").Value.GetString();
            // Acme Co

            decimal? productPrice = o.SelectElement("Manufacturers[0].Products[0].Price").Value.GetDecimal();
            // 50

            string? productName = o.SelectElement("Manufacturers[1].Products[0].Name").Value.GetString();
            // Elbow Grease

            Assert.Equal("Acme Co", name);
            Assert.Equal(50m, productPrice);
            Assert.Equal("Elbow Grease", productName);

            IList<string> storeNames = o.SelectElement("Stores")!.Value.EnumerateArray().Select(s => s.GetString()).ToList();
            // Lambton Quay
            // Willis Street

            IList<string?> firstProductNames = o.RootElement.GetProperty("Manufacturers")!.EnumerateArray().Select(
                m => m.SelectElement("Products[1].Name")?.GetString()).ToList();
            // null
            // Headlight Fluid

            decimal totalPrice = o.RootElement.GetProperty("Manufacturers")!.EnumerateArray().Aggregate(
                0M, (sum, m) => sum + m.SelectElement("Products[0].Price").Value.GetDecimal());
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

            var a = JsonDocument.Parse(json);

            var result = a.SelectElements("$.[?(@.value!=1)]").ToList();
            Assert.Equal(4, result.Count);

            result = a.SelectElements("$.[?(@.value!='2000-12-05T05:07:59-10:00')]").ToList();
            Assert.Equal(4, result.Count);

            result = a.SelectElements("$.[?(@.value!=null)]").ToList();
            Assert.Equal(4, result.Count);

            result = a.SelectElements("$.[?(@.value!=123)]").ToList();
            Assert.Equal(3, result.Count);

            result = a.SelectElements("$.[?(@.value)]").ToList();
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

            var a = JsonDocument.Parse(json);

            var result = a.SelectElements("$.[?($.[0].store.bicycle.price < 20)]").ToList();
            Assert.Equal(1, result.Count);

            result = a.SelectElements("$.[?($.[0].store.bicycle.price < 10)]").ToList();
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

            JsonDocument a = JsonDocument.Parse(json);

            var result = a.SelectElements("$..book[?(@.price <= $['expensive'])]").ToList();
            Assert.Equal(2, result.Count);

            result = a.SelectElements("$.store..[?(@.price > $.expensive)]").ToList();
            Assert.Equal(3, result.Count);
        }

        public const string IsoDateFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";
        [Fact]
        public void RootInFilterWithInitializers()
        {
            var minDate = DateTime.MinValue.ToString(IsoDateFormat);

            JsonDocument rootObject = JsonDocument.Parse(@"
            {
                ""referenceDate"": """ + minDate + @""",
                ""dateObjectsArray"": [
                    { ""date"": """ + minDate + @""" },
                    { ""date"": """ + DateTime.MaxValue.ToString(IsoDateFormat) + @""" },
                    { ""date"": """ + DateTime.Now.ToString(IsoDateFormat) + @""" },
                    { ""date"": """ + minDate + @""" }
                ]
            }");

            var result = rootObject.SelectElements("$.dateObjectsArray[?(@.date == $.referenceDate)]").ToList();
            Assert.Equal(2, result.Count);
        }
    }
}