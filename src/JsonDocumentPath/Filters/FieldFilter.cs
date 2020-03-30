using System.Collections.Generic;

namespace System.Text.Json
{
    internal class FieldFilter : PathFilter
    {
        internal string? Name;

        public FieldFilter(string? name)
        {
            Name = name;
        }

        public override IEnumerable<JsonElement?> ExecuteFilter(JsonElement root, IEnumerable<JsonElement?> current, bool errorWhenNoMatch)
        {
            foreach (JsonElement t in current)
            {
                if (t.ValueKind == JsonValueKind.Object)
                {
                    if (t.TryGetProperty(Name, out JsonElement v))
                    {
                        if (v.ValueKind != JsonValueKind.Null)
                        {
                            yield return v;
                        }
                        else if (errorWhenNoMatch)
                        {
                            throw new JsonException($"Property '{Name}' does not exist on JObject.");
                        }
                    }
                }
                else
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonException($"Property '{Name ?? "*"}' not valid on {t.GetType().Name}.");
                    }
                }
            }
        }
    }
}
