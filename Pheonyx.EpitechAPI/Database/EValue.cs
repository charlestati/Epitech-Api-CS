using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pheonyx.EpitechAPI.Database
{
    public class EValue : EQuery
    {
        private Object _value;

        #region QueryType Link
        private static Dictionary<EQueryType, List<Type>> QueryLink = new Dictionary<EQueryType, List<Type>>
        {
            { EQueryType.Boolean, new List<Type> { typeof(Boolean) } },
            { EQueryType.Char, new List<Type> { typeof(Char) } },
            { EQueryType.String, new List<Type> { typeof(String) } },
            { EQueryType.Integral, new List<Type>
            {
                typeof(SByte), typeof(Int16), typeof(Int32), typeof(Int64),
                typeof(Byte), typeof(UInt16), typeof(UInt32), typeof(UInt64)
            } },
            { EQueryType.Decimal, new List<Type> { typeof(Decimal) } },
            { EQueryType.Floating, new List<Type> { typeof(Single), typeof(Double) } },
            { EQueryType.Date, new List<Type> { typeof(DateTime), typeof(TimeSpan) } }
        };
        #endregion

        #region Constructor
        public EValue(object value) : base(EQueryType.Null)
        {
            _itemCollection = null;
            _type = FindType(value);
            if (value == null || _type == EQueryType.Null)
                _value = value as ENull ?? new ENull(ReasonType.InvalidValue, $"Invalid value ({value})");
            else
                _value = value;
        }
        #endregion

        #region EQuery Override
        public override EQuery this[Object key]
        {
            get
            {
                return new ENull(ReasonType.AccessFailure, $"EValue ({Path}) doesn't have children queries.");
            }

            set { }
        }
        public override Int32 Count => 0;

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
            return new ENull(ReasonType.AccessFailure, $"ENull: Can't access to {ePath} (no child in EValue)");
        }
        #endregion

        private static EQueryType FindType(object oValue)
        {
            if (oValue == null)
                return EQueryType.Null;
            return (from row in QueryLink
                    from type in row.Value
                    where type == oValue.GetType()
                    select row.Key)
                    .FirstOrDefault();
        }
        public TValue Value<TValue>()
        {
            if (_type == EQueryType.Null)
                return default(TValue);
            return (TValue)Convert.ChangeType(_value, typeof(TValue));
        }
        public void Value<TValue>(TValue value)
        {
            _type = FindType(value);
            if (_type == EQueryType.Null)
                _value = new ENull(ReasonType.InvalidValue, $"Null value or unknow type. ({value})");
            else
                _value = value;
        }
        public override String ToString()
        {
            return $"\"{_ptrIndex}\": \"{_value}\"";
        }
    }
}
