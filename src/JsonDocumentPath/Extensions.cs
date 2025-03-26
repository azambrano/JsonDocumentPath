using System.Collections.Generic;
using System.Linq;
#if NET6_0_OR_GREATER

using System.Text.Json.Nodes;

#endif

namespace System.Text.Json
{
    internal static partial class Extensions
    {
        public static bool IsValue(this JsonElement src)
        {
            return src.ValueKind == JsonValueKind.False ||
                   src.ValueKind == JsonValueKind.True ||
                   src.ValueKind == JsonValueKind.String ||
                   src.ValueKind == JsonValueKind.Number ||
                   src.ValueKind == JsonValueKind.Null ||
                   src.ValueKind == JsonValueKind.Undefined;
        }

        public static bool IsContainer(this JsonElement src)
        {
            return src.ValueKind == JsonValueKind.Array || src.ValueKind == JsonValueKind.Object;
        }

        public static bool IsContainer(this JsonElement? src)
        {
            if (src.HasValue)
            {
                return src.Value.IsContainer();
            }
            return false;
        }

        public static bool TryGetFirstFromObject(this JsonElement? src, out JsonProperty? element)
        {
            element = null;
            if (src.HasValue)
            {
                return src.Value.TryGetFirstFromObject(out element);
            }
            return false;
        }

        public static bool TryMoveNextFromObject(this JsonElement src, int cycle, out JsonProperty? element)
        {
            element = null;
            if (src.ValueKind == JsonValueKind.Object)
            {
                var currentObject = src.EnumerateObject();
                for (int i = 0; i < cycle; i++)
                {
                    currentObject.MoveNext();
                }
                element = currentObject.Current;
                return true;
            }
            return false;
        }

