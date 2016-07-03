using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pheonyx.EpitechAPI.Utility.Json
{
    static public class APIConfigLoader
    {
        static public JValue setVar(JToken jValue, Dictionary<string, string> dVar)
        {
            if (jValue == null) throw new System.ArgumentNullException("jValue");
            if (dVar == null) throw new System.ArgumentNullException("dVar");
            if (!(jValue is JValue)) throw new System.ArgumentException("Invalid type (must be JValue)", "jValue");
            if (dVar.Count == 0) return (jValue.Value<JValue>());

            string sValue = jValue.ToString();
            foreach (var pVal in dVar)
                sValue = sValue.Replace("{" + pVal.Key + "}", pVal.Value);
            return (new JValue(sValue));
        }

        static public JValue multiSetVar(JToken jValue, Dictionary<string, List<string>> dVar)
        {
            if (jValue == null) throw new System.ArgumentNullException("jValue");
            //if (dVar == null) throw new System.ArgumentNullException("dVar");
            if (!(jValue is JValue)) throw new System.ArgumentException("Invalid type (must be JValue)", "jValue");
            //if (dVar.Count == 0) return (jValue.Value<JValue>());

            string sValue = jValue.ToString();
            Regex rEngine = new Regex(@"(.+?)\(\[(.+?)\]\)(.*)");
            Match mMatch;
            while ((mMatch = rEngine.Match(sValue)).Success)
            {
                if (mMatch.Groups.Count != 4)
                    continue;
                string sTemporary = mMatch.Groups[1].Value;
                string sArea = mMatch.Groups[2].Value;
                foreach (var pKey in dVar)
                    if (sArea.Contains("{" + pKey.Key + "}"))
                        foreach (var sReplace in pKey.Value)
                            sTemporary += sArea.Replace("{" + pKey.Key + "}", sReplace);
                sTemporary += mMatch.Groups[3].Value;
                sValue = sTemporary;
            }
            return (new JValue(sValue));
        }
    }

    static public class APIDataLoader
    {
        static public JToken accessTo(JToken jValue, JToken jRoot)
        {
            if (jValue == null) throw new System.ArgumentNullException("jValue");
            if (jRoot == null) throw new System.ArgumentNullException("jRoot");
            if (!(jValue is JValue)) throw new System.ArgumentException("Incorrect type (must be JValue)", "jValue");

            Dictionary<Func<String, Boolean>, Func<JToken, JToken, JToken>> accessConditions = new Dictionary<Func<string, bool>, Func<JToken, JToken, JToken>>()
            {
                { (string sValue) => { return (sValue.Contains("(+)")); }, appendItems },
                { (string sValue) => { return (Regex.IsMatch(sValue, @"[\w.]+?\[[\w.]+=\w+\].+")); }, accessRow },
                { (string sValue) => { return (true); }, accessPath }
            };

            var access = accessConditions
                .Where(condition => condition.Key(jValue.ToString()))
                .Select(func => func.Value)
                .First();
            if (access == null)
                throw new Exception("Invalid access argument");
            return (access(jValue, jRoot));
        }

        static public JToken accessPath(JToken jPath, JToken jRoot)
        {
            if (jPath == null) throw new System.ArgumentNullException("sPath");
            if (jRoot == null) throw new System.ArgumentNullException("jRoot");
            if (!(jPath is JValue)) throw new System.ArgumentException("Incorrect type (must be JValue)", "jValue");

            string sPath = jPath.ToString().TrimStart('.');
            foreach (var sNode in sPath.Split(new char[] { '.' }))
                switch (jRoot.Type)
                {
                    case JTokenType.Object:
                        try { jRoot = jRoot[sNode]; }
                        catch { throw new System.ArgumentOutOfRangeException(sNode); }
                        break;

                    case JTokenType.Array:
                        int iNode;

                        if (!int.TryParse(sNode, out iNode))
                            throw new System.ArgumentException(string.Format("'{0}' in '{1}' must be an Int32", sNode, sPath));
                        try { jRoot = jRoot[iNode]; }
                        catch { throw new System.IndexOutOfRangeException(); }
                        break;

                    default:
                        throw new System.ArgumentOutOfRangeException(sNode);
                }
            return (jRoot);
        }

        static public JToken accessRow(JToken jValue, JToken jRoot)
        {
            if (jValue == null) throw new System.ArgumentNullException("jValue");
            if (jRoot == null) throw new System.ArgumentNullException("jRoot");
            if (!(jValue is JValue)) throw new System.ArgumentException("Incorrect type (must be JValue)", "jValue");

            Match matchFinder = Regex.Match(jValue.ToString(), @"([\w.]+?)\[([\w.]+)=(\w+)\](.+)");
            if (!matchFinder.Success || matchFinder.Groups.Count != 5)
                throw new System.ArgumentException();
            JToken jQuery = accessTo(matchFinder.Groups[1].Value, jRoot);
            if (!(jQuery is JArray))
                return (null);

            JToken jRow = null;
            string sPath = matchFinder.Groups[2].Value;
            string sComparator = matchFinder.Groups[3].Value;
            foreach (JToken jContent in jQuery)
            {
                JValue tValue = accessTo(sPath, jContent) as JValue;
                if (tValue == null)
                    throw new Exception("It's not property");
                if (tValue.Value.ToString() != sComparator)
                    continue;
                jRow = jContent;
                break;
            }
            if (jRow == null)
                return (null);
            return (accessTo(matchFinder.Groups[4].Value, jRow));
        }

        static public JArray appendItems(JToken jValue, JToken jRoot)
        {
            if (jValue == null) throw new System.ArgumentNullException("jValue");
            if (jRoot == null) throw new System.ArgumentNullException("jRoot");
            if (!(jValue is JValue)) throw new System.ArgumentException("Incorrect type (must be JValue)", "jValue");

            string sValue = jValue.ToString();
            string[] sItems = { sValue.Substring(0, sValue.IndexOf("(+)")).Trim(' '), sValue.Substring(sValue.IndexOf("(+)") + 3).Trim(' ') };
            JToken[] jItems = { accessTo(new JValue(sItems[0]), jRoot), accessTo(new JValue(sItems[0]), jRoot) };
            JArray jArray = new JArray();

            foreach (var jItem in jItems)
            {
                if (jItem is JArray)
                    jArray.Merge(jItem);
                else if (jItem is JValue)
                    jArray.Add(jItem);
                else
                    throw new Exception("Incorrect Type");
            }
            return (jArray);
        }
    }
}