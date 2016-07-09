using Pheonyx.EpitechAPI.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pheonyx.EpitechAPI
{
    public sealed class EObject : EQuery, IDictionary<String, EQuery>
    {
        private Dictionary<String, EQuery> cInstance;

        #region Constructor
        public EObject() : base(EQueryType.Structure)
        {
            _itemCollection = new Dictionary<string, EQuery>();
            cInstance = _itemCollection as Dictionary<String, EQuery>;
        }
        #endregion

        #region IDictionary Interface
        public EQuery this[String key]
        {
            get
            {
                key.ArgumentNotNull(nameof(key));
                return cInstance[key];
            }

            set
            {
                key.ArgumentNotNull(nameof(key));
                IsUnlocked();

                if (cInstance.ContainsKey(key))
                    cInstance[key] = value;
                else
                    cInstance.Add(key, value);
                value.LockManager = _lockManager;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return _isLocked;
            }
        }
        public ICollection<String> Keys
        {
            get
            {
                return cInstance.Keys;
            }
        }
        public ICollection<EQuery> Values
        {
            get
            {
                return cInstance.Values;
            }
        }
        public bool Contains(KeyValuePair<String, EQuery> item)
        {
            item.Key.ArgumentNotNull(nameof(item.Key));
            return cInstance.Contains(item);
        }
        public bool ContainsKey(String key)
        {
            key.ArgumentNotNull(nameof(key));
            return cInstance.ContainsKey(key);
        }
        public bool TryGetValue(String key, out EQuery value)
        {
            key.ArgumentNotNull(nameof(key));
            return cInstance.TryGetValue(key, out value);
        }
        public void CopyTo(KeyValuePair<String, EQuery>[] array, Int32 arrayIndex)
        {
            array.ArgumentNotNull(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < this.Count)
                throw new ArgumentException("The index is equal to or greater than the length of the array, or the number of elements in the dictionary is greater than the available space from index to the end.", nameof(arrayIndex));

            foreach (var pQuery in cInstance)
                array[arrayIndex++] = new KeyValuePair<String, EQuery>(pQuery.Key, pQuery.Value);
        }
        public void Clear()
        {
            IsUnlocked();
            cInstance.Clear();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        IEnumerator<KeyValuePair<String, EQuery>> IEnumerable<KeyValuePair<String, EQuery>>.GetEnumerator()
        {
            return cInstance.GetEnumerator();
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
            //TODO: Add ENull
            this.IsUnlocked();
            this[key] = value;
        }
        public bool Remove(KeyValuePair<String, EQuery> item)
        {
            item.ArgumentNotNull(nameof(item));
            return Remove(item.Key);
        }
        public bool Remove(String key)
        {
            key.ArgumentNotNull(nameof(key));
            IsUnlocked();
            return cInstance.Remove(key);
        }
        #endregion

        #region AEQuery Override
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
            return cInstance.Select(kv => kv.Value).ToList();
        }
        public override void CopyTo(Array array, int index)
        {
            array.ArgumentNotNull(nameof(array));
            array.ArgumentValidType(typeof(KeyValuePair<String, EQuery>[]), nameof(array));
            CopyTo(array as KeyValuePair<String, EQuery>[], index);
        }
        public override IEnumerator GetEnumerator()
        {
            return cInstance.GetEnumerator();
        }
        public override EQuery AccessTo(EPath ePath)
        {
            ePath.ArgumentNotNull(nameof(ePath));
            if (!ePath.MoveNext())
                return this;
            if (ePath.CurrentPath == null || !ContainsKey(ePath.CurrentPath))
                return (null); //Todo: Create ENull
            return (this[ePath.CurrentPath].AccessTo(ePath));
        } 
        #endregion
    }
}
