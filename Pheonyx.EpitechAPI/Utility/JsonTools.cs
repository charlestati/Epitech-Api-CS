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
            string sValue = jValue.ToString();

            if (jValue == null)
                throw new System.ArgumentNullException("jValue");
            //Todo: Gestion des messages d'erreurs
            if (!(jValue is JValue))
                throw new System.ArgumentException("incorrect type (must be JValue)", "jValue");
            if (dVar == null)
                throw new System.ArgumentNullException("dVar");
            if (dVar.Count == 0)
                return (jValue.Value<JValue>());

            foreach (var pVal in dVar)
                sValue = sValue.Replace("{" + pVal.Key + "}", pVal.Value);
            return (new JValue(sValue));
        }
        static public JToken accessPath(string sPath, JToken jRoot)
        {
            JToken jTemporary;
            Regex rgxEngine = new Regex(@"[(\d+)]");

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
                        if (!rgxEngine.IsMatch(sNode))
                            throw new System.ArgumentException(string.Format("'{0}' in '{1}' must be an array index", sNode, sPath));
                        var iNode = Convert.ToInt32(rgxEngine.Matches(sNode)[0].Value);
                        try { jTemporary = jTemporary[iNode]; }
                        catch { throw new System.IndexOutOfRangeException(); }
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException(sNode);
                }
            return (jTemporary);
        }
    }
}
