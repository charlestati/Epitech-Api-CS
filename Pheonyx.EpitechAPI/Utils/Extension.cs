using System;
using System.Collections.Generic;
using System.Linq;

namespace Pheonyx.EpitechAPI.Extension
{
    public static class StringExtension
    {
        public static void ArgumentNotEmpty(this String self, String nameArgument)
        {
            self.ArgumentNotNull(nameArgument);
            if (self == String.Empty)
                throw new ArgumentException(String.Format("String {0} can't be empty.", nameArgument), nameArgument);
        }

        public static bool Contains(this String self, params Char[] needle)
        {
            return needle.All(c => self.IndexOf(c) != -1);
        }

        public static String Between(this String self, Char start, Char end)
        {
            if (!self.Contains(start, end))
                return (self);
            return self.Substring(self.IndexOf(start) + 1, self.IndexOf(end) - (self.IndexOf(start) + 1));
        }

        public static String LastBetween(this String self, Char start, Char end)
        {
            if (!self.Contains(start, end) && self.LastIndexOf(start) < self.LastIndexOf(end))
                return (self);
            return self.Substring(self.LastIndexOf(start) + 1, self.LastIndexOf(end) - (self.LastIndexOf(start) + 1));
        }
    }

    public static class ObjectExtension
    {
        public static void ArgumentNotNull(this Object self, String nameArgument)
        {
            if (self == null)
                throw new ArgumentNullException(nameArgument);
        }
        public static void ArgumentValidType(this Object self, Type type, String nameArgument)
        {
            if (self.GetType() != type)
                throw new ArgumentException(String.Format("Argument '{0}' has invalid type ({1}). {2} was expected.", nameArgument, self.GetType(), type));
        }
    }

    public static class ListExtension
    {
        public static void Resize<T>(this List<T> self, int resize, T Tdefault)
        {
            int currentSize = self.Count;

            if (currentSize > resize)
                self.RemoveRange(resize, currentSize - resize);
            else if (currentSize < resize)
            {
                if (resize > self.Capacity)
                    self.Capacity = resize;
                self.AddRange(Enumerable.Repeat(Tdefault, resize - currentSize));
            }
        }
        public static void Resize<T>(this List<T> self, int resize)
        {
            self.Resize(resize, default(T));
        }
    }

    public static class EnumerableExtension
    {
        public static bool NotEmpty<T>(this IEnumerable<T> self)
        {
            return self.Count() > 0;
        }
    }

}
