using Newtonsoft.Json.Linq;
using Pheonyx.EpitechAPI.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pheonyx.EpitechAPI.Utils.Json
{
    static public class APIConfigLoader
    {
        static public JValue setVar(JToken jValue, Dictionary<String, Object> dVar)
        {
            if (jValue == null) throw new System.ArgumentNullException("jValue");
            if (dVar == null) throw new System.ArgumentNullException("dVar");
            if (!(jValue is JValue)) throw new System.ArgumentException("Invalid type (must be JValue)", "jValue");
            if (dVar.Count == 0) return (jValue.Value<JValue>());

            jValue = multiSetVar(jValue.Value<JValue>(), dVar
                .Where(d => d.Value is List<String>)
                .ToDictionary(d => d.Key, d => d.Value as List<String>));
            return (singleSetVar(jValue.Value<JValue>(), dVar
                .Where(dict => dict.Value is String)
                .ToDictionary(d => d.Key, d => d.Value as String)));
        }

        static private JValue singleSetVar(JValue jValue, Dictionary<String, String> dVar)
        {
            if (dVar.Count == 0) return (jValue);

            string sValue = jValue.ToString();
            foreach (var pVal in dVar)
                sValue = sValue.Replace("{" + pVal.Key + "}", pVal.Value);
            return (new JValue(sValue));
        }

        static private JValue multiSetVar(JValue jValue, Dictionary<String, List<String>> dVar)
        {
            if (dVar.Count == 0) return (jValue);

            Regex rEngine = new Regex(@"(.+?)\(\[(.+?)\]\)(.*)");
            string sValue = jValue.ToString();
            Match mMatch;
            while ((mMatch = rEngine.Match(sValue)).Success)
            {
                if (mMatch.Groups.Count != 4)
                    continue;
                sValue = mMatch.Groups[1].Value;
                string sArea = mMatch.Groups[2].Value;
                foreach (var pKey in dVar)
                    if (sArea.Contains("{" + pKey.Key + "}"))
                        foreach (var sReplace in pKey.Value)
                            sValue += sArea.Replace("{" + pKey.Key + "}", sReplace);
                sValue += mMatch.Groups[3].Value;
            }
            return (new JValue(sValue));
        }
    }

    static public class APIDataLoader
    {
        static public JToken accessTo(JToken jValue, JToken jRoot)
        {
            if (jValue == null) return (null);
            if (!(jValue is JValue)) throw new System.ArgumentException("Incorrect type (must be JValue)", "jValue");
            if (jRoot == null) throw new System.ArgumentNullException("jRoot");

            Dictionary<Func<String, Boolean>, Func<String, JToken, JToken>> accessConditions = new Dictionary<Func<String, Boolean>, Func<String, JToken, JToken>>()
            {
                { (String _sValue) => { return (_sValue.Contains("(+)")); }, appendItems },
                { (String _sValue) => { return (Regex.IsMatch(_sValue, @"^\((.+?)\|(.+?)\)$")); }, splitItem },
                { (String _sValue) => { return (Regex.IsMatch(_sValue, @"[\w.]+?\[[\w.]+=\w+\].+")); }, accessRow },
                { (String _sValue) => { return (!_sValue.Contains('[', ']', '(', ')', ' ', '\t', '\n')); }, accessPath }
            };

            string sValue = jValue.ToString().Trim(' ');
            var access = accessConditions
                .Where(condition => condition.Key(sValue))
                .Select(func => func.Value);
            if (access.Count() == 0)
                throw new Exception("Invalid access argument");
            return (access.First()(sValue, jRoot));
        }

        static private JToken accessPath(String sPath, JToken jRoot)
        {
            sPath = sPath.TrimStart('.');
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
                            throw new System.ArgumentException(String.Format("'{0}' in '{1}' must be an Int32", sNode, sPath));
                        try { jRoot = jRoot[iNode]; }
                        catch { throw new System.IndexOutOfRangeException(); }
                        break;

                    default:
                        throw new System.ArgumentOutOfRangeException(sNode);
                }
            return (jRoot);
        }

        static private JToken accessRow(String sValue, JToken jRoot)
        {
            Match matchRow = Regex.Match(sValue, @"([\w.]+?)\[([\w.]+)=(\w+)\](.+)");
            if (!matchRow.Success || matchRow.Groups.Count != 5)
                throw new System.ArgumentException();

            JToken jQuery = accessTo(matchRow.Groups[1].Value, jRoot);
            if (!(jQuery is JArray))
                return (null);

            JToken jRow = null;
            string sPath = matchRow.Groups[2].Value;
            string sComparator = matchRow.Groups[3].Value;
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
            return (accessTo(matchRow.Groups[4].Value, jRow));
        }

        static private JArray splitItem(String sValue, JToken jRoot)
        {
            Match matchSplit = Regex.Match(sValue, @"^\((.+?)\|(.+?)\)$");
            if (!matchSplit.Success || matchSplit.Groups.Count != 3)
                throw new System.ArgumentException();

            JToken jItem = accessTo(matchSplit.Groups[1].Value, jRoot);
            char[] cDelimiters = matchSplit.Groups[2].Value.ToCharArray();
            if (!(jItem is JValue))
                throw new System.ArgumentException("Incorrect type (must be JValue)", "jPath");

            JArray jItems = new JArray();
            foreach (string sItem in jItem.ToString().Trim(' ').Split(cDelimiters))
                jItems.Add(new JValue(sItem));
            return (jItems);
        }

        static private JArray appendItems(String sValue, JToken jRoot)
        {
            string[] sItems = { sValue.Substring(0, sValue.IndexOf("(+)")), sValue.Substring(sValue.IndexOf("(+)") + 3) };
            JToken[] jItems = { accessTo(new JValue(sItems[0]), jRoot), accessTo(new JValue(sItems[1]), jRoot) };
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