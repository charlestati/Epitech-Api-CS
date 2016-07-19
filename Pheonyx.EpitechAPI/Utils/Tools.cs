using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Pheonyx.EpitechAPI.Utils
{
    internal static class Tools
    {
        private static readonly Dictionary<Func<string, bool>, Func<string, JToken, JToken>> ConditionsDictionary = new Dictionary
            <Func<string, bool>, Func<string, JToken, JToken>>
        {
            {sValue => sValue.Contains("(+)"), AppendItems},
            {sValue => Regex.IsMatch(sValue, @"^\((.+?)\|(.*?)\)$"), SplitItem}
        };

        public static string SetVar(JToken jValue, Dictionary<string, object> dVar)
        {
            if (jValue == null) return null;
            if (!(jValue is JValue))
                throw new ArgumentException($"Invalid argument type {jValue.GetType()} (Must be {typeof(JValue)})",
                    nameof(jValue));
            if (dVar == null) throw new ArgumentNullException(nameof(dVar));
            if (dVar.Count == 0) return jValue.Value<JValue>().ToString();

            var sValue = jValue.ToString();
            sValue = MultiSetVar(sValue, dVar
                .Where(d => d.Value is List<string>)
                .ToDictionary(d => d.Key, d => d.Value as List<string>));
            return SingleSetVar(sValue, dVar
                .Where(dict => dict.Value is string)
                .ToDictionary(d => d.Key, d => d.Value as string));
        }

        public static JToken AccessTo(JToken jValue, JToken jRoot)
        {
            if (jValue == null) return null;
            if (!(jValue is JValue))
                throw new ArgumentException($"Invalid argument type {jValue.GetType()} (Must be {typeof(JValue)})",
                    nameof(jValue));
            if (jValue.ToString() == "") return jRoot;
            if (jRoot == null) throw new ArgumentNullException(nameof(jRoot));

            var sValue = jValue.ToString().Trim(' ');
            var access = ConditionsDictionary
                .Where(condition => condition.Key(sValue))
                .Select(func => func.Value);
            if (access.Empty())
                return jRoot.SelectToken(sValue);
            return access.First()(sValue, jRoot);
        }

        #region SetVar methods

        private static string SingleSetVar(string sValue, Dictionary<string, string> dVar)
        {
            if (dVar.Empty()) return sValue;

            return dVar.Aggregate(sValue, (current, pVal) => current.Replace("{" + pVal.Key + "}", pVal.Value));
        }

        private static string MultiSetVar(string sValue, Dictionary<string, List<string>> dVar)
        {
            if (dVar.Empty()) return sValue;

            var rEngine = new Regex(@"(.+?)\(\[(.+?)\]\)(.*)");
            Match mMatch;
            while ((mMatch = rEngine.Match(sValue)).Success)
            {
                if (mMatch.Groups.Count != 4)
                    continue;
                sValue = mMatch.Groups[1].Value;
                var sArea = mMatch.Groups[2].Value;
                sValue = dVar
                    .Where(pKey => sArea.Contains("{" + pKey.Key + "}"))
                    .Aggregate(sValue,
                        (current1, pKey) =>
                            pKey.Value.Aggregate(current1,
                                (current, sReplace) => current + sArea.Replace("{" + pKey.Key + "}", sReplace)))
                         + mMatch.Groups[3].Value;
            }
            return sValue;
        }

        #endregion SetVar methods

        #region AccessTo methods

        private static JArray SplitItem(string sValue, JToken jRoot)
        {
            var matchSplit = Regex.Match(sValue, @"^\((.+?)\|(.*?)\)$");
            if (!matchSplit.Success || matchSplit.Groups.Count != 3)
                throw new ArgumentException($"Invalid function '{sValue}'");
            if (matchSplit.Groups[2].Value == "")
                throw new ArgumentException($"Invalid key(s) for spliting '{matchSplit.Groups[1].Value}' in '{sValue}'");

            var jItem = AccessTo(matchSplit.Groups[1].Value, jRoot);
            var cDelimiters = matchSplit.Groups[2].Value.ToCharArray();
            if (!(jItem is JValue))
                throw new TypeAccessException(
                    $"Invalid JSON type {jItem.Type} at '{matchSplit.Groups[1].Value}' (Must be {JTokenType.String})");

            var jItems = new JArray();
            foreach (var sItem in jItem.ToString().Trim(' ').Split(cDelimiters))
                jItems.Add(sItem);
            return jItems;
        }

        private static JArray AppendItems(string sValue, JToken jRoot)
        {
            string[] sItems =
            {
                sValue.Substring(0, sValue.IndexOf("(+)", StringComparison.Ordinal)),
                sValue.Substring(sValue.IndexOf("(+)", StringComparison.Ordinal) + 3)
            };
            JToken[] jItems = {AccessTo(sItems[0], jRoot), AccessTo(sItems[1], jRoot)};
            var jArray = new JArray();

            foreach (var jItem in jItems)
            {
                if (jItem is JArray)
                    jArray.Merge(jItem);
                else if (jItem is JValue)
                    jArray.Add(jItem);
                else
                    continue;
            }
            return jArray;
        }

        #endregion AccessTo methods
    }
}