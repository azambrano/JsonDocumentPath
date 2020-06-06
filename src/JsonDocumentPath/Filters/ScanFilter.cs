using System.Collections.Generic;
using System.Linq;

namespace System.Text.Json
{
    internal class ScanFilter : PathFilter
    {
        internal string? Name;

        public ScanFilter(string? name)
        {
            Name = name;
        }

        public override IEnumerable<JsonElement?> ExecuteFilter(JsonElement root, IEnumerable<JsonElement?> current, bool errorWhenNoMatch)
        {
            foreach (JsonElement c in current)
            {
                if (Name == null)
                {
                    yield return c;
                }

                var de = c.GetDescendantProperties().ToArray();
                foreach (var el in de)
                {
                    if (Name == el.Name)
                    {
                        yield return el.Value;
                    }

                    if (Name == null)
                    {
                        yield return el.Value;
                    }
                }
            }
        }
    }


    public class JsonElementWapper
    {
        private readonly JsonElement? el;
        public JsonElementWapper(JsonElement? element)
        {
            el = element;
        }

        public bool IsContainer()
        {
            return el.IsContainer();
        }
    }
}
