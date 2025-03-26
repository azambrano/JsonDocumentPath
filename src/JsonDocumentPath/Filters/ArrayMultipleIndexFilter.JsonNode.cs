#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    internal partial class ArrayMultipleIndexFilter
    {
        public override IEnumerable<JsonNode?> ExecuteFilter(JsonNode root, IEnumerable<JsonNode?> current, bool errorWhenNoMatch)
        {
            foreach (JsonNode t in current)
            {
                foreach (int i in Indexes)
                {
                    JsonNode? v = GetTokenIndex(t, errorWhenNoMatch, i);

                    if (v != null)
                    {
                        yield return v;
                    }
                }
            }
        }
    }
}

#endif