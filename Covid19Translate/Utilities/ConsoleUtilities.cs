using System;

namespace Covid19Translate.Utilities
{
    public static class ConsoleUtilities
    {
        public static void Highlight(string s)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(s);
            Console.ResetColor();
        }
    }
}