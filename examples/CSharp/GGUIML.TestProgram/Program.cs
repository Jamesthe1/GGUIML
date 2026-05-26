
using System.IO;
using GGUIML;

public class Program {
    static void Main (string[] args) {
        FileStream stream = File.OpenRead ("example.ggui");
        GUILParser<object> parser = new GUILParser<object> (stream);
        parser.DebugPrintRawTree ();
    }
}