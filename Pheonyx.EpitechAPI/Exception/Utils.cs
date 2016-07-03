using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pheonyx.EpitechAPI.Utils
{
    namespace Json
    {
        static public partial class APIConfigLoader
        {
            static private readonly String INV_ARG_TYPE = "Invalid argument type {0} (Must be {1})";
        }
        static public partial class APIDataLoader
        {
            static private readonly String INV_ACTION = "Invalid function '{0}'";
            static private readonly String INV_ARG_TYPE = "Invalid argument type {0} (Must be {1})";
            static private readonly String INV_INDEX = "Invalid index format '{0}' at '{1}' (Must be {2})";
            static private readonly String INV_PATH = "Invalid path '{0}'";
            static private readonly String INV_VALUE = "Invalid JSON type {0} at '{1}' (Must be {2})";
            static private readonly String OUT_OF_RANGE = "'{0}' in '{1}' is out of range ({2} item(s))";
            static private readonly String UNDERSTAND_PATH = "Incomprehensible path '{0}'";
        }
    }
}
