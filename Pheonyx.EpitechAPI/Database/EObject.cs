using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pheonyx.EpitechAPI.Utils;

namespace Pheonyx.EpitechAPI.Database
{
    public sealed class EObject : EQuery, IDictionary<String, EQuery>
    {
        private readonly Dictionary<String, EQuery> _instance;

        #region Constructor
        public EObject() : base(EQueryType.Object)
        {
            _itemCollection = new Dictionary<string, EQuery>();
            _instance = _itemCollection as Dictionary<String, EQuery>;
        }
        #endregion

        #region IDictionary Interface
        public EQuery this[String key]
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

                EQuery eValue = value ?? new ENull(ReasonType.InvalidValue, $"Invalid value insertion (null)"); 
                if (_instance.ContainsKey(key))
                    _instance[key] = eValue;
                else
                    _instance.Add(key, eValue);
                eValue.Path = key;
                eValue.Parent = eValue;
                eValue.LockManager = _lockManager;
            }
        }
        public bool IsReadOnly => _isLocked;
        public ICollection<String> Keys => _instance.Keys;
        public ICollection<EQuery> Values => _instance.Values;
        public bool Contains(KeyValuePair<String, EQuery> item)
        {
            item.Key.ArgumentNotNull(nameof(item.Key));
            return _instance.Contains(item);
        }
        public bool ContainsKey(String key)
        {
            key.ArgumentNotNull(nameof(key));
            return _instance.ContainsKey(key);
        }
        public bool TryGetValue(String key, out EQuery value)
        {
            key.ArgumentNotNull(nameof(key));
            return _instance.TryGetValue(key, out value);
        }
        public void CopyTo(KeyValuePair<String, EQuery>[] array, Int32 arrayIndex)
        {
            array.ArgumentNotNull(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("The index is equal to or greater than the length of the array, or the number of elements in the dictionary is greater than the available space from index to the end.", nameof(arrayIndex));

            foreach (var pQuery in _instance)
                array[arrayIndex++] = new KeyValuePair<String, EQuery>(pQuery.Key, pQuery.Value);
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
        IEnumerator<KeyValuePair<String, EQuery>> IEnumerable<KeyValuePair<String, EQuery>>.GetEnumerator()
        {
            return _instance.GetEnumerator();
        }

        public void Add(KeyValuePair<String, EQuery> item)
        {
            item.ArgumentNotNull(nameof(item));
            Add(item.Key, item.Value);
        }
        public void Add(String key, EQuery value)
        {
            key.ArgumentNotNull(nameof(key));
            key.ArgumentValidType(typeof(String), nameof(key));
            IsUnlocked();
            this[key] = value;
        }
        public bool Remove(KeyValuePair<String, EQuery> item)
        {
            throw new InvalidOperationException();
        }
        public bool Remove(String key)
        {
            throw new InvalidOperationException();
        }
        #endregion

        #region EQuery Override
        public override EQuery this[Object key]
        {
            get
            {
                key.ArgumentNotNull(nameof(key));
                key.ArgumentValidType(typeof(String), nameof(key));
                return this[key as string];
            }
            set
            {
                key.ArgumentNotNull(nameof(key));
                key.ArgumentValidType(typeof(String), nameof(key));
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
            array.ArgumentValidType(typeof(KeyValuePair<String, EQuery>[]), nameof(array));
            CopyTo(array as KeyValuePair<String, EQuery>[], index);
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
        #endregion

        internal void Add(IDictionary<String, EQuery> eQuery)
        {
            foreach (var child in eQuery)
                Add(child);
        }

        public override string ToString()
        {
            if (_parent == null)
                return $"{{ {_instance.Select(c => c.Value.ToString()).Aggregate((i, j) => i + ", " + j)} }}";
            return $"\"{_ptrIndex}\": {{ {_instance.Select(c => c.Value.ToString()).Aggregate((i, j) => i + ", " + j)} }}";
        }
    }
}
