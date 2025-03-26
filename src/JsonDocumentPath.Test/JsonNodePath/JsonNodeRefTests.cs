using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace JNodePath.Test;

public class JsonNodeRefTests
{
    [Fact]
    public void FindSelectNodeParent()
    {
        var jNode = JsonNode.Parse("""
            {
                "a":"",
                "b":
                {
                    "c":1,
                    "d":
                    {
                        "e":2,
                        "f":
                        [
                            {
                                "fa":"result"
                            }
                        ]
                    }
                }
            }
            """);

        var result = jNode.SelectNodes("$.b.d.f.[*].fa");

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(result.First().Parent.Parent.Parent.Parent.Parent, jNode);
        Assert.Equal(jNode.ChildrenNodes(), jNode.SelectNodes("$.*"));
    }
}