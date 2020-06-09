using System.Collections.Generic;

namespace System.Text.Json
{
    internal class QueryScanFilter : PathFilter
    {
        internal QueryExpression Expression;

        public QueryScanFilter(QueryExpression expression)
        {
            Expression = expression;
        }

        public override IEnumerable<JsonElement?> ExecuteFilter(JsonElement root, IEnumerable<JsonElement?> current, bool errorWhenNoMatch)
        {
            foreach (JsonElement t in current)
            {
                foreach (var (_, Value) in GetNextScanValue(t))
                {
                    if (Expression.IsMatch(root, Value))
                    {
                        yield return Value;
                    }
                }
            }
        }
    }
}
