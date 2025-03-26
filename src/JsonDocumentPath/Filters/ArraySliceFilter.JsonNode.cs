#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    internal partial class ArraySliceFilter
    {
        public override IEnumerable<JsonNode?> ExecuteFilter(JsonNode root, IEnumerable<JsonNode?> current, bool errorWhenNoMatch)
        {
            if (Step == 0)
            {
                throw new JsonException("Step cannot be zero.");
            }

            foreach (JsonNode t in current)
            {
                if (t.GetSafeJsonValueKind() == JsonValueKind.Array)
                {
                    var aCount = t.AsArray().Count;
                    // set defaults for null arguments
                    int stepCount = Step ?? 1;
                    int startIndex = Start ?? ((stepCount > 0) ? 0 : aCount - 1);
                    int stopIndex = End ?? ((stepCount > 0) ? aCount : -1);

                    // start from the end of the list if start is negative
                    if (Start < 0)
                    {
                        startIndex = aCount + startIndex;
                    }

                    // end from the start of the list if stop is negative
                    if (End < 0)
                    {
                        stopIndex = aCount + stopIndex;
                    }

                    // ensure indexes keep within collection bounds
                    startIndex = Math.Max(startIndex, (stepCount > 0) ? 0 : int.MinValue);
                    startIndex = Math.Min(startIndex, (stepCount > 0) ? aCount : aCount - 1);
                    stopIndex = Math.Max(stopIndex, -1);
                    stopIndex = Math.Min(stopIndex, aCount);

                    bool positiveStep = (stepCount > 0);

                    if (IsValid(startIndex, stopIndex, positiveStep))
                    {
                        for (int i = startIndex; IsValid(i, stopIndex, positiveStep); i += stepCount)
                        {
                            yield return t[i];
                        }
                    }
                    else
                    {
                        if (errorWhenNoMatch)
                        {
                            throw new JsonException($"Array slice of {(Start != null ? Start.GetValueOrDefault().ToString() : "*")} to {(End != null ? End.GetValueOrDefault().ToString() : "*")} returned no results.");
                        }
                    }
                }
                else
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonException($"Array slice is not valid on {t.GetType().Name}.");
                    }
                }
            }
        }
    }
}

#endif