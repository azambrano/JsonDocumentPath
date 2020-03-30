using System.Collections.Generic;

namespace System.Text.Json
{
    internal class RootFilter : PathFilter
    {
        public static readonly RootFilter Instance = new RootFilter();

        private RootFilter()
        {
        }

        public override IEnumerable<JsonElement?> ExecuteFilter(JsonElement root, IEnumerable<JsonElement?> current, bool errorWhenNoMatch)
        {
            return new JsonElement?[1] { root };
        }
    }
}
