using System;
using System.Collections;
using System.Collections.Generic;

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

    /// <summary>
    ///     Représente une collection d'objet contenant les resultats d'une requête.
    /// </summary>
    public abstract class EQuery : ICollection
    {
        protected bool _isLocked;
        protected ICollection _itemCollection = null;
        protected QueryLock _lockManager;
        protected EQuery _parent;
        protected string _ptrIndex = "";
        protected EQueryType _type = EQueryType.Null;

        #region Constructor

        protected EQuery(EQueryType queryType)
        {
            _type = queryType;
            _parent = null;
        }

        #endregion

        protected abstract ICollection<EQuery> Childs();

        /// <summary>
        ///     Accède à un élément grâce à un chemin spécifique.
        /// </summary>
        /// <param name="ePath">Chemin de l'élément à acceder.</param>
        /// <returns>Élément au niveau du chemin spécifié.</returns>
        public abstract EQuery AccessTo(EPath ePath);

        #region Lock Manager

        /// <summary>
        ///     Représente un système de verouillage de la BDD de l'API.
        /// </summary>
        public sealed class QueryLock
        {
            private EQuery _query;

            /// <summary>
            ///     Définit l'instance de la BDD que doit gérer le 'locker' courant.
            /// </summary>
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

            /// <summary>
            ///     Déverrouille l'instance de la BDD liée.
            /// </summary>
            public void Unlock()
            {
                UnlockQuery(_query);
            }

            /// <summary>
            ///     Verrouille l'instance de la BDD liée.
            /// </summary>
            public void Lock()
            {
                LockQuery(_query);
            }
        }

        #endregion Lock Manager

        #region Properties

        /// <summary>
        ///     Obtient ou définit l'élément au niveau de l'index spécifié.
        /// </summary>
        /// <param name="key">index de l'élément à obtenir ou définir.</param>
        /// <returns>Élément au niveau de l'index spécifié.</returns>
        public abstract EQuery this[object key] { get; set; }

        /// <summary>
        ///     Obtient le type <see cref="EQueryType" /> de l'instance courante.
        /// </summary>
        public EQueryType Type => _type;

        /// <summary>
        ///     Obtient le parent de l'instance courante.
        ///     Retourne une instance <see cref="ENull" /> si aucun parent n'est défini.
        /// </summary>
        public EQuery Parent
        {
            get { return _parent ?? new ENull(ReasonType.AccessFailure, "Current instance doesn't have parent."); }
            internal set { _parent = value; }
        }

        /// <summary>
        ///     Obtient une valeur indiquant si l'instance courante est nulle ou non.
        /// </summary>
        public virtual bool IsNull => false;

        /// <summary>
        ///     Obtient le chemin d'accès de l'instance courante.
        /// </summary>
        public string Path
        {
            get
            {
                return _parent == null
                    ? "$"
                    : _parent.Path + (_parent.Type == EQueryType.Array ? $"[{_ptrIndex}]" : $".{_ptrIndex}");
            }
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

        #endregion

        #region ICollection Interface

        #region Properties

        public virtual int Count => _itemCollection.Count;
        public bool IsSynchronized => false;
        public object SyncRoot => null;

        #endregion Properties

        #region Abstract ICollection

        public abstract void CopyTo(Array array, int index);

        public abstract IEnumerator GetEnumerator();

        #endregion Abstract ICollection

        #endregion

        #region Lock Queries

        /// <summary>
        ///     Obtient une valeur indiquant si l'instance courante est verouillée ou non.
        /// </summary>
        public bool IsLocked => _isLocked;

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
    }
}