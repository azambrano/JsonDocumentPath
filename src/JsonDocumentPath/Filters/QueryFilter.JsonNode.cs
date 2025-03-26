#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    internal partial class QueryFilter
    {
        public override IEnumerable<JsonNode?> ExecuteFilter(JsonNode root, IEnumerable<JsonNode?> current, bool errorWhenNoMatch)
        {
            foreach (JsonNode? el in current)
            {
                if (el?.GetSafeJsonValueKind() == JsonValueKind.Array)
                {
                    var elArray = el.AsArray();
                    foreach (JsonNode? v in elArray)
                    {
                        if (Expression.IsMatch(root, v))
                        {
                            yield return v;
                        }
                    }
                }
                else if (el?.GetSafeJsonValueKind() == JsonValueKind.Object)
                {
                    var elObject = el.AsObject();
                    foreach (KeyValuePair<string, JsonNode> v in elObject)
                    {
                        if (Expression.IsMatch(root, v.Value))
                        {
                            yield return v.Value;
                        }
                    }
                }
            }
        }
    }
}

#endif