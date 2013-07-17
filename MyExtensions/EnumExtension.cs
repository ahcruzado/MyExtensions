using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MyExtensions
{
    public static class EnumExtension
    {
        public static IEnumerable<DisplayAttribute> GetDisplayAttributes<TEnum>(TEnum value)
        {
            return GetDisplayAttributes(typeof(TEnum), value as Enum);
        }

        public static IEnumerable<DisplayAttribute> GetDisplayAttributes(Type enumType, Enum value)
        {
            enumType = UnboxNullable(enumType);

            var values = SplitFlags(value);

            var result = values.Select(v =>
            {
                DisplayAttribute da = null;
                var field = enumType.GetField(v.ToString());
                if (field != null)
                {
                    var customAttribute = (DisplayAttribute)DisplayAttribute.GetCustomAttribute(field, typeof(DisplayAttribute));
                    if (customAttribute != null)
                        da = customAttribute;
                    else
                        da = new DisplayAttribute() { Name = v.ToString() };
                }
                return da;
            }).Where(da => da != null);
            return result;
        }

        public static string GetDisplayA<TEnum>(this TEnum value)
        {
            return GetDisplay<TEnum>(value);
        }

        public static string GetDisplay<TEnum>(TEnum value)
        {
            return GetDisplay(typeof(TEnum), value as Enum);
        }

        public static string GetDisplay(Type enumType, Enum value)
        {
            var displays = GetDisplayAttributes(enumType, value);
            var result = string.Join(", ", displays.Select(d => d.GetName()));
            return result;
        }

        public static T[] SplitFlags<T>(T type)
        {
            Type enumType = UnboxNullable<T>();

            return type.ToString()
                 .Split(new[] { ", " }, StringSplitOptions.None)
                 .Select(v => (T)Enum.Parse(enumType, v)).ToArray();
        }

        #region bitwise
        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        public static T Toggle<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type ^ (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not toggle value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }
        #endregion

        public static IEnumerable ToList<TEnum>(bool useStringValue = false)
        {
            Type enumType = UnboxNullable<TEnum>();

            var values = GetValues<TEnum>();

            IEnumerable result;
            if (useStringValue)
                result = values.Select(e => new { Text = GetDisplay(e), Value = e.ToString() });
            else
                result = values.Select(e => new { Text = GetDisplay(e), Value = (int)Enum.Parse(enumType, e.ToString()) });

            return result;
        }

        public static Type UnboxNullable(Type type)
        {
            Type enumType = Nullable.GetUnderlyingType(type);

            if (enumType == null)
                enumType = type;
            return enumType;
        }

        public static Type UnboxNullable<TEnum>()
        {
            return UnboxNullable(typeof(TEnum));
        }

        public static IEnumerable<TEnum> GetValues<TEnum>()
        {
            Type enumType = UnboxNullable<TEnum>();
            return Enum.GetValues(enumType).OfType<TEnum>();
        }

        public static T Parse<T>(string value)
        {
            if (value == null)
                value = "0";
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T ToEnum<T>(this String value)
        {
            return EnumExtension.Parse<T>(value);
        }

        public static TAttribute GetAttribute<TAttribute, TEnum>(TEnum value)
        {
            var field = typeof(TEnum).GetField(value.ToString());

            var result = (TAttribute)field.GetCustomAttributes(typeof(TAttribute), false).First();
            return result;
        }
    }
}