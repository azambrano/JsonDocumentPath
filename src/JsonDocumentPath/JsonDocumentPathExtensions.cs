using System.Collections.Generic;
using System.Linq;

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
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="JsonDocument"/> that contains the selected elements.</returns>

        public static IEnumerable<JsonElement?> SelectElements(this JsonDocument src, string path)
        {
            return SelectElements(src.RootElement, path, false);
        }

        /// <summary>
        /// Selects a collection of elements using a JSONPath expression.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="JsonDocument"/> that contains the selected elements.</returns>

        public static IEnumerable<JsonElementExt> SelectExtElements(this JsonDocument src, string path)
        {
            return SelectExtElements(src.RootElement, path, false);
        }

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
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="JsonElementExt"/> that contains the selected elements.</returns>

        public static IEnumerable<JsonElementExt> SelectExtElements(this JsonElement src, string path)
        {
            return SelectExtElements(src, path, false);
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
            return parser.Evaluate(src, src, errorWhenNoMatch).Select(x=>x.Element);
        }

        /// <summary>
        /// Selects a collection of elements using a JSONPath expression.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="JsonElementExt"/> that contains the selected elements.</returns>
        public static IEnumerable<JsonElementExt> SelectExtElements(this JsonElement src, string path, bool errorWhenNoMatch)
        {
            var parser = new JsonDocumentPath(path);
            return parser.Evaluate(src, src, errorWhenNoMatch);
        }

        /// <summary>
        /// Selects a collection of elements using a JSONPath expression.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="JsonDocument"/> that contains the selected elements.</returns>
        public static IEnumerable<JsonElement?> SelectElements(this JsonDocument src, string path, bool errorWhenNoMatch)
        {
            var parser = new JsonDocumentPath(path);
            return parser.Evaluate(src.RootElement, src.RootElement, errorWhenNoMatch).Select(x=>x.Element);
        }

        /// <summary>
        /// Selects a collection of elements using a JSONPath expression.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="JsonDocument"/> that contains the selected elements.</returns>
        public static IEnumerable<JsonElementExt> SelectExtElements(this JsonDocument src, string path, bool errorWhenNoMatch)
        {
            var parser = new JsonDocumentPath(path);
            return parser.Evaluate(src.RootElement, src.RootElement, errorWhenNoMatch);
        }

        /// <summary>
        /// Selects a <see cref="JsonElement"/> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <returns>A <see cref="JsonDocument"/>, or <c>null</c>.</returns>
        public static JsonElement? SelectElement(this JsonDocument src, string path)
        {
            return SelectElement(src.RootElement, path, false);
        }

        /// <summary>
        /// Selects a <see cref="JsonElementExt"/> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <returns>A <see cref="JsonDocument"/>, or <c>null</c>.</returns>
        public static JsonElementExt SelectExtElement(this JsonDocument src, string path)
        {
            return SelectExtElement(src.RootElement, path, false);
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
        /// Selects a <see cref="JsonElementExt"/> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <returns>A <see cref="JsonElementExt"/>, or <c>null</c>.</returns>
        public static JsonElementExt SelectExtElement(this JsonElement src, string path)
        {
            return SelectExtElement(src, path, false);
        }

        /// <summary>
        /// Selects a <see cref="JsonElement"/> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>A <see cref="JsonDocument"/>.</returns>
        public static JsonElement? SelectElement(this JsonDocument src, string path, bool errorWhenNoMatch)
        {
            var p = new JsonDocumentPath(path);
            JsonElementExt el = null;
            foreach (JsonElementExt t in p.Evaluate(src.RootElement, src.RootElement, errorWhenNoMatch))
            {
                if (el != null)
                {
                    throw new JsonException("Path returned multiple tokens.");
                }
                el = t;
            }
            return el.Element;
        }

        /// <summary>
        /// Selects a <see cref="JsonElementExt"/> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>A <see cref="JsonDocument"/>.</returns>
        public static JsonElementExt SelectExtElement(this JsonDocument src, string path, bool errorWhenNoMatch)
        {
            var p = new JsonDocumentPath(path);
            JsonElementExt el = null;
            foreach (JsonElementExt t in p.Evaluate(src.RootElement, src.RootElement, errorWhenNoMatch))
            {
                if (el != null)
                {
                    throw new JsonException("Path returned multiple tokens.");
                }
                el = t;
            }
            return el;
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
            JsonElementExt el = null;
            foreach (JsonElementExt t in p.Evaluate(src, src, errorWhenNoMatch))
            {
                if (el != null)
                {
                    throw new JsonException("Path returned multiple tokens.");
                }
                el = t;
            }
            return el?.Element;
        }

        /// <summary>
        /// Selects a <see cref="JsonElementExt"/> using a JSONPath expression. Selects the token that matches the object path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="String"/> that contains a JSONPath expression.
        /// </param>
        /// <param name="errorWhenNoMatch">A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.</param>
        /// <returns>A <see cref="JsonElementExt"/>.</returns>
        public static JsonElementExt SelectExtElement(this JsonElement src, string path, bool errorWhenNoMatch)
        {
            var p = new JsonDocumentPath(path);
            JsonElementExt el = null;
            foreach (JsonElementExt t in p.Evaluate(src, src, errorWhenNoMatch))
            {
                if (el != null)
                {
                    throw new JsonException("Path returned multiple tokens.");
                }
                el = t;
            }
            return el;
        }

        /// <summary>
        /// Gets the value of the element as a System.Object.
        /// </summary>
        public static object GetObjectValue(this JsonElement src)
        {
            if (src.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            if (src.ValueKind == JsonValueKind.String)
            {
                return src.GetString();
            }
            if (src.ValueKind == JsonValueKind.False || src.ValueKind == JsonValueKind.True)
            {
                return src.GetBoolean();
            }
            if (src.ValueKind == JsonValueKind.Number)
            {
                return src.GetDouble();
            }
            return src.GetRawText();
        }
    }
}
