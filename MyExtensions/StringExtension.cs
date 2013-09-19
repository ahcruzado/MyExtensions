using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MyExtensions
{
    public static class StringExtension
    {
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        public static string Format(this string str, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, str, args);
        }

        public static string Format(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        public static string Format(this string str, object obj, IFormatProvider provider = null)
        {
            var type = obj.GetType();
            var props = type.GetProperties();

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                str = str.Replace("{" + prop.Name + "}", "{" + i.ToString() + "}");
            }

            var args = props.Select(p => p.GetValue(obj, null)).ToArray();

            if (provider != null)
                return str.Format(provider, args);
            else
                return str.Format(args);
        }

        public static string Variation(this string str, VariationMode mode)
        {
            var result = str;
            if (mode != VariationMode.None)
            {
                var replaceChars = new[] { '*', '.', ':', '!', '_', '-', '#', '%' };
                //char[] replaceChars = new char[] { (char)160, (char)8203, (char)12288, (char)65279, (char)10240 };

                var shuffled = replaceChars.Shuffle();
                string shuffledArray = new string(shuffled.ToArray());

                switch (mode)
                {
                    case VariationMode.AddPlaceHolders:
                        string placeholder = shuffledArray.Substring(0, 5);

                        char[] plac = placeholder.ToArray();
                        Array.Reverse(plac);
                        string reversePlaceHolder = new string(plac);

                        result = placeholder + " " + str + " " + reversePlaceHolder;
                        break;
                    case VariationMode.AlternativeSpaces:
                        char[] resArray = str.ToArray();
                        for (int i = 0; i < resArray.Length; i++)
                        {
                            if (resArray[i] == ' ')
                            {
                                char replacingChar = shuffledArray[i % shuffledArray.Length];
                                resArray[i] = replacingChar;
                            }
                        }
                        result = new string(resArray);
                        break;
                    case VariationMode.TruncateAndAddNumbers:
                        Random rnd = new Random(DateTime.Now.Millisecond);
                        result = str.Substring(0, Math.Min(46, str.Length));
                        result += " " + rnd.Next(1000).ToString();
                        break;
                }
            }
            return result;
        }

        public static string GetTagInnerText(this string str, string tagContent)
        {
            var result = str.GetStringBetween(tagContent, "<");
            result = result.GetStringBetween(">", null);
            return result;
        }

        public static IEnumerable<string> GetTagInnerTextCollection(this string str, string tagContent)
        {
            var result = str.GetStringCollectionBetween(tagContent, "<");
            foreach (var r in result)
            {
                yield return r.GetStringBetween(">", null);
            }
        }

        public static string GetTagInnerTextCollection(this string str, string[] tagContents)
        {
            IEnumerable<string> result = new List<string>();
            foreach (var c in tagContents)
            {
                result = result.Concat(str.GetTagInnerTextCollection(c));
            }
            return string.Join("\n", result);
        }

        /// <summary>
        /// Ottiene una sotto stringa  apartire da before, fino a after.
        /// Se after è null fino alla fine.
        /// Ritorna una stringa vuota se non trova before
        /// </summary>
        /// <param name="str"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public static string GetStringBetween(this string str, string before, string after)
        {
            string result = "";
            int start = str.IndexOf(before);
            if (start != -1)
            {
                start += before.Length;
                int end;
                if (after != null)
                    end = str.IndexOf(after, start, StringComparison.CurrentCulture);
                else
                    end = str.Length;

                int lenght = end - start;
                if (lenght > 0)
                    result = str.Substring(start, lenght);
            }
            return result;
        }

        /// <summary>
        /// Chiama GetStringBetween progressivamente, in modo da ottenere una lista di occorrenze
        /// </summary>
        /// <param name="str"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public static List<string> GetStringCollectionBetween(this string str, string before, string after)
        {
            List<string> result = new List<string>();
            int end = 0;
            do
            {
                str = str.Substring(end, str.Length - end);

                string block = str.GetStringBetween(before, after);
                if (!string.IsNullOrEmpty(block))
                {
                    result.Add(block);
                    // aggiorna end per continuare con l'occorrenza successiva
                    end = str.IndexOf(before, StringComparison.CurrentCulture);
                    // prende la sotto stringa dall'ultimo punto in poi
                    end += (before.Length + block.Length);
                }
                else
                    end = str.Length;

                // questo funzionava
                //str = str.Substring(end, str.Length - end);

                //start = str.IndexOf(before) + before.Length;
                //end = str.IndexOf(after, start, StringComparison.CurrentCulture);

                //if (end > start)
                //{
                //    string block = str.Substring(start, end - start);
                //    result.Add(block);
                //}
            } while (end < str.Length);

            return result;
        }

        /// <summary>
        /// Count occurrences of strings.
        /// </summary>
        public static int CountStringOccurrences(this string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }


        public static object GetTextPartDashed(this string str, bool toLower = true)
        {
            str = str.Replace(' ', '-');
            Regex textChars = new Regex(@"[^a-zA-Z-]");
            var result = textChars.Replace(str, String.Empty);
            if (toLower)
                result = result.ToLowerInvariant();
            return result;
        }

        public static string GetTextPart(this string str)
        {
            Regex textChars = new Regex(@"[^a-zA-Z]");
            var result = textChars.Replace(str, String.Empty);
            return result;
        }

        public static long? GetNumericPart(this string str)
        {
            Regex nonNumericCharacters = new Regex(@"\D");
            string numericOnlyString = nonNumericCharacters.Replace(str, String.Empty);

            long? result = null;
            if (!string.IsNullOrEmpty(numericOnlyString))
            {
                long res;
                if (long.TryParse(numericOnlyString, out res))
                    result = res;
            }
            return result;
        }


        /// <summary>
        /// Restituisce la lista di tutti i link trovati nella stringa
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static List<string> GetLinks(this string txt, bool unstrict = false)
        {
            if (!unstrict)
            {
                var exp = "http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\,]*)?";
                Regex regx = new Regex(exp, RegexOptions.IgnoreCase);
                MatchCollection mactches = regx.Matches(txt);
                List<string> result = new List<string>();
                foreach (Match match in mactches)
                {
                    result.Add(match.Value);
                }
                return result.Distinct().Where(l => !l.Contains("w3.org")).ToList();
            }
            else
            {
                Regex matchRegx = new Regex(@".*\..*", RegexOptions.IgnoreCase);

                List<string> result = new List<string>();
                foreach (var match in txt.Split(new[] { " ", "\t", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var val = match.Trim();

                    if (match.Count(c => c == '"') == 2)
                    {
                        val = val.GetStringBetween("\"", "\"").Trim();
                    }

                    if (matchRegx.IsMatch(val))
                        result.Add(val);
                }
                return result.Distinct().Where(l => !l.Contains("w3.org")).ToList();
            }
        }
    }

    /// <summary>
    /// Preso dal sorgente di Membership.GeneratePassword()
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private static string generatePassword(int length)
    {
        if (length < 1 || length > 128)
        {
            throw new ArgumentException("Length non corretta");
        }
        string text;
        byte[] array = new byte[length];
        char[] array2 = new char[length];
        new RNGCryptoServiceProvider().GetBytes(array);
        for (int i = 0; i < length; i++)
        {
            int num2 = (int)(array[i] % 62);
            if (num2 < 10)
            {   // numeri
                array2[i] = (char)(48 + num2);
            }
            else
            {   // Maiuscole
                if (num2 < 36)
                {
                    array2[i] = (char)(65 + num2 - 10);
                }
                else
                {   // minuscole
                    if (num2 < 62)
                    {
                        array2[i] = (char)(97 + num2 - 36);
                    }
                }
            }
        }
        text = new string(array2);
        return text;
    }

    public enum VariationMode
    {
        AddPlaceHolders,
        AlternativeSpaces,
        TruncateAndAddNumbers,
        None
    }
}
