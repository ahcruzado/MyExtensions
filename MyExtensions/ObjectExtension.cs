using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyExtensions
{
    public static class ObjectExtension
    {
        public static TOut? SafeNullable<TIn, TOut>(this TIn v, Func<TIn, TOut> f) where TOut : struct
        {
            var result = Safe(v, f);
            var def = default(TOut);
            if (result.Equals(def))
                return null;
            else
                return result;
        }

        public static TOut Safe<TIn, TOut>(this TIn v, Func<TIn, TOut> f, TOut def = default(TOut))
        {
            TOut result;
            try
            {
                result = f(v);
            }
            catch
            {
                result = def;
            }
            return result;
        }
    }
}
