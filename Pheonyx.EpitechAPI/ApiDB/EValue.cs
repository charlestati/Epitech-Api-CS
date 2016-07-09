using System;
using System.Collections;
using System.Collections.Generic;
using Pheonyx.EpitechAPI.Extension;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pheonyx.EpitechAPI
{
    public class EValue : EQuery
    {
        private Object _value = null;
        private String _errorMessage = "";

        #region QueryType Link
        static private Dictionary<EQueryType, List<Type>> QueryLink = new Dictionary<EQueryType, List<System.Type>>()
        {
            { EQueryType.Boolean, new List<System.Type>() { typeof(Boolean) } },
            { EQueryType.Char, new List<System.Type>() { typeof(Char) } },
            { EQueryType.String, new List<System.Type>() { typeof(String) } },
            { EQueryType.Integral, new List<System.Type>() {
                typeof(SByte), typeof(Int16), typeof(Int32), typeof(Int64),
                typeof(Byte), typeof(UInt16), typeof(UInt32), typeof(UInt64)
            } },
            { EQueryType.Decimal, new List<System.Type>() { typeof(Decimal) } },
            { EQueryType.Floating, new List<System.Type>() { typeof(Single), typeof(Double) } },
            { EQueryType.Date, new List<System.Type>() { typeof(DateTime), typeof(TimeSpan) } }
        };
        #endregion

        #region Constructor
        public EValue(object value) : base(EQueryType.DBNull)
        {
            _itemCollection = null;
            _type = FindType(value);
            //Todo: ENull: Null value or Uknown type TYPE
            _value = (_type == EQueryType.DBNull) ? null : value;
        }
        #endregion

        #region AEQuery Override
        public override EQuery this[Object key]
        {
            get
            {
                throw new InvalidOperationException("EValue doesn't have children queries.");
            }

            set { }
        }
        public override int Count
        {
            get
            {
                return 0;
            }
        }
        protected override ICollection<EQuery> Childs()
        {
            return new List<EQuery>();
        }
        public override void CopyTo(Array array, int index) { }
        public override IEnumerator GetEnumerator()
        {
            yield break;
        }
        public override EQuery AccessTo(EPath ePath)
        {
            return null; //Todo: ENull: Can't access child (no child in EValue)
        }
        #endregion

        static private EQueryType FindType(object oValue)
        {
            if (oValue == null)
                return EQueryType.DBNull;
            foreach (var row in QueryLink)
                foreach (var type in row.Value)
                    if (type == oValue.GetType())
                        return row.Key;
            return EQueryType.DBNull;
        }
        public VType Value<VType>()
        {
            if (_type == EQueryType.DBNull)
                return default(VType);
            return (VType)Convert.ChangeType(_value, typeof(VType));
        }
        public void Value<VType>(VType value)
        {
            _type = FindType(value);
            //Todo: ENull: Null value or Uknown type TYPE
            if (_type == EQueryType.DBNull)
                _value = null;
            else
                _value = value;
        }
    }
}
