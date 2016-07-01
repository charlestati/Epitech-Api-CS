using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pheonyx.EpitechAPI.Utility
{
    static public class JsonTools
    {
        static public JValue setVariable(JToken jValue, Dictionary<string, string> dVar)
        {
            string sValue;

            if (jValue == null)
                throw new System.ArgumentNullException("jValue");
            if (!(jValue is JValue))
                throw new System.ArgumentException("incorrect type (must be JValue)", "jValue");
            if (dVar == null)
                throw new System.ArgumentNullException("dVar");
            if (dVar.Count == 0)
                return (jValue.Value<JValue>());

            sValue = jValue.ToString();

            foreach (var pVal in dVar)
                sValue = sValue.Replace("{" + pVal.Key + "}", pVal.Value);
            return (new JValue(sValue));
        }

        static public JToken accessPath(string sPath, JToken jRoot)
        {
            JToken jTemporary;

            if (sPath == null)
                throw new System.ArgumentNullException("sPath");
            if (jRoot == null)
                throw new System.ArgumentNullException("jRoot");

            jTemporary = jRoot;
            foreach (var sNode in sPath.Split(new char[] { '.' }))
                switch (jTemporary.Type)
                {
                    case JTokenType.Object:
                        try { jTemporary = jTemporary[sNode]; }
                        catch { throw new System.ArgumentOutOfRangeException(sNode); }
                        break;

                    case JTokenType.Array:
                        int iNode;

                        if (!int.TryParse(sNode, out iNode))
                            throw new System.ArgumentException(string.Format("'{0}' in '{1}' must be an Int32", sNode, sPath));
                        try { jTemporary = jTemporary[iNode]; }
                        catch { throw new System.IndexOutOfRangeException(); }
                        break;

                    default:
                        throw new System.ArgumentOutOfRangeException(sNode);
                }
            return (jTemporary);
        }

        static public JToken findRow(JToken jValue, JToken jRoot)
        {
            string sValue;
            Regex engineArguments = new Regex(@"([\w.\[\]]+)\[([\w.]+)=(\w+)\]([\w.\[\]]+)");
            Match matchFinder;
            JToken jQuery;

            if (jValue == null)
                throw new System.ArgumentNullException("jValue");
            if (!(jValue is JValue))
                throw new System.ArgumentException("incorrect type (must be JValue)", "jValue");
            if (jRoot == null)
                throw new System.ArgumentNullException("jRoot");

            sValue = jValue.ToString();
            if (!(matchFinder = engineArguments.Match(sValue)).Success || matchFinder.Groups.Count != 5)
                throw new System.ArgumentException();
            if ((jQuery = accessPath(matchFinder.Groups[1].Value, jRoot)).Type != JTokenType.Array)
                throw new System.Exception("Can't access to this");
            string sComparator = matchFinder.Groups[3].Value;
            JToken tQuery = null;
            foreach (JToken jContent in jQuery)
            {
                tQuery = jContent;
                JValue tValue = accessPath(matchFinder.Groups[2].Value, jContent) as JValue;
                if (tValue == null)
                    throw new Exception("It's not property");
                if (tValue.Value.ToString() == matchFinder.Groups[3].Value)
                    break;
            }
            return (accessPath(matchFinder.Groups[4].Value.Remove(0, 1), tQuery));
        }
    }
}