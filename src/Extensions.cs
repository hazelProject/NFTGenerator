using System;
using System.Collections.Generic;
using System.Text;

namespace HazelProject.NFTGenerator
{
    public static class Extensions
    {
        public static Random randomGenerator;
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            if (randomGenerator == null)
                randomGenerator = new Random();

            while (n > 1)
            {
                n--;
                int k = randomGenerator.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static IDictionary<T,V> DeepClone<T,V>(this IDictionary<T,V> dictionary)
        {
            var outputDictionary = new Dictionary<T, V>();
            foreach(var entry in dictionary)
            {
                outputDictionary.Add(entry.Key, entry.Value);
            }

            return outputDictionary;
        }

        public static void WriteColoredLine(object text, ConsoleColor foreground, ConsoleColor background){
            WriteColored(text, foreground, background);
            Console.WriteLine();
        }

        public static void WriteColored(object text, ConsoleColor foreground, ConsoleColor background){
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
            Console.Write(text);
            Console.ResetColor();
        }

         public static void WriteError(object text){
           WriteColoredLine(text, ConsoleColor.Black, ConsoleColor.Red);
        }
    }

}
