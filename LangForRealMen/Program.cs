using System;
using System.IO;
using LangForRealMen.ParserLogic;

namespace LangForRealMen
{
    class Program
    {
        static void Main()
        {
            var sr = new StreamReader("input.txt");
            var data = sr.ReadToEnd();
            Parser.GetParser().Parse(data);
            Console.WriteLine(Parser.GetParser()._program);
            Console.ReadKey();
        }
    }
}
