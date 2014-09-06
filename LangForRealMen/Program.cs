using System;
using LangForRealMen.ParserLogic;

namespace LangForRealMen
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.Write(" > ");
                Parser.GetParser().Execute(Console.ReadLine());
            }
        }
    }
}
