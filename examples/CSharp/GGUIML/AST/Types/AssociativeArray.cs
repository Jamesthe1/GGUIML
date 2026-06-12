using System;
using System.Collections.Generic;
using System.Linq;
using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    public struct AssociativeArray<T> : IArgumentType {
        public Dictionary<LineSplitString, T> Data { get; set; }

        public static AssociativeArray<T> Parse (string data, Func<string, T> valueParser) {
            Dictionary<LineSplitString, T> result = new Dictionary<LineSplitString, T> ();
            string[] lines = data.Split ('\n');
            foreach (string line in lines) {
                if (!line.StartsWith ("-") || !line.Contains (':'))
                    throw new FormatException ("One or more lines do not contain proper associative array formatting");
                string[] pair = line.Substring (1).NoQuotesSplit (':').Select (s => s.Trim ()).ToArray ();
                if (pair.Length != 2)
                    throw new FormatException ("Array entries must be a pair");
                
                LineSplitString key = LineSplitString.Parse (pair[0]);
                T value = valueParser.Invoke (pair[1]);
                result.Add (key, value);
            }
            
            return new AssociativeArray<T> {
                Data = result
            };
        }
    }
}