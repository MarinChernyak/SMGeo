using System;
using System.Data;

namespace NostraDataService
{
    internal class SMPropertiesUpdater<T> : PropertySelector
    {
        private readonly DataRow _dr;
        private DataColumnCollection _columns;
        private readonly T PropertiesKeeper;
        public SMPropertiesUpdater(DataRow from, T to)
        {
            _dr = from;
            PropertiesKeeper = to;
            UpdateProperties();
        }
        protected void UpdateProperties()
        {
            if (_dr != null && PropertiesKeeper != null)
            {
                _columns = _dr.Table.Columns;
                foreach (DataColumn dc in _columns)
                {
                    String propName = dc.ColumnName;
                    object value = null;
                    if (!Convert.IsDBNull(_dr[dc]))
                        value = _dr[dc];


                    SetPropValue(PropertiesKeeper, propName, value);
                }
            }
        }

        protected void SetPropValue(object src, string propName, object value)
        {
            if (src.GetType().GetProperty(propName) != null)
                src.GetType().GetProperty(propName).SetValue(src, value);
        }


    }
}