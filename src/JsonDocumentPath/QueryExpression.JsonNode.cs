#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace System.Text.Json
{
    internal partial class QueryExpression
    {
        public abstract bool IsMatch(JsonNode root, JsonNode t);
    }

    internal partial class CompositeExpression
    {
        public override bool IsMatch(JsonNode root, JsonNode t)
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

    internal partial class BooleanQueryExpression
    {
        private IEnumerable<JsonNode?> GetResult(JsonNode root, JsonNode t, object? o)
        {
            if (o is null)
            {
                return [null];
            }
            if (o is JsonNode resultToken)
            {
                return [resultToken];
            }

            if (o is JsonElement resultElement)
            {
                return [JsonNode.Parse(resultElement.GetRawText())];
            }

            if (o is List<PathFilter> pathFilters)
            {
                return JsonNodePath.Evaluate(pathFilters, root, t, false);
            }

            return Enumerable.Empty<JsonNode?>();
        }

        public override bool IsMatch(JsonNode root, JsonNode t)
        {
            if (Operator == QueryOperator.Exists)
            {
                return GetResult(root, t, Left).Any();
            }

            using (IEnumerator<JsonNode?> leftResults = GetResult(root, t, Left).GetEnumerator())
            {
                if (leftResults.MoveNext())
                {
                    IEnumerable<JsonNode?> rightResultsEn = GetResult(root, t, Right);
                    ICollection<JsonNode?> rightResults = rightResultsEn as ICollection<JsonNode?> ?? rightResultsEn.ToList();

                    do
                    {
                        JsonNode? leftResult = leftResults.Current;
                        foreach (JsonNode? rightResult in rightResults)
                        {
                            if (MatchTokens(leftResult, rightResult))
                            {
                                return true;
                            }
                        }
                    } while (leftResults.MoveNext());
                }
            }

            return false;
        }

        private bool MatchTokens(JsonNode leftResult, JsonNode rightResult)
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

        private static bool RegexEquals(JsonNode input, JsonNode pattern)
        {
            var inputValueKind = input.GetSafeJsonValueKind();
            var patternValueKind = pattern.GetSafeJsonValueKind();
            if (inputValueKind != JsonValueKind.String || patternValueKind != JsonValueKind.String)
            {
                return false;
            }

            string regexText = pattern.GetValue<string>();
            int patternOptionDelimiterIndex = regexText.LastIndexOf('/');

            string patternText = regexText.Substring(1, patternOptionDelimiterIndex - 1);
            string optionsText = regexText.Substring(patternOptionDelimiterIndex + 1);

            return Regex.IsMatch(input.GetValue<string>(), patternText, GetRegexOptions(optionsText));
        }

        internal static bool EqualsWithStringCoercion(JsonNode value, JsonNode queryValue)
        {
            if (value.Equals(queryValue))
            {
                return true;
            }

            // Handle comparing an integer with a float
            // e.g. Comparing 1 and 1.0

            var valueKind = value.GetSafeJsonValueKind();
            var queryValueKind = queryValue.GetSafeJsonValueKind();

            if (valueKind == JsonValueKind.Number && queryValueKind == JsonValueKind.Number)
            {
                return value.GetDouble() == queryValue.GetDouble();
            }

            if (queryValueKind != JsonValueKind.String)
            {
                return false;
            }

            return string.Equals(value.ToString(), queryValue.GetString(), StringComparison.Ordinal);
        }

        internal static bool EqualsWithStrictMatch(JsonNode value, JsonNode queryValue)
        {
            // we handle floats and integers the exact same way, so they are pseudo equivalent

            JsonValueKind thisValueKind = value.GetSafeJsonValueKind();
            JsonValueKind queryValueKind = queryValue.GetSafeJsonValueKind();

            if (thisValueKind != queryValueKind)
            {
                return false;
            }

            // Handle comparing an integer with a float
            // e.g. Comparing 1 and 1.0
            if (thisValueKind == JsonValueKind.Number && queryValueKind == JsonValueKind.Number)
            {
                return value.GetValue<double>() == queryValue.GetValue<double>();
            }

            if (thisValueKind == JsonValueKind.String && queryValueKind == JsonValueKind.String)
            {
                return value.GetValue<string>() == queryValue.GetValue<string>();
            }

            if (thisValueKind == JsonValueKind.Null && queryValueKind == JsonValueKind.Null)
            {
                return true;
            }

            if (thisValueKind == JsonValueKind.Undefined && queryValueKind == JsonValueKind.Undefined)
            {
                return true;
            }

            if ((thisValueKind == JsonValueKind.False || thisValueKind == JsonValueKind.True) &&
                (queryValueKind == JsonValueKind.False || queryValueKind == JsonValueKind.True))
            {
                return value.GetValue<bool>() == queryValue.GetValue<bool>();
            }

            return value.Equals(queryValue);
        }
    }
}

#endif