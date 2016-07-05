using System;

namespace Pheonyx.EpitechAPI.Utils
{
    static class ExceptionMessage
    {
        static public readonly string INV_ACTION = "Invalid function '{0}'";
        static public readonly string INV_ARG_TYPE = "Invalid argument type {0} (Must be {1})";
        static public readonly string INV_KEY = "Invalid key(s) for spliting '{0}' in '{1}'";
        static public readonly string INV_VALUE = "Invalid JSON type {0} at '{1}' (Must be {2})";
    }
}
