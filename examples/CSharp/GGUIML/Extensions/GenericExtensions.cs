using System;
using System.Linq;
using System.Collections.Generic;

namespace GGUIML.Extensions {
    internal static class GenericExtensions {
        public static void ForEachIter<T> (this IEnumerable<T> values, Action<T, int> callback) {
            for (int i = 0; i < values.Count (); i++) {
                T item = values.ElementAt (i);
                callback.Invoke (item, i);
            }
        }

        public static IEnumerable<U> SelectIter<T, U> (this IEnumerable<T> values, Func<T, int, U> callback) {
            for (int i = 0; i < values.Count (); i++) {
                T item = values.ElementAt (i);
                yield return callback.Invoke (item, i);
            }
        }

        public static string KebabCaseToPascal (this string value) {
            if (string.IsNullOrEmpty (value))
                throw new ArgumentException ("Value must not be null or empty");
            
            string[] parts = value.Split ('-').Select (s => s.ToUpper ()[0] + s.Substring (1)).ToArray ();
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
        public static int ProtectedFindIndex (this string value, params char[] separator) {
            char endChar = '\0';
            for (int i = 0; i < value.Length; i++) {
                char c = value[i];
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
                    return i;
                }
            }
            return -1;
        }

        public static bool ProtectedContains (this string value, params char[] chars) {
            return value.ProtectedFindIndex (chars) != -1;
        }

        public static string[] ProtectedSplit (this string value, params char[] separator) {
            List<string> strings = new List<string> { value };
            int idx;
            while ((idx = strings.Last ().ProtectedFindIndex (separator)) != -1) {
                string lastStr = strings.Last ();
                strings[strings.Count - 1] = lastStr.Substring (0, idx);

                lastStr = lastStr.Substring (idx + 1);
                if (lastStr != "")  // We don't want empty data in our split
                    strings.Add (lastStr);
            }
            return strings.ToArray ();
        }
    }
}