using System;
using System.Collections.Generic;
using System.Linq;

namespace Pheonyx.EpitechAPI.Utils
{
    public static class StringExtension
    {
        public static void ArgumentNotEmpty(this String self, String nameArgument)
        {
            self.ArgumentNotNull(nameArgument);
            if (self == String.Empty)
                throw new ArgumentException($"String {nameArgument} can't be empty.", nameArgument);
        }
        public static Boolean Contains(this String self, params Char[] needle)
        {
            return needle.All(c => self.IndexOf(c) != -1);
        }
        public static String Between(this String self, Char start, Char end)
        {
            if (!self.Contains(start, end))
                return self;
            return self.Substring(self.IndexOf(start) + 1, self.IndexOf(end) - (self.IndexOf(start) + 1));
        }
        public static String LastBetween(this String self, Char start, Char end)
        {
            if (!self.Contains(start, end) && self.LastIndexOf(start) < self.LastIndexOf(end))
                return self;
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
                throw new ArgumentException($"Argument '{nameArgument}' has invalid type ({self.GetType()}). {type} was expected.");
        }
    }

    public static class ListExtension
    {
        public static void Resize<T>(this List<T> self, int resize, T tdefault)
        {
            var currentSize = self.Count;

            if (currentSize > resize)
                self.RemoveRange(resize, currentSize - resize);
            else if (currentSize < resize)
            {
                if (resize > self.Capacity)
                    self.Capacity = resize;
                self.AddRange(Enumerable.Repeat(tdefault, resize - currentSize));
            }
        }
        public static void Resize<T>(this List<T> self, int resize)
        {
            self.Resize(resize, default(T));
        }
    }

    public static class EnumerableExtension
    {
        public static Boolean Empty<T>(this IEnumerable<T> self)
        {
            return !self.Any();
        }
        public static Boolean NotEmpty<T>(this IEnumerable<T> self)
        {
            return !self.Empty();
        }
    }

}
