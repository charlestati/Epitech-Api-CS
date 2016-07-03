using System.Linq;

namespace Pheonyx.EpitechAPI.Extension
{
    public static class StringExtension
    {
        public static bool Contains(this string stack, params char[] needle)
        {
            return (needle.Any(c => stack.IndexOf(c) != -1));
        }
    }
}
