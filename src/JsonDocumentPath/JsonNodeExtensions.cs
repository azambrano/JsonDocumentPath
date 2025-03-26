#if NET6_0_OR_GREATER

using System.Text.Json.Nodes;

namespace System.Text.Json;

internal static class JsonNodeExtensions
{
    public static string GetString(this JsonNode node)
    {
        return node.GetValue<string>();
    }

    public static bool GetBoolean(this JsonNode node)
    {
        return node.GetValue<bool>();
    }

    public static double GetDouble(this JsonNode node)
    {
        return node.GetValue<double>();
    }

    public static int GetInt32(this JsonNode node)
    {
        return node.GetValue<int>();
    }

    public static object? GetObjectValue(this JsonNode node)
    {
        var nodeValueKind = node.GetSafeJsonValueKind();

        return nodeValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.String => node.GetString(),
            JsonValueKind.Number => node.GetDouble(),
            JsonValueKind.True or JsonValueKind.False => node.GetBoolean(),
            _ => node.ToJsonString(),
        };
    }

    /// <summary>
    /// 获取安全的 JsonValueKind
    /// </summary>
    /// <remarks>JsonNode 无法表示 JsonValueKind.Null </remarks>
    /// <param name="node"></param>
    /// <returns></returns>
    public static JsonValueKind GetSafeJsonValueKind(this JsonNode? node)
    {
        if (node == null)
            return JsonValueKind.Null;

        return node.GetValueKind();
    }
}

#endif