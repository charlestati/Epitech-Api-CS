using Pheonyx.EpitechAPI.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pheonyx.EpitechAPI
{
    public enum EQueryType
    {
        Null,
        Object,
        Array,
        Char,
        String,
        Boolean,
        Integral,
        Decimal,
        Floating,
        Date
    }

    abstract public class EQuery : ICollection
    {
        protected QueryLock _lockManager = null;
        protected ICollection _itemCollection = null;
        protected EQueryType _type = EQueryType.Null;
        protected bool _isLocked = false;

        #region Lock Manager
        public sealed class QueryLock
        {
            private EQuery _query = null;

            ~QueryLock()
            {
                QueryInstance = null;
            }

            public EQuery QueryInstance
            {
                set
                {
                    if (value == null)
                        return;
                    value.LockManager = this;
                    _query = value;
                }
            }
            public void Unlock()
            {
                UnlockQuery(_query);
            }
            public void Lock()
            {
               LockQuery(_query);
            }
        }
        #endregion

        protected EQuery(EQueryType queryType)
        {
            _type = queryType;
        }
        private EQuery() { throw new System.NotImplementedException(); }

        abstract public EQuery this[Object key]
        {
            get;
            set;
        }
        public EQueryType Type
        {
            get
            {
                return _type;
            }
        }
        public bool IsNull
        {
            get
            {
                return false;
            }
        }
        internal QueryLock LockManager
        {
            set
            {
                if (_lockManager == value)
                    return;
                if (_lockManager != null)
                    throw new InvalidOperationException("An AEQuery instance can only use one QueryLock.");
                _lockManager = value;
                foreach (EQuery query in Childs())
                    query.LockManager = value;
            }
        }

        #region ICollection Interface
        #region Properties
        virtual public int Count
        {
            get
            {
                return _itemCollection.Count;
            }
        }
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }
        public object SyncRoot
        {
            get
            {
                throw null;
            }
        }
        #endregion

        #region Abstract ICollection
        abstract public void CopyTo(Array array, int index);
        abstract public IEnumerator GetEnumerator();
        #endregion
        #endregion

        #region Lock Queries
        public bool IsLocked
        {
            get
            {
                return _isLocked;
            }
        }
        protected void IsUnlocked()
        {
            if (_isLocked)
                throw new InvalidOperationException(String.Format("This {0} instance is read only.", _type));
        }
        static protected bool LockQuery(EQuery self)
        {
            if (self._isLocked)
                throw new InvalidOperationException(String.Format("This {0} instance is already read only.", self._type));
            foreach (EQuery query in self.Childs())
                LockQuery(query);
            self._isLocked = true;
            return self.IsLocked;
        }
        static protected bool UnlockQuery(EQuery self)
        {
            foreach (EQuery query in self.Childs())
                UnlockQuery(query);
            self._isLocked = false;
            return !self.IsLocked;
        }
        #endregion

        abstract protected ICollection<EQuery> Childs();
        abstract public EQuery AccessTo(EPath ePath);
    }
}
