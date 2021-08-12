using System;
using System.Collections.Generic;


namespace NostraDataService
{
    internal class PropertySelector
    {
        public PropertySelector()
        {

        }
        public object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}