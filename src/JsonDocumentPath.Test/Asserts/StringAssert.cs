using System.Text.RegularExpressions;
using Xunit;

namespace JDocument.Test
{
    public static class StringAssert
    {
        private static readonly Regex Regex = new Regex(@"\r\n|\n\r|\n|\r", RegexOptions.CultureInvariant);

        public static void AreEqual(string expected, string actual)
        {
            expected = Normalize(expected);
            actual = Normalize(actual);

            Assert.Equal(expected, actual);
        }

        public static bool Equals(string s1, string s2)
        {
            s1 = Normalize(s1);
            s2 = Normalize(s2);

            return string.Equals(s1, s2);
        }

        public static string Normalize(string s)
        {
            if (s != null)
            {
                s = Regex.Replace(s, "\r\n");
            }

            return s;
        }
    }
}
