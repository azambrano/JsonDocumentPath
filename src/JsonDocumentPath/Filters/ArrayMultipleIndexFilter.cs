using System.Collections.Generic;

namespace System.Text.Json
{
    internal class ArrayMultipleIndexFilter : PathFilter
    {
        internal List<int> Indexes;

        public ArrayMultipleIndexFilter(List<int> indexes)
        {
            Indexes = indexes;
        }

        public override IEnumerable<JsonElementExt> ExecuteFilter(JsonElement root, IEnumerable<JsonElementExt> current, bool errorWhenNoMatch)
        {
            foreach (JsonElementExt t in current)
            {
                foreach (int i in Indexes)
                {
                    JsonElementExt v = GetTokenIndex(t.Element.Value, errorWhenNoMatch, i);

                    if (v != null)
                    {
                        yield return v;
                    }
                }
            }
        }
    }
}
