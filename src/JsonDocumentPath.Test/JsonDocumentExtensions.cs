using System.Text.Json;
using System.Text.Json.Nodes;

namespace JDocument.Test
{
    public static class JsonDocumentExtensions
    {
        public static bool DeepEquals(this JsonElement left, JsonElement? right)
        {
            if (right == null)
            {
                return false;
            }
            var jsonString = left.ToString();
            var jsonStringR = right.Value.ToString();
            return jsonString == jsonStringR;
        }

        public static bool DeepEquals(this JsonDocument left, JsonElement? right)
        {
            return DeepEquals(left.RootElement, right);
        }

        public static bool DeepEquals(this JsonDocument left, JsonDocument? right)
        {
            return DeepEquals(left.RootElement, right?.RootElement);
        }
    }

    public static class JsonNodeExtensions
    {
        public static bool DeepEquals(this JsonNode left, JsonNode? right)
        {
            if (right == null)
            {
                return false;
            }
            var jsonString = left.ToJsonString();
            var jsonStringR = right.ToJsonString();
            return jsonString == jsonStringR;
        }
    }
}