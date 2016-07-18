using System;
using System.Collections;
using System.Collections.Generic;

namespace Pheonyx.EpitechAPI.Database
{
    public enum ReasonType
    {
        AccessFailure,
        ConfigurationFile,
        InvalidKey,
        InvalidValue,
        InvalidJsonType,
        JsonNetFailure
    }

    public sealed class ENull : EQuery
    {
        public ENull(ReasonType reason, string message) : base(EQueryType.Null)
        {
            Reason = reason;
            Message = message;
        }

        /// <summary>
        ///     Obtient le message défini lors de la construction de <see cref="ENull" />.
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///     Obtient la raison définie lors de la construction de <see cref="ENull" />.
        /// </summary>
        public ReasonType Reason { get; }

        public override string ToString()
        {
            return $"\"{_ptrIndex}\": \"#{Reason}: {Message}\"";
        }

        #region EQuery Override

        public override EQuery this[object key]
        {
            get { return this; }
            set { }
        }

        public override bool IsNull => true;

        public override void CopyTo(Array array, int index)
        {
        }

        public override IEnumerator GetEnumerator()
        {
            yield break;
        }

        protected override ICollection<EQuery> Childs()
        {
            return new List<EQuery>();
        }

        public override EQuery AccessTo(EPath ePath)
        {
            return this;
        }

        #endregion EQuery Override
    }
}