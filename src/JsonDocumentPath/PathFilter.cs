using System.Collections.Generic;

namespace System.Text.Json
{
    public abstract class PathFilter
    {
        public abstract IEnumerable<JsonElement?> ExecuteFilter(JsonElement root, IEnumerable<JsonElement?> current, bool errorWhenNoMatch);

        protected static JsonElement? GetTokenIndex(JsonElement t, bool errorWhenNoMatch, int index)
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

                return t[index];
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

        protected static IEnumerable<(string Name, JsonElement Value)> GetNextScanValue(JsonElement value)
        {
            yield return (null, value);

            if (value.ValueKind == JsonValueKind.Array)
            {
                foreach (var e in value.EnumerateArray())
                {
                    foreach (var c in GetNextScanValue(e))
                    {
                        yield return c;
                    }
                }
            }
            else if (value.ValueKind == JsonValueKind.Object)
            {
                foreach (var e in value.EnumerateObject())
                {
                    yield return (e.Name, e.Value);

                    foreach (var c in GetNextScanValue(e.Value))
                    {
                        yield return c;
                    }
                }
            }
        }
    }
}