        public static bool TryGetFirstFromObject(this JsonElement src, out JsonProperty? element)
        {
            element = null;
            if (src.ValueKind == JsonValueKind.Object)
            {
                var currentObject = src.EnumerateObject();
                if (currentObject.MoveNext())
                {
                    element = currentObject.Current;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetFirstFromArray(this JsonElement? src, out JsonElement? element)
        {
            element = null;
            if (src.HasValue)
            {
                return src.Value.TryGetFirstFromArray(out element);
            }
            return false;
        }

        public static bool TryGetFirstFromArray(this JsonElement src, out JsonElement? element)
        {
            element = null;
            if (src.ValueKind == JsonValueKind.Array && src.GetArrayLength() > 0)
            {
                if (src.EnumerateArray().MoveNext())
                {
                    element = src.EnumerateArray().Current;
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<JsonElement> DescendantsAndSelf(this IEnumerable<JsonElement> source)
        {
            return source.SelectMany(j => j.DescendantsAndSelf());
        }

        public static IEnumerable<JsonElement> DescendantElements(this JsonElement src)
        {
            return GetDescendantElementsCore(src, false);
        }

        public static IEnumerable<JsonElement> DescendantsAndSelf(this JsonElement src)
        {
            return GetDescendantElementsCore(src, true);
        }

        public static IEnumerable<JsonElement> ChildrenTokens(this JsonElement src)
        {
            if (src.ValueKind == JsonValueKind.Object)
            {
                foreach (var item in src.EnumerateObject())
                {
                    yield return item.Value;
                }
            }

            if (src.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in src.EnumerateArray())
                {
                    yield return item;
                }
            }
        }

        internal static IEnumerable<JsonElement> GetDescendantElementsCore(JsonElement src, bool self)
        {
            if (self)
            {
                yield return src;
            }

            foreach (JsonElement o in src.ChildrenTokens())
            {
                yield return o;
                if (o.IsContainer())
                {
                    foreach (JsonElement d in o.DescendantElements())
                    {
                        yield return d;
                    }
                }
            }
        }

        public static IEnumerable<JsonProperty> GetDescendantProperties(this JsonElement src)
        {
            return GetDescendantPropertiesCore(src);
        }

        internal static IEnumerable<JsonProperty> GetDescendantPropertiesCore(JsonElement src)
        {
            foreach (JsonProperty o in src.ChildrenPropertiesCore())
            {
                yield return o;
                if (o.Value.IsContainer())
                {
                    foreach (JsonProperty d in o.Value.GetDescendantProperties())
                    {
                        yield return d;
                    }
                }
            }
        }

        internal static IEnumerable<JsonProperty> ChildrenPropertiesCore(this JsonElement src)
        {
            if (src.ValueKind == JsonValueKind.Object)
            {
                foreach (var item in src.EnumerateObject())
                {
                    yield return item;
                }
            }

            if (src.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in src.EnumerateArray())
                {
                    foreach (JsonProperty o in item.ChildrenPropertiesCore())
                    {
                        yield return o;
                    }
                }
            }
        }

        public static int CompareTo(this JsonElement value, JsonElement queryValue)
        {
            JsonValueKind comparisonType = (value.ValueKind == JsonValueKind.String && value.ValueKind != queryValue.ValueKind)
                                            ? queryValue.ValueKind
                                            : value.ValueKind;
            return Compare(comparisonType, value, queryValue);
        }

        private static int Compare(JsonValueKind valueType, JsonElement objA, JsonElement objB)
        {
            /*Same types*/
            if (objA.ValueKind == JsonValueKind.Null && objB.ValueKind == JsonValueKind.Null)
            {
                return 0;
            }
            if (objA.ValueKind == JsonValueKind.Undefined && objB.ValueKind == JsonValueKind.Undefined)
            {
                return 0;
            }
            if (objA.ValueKind == JsonValueKind.True && objB.ValueKind == JsonValueKind.True)
            {
                return 0;
            }
            if (objA.ValueKind == JsonValueKind.False && objB.ValueKind == JsonValueKind.False)
            {
                return 0;
            }
            if (objA.ValueKind == JsonValueKind.Number && objB.ValueKind == JsonValueKind.Number)
            {
                return objA.GetDouble().CompareTo(objB.GetDouble());
            }
            if (objA.ValueKind == JsonValueKind.String && objB.ValueKind == JsonValueKind.String)
            {
                return objA.GetString().CompareTo(objB.GetString());
            }
            //When objA is a number and objB is not.
            if (objA.ValueKind == JsonValueKind.Number)
            {
                var valueObjA = objA.GetDouble();
                if (objB.ValueKind == JsonValueKind.String)
                {
                    if (double.TryParse(objB.GetRawText().AsSpan().TrimStart('"').TrimEnd('"'), out double queryValueTyped))
                    {
                        return valueObjA.CompareTo(queryValueTyped);
                    }
                }
            }
            //When objA is a string and objB is not.
            if (objA.ValueKind == JsonValueKind.String)
            {
                if (objB.ValueKind == JsonValueKind.Number)
                {
                    if (double.TryParse(objA.GetRawText().AsSpan().TrimStart('"').TrimEnd('"'), out double valueTyped))
                    {
                        return valueTyped.CompareTo(objB.GetDouble());
                    }
                }
            }
            return -1;
        }
    }

#if NET6_0 || NET7_0
    internal partial class Extensions
    {
        public static JsonValueKind GetValueKind(this JsonNode jsonNode)
        {
            var el = JsonSerializer.SerializeToElement(jsonNode);

            return el.ValueKind;
        }
    }
#endif

#if NET6_0_OR_GREATER

    internal static partial class Extensions
    {
        public static bool IsValue(this JsonNode? src)
        {
            // JsonNode 无法表示 JsonValueKind.Null 但 null 是正常 JsonValue
            if (src == null)
                return true;

            var nodeValueKind = src.GetSafeJsonValueKind();

            return nodeValueKind == JsonValueKind.False ||
                   nodeValueKind == JsonValueKind.True ||
                   nodeValueKind == JsonValueKind.String ||
                   nodeValueKind == JsonValueKind.Number ||
                   nodeValueKind == JsonValueKind.Null ||
                   nodeValueKind == JsonValueKind.Undefined;
        }

        public static bool IsContainer(this JsonNode src)
        {
            var nodeValueKind = src.GetSafeJsonValueKind();

            return nodeValueKind == JsonValueKind.Array || nodeValueKind == JsonValueKind.Object;
        }

        public static bool TryGetFirstFromObject(this JsonNode? src, out JsonNode? element)
        {
            element = null;

            if (src == null)
                return false;

            var nodeValueKind = src.GetSafeJsonValueKind();
            if (nodeValueKind == JsonValueKind.Object)
            {
                var currentObject = src.AsObject();
                var enumerator = currentObject.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    element = enumerator.Current.Value;
                    return true;
                }
            }
            return false;
        }

        public static bool TryMoveNextFromObject(this JsonNode? src, int cycle, out JsonNode? element)
        {
            element = null;

            if (src == null)
                return false;

            var nodeValueKind = src.GetSafeJsonValueKind();

            if (nodeValueKind == JsonValueKind.Object)
            {
                var currentObject = src.AsObject().GetEnumerator();
                for (int i = 0; i < cycle; i++)
                {
                    currentObject.MoveNext();
                }
                element = currentObject.Current.Value;
                return true;
            }
            return false;
        }

        public static IEnumerable<JsonNode?> ChildrenNodes(this JsonNode src)
        {
            var srcValueKind = src.GetSafeJsonValueKind();

            if (srcValueKind == JsonValueKind.Object)
            {
                var srcObject = src.AsObject();
                foreach (var item in srcObject)
                {
                    yield return item.Value;
                }
            }

            if (srcValueKind == JsonValueKind.Array)
            {
                var srcArray = src.AsArray();
                foreach (var item in srcArray)
                {
                    yield return item;
                }
            }
        }

        public static int CompareTo(this JsonNode value, JsonNode queryValue)
        {
            JsonValueKind comparisonType = (value.GetSafeJsonValueKind() == JsonValueKind.String && value.GetSafeJsonValueKind() != queryValue.GetSafeJsonValueKind())
                                            ? queryValue.GetSafeJsonValueKind()
                                            : value.GetSafeJsonValueKind();

            return Compare(comparisonType, value, queryValue);
        }

        private static int Compare(JsonValueKind valueType, JsonNode objA, JsonNode objB)
        {
            JsonValueKind aValueKind = objA.GetSafeJsonValueKind();
            JsonValueKind bValueKind = objB.GetSafeJsonValueKind();

            /*Same types*/
            if (aValueKind == JsonValueKind.Null && bValueKind == JsonValueKind.Null)
            {
                return 0;
            }
            if (aValueKind == JsonValueKind.Undefined && bValueKind == JsonValueKind.Undefined)
            {
                return 0;
            }
            if (aValueKind == JsonValueKind.True && bValueKind == JsonValueKind.True)
            {
                return 0;
            }
            if (aValueKind == JsonValueKind.False && bValueKind == JsonValueKind.False)
            {
                return 0;
            }
            if (aValueKind == JsonValueKind.Number && bValueKind == JsonValueKind.Number)
            {
                return objA.GetDouble().CompareTo(objB.GetDouble());
            }
            if (aValueKind == JsonValueKind.String && bValueKind == JsonValueKind.String)
            {
                return objA.GetString().CompareTo(objB.GetString());
            }
            //When objA is a number and objB is not.
            if (aValueKind == JsonValueKind.Number)
            {
                var valueObjA = objA.GetDouble();
                if (bValueKind == JsonValueKind.String)
                {
                    if (double.TryParse(objB.GetValue<string>().AsSpan().TrimStart('"').TrimEnd('"'), out double queryValueTyped))
                    {
                        return valueObjA.CompareTo(queryValueTyped);
                    }
                }
            }
            //When objA is a string and objB is not.
            if (aValueKind == JsonValueKind.String)
            {
                if (bValueKind == JsonValueKind.Number)
                {
                    if (double.TryParse(objA.GetValue<string>().AsSpan().TrimStart('"').TrimEnd('"'), out double valueTyped))
                    {
                        return valueTyped.CompareTo(objB.GetDouble());
                    }
                }
            }
            return -1;
        }
    }

#endif
}