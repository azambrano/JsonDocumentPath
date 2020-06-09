using System.Text.Json;

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
}
