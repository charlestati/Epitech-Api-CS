using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    sealed class ENull : EQuery
    {
        private readonly String _message;
        private readonly ReasonType _reason;

        public ENull(ReasonType reason, string message) : base(EQueryType.Null)
        {
            _reason = reason;
            _message = message;
        }

        public String Message => _message;
        public ReasonType Reason => _reason;

        #region EQuery Override
        public override EQuery this[object key]
        {
            get { return this; }
            set { }
        }
        public override void CopyTo(Array array, int index) { }
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
        #endregion

        public override String ToString()
        {
            return $"\"{_ptrIndex}\": \"#{_reason}: {_message}\"";
        }
    }
}
