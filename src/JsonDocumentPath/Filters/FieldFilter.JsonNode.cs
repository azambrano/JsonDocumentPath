#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    internal partial class FieldFilter
    {
        public override IEnumerable<JsonNode?> ExecuteFilter(JsonNode root, IEnumerable<JsonNode?> current, bool errorWhenNoMatch)
        {
            foreach (JsonNode t in current)
            {
                if (t.GetSafeJsonValueKind() == JsonValueKind.Object)
                {
                    if (Name != null)
                    {
                        var tObject = t.AsObject();
                        if (tObject.TryGetPropertyValue(Name, out JsonNode? v))
                        {
                            if (v?.GetSafeJsonValueKind() != JsonValueKind.Null)
                            {
                                yield return v;
                            }
                            else if (errorWhenNoMatch)
                            {
                                throw new JsonException($"Property '{Name}' does not exist on JObject.");
                            }
                        }
                    }
                    else
                    {
                        foreach (var p in
#if NET6_0 || NET7_0
                            Extensions.ChildrenNodes(t)
#else
                            t.ChildrenNodes()
#endif
                            )
                        {
                            yield return p;
                        }
                    }
                }
                else
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonException($"Property '{Name ?? "*"}' not valid on {t.GetType().Name}.");
                    }
                }
            }
        }
    }
}

#endif