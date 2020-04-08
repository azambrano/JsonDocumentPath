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
            foreach (JsonElement t in current)
            {
                foreach (JsonElement v in t.ChildrenTokens())
                {
                    if (Expression.IsMatch(root, v))
                    {
                        yield return v;
                    }
                }
            }
        }
    }
}
