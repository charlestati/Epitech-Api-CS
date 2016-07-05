using System;
using System.Linq;

namespace Pheonyx.EpitechAPI.Extension
{
    public static class StringExtension
    {
        public static bool Contains(this String self, params Char[] needle)
        {
            return (needle.Any(c => self.IndexOf(c) != -1));
        }
        public static void ThrowIfNull(this String self, String nameParameter = null)
        {
            if (self != null)
                return;
            if (nameParameter == null)
                throw new NullReferenceException();
            else
                throw new ArgumentNullException(nameParameter);
        }
    }
}
