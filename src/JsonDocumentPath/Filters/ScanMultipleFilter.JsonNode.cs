#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace System.Text.Json
{
    internal partial class ScanMultipleFilter
    {
        public override IEnumerable<JsonNode?> ExecuteFilter(JsonNode root, IEnumerable<JsonNode?> current, bool errorWhenNoMatch)
        {
            foreach (JsonNode c in current)
            {
                JsonNode? value = c;

                foreach (var e in GetNextScanValue(c))
                {
                    if (e.Name != null)
                    {
                        foreach (string name in _names)
                        {
                            if (e.Name == name)
                            {
                                yield return e.Value;
                            }
                        }
                    }
                }
            }
        }
    }
}

#endif