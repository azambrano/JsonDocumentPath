using System.Linq;
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

        //        [Fact]
        //        public void RecursiveWildcard()
        //        {
        //            string json = @"{
        //    ""a"": [
        //        {
        //            ""id"": 1
        //        }
        //    ],
        //    ""b"": [
        //        {
        //            ""id"": 2
        //        },
        //        {
        //            ""id"": 3,
        //            ""c"": {
        //                ""id"": 4
        //            }
        //        }
        //    ],
        //    ""d"": [
        //        {
        //            ""id"": 5
        //        }
        //    ]
        //}";

        //            var models = JsonDocument.Parse(json).RootElement;

        //            var results = models.SelectElements("$.b..*.id").ToList();

        //            Assert.Equal(3, results.Count);
        //            Assert.Equal(2, (int)results[0]);
        //            Assert.Equal(3, (int)results[1]);
        //            Assert.Equal(4, (int)results[2]);
        //        }

        //        [Fact]
        //        public void ScanFilter()
        //        {
        //            string json = @"{
        //  ""elements"": [
        //    {
        //      ""id"": ""A"",
        //      ""children"": [
        //        {
        //          ""id"": ""AA"",
        //          ""children"": [
        //            {
        //              ""id"": ""AAA""
        //            },
        //            {
        //              ""id"": ""AAB""
        //            }
        //          ]
        //        },
        //        {
        //          ""id"": ""AB""
        //        }
        //      ]
        //    },
        //    {
        //      ""id"": ""B"",
        //      ""children"": []
        //    }
        //  ]
        //}";

        //            var models = JsonDocument.Parse(json).RootElement;

        //            var results = models.SelectElements("$.elements..[?(@.id=='AAA')]").ToList();

        //            Assert.Equal(1, results.Count);
        //            Assert.Equal(models["elements"][0]["children"][0]["children"][0], results[0]);
        //        }

        //        [Fact]
        //        public void FilterTrue()
        //        {
        //            string json = @"{
        //  ""elements"": [
        //    {
        //      ""id"": ""A"",
        //      ""children"": [
        //        {
        //          ""id"": ""AA"",
        //          ""children"": [
        //            {
        //              ""id"": ""AAA""
        //            },
        //            {
        //              ""id"": ""AAB""
        //            }
        //          ]
        //        },
        //        {
        //          ""id"": ""AB""
        //        }
        //      ]
        //    },
        //    {
        //      ""id"": ""B"",
        //      ""children"": []
        //    }
        //  ]
        //}";

        //            var models = JsonDocument.Parse(json).RootElement;

        //            var results = models.SelectElements("$.elements[?(true)]").ToList();

        //            Assert.Equal(2, results.Count);
        //            Assert.Equal(results[0], models["elements"][0]);
        //            Assert.Equal(results[1], models["elements"][1]);
        //        }

        //        [Fact]
        //        public void ScanFilterTrue()
        //        {
        //            string json = @"{
        //  ""elements"": [
        //    {
        //      ""id"": ""A"",
        //      ""children"": [
        //        {
        //          ""id"": ""AA"",
        //          ""children"": [
        //            {
        //              ""id"": ""AAA""
        //            },
        //            {
        //              ""id"": ""AAB""
        //            }
        //          ]
        //        },
        //        {
        //          ""id"": ""AB""
        //        }
        //      ]
        //    },
        //    {
        //      ""id"": ""B"",
        //      ""children"": []
        //    }
        //  ]
        //}";

        //            var models = JsonDocument.Parse(json).RootElement;

        //            var results = models.SelectElements("$.elements..[?(true)]").ToList();

        //            Assert.Equal(25, results.Count);
        //        }

        //        [Fact]
        //        public void ScanQuoted()
        //        {
        //            string json = @"{
        //    ""Node1"": {
        //        ""Child1"": {
        //            ""Name"": ""IsMe"",
        //            ""TargetNode"": {
        //                ""Prop1"": ""Val1"",
        //                ""Prop2"": ""Val2""
        //            }
        //        },
        //        ""My.Child.Node"": {
        //            ""TargetNode"": {
        //                ""Prop1"": ""Val1"",
        //                ""Prop2"": ""Val2""
        //            }
        //        }
        //    },
        //    ""Node2"": {
        //        ""TargetNode"": {
        //            ""Prop1"": ""Val1"",
        //            ""Prop2"": ""Val2""
        //        }
        //    }
        //}";

        //            var models = JsonDocument.Parse(json).RootElement;

        //            int result = models.SelectElements("$..['My.Child.Node']").Count();
        //            Assert.Equal(1, result);

        //            result = models.SelectElements("..['My.Child.Node']").Count();
        //            Assert.Equal(1, result);
        //        }

        //        [Fact]
        //        public void ScanMultipleQuoted()
        //        {
        //            string json = @"{
        //    ""Node1"": {
        //        ""Child1"": {
        //            ""Name"": ""IsMe"",
        //            ""TargetNode"": {
        //                ""Prop1"": ""Val1"",
        //                ""Prop2"": ""Val2""
        //            }
        //        },
        //        ""My.Child.Node"": {
        //            ""TargetNode"": {
        //                ""Prop1"": ""Val3"",
        //                ""Prop2"": ""Val4""
        //            }
        //        }
        //    },
        //    ""Node2"": {
        //        ""TargetNode"": {
        //            ""Prop1"": ""Val5"",
        //            ""Prop2"": ""Val6""
        //        }
        //    }
        //}";

        //            var models = JsonDocument.Parse(json).RootElement;

        //            var results = models.SelectElements("$..['My.Child.Node','Prop1','Prop2']").ToList();
        //            Assert.Equal("Val1", (string)results[0]);
        //            Assert.Equal("Val2", (string)results[1]);
        //            Assert.Equal(JTokenType.Object, results[2].Type);
        //            Assert.Equal("Val3", (string)results[3]);
        //            Assert.Equal("Val4", (string)results[4]);
        //            Assert.Equal("Val5", (string)results[5]);
        //            Assert.Equal("Val6", (string)results[6]);
        //        }

        //        [Fact]
        //        public void ParseWithEmptyArrayContent()
        //        {
        //            var json = @"{
        //    'controls': [
        //        {
        //            'messages': {
        //                'addSuggestion': {
        //                    'en-US': 'Add'
        //                }
        //            }
        //        },
        //        {
        //            'header': {
        //                'controls': []
        //            },
        //            'controls': [
        //                {
        //                    'controls': [
        //                        {
        //                            'defaultCaption': {
        //                                'en-US': 'Sort by'
        //                            },
        //                            'sortOptions': [
        //                                {
        //                                    'label': {
        //                                        'en-US': 'Name'
        //                                    }
        //                                }
        //                            ]
        //                        }
        //                    ]
        //                }
        //            ]
        //        }
        //    ]
        //}";
        //            JObject jToken = JObject.Parse(json);
        //            IList<JToken> tokens = jToken.SelectElements("$..en-US").ToList();

        //            Assert.Equal(3, tokens.Count);
        //            Assert.Equal("Add", (string)tokens[0]);
        //            Assert.Equal("Sort by", (string)tokens[1]);
        //            Assert.Equal("Name", (string)tokens[2]);
        //        }

        //        [Fact]
        //        public void SelectElementAfterEmptyContainer()
        //        {
        //            string json = @"{
        //    'cont': [],
        //    'test': 'no one will find me'
        //}";

        //            JObject o = JObject.Parse(json);

        //            IList<JToken> results = o.SelectElements("$..test").ToList();

        //            Assert.Equal(1, results.Count);
        //            Assert.Equal("no one will find me", (string)results[0]);
        //        }

        //        [Fact]
        //        public void EvaluatePropertyWithRequired()
        //        {
        //            string json = "{\"bookId\":\"1000\"}";
        //            JObject o = JObject.Parse(json);

        //            string bookId = (string)o.SelectElement("bookId", true);

        //            Assert.Equal("1000", bookId);
        //        }

        //        [Fact]
        //        public void EvaluateEmptyPropertyIndexer()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("", 1));

        //            JToken t = o.SelectElement("['']");
        //            Assert.Equal(1, (int)t);
        //        }

        //        [Fact]
        //        public void EvaluateEmptyString()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            JToken t = o.SelectElement("");
        //            Assert.Equal(o, t);

        //            t = o.SelectElement("['']");
        //            Assert.Equal(null, t);
        //        }

        //        [Fact]
        //        public void EvaluateEmptyStringWithMatchingEmptyProperty()
        //        {
        //            JObject o = new JObject(
        //                new JProperty(" ", 1));

        //            JToken t = o.SelectElement("[' ']");
        //            Assert.Equal(1, (int)t);
        //        }

        //        [Fact]
        //        public void EvaluateWhitespaceString()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            JToken t = o.SelectElement(" ");
        //            Assert.Equal(o, t);
        //        }

        //        [Fact]
        //        public void EvaluateDollarString()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            JToken t = o.SelectElement("$");
        //            Assert.Equal(o, t);
        //        }

        //        [Fact]
        //        public void EvaluateDollarTypeString()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("$values", new JArray(1, 2, 3)));

        //            JToken t = o.SelectElement("$values[1]");
        //            Assert.Equal(2, (int)t);
        //        }

        //        [Fact]
        //        public void EvaluateSingleProperty()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            JToken t = o.SelectElement("Blah");
        //            Assert.IsNotNull(t);
        //            Assert.Equal(JTokenType.Integer, t.Type);
        //            Assert.Equal(1, (int)t);
        //        }

        //        [Fact]
        //        public void EvaluateWildcardProperty()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1),
        //                new JProperty("Blah2", 2));

        //            IList<JToken> t = o.SelectElements("$.*").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(2, t.Count);
        //            Assert.Equal(1, (int)t[0]);
        //            Assert.Equal(2, (int)t[1]);
        //        }

        //        [Fact]
        //        public void QuoteName()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            JToken t = o.SelectElement("['Blah']");
        //            Assert.IsNotNull(t);
        //            Assert.Equal(JTokenType.Integer, t.Type);
        //            Assert.Equal(1, (int)t);
        //        }

        //        [Fact]
        //        public void EvaluateMissingProperty()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            JToken t = o.SelectElement("Missing[1]");
        //            Assert.IsNull(t);
        //        }

        //        [Fact]
        //        public void EvaluateIndexerOnObject()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            JToken t = o.SelectElement("[1]");
        //            Assert.IsNull(t);
        //        }

        //        [Fact]
        //        public void EvaluateIndexerOnObjectWithError()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            ExceptionAssert.Throws<JsonException>(() => { o.SelectElement("[1]", true); }, @"Index 1 not valid on JObject.");
        //        }

        //        [Fact]
        //        public void EvaluateWildcardIndexOnObjectWithError()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            ExceptionAssert.Throws<JsonException>(() => { o.SelectElement("[*]", true); }, @"Index * not valid on JObject.");
        //        }

        //        [Fact]
        //        public void EvaluateSliceOnObjectWithError()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            ExceptionAssert.Throws<JsonException>(() => { o.SelectElement("[:]", true); }, @"Array slice is not valid on JObject.");
        //        }

        //        [Fact]
        //        public void EvaluatePropertyOnArray()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4, 5);

        //            JToken t = a.SelectElement("BlahBlah");
        //            Assert.IsNull(t);
        //        }

        //        [Fact]
        //        public void EvaluateMultipleResultsError()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4, 5);

        //            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[0, 1]"); }, @"Path returned multiple tokens.");
        //        }

        //        [Fact]
        //        public void EvaluatePropertyOnArrayWithError()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4, 5);

        //            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("BlahBlah", true); }, @"Property 'BlahBlah' not valid on JArray.");
        //        }

        //        [Fact]
        //        public void EvaluateNoResultsWithMultipleArrayIndexes()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4, 5);

        //            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[9,10]", true); }, @"Index 9 outside the bounds of JArray.");
        //        }

        //        [Fact]
        //        public void EvaluateConstructorOutOfBoundsIndxerWithError()
        //        {
        //            JConstructor c = new JConstructor("Blah");

        //            ExceptionAssert.Throws<JsonException>(() => { c.SelectElement("[1]", true); }, @"Index 1 outside the bounds of JConstructor.");
        //        }

        //        [Fact]
        //        public void EvaluateConstructorOutOfBoundsIndxer()
        //        {
        //            JConstructor c = new JConstructor("Blah");

        //            Assert.IsNull(c.SelectElement("[1]"));
        //        }

        //        [Fact]
        //        public void EvaluateMissingPropertyWithError()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            ExceptionAssert.Throws<JsonException>(() => { o.SelectElement("Missing", true); }, "Property 'Missing' does not exist on JObject.");
        //        }

        //        [Fact]
        //        public void EvaluatePropertyWithoutError()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            JValue v = (JValue)o.SelectElement("Blah", true);
        //            Assert.Equal(1, v.Value);
        //        }

        //        [Fact]
        //        public void EvaluateMissingPropertyIndexWithError()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", 1));

        //            ExceptionAssert.Throws<JsonException>(() => { o.SelectElement("['Missing','Missing2']", true); }, "Property 'Missing' does not exist on JObject.");
        //        }

        //        [Fact]
        //        public void EvaluateMultiPropertyIndexOnArrayWithError()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4, 5);

        //            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("['Missing','Missing2']", true); }, "Properties 'Missing', 'Missing2' not valid on JArray.");
        //        }

        //        [Fact]
        //        public void EvaluateArraySliceWithError()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4, 5);

        //            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[99:]", true); }, "Array slice of 99 to * returned no results.");

        //            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[1:-19]", true); }, "Array slice of 1 to -19 returned no results.");

        //            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[:-19]", true); }, "Array slice of * to -19 returned no results.");

        //            a = new JArray();

        //            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[:]", true); }, "Array slice of * to * returned no results.");
        //        }

        //        [Fact]
        //        public void EvaluateOutOfBoundsIndxer()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4, 5);

        //            JToken t = a.SelectElement("[1000].Ha");
        //            Assert.IsNull(t);
        //        }

        //        [Fact]
        //        public void EvaluateArrayOutOfBoundsIndxerWithError()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4, 5);

        //            ExceptionAssert.Throws<JsonException>(() => { a.SelectElement("[1000].Ha", true); }, "Index 1000 outside the bounds of JArray.");
        //        }

        //        [Fact]
        //        public void EvaluateArray()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4);

        //            JToken t = a.SelectElement("[1]");
        //            Assert.IsNotNull(t);
        //            Assert.Equal(JTokenType.Integer, t.Type);
        //            Assert.Equal(2, (int)t);
        //        }

        //        [Fact]
        //        public void EvaluateArraySlice()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4, 5, 6, 7, 8, 9);
        //            IList<JToken> t = null;

        //            t = a.SelectElements("[-3:]").ToList();
        //            Assert.Equal(3, t.Count);
        //            Assert.Equal(7, (int)t[0]);
        //            Assert.Equal(8, (int)t[1]);
        //            Assert.Equal(9, (int)t[2]);

        //            t = a.SelectElements("[-1:-2:-1]").ToList();
        //            Assert.Equal(1, t.Count);
        //            Assert.Equal(9, (int)t[0]);

        //            t = a.SelectElements("[-2:-1]").ToList();
        //            Assert.Equal(1, t.Count);
        //            Assert.Equal(8, (int)t[0]);

        //            t = a.SelectElements("[1:1]").ToList();
        //            Assert.Equal(0, t.Count);

        //            t = a.SelectElements("[1:2]").ToList();
        //            Assert.Equal(1, t.Count);
        //            Assert.Equal(2, (int)t[0]);

        //            t = a.SelectElements("[::-1]").ToList();
        //            Assert.Equal(9, t.Count);
        //            Assert.Equal(9, (int)t[0]);
        //            Assert.Equal(8, (int)t[1]);
        //            Assert.Equal(7, (int)t[2]);
        //            Assert.Equal(6, (int)t[3]);
        //            Assert.Equal(5, (int)t[4]);
        //            Assert.Equal(4, (int)t[5]);
        //            Assert.Equal(3, (int)t[6]);
        //            Assert.Equal(2, (int)t[7]);
        //            Assert.Equal(1, (int)t[8]);

        //            t = a.SelectElements("[::-2]").ToList();
        //            Assert.Equal(5, t.Count);
        //            Assert.Equal(9, (int)t[0]);
        //            Assert.Equal(7, (int)t[1]);
        //            Assert.Equal(5, (int)t[2]);
        //            Assert.Equal(3, (int)t[3]);
        //            Assert.Equal(1, (int)t[4]);
        //        }

        //        [Fact]
        //        public void EvaluateWildcardArray()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4);

        //            List<JToken> t = a.SelectElements("[*]").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(4, t.Count);
        //            Assert.Equal(1, (int)t[0]);
        //            Assert.Equal(2, (int)t[1]);
        //            Assert.Equal(3, (int)t[2]);
        //            Assert.Equal(4, (int)t[3]);
        //        }

        //        [Fact]
        //        public void EvaluateArrayMultipleIndexes()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4);

        //            IEnumerable<JToken> t = a.SelectElements("[1,2,0]");
        //            Assert.IsNotNull(t);
        //            Assert.Equal(3, t.Count());
        //            Assert.Equal(2, (int)t.ElementAt(0));
        //            Assert.Equal(3, (int)t.ElementAt(1));
        //            Assert.Equal(1, (int)t.ElementAt(2));
        //        }

        //        [Fact]
        //        public void EvaluateScan()
        //        {
        //            JObject o1 = new JObject { { "Name", 1 } };
        //            JObject o2 = new JObject { { "Name", 2 } };
        //            JArray a = new JArray(o1, o2);

        //            IList<JToken> t = a.SelectElements("$..Name").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(2, t.Count);
        //            Assert.Equal(1, (int)t[0]);
        //            Assert.Equal(2, (int)t[1]);
        //        }

        //        [Fact]
        //        public void EvaluateWildcardScan()
        //        {
        //            JObject o1 = new JObject { { "Name", 1 } };
        //            JObject o2 = new JObject { { "Name", 2 } };
        //            JArray a = new JArray(o1, o2);

        //            IList<JToken> t = a.SelectElements("$..*").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(5, t.Count);
        //            Assert.IsTrue(JToken.DeepEquals(a, t[0]));
        //            Assert.IsTrue(JToken.DeepEquals(o1, t[1]));
        //            Assert.Equal(1, (int)t[2]);
        //            Assert.IsTrue(JToken.DeepEquals(o2, t[3]));
        //            Assert.Equal(2, (int)t[4]);
        //        }

        //        [Fact]
        //        public void EvaluateScanNestResults()
        //        {
        //            JObject o1 = new JObject { { "Name", 1 } };
        //            JObject o2 = new JObject { { "Name", 2 } };
        //            JObject o3 = new JObject { { "Name", new JObject { { "Name", new JArray(3) } } } };
        //            JArray a = new JArray(o1, o2, o3);

        //            IList<JToken> t = a.SelectElements("$..Name").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(4, t.Count);
        //            Assert.Equal(1, (int)t[0]);
        //            Assert.Equal(2, (int)t[1]);
        //            Assert.IsTrue(JToken.DeepEquals(new JObject { { "Name", new JArray(3) } }, t[2]));
        //            Assert.IsTrue(JToken.DeepEquals(new JArray(3), t[3]));
        //        }

        //        [Fact]
        //        public void EvaluateWildcardScanNestResults()
        //        {
        //            JObject o1 = new JObject { { "Name", 1 } };
        //            JObject o2 = new JObject { { "Name", 2 } };
        //            JObject o3 = new JObject { { "Name", new JObject { { "Name", new JArray(3) } } } };
        //            JArray a = new JArray(o1, o2, o3);

        //            IList<JToken> t = a.SelectElements("$..*").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(9, t.Count);

        //            Assert.IsTrue(JToken.DeepEquals(a, t[0]));
        //            Assert.IsTrue(JToken.DeepEquals(o1, t[1]));
        //            Assert.Equal(1, (int)t[2]);
        //            Assert.IsTrue(JToken.DeepEquals(o2, t[3]));
        //            Assert.Equal(2, (int)t[4]);
        //            Assert.IsTrue(JToken.DeepEquals(o3, t[5]));
        //            Assert.IsTrue(JToken.DeepEquals(new JObject { { "Name", new JArray(3) } }, t[6]));
        //            Assert.IsTrue(JToken.DeepEquals(new JArray(3), t[7]));
        //            Assert.Equal(3, (int)t[8]);
        //        }

        //        [Fact]
        //        public void EvaluateSinglePropertyReturningArray()
        //        {
        //            JObject o = new JObject(
        //                new JProperty("Blah", new[] { 1, 2, 3 }));

        //            JToken t = o.SelectElement("Blah");
        //            Assert.IsNotNull(t);
        //            Assert.Equal(JTokenType.Array, t.Type);

        //            t = o.SelectElement("Blah[2]");
        //            Assert.Equal(JTokenType.Integer, t.Type);
        //            Assert.Equal(3, (int)t);
        //        }

        //        [Fact]
        //        public void EvaluateLastSingleCharacterProperty()
        //        {
        //            JObject o2 = JObject.Parse("{'People':[{'N':'Jeff'}]}");
        //            string a2 = (string)o2.SelectElement("People[0].N");

        //            Assert.Equal("Jeff", a2);
        //        }

        //        [Fact]
        //        public void ExistsQuery()
        //        {
        //            JArray a = new JArray(new JObject(new JProperty("hi", "ho")), new JObject(new JProperty("hi2", "ha")));

        //            IList<JToken> t = a.SelectElements("[ ?( @.hi ) ]").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(1, t.Count);
        //            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", "ho")), t[0]));
        //        }

        //        [Fact]
        //        public void EqualsQuery()
        //        {
        //            JArray a = new JArray(
        //                new JObject(new JProperty("hi", "ho")),
        //                new JObject(new JProperty("hi", "ha")));

        //            IList<JToken> t = a.SelectElements("[ ?( @.['hi'] == 'ha' ) ]").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(1, t.Count);
        //            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", "ha")), t[0]));
        //        }

        //        [Fact]
        //        public void NotEqualsQuery()
        //        {
        //            JArray a = new JArray(
        //                new JArray(new JObject(new JProperty("hi", "ho"))),
        //                new JArray(new JObject(new JProperty("hi", "ha"))));

        //            IList<JToken> t = a.SelectElements("[ ?( @..hi <> 'ha' ) ]").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(1, t.Count);
        //            Assert.IsTrue(JToken.DeepEquals(new JArray(new JObject(new JProperty("hi", "ho"))), t[0]));
        //        }

        //        [Fact]
        //        public void NoPathQuery()
        //        {
        //            JArray a = new JArray(1, 2, 3);

        //            IList<JToken> t = a.SelectElements("[ ?( @ > 1 ) ]").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(2, t.Count);
        //            Assert.Equal(2, (int)t[0]);
        //            Assert.Equal(3, (int)t[1]);
        //        }

        //        [Fact]
        //        public void MultipleQueries()
        //        {
        //            JArray a = new JArray(1, 2, 3, 4, 5, 6, 7, 8, 9);

        //            // json path does item based evaluation - http://www.sitepen.com/blog/2008/03/17/jsonpath-support/
        //            // first query resolves array to ints
        //            // int has no children to query
        //            IList<JToken> t = a.SelectElements("[?(@ <> 1)][?(@ <> 4)][?(@ < 7)]").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(0, t.Count);
        //        }

        //        [Fact]
        //        public void GreaterQuery()
        //        {
        //            JArray a = new JArray(
        //                new JObject(new JProperty("hi", 1)),
        //                new JObject(new JProperty("hi", 2)),
        //                new JObject(new JProperty("hi", 3)));

        //            IList<JToken> t = a.SelectElements("[ ?( @.hi > 1 ) ]").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(2, t.Count);
        //            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 2)), t[0]));
        //            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 3)), t[1]));
        //        }

        //        [Fact]
        //        public void LesserQuery_ValueFirst()
        //        {
        //            JArray a = new JArray(
        //                new JObject(new JProperty("hi", 1)),
        //                new JObject(new JProperty("hi", 2)),
        //                new JObject(new JProperty("hi", 3)));

        //            IList<JToken> t = a.SelectElements("[ ?( 1 < @.hi ) ]").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(2, t.Count);
        //            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 2)), t[0]));
        //            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 3)), t[1]));
        //        }

        //        [Fact]
        //        public void GreaterOrEqualQuery()
        //        {
        //            JArray a = new JArray(
        //                new JObject(new JProperty("hi", 1)),
        //                new JObject(new JProperty("hi", 2)),
        //                new JObject(new JProperty("hi", 2.0)),
        //                new JObject(new JProperty("hi", 3)));

        //            IList<JToken> t = a.SelectElements("[ ?( @.hi >= 1 ) ]").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(4, t.Count);
        //            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 1)), t[0]));
        //            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 2)), t[1]));
        //            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 2.0)), t[2]));
        //            Assert.IsTrue(JToken.DeepEquals(new JObject(new JProperty("hi", 3)), t[3]));
        //        }

        //        [Fact]
        //        public void NestedQuery()
        //        {
        //            JArray a = new JArray(
        //                new JObject(
        //                    new JProperty("name", "Bad Boys"),
        //                    new JProperty("cast", new JArray(
        //                        new JObject(new JProperty("name", "Will Smith"))))),
        //                new JObject(
        //                    new JProperty("name", "Independence Day"),
        //                    new JProperty("cast", new JArray(
        //                        new JObject(new JProperty("name", "Will Smith"))))),
        //                new JObject(
        //                    new JProperty("name", "The Rock"),
        //                    new JProperty("cast", new JArray(
        //                        new JObject(new JProperty("name", "Nick Cage")))))
        //                );

        //            IList<JToken> t = a.SelectElements("[?(@.cast[?(@.name=='Will Smith')])].name").ToList();
        //            Assert.IsNotNull(t);
        //            Assert.Equal(2, t.Count);
        //            Assert.Equal("Bad Boys", (string)t[0]);
        //            Assert.Equal("Independence Day", (string)t[1]);
        //        }

        //        [Fact]
        //        public void PathWithConstructor()
        //        {
        //            JArray a = JArray.Parse(@"[
        //  {
        //    ""Property1"": [
        //      1,
        //      [
        //        [
        //          []
        //        ]
        //      ]
        //    ]
        //  },
        //  {
        //    ""Property2"": new Constructor1(
        //      null,
        //      [
        //        1
        //      ]
        //    )
        //  }
        //]");

        //            JValue v = (JValue)a.SelectElement("[1].Property2[1][0]");
        //            Assert.Equal(1L, v.Value);
        //        }

        //        [Fact]
        //        public void MultiplePaths()
        //        {
        //            JArray a = JArray.Parse(@"[
        //  {
        //    ""price"": 199,
        //    ""max_price"": 200
        //  },
        //  {
        //    ""price"": 200,
        //    ""max_price"": 200
        //  },
        //  {
        //    ""price"": 201,
        //    ""max_price"": 200
        //  }
        //]");

        //            var results = a.SelectElements("[?(@.price > @.max_price)]").ToList();
        //            Assert.Equal(1, results.Count);
        //            Assert.Equal(a[2], results[0]);
        //        }

        //        [Fact]
        //        public void Exists_True()
        //        {
        //            JArray a = JArray.Parse(@"[
        //  {
        //    ""price"": 199,
        //    ""max_price"": 200
        //  },
        //  {
        //    ""price"": 200,
        //    ""max_price"": 200
        //  },
        //  {
        //    ""price"": 201,
        //    ""max_price"": 200
        //  }
        //]");

        //            var results = a.SelectElements("[?(true)]").ToList();
        //            Assert.Equal(3, results.Count);
        //            Assert.Equal(a[0], results[0]);
        //            Assert.Equal(a[1], results[1]);
        //            Assert.Equal(a[2], results[2]);
        //        }

        //        [Fact]
        //        public void Exists_Null()
        //        {
        //            JArray a = JArray.Parse(@"[
        //  {
        //    ""price"": 199,
        //    ""max_price"": 200
        //  },
        //  {
        //    ""price"": 200,
        //    ""max_price"": 200
        //  },
        //  {
        //    ""price"": 201,
        //    ""max_price"": 200
        //  }
        //]");

        //            var results = a.SelectElements("[?(true)]").ToList();
        //            Assert.Equal(3, results.Count);
        //            Assert.Equal(a[0], results[0]);
        //            Assert.Equal(a[1], results[1]);
        //            Assert.Equal(a[2], results[2]);
        //        }

        //        [Fact]
        //        public void WildcardWithProperty()
        //        {
        //            JObject o = JObject.Parse(@"{
        //    ""station"": 92000041000001, 
        //    ""containers"": [
        //        {
        //            ""id"": 1,
        //            ""text"": ""Sort system"",
        //            ""containers"": [
        //                {
        //                    ""id"": ""2"",
        //                    ""text"": ""Yard 11""
        //                },
        //                {
        //                    ""id"": ""92000020100006"",
        //                    ""text"": ""Sort yard 12""
        //                },
        //                {
        //                    ""id"": ""92000020100005"",
        //                    ""text"": ""Yard 13""
        //                } 
        //            ]
        //        }, 
        //        {
        //            ""id"": ""92000020100011"",
        //            ""text"": ""TSP-1""
        //        }, 
        //        {
        //            ""id"":""92000020100007"",
        //            ""text"": ""Passenger 15""
        //        }
        //    ]
        //}");

        //            IList<JToken> tokens = o.SelectElements("$..*[?(@.text)]").ToList();
        //            int i = 0;
        //            Assert.Equal("Sort system", (string)tokens[i++]["text"]);
        //            Assert.Equal("TSP-1", (string)tokens[i++]["text"]);
        //            Assert.Equal("Passenger 15", (string)tokens[i++]["text"]);
        //            Assert.Equal("Yard 11", (string)tokens[i++]["text"]);
        //            Assert.Equal("Sort yard 12", (string)tokens[i++]["text"]);
        //            Assert.Equal("Yard 13", (string)tokens[i++]["text"]);
        //            Assert.Equal(6, tokens.Count);
        //        }

        //        [Fact]
        //        public void QueryAgainstNonStringValues()
        //        {
        //            IList<object> values = new List<object>
        //            {
        //                "ff2dc672-6e15-4aa2-afb0-18f4f69596ad",
        //                new Guid("ff2dc672-6e15-4aa2-afb0-18f4f69596ad"),
        //                "http://localhost",
        //                new Uri("http://localhost"),
        //                "2000-12-05T05:07:59Z",
        //                new DateTime(2000, 12, 5, 5, 7, 59, DateTimeKind.Utc),
        //#if !NET20
        //                "2000-12-05T05:07:59-10:00",
        //                new DateTimeOffset(2000, 12, 5, 5, 7, 59, -TimeSpan.FromHours(10)),
        //#endif
        //                "SGVsbG8gd29ybGQ=",
        //                Encoding.UTF8.GetBytes("Hello world"),
        //                "365.23:59:59",
        //                new TimeSpan(365, 23, 59, 59)
        //            };

        //            JObject o = new JObject(
        //                new JProperty("prop",
        //                    new JArray(
        //                        values.Select(v => new JObject(new JProperty("childProp", v)))
        //                        )
        //                    )
        //                );

        //            IList<JToken> t = o.SelectElements("$.prop[?(@.childProp =='ff2dc672-6e15-4aa2-afb0-18f4f69596ad')]").ToList();
        //            Assert.Equal(2, t.Count);

        //            t = o.SelectElements("$.prop[?(@.childProp =='http://localhost')]").ToList();
        //            Assert.Equal(2, t.Count);

        //            t = o.SelectElements("$.prop[?(@.childProp =='2000-12-05T05:07:59Z')]").ToList();
        //            Assert.Equal(2, t.Count);

        //#if !NET20
        //            t = o.SelectElements("$.prop[?(@.childProp =='2000-12-05T05:07:59-10:00')]").ToList();
        //            Assert.Equal(2, t.Count);
        //#endif

        //            t = o.SelectElements("$.prop[?(@.childProp =='SGVsbG8gd29ybGQ=')]").ToList();
        //            Assert.Equal(2, t.Count);

        //            t = o.SelectElements("$.prop[?(@.childProp =='365.23:59:59')]").ToList();
        //            Assert.Equal(2, t.Count);
        //        }

        //        [Fact]
        //        public void Example()
        //        {
        //            JObject o = JObject.Parse(@"{
        //        ""Stores"": [
        //          ""Lambton Quay"",
        //          ""Willis Street""
        //        ],
        //        ""Manufacturers"": [
        //          {
        //            ""Name"": ""Acme Co"",
        //            ""Products"": [
        //              {
        //                ""Name"": ""Anvil"",
        //                ""Price"": 50
        //              }
        //            ]
        //          },
        //          {
        //            ""Name"": ""Contoso"",
        //            ""Products"": [
        //              {
        //                ""Name"": ""Elbow Grease"",
        //                ""Price"": 99.95
        //              },
        //              {
        //                ""Name"": ""Headlight Fluid"",
        //                ""Price"": 4
        //              }
        //            ]
        //          }
        //        ]
        //      }");

        //            string name = (string)o.SelectElement("Manufacturers[0].Name");
        //            // Acme Co

        //            decimal productPrice = (decimal)o.SelectElement("Manufacturers[0].Products[0].Price");
        //            // 50

        //            string productName = (string)o.SelectElement("Manufacturers[1].Products[0].Name");
        //            // Elbow Grease

        //            Assert.Equal("Acme Co", name);
        //            Assert.Equal(50m, productPrice);
        //            Assert.Equal("Elbow Grease", productName);

        //            IList<string> storeNames = o.SelectElement("Stores").Select(s => (string)s).ToList();
        //            // Lambton Quay
        //            // Willis Street

        //            IList<string> firstProductNames = o["Manufacturers"].Select(m => (string)m.SelectElement("Products[1].Name")).ToList();
        //            // null
        //            // Headlight Fluid

        //            decimal totalPrice = o["Manufacturers"].Sum(m => (decimal)m.SelectElement("Products[0].Price"));
        //            // 149.95

        //            Assert.Equal(2, storeNames.Count);
        //            Assert.Equal("Lambton Quay", storeNames[0]);
        //            Assert.Equal("Willis Street", storeNames[1]);
        //            Assert.Equal(2, firstProductNames.Count);
        //            Assert.Equal(null, firstProductNames[0]);
        //            Assert.Equal("Headlight Fluid", firstProductNames[1]);
        //            Assert.Equal(149.95m, totalPrice);
        //        }

        //        [Fact]
        //        public void NotEqualsAndNonPrimativeValues()
        //        {
        //            string json = @"[
        //  {
        //    ""name"": ""string"",
        //    ""value"": ""aString""
        //  },
        //  {
        //    ""name"": ""number"",
        //    ""value"": 123
        //  },
        //  {
        //    ""name"": ""array"",
        //    ""value"": [
        //      1,
        //      2,
        //      3,
        //      4
        //    ]
        //  },
        //  {
        //    ""name"": ""object"",
        //    ""value"": {
        //      ""1"": 1
        //    }
        //  }
        //]";

        //            JArray a = JArray.Parse(json);

        //            List<JToken> result = a.SelectElements("$.[?(@.value!=1)]").ToList();
        //            Assert.Equal(4, result.Count);

        //            result = a.SelectElements("$.[?(@.value!='2000-12-05T05:07:59-10:00')]").ToList();
        //            Assert.Equal(4, result.Count);

        //            result = a.SelectElements("$.[?(@.value!=null)]").ToList();
        //            Assert.Equal(4, result.Count);

        //            result = a.SelectElements("$.[?(@.value!=123)]").ToList();
        //            Assert.Equal(3, result.Count);

        //            result = a.SelectElements("$.[?(@.value)]").ToList();
        //            Assert.Equal(4, result.Count);
        //        }

        //        [Fact]
        //        public void RootInFilter()
        //        {
        //            string json = @"[
        //   {
        //      ""store"" : {
        //         ""book"" : [
        //            {
        //               ""category"" : ""reference"",
        //               ""author"" : ""Nigel Rees"",
        //               ""title"" : ""Sayings of the Century"",
        //               ""price"" : 8.95
        //            },
        //            {
        //               ""category"" : ""fiction"",
        //               ""author"" : ""Evelyn Waugh"",
        //               ""title"" : ""Sword of Honour"",
        //               ""price"" : 12.99
        //            },
        //            {
        //               ""category"" : ""fiction"",
        //               ""author"" : ""Herman Melville"",
        //               ""title"" : ""Moby Dick"",
        //               ""isbn"" : ""0-553-21311-3"",
        //               ""price"" : 8.99
        //            },
        //            {
        //               ""category"" : ""fiction"",
        //               ""author"" : ""J. R. R. Tolkien"",
        //               ""title"" : ""The Lord of the Rings"",
        //               ""isbn"" : ""0-395-19395-8"",
        //               ""price"" : 22.99
        //            }
        //         ],
        //         ""bicycle"" : {
        //            ""color"" : ""red"",
        //            ""price"" : 19.95
        //         }
        //      },
        //      ""expensive"" : 10
        //   }
        //]";

        //            JArray a = JArray.Parse(json);

        //            List<JToken> result = a.SelectElements("$.[?($.[0].store.bicycle.price < 20)]").ToList();
        //            Assert.Equal(1, result.Count);

        //            result = a.SelectElements("$.[?($.[0].store.bicycle.price < 10)]").ToList();
        //            Assert.Equal(0, result.Count);
        //        }

        //        [Fact]
        //        public void RootInFilterWithRootObject()
        //        {
        //            string json = @"{
        //                ""store"" : {
        //                    ""book"" : [
        //                        {
        //                            ""category"" : ""reference"",
        //                            ""author"" : ""Nigel Rees"",
        //                            ""title"" : ""Sayings of the Century"",
        //                            ""price"" : 8.95
        //                        },
        //                        {
        //                            ""category"" : ""fiction"",
        //                            ""author"" : ""Evelyn Waugh"",
        //                            ""title"" : ""Sword of Honour"",
        //                            ""price"" : 12.99
        //                        },
        //                        {
        //                            ""category"" : ""fiction"",
        //                            ""author"" : ""Herman Melville"",
        //                            ""title"" : ""Moby Dick"",
        //                            ""isbn"" : ""0-553-21311-3"",
        //                            ""price"" : 8.99
        //                        },
        //                        {
        //                            ""category"" : ""fiction"",
        //                            ""author"" : ""J. R. R. Tolkien"",
        //                            ""title"" : ""The Lord of the Rings"",
        //                            ""isbn"" : ""0-395-19395-8"",
        //                            ""price"" : 22.99
        //                        }
        //                    ],
        //                    ""bicycle"" : [
        //                        {
        //                            ""color"" : ""red"",
        //                            ""price"" : 19.95
        //                        }
        //                    ]
        //                },
        //                ""expensive"" : 10
        //            }";

        //            JObject a = JObject.Parse(json);

        //            List<JToken> result = a.SelectElements("$..book[?(@.price <= $['expensive'])]").ToList();
        //            Assert.Equal(2, result.Count);

        //            result = a.SelectElements("$.store..[?(@.price > $.expensive)]").ToList();
        //            Assert.Equal(3, result.Count);
        //        }

        //        [Fact]
        //        public void RootInFilterWithInitializers()
        //        {
        //            JObject rootObject = new JObject
        //            {
        //                { "referenceDate", new JValue(DateTime.MinValue) },
        //                {
        //                    "dateObjectsArray",
        //                    new JArray()
        //                    {
        //                        new JObject { { "date", new JValue(DateTime.MinValue) } },
        //                        new JObject { { "date", new JValue(DateTime.MaxValue) } },
        //                        new JObject { { "date", new JValue(DateTime.Now) } },
        //                        new JObject { { "date", new JValue(DateTime.MinValue) } },
        //                    }
        //                }
        //            };

        //            List<JToken> result = rootObject.SelectElements("$.dateObjectsArray[?(@.date == $.referenceDate)]").ToList();
        //            Assert.Equal(2, result.Count);
        //        }
    }
}