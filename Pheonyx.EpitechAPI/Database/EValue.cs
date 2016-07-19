using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pheonyx.EpitechAPI.Database
{
    /// <summary>
    ///     Fournit des méthodes pour la création et la manipulation des requêtes contenant des valeurs.
    /// </summary>
    public class EValue : EQuery
    {
        #region QueryType Link

        private static readonly Dictionary<EQueryType, List<Type>> QueryLink = new Dictionary<EQueryType, List<Type>>
        {
            {EQueryType.Boolean, new List<Type> {typeof(bool)}},
            {EQueryType.Char, new List<Type> {typeof(char)}},
            {EQueryType.String, new List<Type> {typeof(string)}},
            {
                EQueryType.Integral, new List<Type>
                {
                    typeof(sbyte),
                    typeof(short),
                    typeof(int),
                    typeof(long),
                    typeof(byte),
                    typeof(ushort),
                    typeof(uint),
                    typeof(ulong)
                }
            },
            {EQueryType.Decimal, new List<Type> {typeof(decimal)}},
            {EQueryType.Floating, new List<Type> {typeof(float), typeof(double)}},
            {EQueryType.Date, new List<Type> {typeof(DateTime), typeof(TimeSpan)}}
        };

        #endregion QueryType Link

        private object _value;

        #region Constructor

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="EValue"/> à l'aide d'un objet à lier.
        /// </summary>
        /// <param name="value">Objet à lier avec l'instance de <see cref="EValue"/>.</param>
        public EValue(object value) : base(EQueryType.Null)
        {
            _itemCollection = null;
            _type = FindType(value);
            if (value == null || _type == EQueryType.Null)
                _value = value as ENull ?? new ENull(ReasonType.InvalidValue, $"Invalid value ({value})");
            else
                _value = value;
        }

        #endregion Constructor

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

        /// <summary>
        ///     Obtient la valeur lié à <see cref="EValue" /> de type <typeparamref name="TValue" />.
        /// </summary>
        /// <typeparam name="TValue"><see cref="Type" /> de la valeur à obtenir.</typeparam>
        /// <returns>
        ///     Valeur associée à <see cref="EValue" />. Si la valeur est nulle, on retourne la valeur par défaut de
        ///     <typeparamref name="TValue" />.
        /// </returns>
        public TValue Value<TValue>()
        {
            if (_type == EQueryType.Null)
                return default(TValue);
            return (TValue) Convert.ChangeType(_value, typeof(TValue));
        }

        /// <summary>
        ///     Définie la valeur lié à <see cref="EValue" />.
        /// </summary>
        /// <typeparam name="TValue"><see cref="Type" /> de la valeur à définir.</typeparam>
        /// <param name="value">Valeur à définir.</param>
        public void Value<TValue>(TValue value)
        {
            _type = FindType(value);
            if (_type == EQueryType.Null)
                _value = new ENull(ReasonType.InvalidValue, $"Null value or unknow type. ({value})");
            else
                _value = value;
        }

        public override string ToString()
        {
            return $"\"{_ptrIndex}\": \"{_value}\"";
        }

        #region EQuery Override

        public override EQuery this[object key]
        {
            get { return new ENull(ReasonType.AccessFailure, $"EValue ({Path}) doesn't have children queries."); }

            set { }
        }

        public override int Count => 0;

        protected override ICollection<EQuery> Childs()
        {
            return new List<EQuery>();
        }

        public override void CopyTo(Array array, int index)
        {
        }

        public override IEnumerator GetEnumerator()
        {
            yield break;
        }

        public override EQuery AccessTo(EPath ePath)
        {
            return new ENull(ReasonType.AccessFailure, $"ENull: Can't access to {ePath} (no child in EValue)");
        }

        #endregion EQuery Override
    }
}