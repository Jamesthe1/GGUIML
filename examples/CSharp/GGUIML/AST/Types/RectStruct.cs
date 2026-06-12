using System;
using System.Linq;
using GGUIML.Extensions;

namespace GGUIML.AST.Types {
    public struct RectStruct : IArgumentType {
        public int Top { get; set; }
        public int Right { get; set; }
        public int Down { get; set; }
        public int Left { get; set; }

        public static readonly RectStruct Zero = new RectStruct { Left = 0, Top = 0, Right = 0, Down = 0 };

        public static RectStruct Parse (string data) {
            data = data.Trim ();
            if (!data.StartsWith ("[") || !data.EndsWith ("]"))
                throw new FormatException ("Provided data missing square brackets");
            
            data = data.Substring (1, data.Length - 2);
            int[] args = data.Split (',').Select (a => int.Parse (a.Trim())).ToArray ();
            switch (args.Length) {
                case 4:
                    return new RectStruct {
                        Top = args[0],
                        Right = args[1],
                        Down = args[2],
                        Left = args[3]
                    };
                case 2:
                    return new RectStruct {
                        Top = args[0],
                        Right = args[1],
                        Down = args[0],
                        Left = args[1]
                    };
                case 1:
                    return new RectStruct {
                        Top = args[0],
                        Right = args[0],
                        Down = args[0],
                        Left = args[0]
                    };
                default:
                    throw new FormatException ("Invalid number of arguments");
            }
        }
    }
}