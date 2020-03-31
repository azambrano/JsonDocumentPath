# JsonDocumentPath
JsonDocumentPath is a class library to extract values from JSON (System.Text.Json.JsonDocument) with single line expressions

The JsonPath parser is based on the [Json.Net](https://github.com/JamesNK/Newtonsoft.Json)

### How to use it

```csharp
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
```

### Implementation
#### Json.Net Filters:
- [x] ArrayIndexFilter
- [x] ArrayMultipleIndexFilter
- [x] ArraySliceFilter
- [x] FieldFilter
- [x] FieldMultipleFilter
- [x] QueryFilter
- [x] QueryScanFilter
- [x] RootFilter
- [ ] ScanFilter
- [ ] ScanMultipleFilter

#### Json.Net Unit Test:
- [x] JPathParseTests
- [x] QueryExpressionTests
- [ ] JPathExecuteTests

