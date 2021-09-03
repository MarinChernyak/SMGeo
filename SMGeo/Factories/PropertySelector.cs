using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace NostraDataService
{
    internal class PropertySelector
    {
        protected static Dictionary<string, string> _aliasMap = new Dictionary<string, string>();
        public PropertySelector()
        {
            CreateAliasCollection();
        }
        // AliasCollection["Name in Contract"]="Name in DB"
        protected void CreateAliasCollection()
        {
            _aliasMap["CountryID"] = "ID";
            _aliasMap["CountryAcr"] = "Acronym";

        }

        public object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        protected String GetAliasName(String propName)
        {
            String sAliasProp = propName;
            ObjectCache cache = MemoryCache.Default;
            if (_aliasMap != null && _aliasMap.ContainsKey(propName))
            {
                sAliasProp = _aliasMap[propName];
            }
            return sAliasProp;
        }
    }
}