using System.Collections.Generic;

namespace System.Text.Json
{
    internal class ScanMultipleFilter : PathFilter
    {
        private List<string> _names;

        public ScanMultipleFilter(List<string> names)
        {
            _names = names;
        }

        public override IEnumerable<JsonElement?> ExecuteFilter(JsonElement root, IEnumerable<JsonElement?> current, bool errorWhenNoMatch)
        {
            foreach (JsonElement c in current)
            {
                JsonElement? value = c;

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
