using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pheonyx.EpitechAPI.Utils;

namespace Pheonyx.EpitechAPI.Database
{
    public sealed class EArray : EQuery, IList<EQuery>
    {
        private EQuery[] _instance;

        #region Constructor
        public EArray() : base(EQueryType.Array)
        {
            _itemCollection = new EQuery[0];
            _instance = _itemCollection as EQuery[];
        }
        #endregion

        #region IDictionary Interface
        public EQuery this[Int32 key]
        {
            get
            {
                if (key > 0 && key < Count)
                    return _instance[key];
                return new ENull(ReasonType.InvalidKey, $"Key '{key}' out of range (< 0 or > {Count - 1})");
            }

            set
            {
                IsUnlocked();

                EQuery eValue = value ?? new ENull(ReasonType.InvalidValue, $"Invalid value (null)");
                _instance[key] = eValue;
                eValue.LockManager = _lockManager;
            }
        }
        public bool IsReadOnly => _isLocked;
        public override int Count => _instance.Length;
        public ICollection<Int32> Keys => Enumerable.Range(0, _instance.Length).ToList();
        public ICollection<EQuery> Values => _instance;
        public int IndexOf(EQuery query)
        {
            for (var i = 0; i < Count; i++)
                if (_instance[i] == query)
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

            for (var i = 0; i < Count; i++)
                array[arrayIndex + i] = _instance[i];
        }
        public void Clear()
        {
            IsUnlocked();
            for (var i = 0; i < Count; i++)
                _instance[i] = null;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        IEnumerator<EQuery> IEnumerable<EQuery>.GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return _instance[i];
        }
        public void Add(EQuery value)
        {
            IsUnlocked();
            Array.Resize(ref _instance, Count + 1);
            this[Count - 1] = value;
        }
        public void Insert(Int32 index, EQuery value)
        {
            this[index] = value;
        }
        public void RemoveAt(Int32 index)
        {
            throw new InvalidOperationException();
        }
        public bool Remove(EQuery value)
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
                key.ArgumentValidType(typeof(Int32), nameof(key));
                return this[(int)key];
            }
            set
            {
                key.ArgumentNotNull(nameof(key));
                key.ArgumentValidType(typeof(Int32), nameof(key));
                value.Path = key.ToString();
                value.Parent = this;
                value.LockManager = _lockManager;
                this[(int)key] = value;
            }
        }
        protected override ICollection<EQuery> Childs()
        {
            return _instance;
        }
        public override void CopyTo(Array array, int index)
        {
            array.ArgumentNotNull(nameof(array));
            array.ArgumentValidType(typeof(EQuery[]), nameof(array));
            CopyTo(array as EQuery[], index);
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
            int? row;
            if (ePath.CurrentPath == null || (row = ePath.CurrentRow) == null)
                return new ENull(ReasonType.AccessFailure, $"Invalid path (Can't access {ePath} from {Path})");
            if (row < 0 || row >= Count)
                return new ENull(ReasonType.AccessFailure, $"{row} out of range in {ePath}");
            return this[(int)row].AccessTo(ePath);
        }
        #endregion

        public override String ToString()
        {
            if (_parent == null)
                return $"[ {_instance.Select(c => c.ToString()).Aggregate((i, j) => i + ", " + j)} ]";
            return $"\"{_ptrIndex}\": [ {_instance.Select(c => c.ToString()).Aggregate((i, j) => i + ", " + j)} ]";
        }
    }
}
