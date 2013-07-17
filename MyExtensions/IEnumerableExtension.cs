using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MyExtensions
{
    public static class IEnumerableExtension
    {
        public static string ToStringAsCollection(this IEnumerable coll)
        {
            var vals = new List<string>();
            foreach (var val in coll)
                vals.Add(val.ToString());

            var result = vals.Count() > 0 ?
                         string.Join("|", vals) :
                         null;
            return result;
        }

        public static IEnumerable<T> ToCollection<T>(this string collString)
        {
            var collectionType = typeof(T);
            foreach (var split in collString.Split('|'))
            {
                object obj;
                if (collectionType.IsEnum)
                    obj = Enum.Parse(collectionType, split);
                else
                    obj = Convert.ChangeType(split, collectionType);

                yield return (T)obj;
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> coll)
        {
            // todo: valutare se questo è corretto e può sostituire quello sotto
            //return coll.Random().ToList();
            Random random = new Random(DateTime.Now.Millisecond);
            if (coll == null) throw new ArgumentNullException("source");
            if (random == null) throw new ArgumentNullException("generator");

            //copy
            var result = coll.ToList();
            //shuffle the copy
            for (int i = result.Count - 1; i > 0; i--)
            {
                int RandomIndex = random.Next(i + 1);
                T temp = result[i];
                result[i] = result[RandomIndex];
                result[RandomIndex] = temp;
            }

            return result;

            //SortedDictionary<int, T> dict = new SortedDictionary<int, T>();

            //// Add all strings from array
            //// Add new random int each time
            //foreach (T s in coll)
            //{
            //    dict.Add(random.Next(), s);
            //}

            //return dict.Values;
        }

    }
}
