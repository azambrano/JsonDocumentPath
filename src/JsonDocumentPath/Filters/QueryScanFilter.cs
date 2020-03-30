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
                if (t.IsContainer())
                {
                    foreach (JsonElement d in t.DescendantsAndSelf())
                    {
                        if (Expression.IsMatch(root, d))
                        {
                            yield return d;
                        }
                    }
                }
                else
                {
                    if (Expression.IsMatch(root, t))
                    {
                        yield return t;
                    }
                }
            }
        }
    }
}
