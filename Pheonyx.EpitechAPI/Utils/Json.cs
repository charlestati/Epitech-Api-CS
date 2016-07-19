using Newtonsoft.Json.Linq;
using Pheonyx.EpitechAPI.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pheonyx.EpitechAPI.Utils
{
    static public class Json
   {
        private static Dictionary<Func<String, Boolean>, Func<String, JToken, JToken>> accessConditions = new Dictionary<Func<String, Boolean>, Func<String, JToken, JToken>>()
            {
                { (String _sValue) => { return (_sValue.Contains("(+)")); }, appendItems },
                { (String _sValue) => { return (Regex.IsMatch(_sValue, @"^\((.+?)\|(.*?)\)$")); }, splitItem },
            };

        #region SetVar methods
        static private String singleSetVar(String sValue, Dictionary<String, String> dVar)
        {
            if (dVar.Count == 0) return (sValue);

            foreach (var pVal in dVar)
                sValue = sValue.Replace("{" + pVal.Key + "}", pVal.Value);
            return (sValue);
        }
        static private String multiSetVar(String sValue, Dictionary<String, List<String>> dVar)
        {
            if (dVar.Count == 0) return (sValue);

            Regex rEngine = new Regex(@"(.+?)\(\[(.+?)\]\)(.*)");
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
            return (sValue);
        }
        #endregion

        #region AccessTo methods
        static private JArray splitItem(String sValue, JToken jRoot)
        {
            Match matchSplit = Regex.Match(sValue, @"^\((.+?)\|(.*?)\)$");
            if (!matchSplit.Success || matchSplit.Groups.Count != 3)
                throw new System.ArgumentException(String.Format(ExceptionMessage.INV_ACTION, sValue));
            if (matchSplit.Groups[2].Value == "")
                throw new System.ArgumentException(String.Format(ExceptionMessage.INV_KEY, matchSplit.Groups[1].Value, sValue));

            JToken jItem = accessTo(matchSplit.Groups[1].Value, jRoot);
            char[] cDelimiters = matchSplit.Groups[2].Value.ToCharArray();
            if (!(jItem is JValue))
                throw new System.TypeAccessException(String.Format(ExceptionMessage.INV_VALUE, jItem.Type, matchSplit.Groups[1].Value, JTokenType.String));

            JArray jItems = new JArray();
            foreach (string sItem in jItem.ToString().Trim(' ').Split(cDelimiters))
                jItems.Add(sItem);
            return (jItems);
        }
        static private JArray appendItems(String sValue, JToken jRoot)
        {
            string[] sItems = { sValue.Substring(0, sValue.IndexOf("(+)")), sValue.Substring(sValue.IndexOf("(+)") + 3) };
            JToken[] jItems = { accessTo(sItems[0], jRoot), accessTo(sItems[1], jRoot) };
            JArray jArray = new JArray();

            foreach (var jItem in jItems)
            {
                if (jItem is JArray)
                    jArray.Merge(jItem);
                else if (jItem is JValue)
                    jArray.Add(jItem);
                else
                    continue;
            }
            return (jArray);
        }
        #endregion

        static public String setVar(JToken jValue, Dictionary<String, Object> dVar)
        {
            if (jValue == null) return (null);
            if (!(jValue is JValue)) throw new System.ArgumentException(String.Format(ExceptionMessage.INV_ARG_TYPE, jValue.GetType(), typeof(JValue)), "jValue");
            if (dVar == null) throw new System.ArgumentNullException("dVar");
            if (dVar.Count == 0) return (jValue.Value<JValue>().ToString());

            string sValue = jValue.ToString();
            sValue = multiSetVar(sValue, dVar
                .Where(d => d.Value is List<String>)
                .ToDictionary(d => d.Key, d => d.Value as List<String>));
            return (singleSetVar(sValue, dVar
                .Where(dict => dict.Value is String)
                .ToDictionary(d => d.Key, d => d.Value as String)));
        }
        static public JToken accessTo(JToken jValue, JToken jRoot)
        {
            if (jValue == null) return (null);
            if (!(jValue is JValue)) throw new System.ArgumentException(String.Format(ExceptionMessage.INV_ARG_TYPE, jValue.GetType(), typeof(JValue)), "jValue");
            if (jValue.ToString() == "") return (jRoot);
            if (jRoot == null) throw new System.ArgumentNullException("jRoot");

            string sValue = jValue.ToString().Trim(' ');
            var access = accessConditions
                .Where(condition => condition.Key(sValue))
                .Select(func => func.Value);
            if (access.Count() == 0)
                return (jRoot.SelectToken(sValue));
            return (access.First()(sValue, jRoot));
        }

    }
}