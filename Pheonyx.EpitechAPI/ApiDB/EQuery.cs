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
        DBNull,
        Structure,
        Array,
        Int64,
        Double,
        String,
        DateTime,
        Boolean
    }

    public class EQuery : IDictionary<Object, EQuery>
    {
        private Dictionary<string, EQuery> itemContainer;
        private bool readOnly = false;

        #region Constructor
        public EQuery()
        {
            itemContainer = new Dictionary<string, EQuery>();
        }
        protected EQuery(Dictionary<string, EQuery> itemContainer = null)
        {
            this.itemContainer = itemContainer;
        }
        #endregion

        #region Interface Implementation
        public EQuery this[Object key]
        {
            get
            {
                key.ArgumentNotNull(nameof(key));
                key.ArgumentValidType(typeof(String), nameof(key));
                return AccessTo(key as string);
            }

            set
            {
                key.ArgumentNotNull(nameof(key));
                key.ArgumentValidType(typeof(String), nameof(key));

                if (readOnly)
                    throw new InvalidOperationException("This Query instance is read only.");
                if (itemContainer.ContainsKey(key as string))
                    itemContainer[key as string] = value;
                else
                    itemContainer.Add(key as string, value);
            }
        }

        public int Count
        {
            get
            {
                return itemContainer.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return readOnly;
            }
        }

        public ICollection<Object> Keys
        {
            get
            {
                return itemContainer.Select(kv => kv.Key as object).ToList();
            }
        }

        public ICollection<EQuery> Values
        {
            get
            {
                return itemContainer.Select(kv => kv.Value).ToList();
            }
        }

        public void Add(KeyValuePair<Object, EQuery> item)
        {
            item.ArgumentNotNull(nameof(item));
            item.ArgumentValidType(typeof(KeyValuePair<String, EQuery>), nameof(item));
            item.Key.ArgumentNotNull(nameof(item.Key));
            item.Value.ArgumentNotNull(nameof(item.Value));

            if (readOnly)
                throw new InvalidOperationException("This Query instance is read only.");
            itemContainer.Add(item.Key as string, item.Value);
        }

        public void Add(Object key, EQuery value)
        {
            key.ArgumentNotNull(nameof(key));
            key.ArgumentValidType(typeof(String), nameof(key));
            value.ArgumentNotNull(nameof(value));

            if (readOnly)
                throw new InvalidOperationException("This Query instance is read only.");
            itemContainer.Add(key as string, value);
        }

        public void Clear()
        {
            if (readOnly)
                throw new InvalidOperationException("This Query instance is read only.");
            itemContainer.Clear();
        }

        public bool Contains(KeyValuePair<Object, EQuery> item)
        {
            item.ArgumentValidType(typeof(KeyValuePair<String, EQuery>), nameof(item));
            item.Key.ArgumentNotNull(nameof(item.Key));
            item.Value.ArgumentNotNull(nameof(item.Value));

            return itemContainer.Contains(new KeyValuePair<string, EQuery>(item.Key as string, item.Value));
        }

        public bool ContainsKey(Object key)
        {
            key.ArgumentNotNull(nameof(key));
            key.ArgumentValidType(typeof(String), nameof(key));

            return itemContainer.ContainsKey(key as string);
        }

        public void CopyTo(KeyValuePair<Object, EQuery>[] array, int arrayIndex)
        {
            array.ArgumentNotNull(nameof(array));
            array.ArgumentValidType(typeof(KeyValuePair<String, EQuery>[]), nameof(array));
            arrayIndex.ArgumentValidType(typeof(Int32), nameof(arrayIndex));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Count() - arrayIndex < this.Count())
                throw new ArgumentException("The index is equal to or greater than the length of the array, or the number of elements in the dictionary is greater than the available space from index to the end.", nameof(arrayIndex));

            foreach (var pItem in itemContainer)
                array[arrayIndex++] = new KeyValuePair<Object, EQuery>(pItem.Key, pItem.Value);
        }

        public IEnumerator<KeyValuePair<Object, EQuery>> GetEnumerator()
        {
            return itemContainer.Cast<KeyValuePair<Object, EQuery>>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Remove(KeyValuePair<Object, EQuery> item)
        {
            item.ArgumentValidType(typeof(KeyValuePair<String, EQuery>), nameof(item));
            item.Key.ArgumentNotNull(nameof(item.Key));

            if (readOnly)
                throw new InvalidOperationException("This Query instance is read only.");
            return itemContainer.Remove(item.Key as string);
        }

        public bool Remove(Object key)
        {
            key.ArgumentNotNull(nameof(key));
            key.ArgumentValidType(typeof(String), nameof(key));

            if (readOnly)
                throw new InvalidOperationException("This Query instance is read only.");
            return itemContainer.Remove(key as string);
        }

        public bool TryGetValue(Object key, out EQuery value)
        {
            key.ArgumentNotNull(nameof(key));
            key.ArgumentValidType(typeof(String), nameof(key));

            return itemContainer.TryGetValue(key as String, out value);
        }

        #endregion

        public bool LockQuery()
        {
            if (readOnly)
                throw new InvalidOperationException("This Query instance is already read only.");
            readOnly = true;
            return IsReadOnly;
        }

        public EQuery AccessTo(EPath path)
        {
            if (!path.MoveNext())
                return this;
            if (path.CurrentPath == null || !itemContainer.ContainsKey(path.CurrentPath))
                return (null); //Create ENull
            return (itemContainer[path.CurrentPath].AccessTo(path));
        }
    }
}
