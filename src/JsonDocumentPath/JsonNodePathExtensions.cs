#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Text.Json.Nodes;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.Text.Json
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配
{
    public static class JsonNodePathExtensions
    {
        /// <summary>
        /// Selects a collection of elements using a JSONPath expression.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="JsonElement"/> that contains the selected elements.</returns>
        public static IEnumerable<System.Text.Json.Nodes.JsonNode?> SelectNodes(this System.Text.Json.Nodes.JsonNode src, string path, bool errorWhenNoMatch = false)
        {
            var parser = new JsonNodePath(path);
            return parser.Evaluate(src, src, errorWhenNoMatch);
        }

        /// <summary>
        /// Selects a <see cref="JsonElement"/> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>A <see cref="JsonDocument"/>.</returns>
        public static JsonNode? SelectNode(this JsonNode src, string path, bool errorWhenNoMatch = false)
        {
            var p = new JsonNodePath(path);
            JsonNode? el = null;
            foreach (JsonNode? t in p.Evaluate(src, src, errorWhenNoMatch))
            {
                if (el != null)
                {
                    throw new JsonException("Path returned multiple tokens.");
                }
                el = t;
            }
            return el;
        }
    }
}

#endif