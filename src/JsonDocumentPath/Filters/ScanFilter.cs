using System.Collections.Generic;

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
            throw new NotImplementedException("ScanFilter is not supported");

            foreach (JsonElement c in current)
            {
                if (Name == null)
                {
                    yield return c;
                }

                JsonElement? value = c;

                while (true)
                {
                    JsonElement? container = value.IsContainer() ? value : new JsonElement?();
                    value = GetNextScanValue(c, container, value);


                    //JContainer? container = value as JContainer;

                    //value = GetNextScanValue(c, container, value);
                    //if (value == null)
                    //{
                    //    break;
                    //}

                    //if (value is JProperty property)
                    //{
                    //    if (property.Name == Name)
                    //    {
                    //        yield return property.Value;
                    //    }
                    //}
                    //else
                    //{
                    //    if (Name == null)
                    //    {
                    //        yield return value;
                    //    }
                    //}
                }
            }
        }
    }
}
