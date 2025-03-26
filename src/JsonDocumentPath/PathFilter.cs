using System.Collections.Generic;

#if NET6_0_OR_GREATER

using System.Text.Json.Nodes;

#endif
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

#if NET6_0_OR_GREATER

        public abstract IEnumerable<JsonNode?> ExecuteFilter(JsonNode root, IEnumerable<JsonNode?> current, bool errorWhenNoMatch);

        protected static JsonNode? GetTokenIndex(JsonNode t, bool errorWhenNoMatch, int index)
        {
            if (t.GetSafeJsonValueKind() == JsonValueKind.Array)
            {
                var tArray = t.AsArray();
                if (tArray.Count <= index)
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonException($"Index {index} outside the bounds of JArray.");
                    }

                    return null;
                }

                return tArray[index];
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

        protected static IEnumerable<(string? Name, JsonNode? Value)> GetNextScanValue(JsonNode? value)
        {
            yield return (null, value);

            if (value.GetSafeJsonValueKind() == JsonValueKind.Array)
            {
                foreach (var e in value?.AsArray())
                {
                    foreach (var c in GetNextScanValue(e))
                    {
                        yield return c;
                    }
                }
            }
            else if (value.GetSafeJsonValueKind() == JsonValueKind.Object)
            {
                var propertyEnumerator = value?.AsObject().GetEnumerator();

                while (propertyEnumerator.MoveNext())
                {
                    yield return (propertyEnumerator.Current.Key, propertyEnumerator.Current.Value);

                    foreach (var c in GetNextScanValue(propertyEnumerator.Current.Value))
                    {
                        yield return c;
                    }
                }
            }
        }

#endif
    }
}