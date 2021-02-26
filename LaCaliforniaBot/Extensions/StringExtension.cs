using System;
using System.Text;

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

        public static string RemoveEmojis(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            foreach (var a in str)
            {
                byte[] bts = Encoding.UTF32.GetBytes(a.ToString());

                if (bts[0].ToString() == "253" && bts[1].ToString() == "255")
                {
                    str = str.Replace(a.ToString(), "");
                }

            }
            return str;
        }
    }
}
