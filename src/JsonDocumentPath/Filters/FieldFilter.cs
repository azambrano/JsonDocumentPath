using System.Collections.Generic;

namespace System.Text.Json
{
    internal class FieldFilter : PathFilter
    {
        internal string Name;

        public FieldFilter(string name)
        {
            Name = name;
        }

        public override IEnumerable<JsonElementExt> ExecuteFilter(JsonElement root, IEnumerable<JsonElementExt> current, bool errorWhenNoMatch)
        {
            foreach (JsonElementExt t in current)
            {
                if (t.Element.Value.ValueKind == JsonValueKind.Object)
                {
                    if (Name != null)
                    {
                        if (t.Element.Value.TryGetProperty(Name, out JsonElement v))
                        {
                            if (v.ValueKind != JsonValueKind.Null)
                            {
                                yield return new JsonElementExt(){Element = v, Name =Name };
                            }
                            else if (errorWhenNoMatch)
                            {
                                throw new JsonException($"Property '{Name}' does not exist on JObject.");
                            }
                        }
                    }
                    else
                    {
                        foreach (var p in t.Element.Value.ChildrenTokens())
                        {
                            yield return new JsonElementExt() {Element = p};
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
