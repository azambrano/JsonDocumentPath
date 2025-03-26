#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    internal partial class RootFilter
    {
        public override IEnumerable<JsonNode?> ExecuteFilter(JsonNode root, IEnumerable<JsonNode?> current, bool errorWhenNoMatch)
        {
            return [root];
        }
    }
}

#endif