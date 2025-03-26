#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    internal partial class FieldMultipleFilter
    {
        public override IEnumerable<JsonNode?> ExecuteFilter(JsonNode root, IEnumerable<JsonNode?> current, bool errorWhenNoMatch)
        {
            foreach (JsonNode? t in current)
            {
                if (t?.GetSafeJsonValueKind() == JsonValueKind.Object)
                {
                    var tObject = t.AsObject();
                    foreach (string name in Names)
                    {
                        if (tObject.TryGetPropertyValue(name, out JsonNode? v))
                        {
                            if (v?.GetSafeJsonValueKind() != JsonValueKind.Null)
                            {
                                yield return v;
                            }
                            else if (errorWhenNoMatch)
                            {
                                throw new JsonException($"Property '{name}' does not exist on JObject.");
                            }
                        }
                    }
                }
                else
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonException($"Properties {string.Join(", ", Names.Select(n => "'" + n + "'").ToArray())} not valid on {t.GetType().Name}.");
                    }
                }
            }
        }
    }
}

#endif