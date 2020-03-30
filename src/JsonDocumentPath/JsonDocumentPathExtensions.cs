using System.Collections.Generic;

namespace System.Text.Json
{
    public static class JsonDocumentPathExtensions
    {
        /// <summary>
        /// Selects a collection of elements using a JSONPath expression.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="JsonElement"/> that contains the selected elements.</returns>

        public static IEnumerable<JsonElement?> SelectElements(this JsonElement src, string path)
        {
            return SelectElements(src, path, false);
        }

        /// <summary>
        /// Selects a collection of elements using a JSONPath expression.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="JsonElement"/> that contains the selected elements.</returns>
        public static IEnumerable<JsonElement?> SelectElements(this JsonElement src, string path, bool errorWhenNoMatch)
        {
            var parser = new JsonDocumentPath(path);
            return parser.Evaluate(src, src, errorWhenNoMatch);
        }

        /// <summary>
        /// Selects a <see cref="JsonElement"/> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <returns>A <see cref="JsonElement"/>, or <c>null</c>.</returns>
        public static JsonElement? SelectElement(this JsonElement src, string path)
        {
            return SelectElement(src, path, false);
        }

        /// <summary>
        /// Selects a <see cref="JsonElement"/> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>A <see cref="JsonElement"/>.</returns>
        public static JsonElement? SelectElement(this JsonElement src, string path, bool errorWhenNoMatch)
        {
            var p = new JsonDocumentPath(path);
            JsonElement? el = null;
            foreach (JsonElement t in p.Evaluate(src, src, errorWhenNoMatch))
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
