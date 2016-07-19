using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pheonyx.EpitechAPI.Utils;

namespace Pheonyx.EpitechAPI.Database
{
    /// <summary>
    ///     Fournit des méthodes pour la création, la manipulation et la recherche des requêtes de type
    ///     <see cref="EQueryType.Object" />.
    /// </summary>
    public sealed class EObject : EQuery, IDictionary<string, EQuery>
    {
        private readonly Dictionary<string, EQuery> _instance;

        #region Constructor

        /// <summary>
        ///     Initialise une nouvelle instance de la classe <see cref="EObject" /> qui est vide.
        /// </summary>
        public EObject() : base(EQueryType.Object)
        {
            _itemCollection = new Dictionary<string, EQuery>();
            _instance = _itemCollection as Dictionary<string, EQuery>;
        }

        #endregion

        internal void Add(IDictionary<string, EQuery> eQuery)
        {
            foreach (var child in eQuery)
                Add(child);
        }

        public override string ToString()
        {
            if (_parent == null)
                return $"{{ {_instance.Select(c => c.Value.ToString()).Aggregate((i, j) => i + ", " + j)} }}";
            return
                $"\"{_ptrIndex}\": {{ {_instance.Select(c => c.Value.ToString()).Aggregate((i, j) => i + ", " + j)} }}";
        }

        #region IDictionary Interface

        #region Properties

        public EQuery this[string key]
        {
            get
            {
                key.ArgumentNotNull(nameof(key));
                return ContainsKey(key) ? _instance[key] : new ENull(ReasonType.InvalidKey, $"Key '{key}' not found");
            }

            set
            {
                key.ArgumentNotNull(nameof(key));
                IsUnlocked();

                var eValue = value ?? new ENull(ReasonType.InvalidValue, $"Invalid value insertion (null)");
                if (_instance.ContainsKey(key))
                    _instance[key] = eValue;
                else
                    _instance.Add(key, eValue);
                eValue.Path = key;
                eValue.Parent = this;
                eValue.LockManager = _lockManager;
            }
        }

        public bool IsReadOnly => _isLocked;
        public ICollection<string> Keys => _instance.Keys;
        public ICollection<EQuery> Values => _instance.Values;

        #endregion

        public bool Contains(KeyValuePair<string, EQuery> item)
        {
            item.Key.ArgumentNotNull(nameof(item.Key));
            return _instance.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            key.ArgumentNotNull(nameof(key));
            return _instance.ContainsKey(key);
        }

        public bool TryGetValue(string key, out EQuery value)
        {
            key.ArgumentNotNull(nameof(key));
            return _instance.TryGetValue(key, out value);
        }

        public void CopyTo(KeyValuePair<string, EQuery>[] array, int arrayIndex)
        {
            array.ArgumentNotNull(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException(
                    "The index is equal to or greater than the length of the array, or the number of elements in the dictionary is greater than the available space from index to the end.",
                    nameof(arrayIndex));

            foreach (var pQuery in _instance)
                array[arrayIndex++] = new KeyValuePair<string, EQuery>(pQuery.Key, pQuery.Value);
        }

        public void Clear()
        {
            IsUnlocked();
            _instance.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, EQuery>> IEnumerable<KeyValuePair<string, EQuery>>.GetEnumerator()
        {
            return _instance.GetEnumerator();
        }

        public void Add(KeyValuePair<string, EQuery> item)
        {
            item.ArgumentNotNull(nameof(item));
            Add(item.Key, item.Value);
        }

        public void Add(string key, EQuery value)
        {
            key.ArgumentNotNull(nameof(key));
            key.ArgumentValidType(typeof(string), nameof(key));
            IsUnlocked();
            this[key] = value;
        }

        public bool Remove(KeyValuePair<string, EQuery> item)
        {
            throw new InvalidOperationException();
        }

        public bool Remove(string key)
        {
            throw new InvalidOperationException();
        }

        #endregion

        #region EQuery Override

        public override EQuery this[object key]
        {
            get
            {
                key.ArgumentNotNull(nameof(key));
                key.ArgumentValidType(typeof(string), nameof(key));
                return this[key as string];
            }
            set
            {
                key.ArgumentNotNull(nameof(key));
                key.ArgumentValidType(typeof(string), nameof(key));
                this[key as string] = value;
            }
        }

        protected override ICollection<EQuery> Childs()
        {
            return _instance.Select(kv => kv.Value).ToList();
        }

        public override void CopyTo(Array array, int index)
        {
            array.ArgumentNotNull(nameof(array));
            array.ArgumentValidType(typeof(KeyValuePair<string, EQuery>[]), nameof(array));
            CopyTo(array as KeyValuePair<string, EQuery>[], index);
        }

        public override IEnumerator GetEnumerator()
        {
            return _instance.GetEnumerator();
        }

        public override EQuery AccessTo(EPath ePath)
        {
            ePath.ArgumentNotNull(nameof(ePath));
            if (!ePath.MoveNext())
                return this;
            if (ePath.CurrentPath == null || !ContainsKey(ePath.CurrentPath))
                return new ENull(ReasonType.AccessFailure, $"Invalid path (Can't access {ePath} from {Path})");
            return this[ePath.CurrentPath].AccessTo(ePath);
        }

        #endregion EQuery Override
    }
}