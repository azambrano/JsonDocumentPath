using System.Collections.Generic;

namespace System.Text.Json
{
    internal class QueryFilter : PathFilter
    {
        internal QueryExpression Expression;

        public QueryFilter(QueryExpression expression)
        {
            Expression = expression;
        }

        public override IEnumerable<JsonElement?> ExecuteFilter(JsonElement root, IEnumerable<JsonElement?> current, bool errorWhenNoMatch)
        {
            foreach (JsonElement el in current)
            {
                if (el.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement v in el.EnumerateArray())
                    {
                        if (Expression.IsMatch(root, v))
                        {
                            yield return v;
                        }
                    }
                }
                else if (el.ValueKind == JsonValueKind.Object)
                {
                    foreach (JsonProperty v in el.EnumerateObject())
                    {
                        if (Expression.IsMatch(root, v.Value))
                        {
                            yield return v.Value;
                        }
                    }
                }
            }
        }
    }
}
