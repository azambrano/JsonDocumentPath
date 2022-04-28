using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.Text.Json
{
    internal enum QueryOperator
    {
        None = 0,
        Equals = 1,
        NotEquals = 2,
        Exists = 3,
        LessThan = 4,
        LessThanOrEquals = 5,
        GreaterThan = 6,
        GreaterThanOrEquals = 7,
        And = 8,
        Or = 9,
        RegexEquals = 10,
        StrictEquals = 11,
        StrictNotEquals = 12
    }

    internal abstract class QueryExpression
    {
        internal QueryOperator Operator;

        public QueryExpression(QueryOperator @operator)
        {
            Operator = @operator;
        }

        public abstract bool IsMatch(JsonElement root, JsonElement t);
    }

    internal class CompositeExpression : QueryExpression
    {
        public List<QueryExpression> Expressions { get; set; }

        public CompositeExpression(QueryOperator @operator) : base(@operator)
        {
            Expressions = new List<QueryExpression>();
        }

        public override bool IsMatch(JsonElement root, JsonElement t)
        {
            switch (Operator)
            {
                case QueryOperator.And:
                    foreach (QueryExpression e in Expressions)
                    {
                        if (!e.IsMatch(root, t))
                        {
                            return false;
                        }
                    }
                    return true;
                case QueryOperator.Or:
                    foreach (QueryExpression e in Expressions)
                    {
                        if (e.IsMatch(root, t))
                        {
                            return true;
                        }
                    }
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal class BooleanQueryExpression : QueryExpression
    {
        public readonly object Left;
        public readonly object? Right;

        public BooleanQueryExpression(QueryOperator @operator, object left, object? right) : base(@operator)
        {
            Left = left;
            Right = right;
        }

        private IEnumerable<JsonElementExt> GetResult(JsonElement root, JsonElement t, object? o)
        {
            if (o is JsonElement resultToken)
            {
                return new JsonElementExt[1] { new JsonElementExt(){ Element=resultToken } };
            }

            if (o is List<PathFilter> pathFilters)
            {
                return JsonDocumentPath.Evaluate(pathFilters, root, t, false);
            }

            return Enumerable.Empty<JsonElementExt>();
        }

        public override bool IsMatch(JsonElement root, JsonElement t)
        {
            if (Operator == QueryOperator.Exists)
            {
                return GetResult(root, t, Left).Any();
            }

            using (IEnumerator<JsonElementExt> leftResults = GetResult(root, t, Left).GetEnumerator())
            {
                if (leftResults.MoveNext())
                {
                    IEnumerable<JsonElementExt> rightResultsEn = GetResult(root, t, Right);
                    ICollection<JsonElementExt> rightResults = rightResultsEn as ICollection<JsonElementExt> ?? rightResultsEn.ToList();

                    do
                    {
                        JsonElement? leftResult = leftResults.Current.Element;
                        foreach (JsonElementExt rightResult in rightResults)
                        {
                            if (MatchTokens(leftResult.Value, rightResult.Element.Value))
                            {
                                return true;
                            }
                        }
                    } while (leftResults.MoveNext());
                }
            }

            return false;
        }

        private bool MatchTokens(JsonElement leftResult, JsonElement rightResult)
        {
            if (leftResult.IsValue() && rightResult.IsValue())
            {
                switch (Operator)
                {
                    case QueryOperator.RegexEquals:
                        if (RegexEquals(leftResult, rightResult))
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.Equals:
                        if (EqualsWithStringCoercion(leftResult, rightResult))
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.StrictEquals:
                        if (EqualsWithStrictMatch(leftResult, rightResult))
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.NotEquals:
                        if (!EqualsWithStringCoercion(leftResult, rightResult))
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.StrictNotEquals:
                        if (!EqualsWithStrictMatch(leftResult, rightResult))
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.GreaterThan:
                        if (leftResult.CompareTo(rightResult) > 0)
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.GreaterThanOrEquals:
                        if (leftResult.CompareTo(rightResult) >= 0)
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.LessThan:
                        if (leftResult.CompareTo(rightResult) < 0)
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.LessThanOrEquals:
                        if (leftResult.CompareTo(rightResult) <= 0)
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.Exists:
                        return true;
                }
            }
            else
            {
                switch (Operator)
                {
                    case QueryOperator.Exists:
                    // you can only specify primitive types in a comparison
                    // notequals will always be true
                    case QueryOperator.NotEquals:
                        return true;
                }
            }

            return false;
        }

        private static bool RegexEquals(JsonElement input, JsonElement pattern)
        {
            if (input.ValueKind != JsonValueKind.String || pattern.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            string regexText = pattern.GetString();
            int patternOptionDelimiterIndex = regexText.LastIndexOf('/');

            string patternText = regexText.Substring(1, patternOptionDelimiterIndex - 1);
            string optionsText = regexText.Substring(patternOptionDelimiterIndex + 1);

            return Regex.IsMatch(input.GetString(), patternText, GetRegexOptions(optionsText));
        }

        internal static bool EqualsWithStringCoercion(JsonElement value, JsonElement queryValue)
        {
            if (value.Equals(queryValue))
            {
                return true;
            }

            // Handle comparing an integer with a float
            // e.g. Comparing 1 and 1.0
            if (value.ValueKind == JsonValueKind.Number && queryValue.ValueKind == JsonValueKind.Number)
            {
                return value.GetDouble() == queryValue.GetDouble();
            }

            if (queryValue.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            return string.Equals(value.ToString(), queryValue.GetString(), StringComparison.Ordinal);
        }

        internal static bool EqualsWithStrictMatch(JsonElement value, JsonElement queryValue)
        {
            // we handle floats and integers the exact same way, so they are pseudo equivalent
            if (value.ValueKind != queryValue.ValueKind)
            {
                return false;
            }

            // Handle comparing an integer with a float
            // e.g. Comparing 1 and 1.0
            if (value.ValueKind == JsonValueKind.Number && queryValue.ValueKind == JsonValueKind.Number)
            {
                return value.GetDouble() == queryValue.GetDouble();
            }

            if (value.ValueKind == JsonValueKind.String && queryValue.ValueKind == JsonValueKind.String)
            {
                return value.GetString() == queryValue.GetString();
            }

            if (value.ValueKind == JsonValueKind.Null && queryValue.ValueKind == JsonValueKind.Null)
            {
                return true;
            }

            if (value.ValueKind == JsonValueKind.Undefined && queryValue.ValueKind == JsonValueKind.Undefined)
            {
                return true;
            }

            if ((value.ValueKind == JsonValueKind.False || value.ValueKind == JsonValueKind.True) && 
                (queryValue.ValueKind == JsonValueKind.False || queryValue.ValueKind == JsonValueKind.True))
            {
                return value.GetBoolean() == queryValue.GetBoolean();
            }

            return value.Equals(queryValue);
        }

        internal static RegexOptions GetRegexOptions(string optionsText)
        {
            RegexOptions options = RegexOptions.None;

            for (int i = 0; i < optionsText.Length; i++)
            {
                switch (optionsText[i])
                {
                    case 'i':
                        options |= RegexOptions.IgnoreCase;
                        break;
                    case 'm':
                        options |= RegexOptions.Multiline;
                        break;
                    case 's':
                        options |= RegexOptions.Singleline;
                        break;
                    case 'x':
                        options |= RegexOptions.ExplicitCapture;
                        break;
                }
            }

            return options;
        }
    }
}
