#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    internal partial class ScanFilter
    {
        public override IEnumerable<JsonNode?> ExecuteFilter(JsonNode root, IEnumerable<JsonNode?> current, bool errorWhenNoMatch)
        {
            foreach (JsonNode? c in current)
            {
                foreach (var e in GetNextScanValue(c))
                {
                    if (e.Name == Name)
                    {
                        yield return e.Value;
                    }
                }
            }
        }
    }
}

#endif