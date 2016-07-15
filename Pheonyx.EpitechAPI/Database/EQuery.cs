using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pheonyx.EpitechAPI.Database
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

    public abstract class EQuery : ICollection
    {
        protected EQuery _parent = null;
        protected String _ptrIndex = "";
        protected QueryLock _lockManager;
        protected ICollection _itemCollection = null;
        protected EQueryType _type = EQueryType.Null;
        protected Boolean _isLocked;

        #region Lock Manager
        public sealed class QueryLock
        {
            private EQuery _query;

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

        public abstract EQuery this[Object key]
        {
            get;
            set;
        }
        public EQueryType Type => _type;

        public EQuery Parent
        {
            get { return _parent; }
            internal set { _parent = value; }
        }
        public Boolean IsNull => false;
        public String Path
        {
            get { return _parent == null ? "" : _parent.Path + (_parent.Type == EQueryType.Array ? $"[{_ptrIndex}]" : $".{_ptrIndex}"); }
            internal set { _ptrIndex = value; }
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
                foreach (var query in Childs())
                    query.LockManager = value;
            }
        }

        #region ICollection Interface
        #region Properties
        public virtual int Count => _itemCollection.Count;
        public Boolean IsSynchronized => false;
        public object SyncRoot => null;
        #endregion

        #region Abstract ICollection
        public abstract void CopyTo(Array array, int index);
        public abstract IEnumerator GetEnumerator();
        #endregion
        #endregion

        #region Lock Queries
        public Boolean IsLocked => _isLocked;
        protected void IsUnlocked()
        {
            if (_isLocked)
                throw new InvalidOperationException($"This {_type} instance is read only.");
        }
        private static void LockQuery(EQuery self)
        {
            if (self._isLocked)
                throw new InvalidOperationException($"This {self._type} instance is already read only.");
            foreach (var query in self.Childs())
                LockQuery(query);
            self._isLocked = true;
        }
        private static void UnlockQuery(EQuery self)
        {
            foreach (var query in self.Childs())
                UnlockQuery(query);
            self._isLocked = false;
        }
        #endregion

        protected abstract ICollection<EQuery> Childs();
        public abstract EQuery AccessTo(EPath ePath);
    }
}
