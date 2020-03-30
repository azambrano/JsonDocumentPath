using System.Collections.Generic;
using System.Linq;

namespace System.Text.Json
{
    internal class FieldMultipleFilter : PathFilter
    {
        internal List<string> Names;

        public FieldMultipleFilter(List<string> names)
        {
            Names = names;
        }

        public override IEnumerable<JsonElement?> ExecuteFilter(JsonElement root, IEnumerable<JsonElement?> current, bool errorWhenNoMatch)
        {
            foreach (JsonElement t in current)
            {
                if (t.ValueKind == JsonValueKind.Object)
                {
                    foreach (string name in Names)
                    {
                        if (t.TryGetProperty(name, out JsonElement v))
                        {
                            if (v.ValueKind != JsonValueKind.Null)
                            {
                                yield return v;
                            }
                            else if (errorWhenNoMatch)
                            {
                                throw new JsonException($"Property '{name}' does not exist on JObject.");
                            }
                        }
                    }
                }
                else
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonException($"Properties {string.Join(", ", Names.Select(n => "'" + n + "'").ToArray())} not valid on {t.GetType().Name}.");
                    }
                }
            }
        }
    }
}
