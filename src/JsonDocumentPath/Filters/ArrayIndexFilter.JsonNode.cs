#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    internal partial class ArrayIndexFilter
    {
        public override IEnumerable<JsonNode?> ExecuteFilter(JsonNode root, IEnumerable<JsonNode?> current, bool errorWhenNoMatch)
        {
            foreach (JsonNode t in current)
            {
                if (Index != null)
                {
                    JsonNode? v = GetTokenIndex(t, errorWhenNoMatch, Index.GetValueOrDefault());

                    if (v != null)
                    {
                        yield return v;
                    }
                }
                else
                {
                    if (t.GetSafeJsonValueKind() == JsonValueKind.Array)
                    {
                        var tArrayEnumerator = t.AsArray().GetEnumerator();
                        while (tArrayEnumerator.MoveNext())
                        {
                            yield return tArrayEnumerator.Current;
                        }
                    }
                    else
                    {
                        if (errorWhenNoMatch)
                        {
                            throw new JsonException($"Index * not valid on {t.GetType().Name}.");
                        }
                    }
                }
            }
        }
    }
}

#endif