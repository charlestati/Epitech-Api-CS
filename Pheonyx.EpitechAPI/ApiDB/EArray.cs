using Pheonyx.EpitechAPI.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pheonyx.EpitechAPI
{
    public sealed class EArray : EQuery, IList<EQuery>
    {
        private EQuery[] cInstance;

        #region Constructor
        public EArray() : base(EQueryType.Array)
        {
            _itemCollection = new EQuery[0];
            cInstance = _itemCollection as EQuery[];
        }
        #endregion

        #region IDictionary Interface
        public EQuery this[Int32 key]
        {
            get
            {
                return cInstance[key];
            }

            set
            {
                IsUnlocked();

                cInstance[key] = value;
                if (value != null)
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
        override public int Count
        {
            get
            {
                return cInstance.Length;
            }
        }
        public ICollection<Int32> Keys
        {
            get
            {
                return Enumerable.Range(0, cInstance.Length).ToList();
            }
        }
        public ICollection<EQuery> Values
        {
            get
            {
                return cInstance;
            }
        }
        public int IndexOf(EQuery query)
        {
            for (int i = 0; i < Count; i++)
                if (cInstance[i] == query)
                    return i;
            return -1;
        }
        public bool Contains(EQuery query)
        {
            return IndexOf(query) < 0;
        }
        public void CopyTo(EQuery[] array, Int32 arrayIndex)
        {
            array.ArgumentNotNull(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("The index is equal to or greater than the length of the array, or the number of elements in the dictionary is greater than the available space from index to the end.", nameof(arrayIndex));

            for (int i = 0; i < Count; i++)
                array[arrayIndex + i] = cInstance[i];
        }
        public void Clear()
        {
            IsUnlocked();
            for (int i = 0; i < Count; i++)
                cInstance[i] = null;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        IEnumerator<EQuery> IEnumerable<EQuery>.GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return cInstance[i];
            yield break;
        }
        public void Add(EQuery value)
        {
            //Todo: Add ENull
            this.IsUnlocked();
            Array.Resize(ref cInstance, Count + 1);
            this[Count - 1] = value;
        }
        public void Insert(Int32 index, EQuery value)
        {
            this[index] = value;
        }
        public void RemoveAt(Int32 index)
        {
            index.ArgumentNotNull(nameof(index));
            IsUnlocked();
            this[index] = null;
        }
        public bool Remove(EQuery value)
        {
            int index = IndexOf(value);
            RemoveAt(index);
            return this[index] == null;
        }
        #endregion

        #region AEQuery Override
        public override EQuery this[Object key]
        {
            get
            {
                key.ArgumentNotNull(nameof(key));
                key.ArgumentValidType(typeof(Int32), nameof(key));
                return this[(int)key];
            }
            set
            {
                key.ArgumentNotNull(nameof(key));
                key.ArgumentValidType(typeof(Int32), nameof(key));
                this[(int)key] = value;
            }
        }
        protected override ICollection<EQuery> Childs()
        {
            return cInstance;
        }
        public override void CopyTo(Array array, int index)
        {
            array.ArgumentNotNull(nameof(array));
            array.ArgumentValidType(typeof(KeyValuePair<Int32, EQuery>[]), nameof(array));
            CopyTo(array as KeyValuePair<Int32, EQuery>[], index);
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
            int? row;
            if (ePath.CurrentPath == null || (row = ePath.CurrentRow) == null)
                return (null); //Todo: Create ENull: Invalid path
            if (row < 0 || row >= Count)
                return (null); //Todo: Create ENull: OutOfRange
            return (this[(int)row].AccessTo(ePath));
        }
        #endregion

    }
}
