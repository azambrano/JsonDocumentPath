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

                while (true)
                {
                    JsonElement? container = value.IsContainer() ? value : new JsonElement?();

                    value = GetNextScanValue(c, container, value);
                    if (value == null)
                    {
                        break;
                    }
                    yield return null;

                    //if (value is JProperty property)
                    //{
                    //    foreach (string name in _names)
                    //    {
                    //        if (property.Name == name)
                    //        {
                    //            yield return property.Value;
                    //        }
                    //    }
                    //}
                }
            }
        }
    }
}
