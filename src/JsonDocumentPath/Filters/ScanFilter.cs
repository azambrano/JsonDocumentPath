using System.Collections.Generic;

namespace System.Text.Json
{
    internal partial class ScanFilter : PathFilter
    {
        internal string? Name;

        public ScanFilter(string? name)
        {
            Name = name;
        }

        public override IEnumerable<JsonElement?> ExecuteFilter(JsonElement root, IEnumerable<JsonElement?> current, bool errorWhenNoMatch)
        {
            foreach (JsonElement c in current)
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