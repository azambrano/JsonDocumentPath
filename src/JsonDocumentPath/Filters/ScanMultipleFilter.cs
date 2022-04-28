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

        public override IEnumerable<JsonElementExt> ExecuteFilter(JsonElement root, IEnumerable<JsonElementExt> current, bool errorWhenNoMatch)
        {
            foreach (JsonElementExt c in current)
            {
                foreach (var e in GetNextScanValue(c))
                {
                    if (e.Name != null)
                    {
                        foreach (string name in _names)
                        {
                            if (e.Name == name)
                            {
                                yield return new JsonElementExt(){ Element = e.Value.Element, Name = e.Name };
                            }
                        }
                    }
                }
            }
        }
    }
}
