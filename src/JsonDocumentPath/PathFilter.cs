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

        protected static JsonElement? GetNextScanValue(JsonElement originalParent, JsonElement? container, JsonElement? value)
        {
            // step into container's values
            if (container != null && container.Value.TryGetFirstContainerValue(out JsonElement first))
            {
                value = first;
            }
            else
            {
                // finished container, move to parent
                //while (value != null && value != originalParent && value == value.Parent!.Last)
                //{
                //    value = value.Parent;
                //}

                //// finished
                //if (value == null || value == originalParent)
                //{
                //    return null;
                //}

                //// move to next value in container
                //value = value.Next;
            }

            return value;
        }
    }
}
