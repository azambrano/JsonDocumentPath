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

        public override IEnumerable<JsonElementExt> ExecuteFilter(JsonElement root, IEnumerable<JsonElementExt> current, bool errorWhenNoMatch)
        {
            foreach (JsonElementExt el in current)
            {
                if (el.Element.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement v in el.Element.Value.EnumerateArray())
                    {
                        if (Expression.IsMatch(root, v))
                        {
                            yield return new JsonElementExt(){ Element = v };
                        }
                    }
                }
                else if (el.Element.Value.ValueKind == JsonValueKind.Object)
                {
                    foreach (JsonProperty v in el.Element.Value.EnumerateObject())
                    {
                        if (Expression.IsMatch(root, v.Value))
                        {
                            yield return new JsonElementExt(){ Element = v.Value, Name = v.Name };
                        }
                    }
                }
            }
        }
    }
}
