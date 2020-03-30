using System.Collections.Generic;
using System.Linq;

namespace System.Text.Json
{
    internal static class Extensions
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
        public static bool TryGetFirstContainerValue(this JsonElement? src, out JsonElement element)
        {
            element = default;
            if (src.HasValue)
            {
                return src.Value.TryGetFirstContainerValue(out element);
            }
            return false;
        }
        public static bool TryGetFirstContainerValue(this JsonElement src, out JsonElement element)
        {
            element = default;

            if (src.ValueKind == JsonValueKind.Object)
            {
                var elValue = src.EnumerateObject().Current.Value;
                if (elValue.ValueKind != JsonValueKind.Undefined)
                {
                    element = elValue;
                }
            }

            if (src.ValueKind == JsonValueKind.Array && src.GetArrayLength() > 0)
            {
                element = src.EnumerateArray().Current;
            }

            return false;
        }

        public static IEnumerable<JsonElement> DescendantsAndSelf(this IEnumerable<JsonElement> source)
        {
            return source.SelectMany(j => j.DescendantsAndSelf());
        }

        public static IEnumerable<JsonElement> Descendants(this JsonElement src)
        {
            return GetDescendants(src, false);
        }

        public static IEnumerable<JsonElement> DescendantsAndSelf(this JsonElement src)
        {
            return GetDescendants(src, true);
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

        internal static IEnumerable<JsonElement> GetDescendants(JsonElement src, bool self)
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
                    foreach (JsonElement d in o.Descendants())
                    {
                        yield return d;
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
}
