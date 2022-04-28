using System.Collections.Generic;

namespace System.Text.Json
{
    public abstract class PathFilter
    {
        public abstract IEnumerable<JsonElementExt> ExecuteFilter(JsonElement root, IEnumerable<JsonElementExt> current, bool errorWhenNoMatch);

        protected static JsonElementExt GetTokenIndex(JsonElement t, bool errorWhenNoMatch, int index)
        {
            if (t.ValueKind == JsonValueKind.Array)
            {
                if (t.GetArrayLength() <= index)
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonException($"Index {index} outside the bounds of JArray.");
                    }

                    return null;
                }

                return new JsonElementExt() {Element = t[index]};
            }
            else
            {
                if (errorWhenNoMatch)
                {
                    throw new JsonException($"Index {index} not valid on {t.GetType().Name}.");
                }

                return null;
            }
        }

        protected static IEnumerable<(string Name, JsonElementExt Value)> GetNextScanValue(JsonElementExt value)
        {
            yield return (null, value);

            if (value.Element?.ValueKind == JsonValueKind.Array)
            {
                foreach (var e in value.Element?.EnumerateArray())
                {
                    foreach (var c in GetNextScanValue(new JsonElementExt(){ Element = e }))
                    {
                        yield return c;
                    }
                }
            }
            else if (value.Element?.ValueKind == JsonValueKind.Object)
            {
                foreach (var e in value.Element?.EnumerateObject())
                {
                    yield return (e.Name, new JsonElementExt{Element = e.Value, Name = e.Name });

                    foreach (var c in GetNextScanValue(new JsonElementExt(){ Element = e.Value }))
                    {
                        yield return c;
                    }
                }
            }
        }
    }
}
