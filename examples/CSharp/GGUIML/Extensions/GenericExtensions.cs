namespace GGUIML.Extensions {
    internal static class GenericExtensions {
        public static void ForEachIter<T> (this IEnumerable<T> values, Action<T, int> callback) {
            for (int i = 0; i < values.Count (); i++) {
                T item = values.ElementAt (i);
                callback.Invoke (item, i);
            }
        }

        public static string KebabCaseToPascal (this string value) {
            if (string.IsNullOrEmpty (value))
                throw new ArgumentException ("Value must not be null or empty");
            
            string[] parts = (string[])value.Split ('-').Select (s => s.ToUpper ()[0] + s.Substring (1));
            return string.Join ("", parts);
        }

        public static bool EnumContainsConverted (this Type enumType, string value) {
            string[] names = Enum.GetNames (enumType);
            return names.Contains (value.KebabCaseToPascal ());
        }

        private static Dictionary<char, char> ProtectedRegions = new Dictionary<char, char> {
            { '\'', '\'' },
            { '"', '"' },
            { '(', ')' },
            { '[', ']' },
            { '{', '}' }
        };
        public static string[] ProtectedSplit (this string value, params char[] separator) {
            List<string> strings = new List<string> { "" };
            char endChar = '\0';
            foreach (char c in value) {
                if (endChar != '\0') {
                    if (c == endChar)
                        endChar = '\0';
                    continue;
                }
                if (ProtectedRegions.ContainsKey (c)) {
                    endChar = ProtectedRegions[c];
                    continue;
                }

                if (separator.Contains (c)) {
                    if (strings.Last () != "")
                    strings.Add ("");
                    continue;
                }
                strings[strings.Count - 1] += c.ToString ();    // Can't assign to the result returned by a .Last() call...
            }
            return strings.ToArray ();
        }
    }
}