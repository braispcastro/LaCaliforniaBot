using System;
using System.Text.RegularExpressions;

namespace LaCaliforniaBot.Extensions
{
    public static class StringExtension
    {
        public static string LimitWords(this string str, int maxChars = 100)
        {
            // Si ponemos 0 o menos nos saltamos la limitación
            if (maxChars < 1)
                return str;

            int acumulado = 0;
            var result = string.Empty;

            var strArray = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in strArray)
            {
                acumulado += s.Length;
                if (acumulado > maxChars)
                    break;
                result = string.Concat(result, s, " ");
            }

            return result.Trim();
        }

        public static string RemoveLinks(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return Regex.Replace(str.ToLowerInvariant(), @"(http[^\s]+)|(www\.[^\s]+)", "");
        }
    }
}
