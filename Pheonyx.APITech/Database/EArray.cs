using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pheonyx.APITech.Utils;

namespace Pheonyx.APITech.Database
{
    /// <summary>
    ///     Fournit des méthodes pour la création, la manipulation et la recherche des requêtes de type
    ///     <see cref="EQueryType.Array" />.
    /// </summary>
    public sealed class EArray : EQuery, IList<EQuery>
    {
        private EQuery[] _instance;

        #region Constructor

        /// <summary>
        ///     Initialise une nouvelle instance de la classe <see cref="EArray" /> qui est vide.
        /// </summary>
        public EArray() : base(EQueryType.Array)
        {
            ItemCollection = new EQuery[0];
            _instance = ItemCollection as EQuery[];
        }

        #endregion Constructor

        public override string ToString()
        {
            if (_parent == null)
                return $"[ {_instance.Select(c => c.ToString()).Aggregate((i, j) => i + ", " + j)} ]";
            return $"\"{PtrIndex}\": [ {_instance.Select(c => c.ToString()).Aggregate((i, j) => i + ", " + j)} ]";
        }

        #region IDictionary Interface

        #region Properties

        public EQuery this[int key]
        {
            get
            {
                if (key >= 0 && key < Count)
                    return _instance[key];
                return new ENull(ReasonType.InvalidKey, $"Key '{key}' out of range (< 0 or > {Count - 1})");
            }

            set
            {
                IsUnlocked();

                var eValue = value ?? new ENull(ReasonType.InvalidValue, $"Invalid value (null)");
                eValue.Path = key.ToString();
                eValue.Parent = this;
                eValue.LockManager = _lockManager;
                _instance[key] = eValue;
            }
        }

        public bool IsReadOnly => _isLocked;

        public override int Count => _instance.Length;

        /// <summary>
        ///     Obtient un <see cref="ICollection" /> contenant les clés de <see cref="EArray" />.
        /// </summary>
        public ICollection<int> Keys => Enumerable.Range(0, _instance.Length).ToList();

        /// <summary>
        ///     Obtient un <see cref="ICollection" /> contenant les valeurs de <see cref="EArray" />.
        /// </summary>
        public ICollection<EQuery> Values => _instance;

        #endregion

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

        public void CopyTo(EQuery[] array, int arrayIndex)
        {
            array.ArgumentNotNull(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException(
                    "The index is equal to or greater than the length of the array, or the number of elements in the dictionary is greater than the available space from index to the end.",
                    nameof(arrayIndex));

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

        public void Insert(int index, EQuery value)
        {
            this[index] = value;
        }

        public void RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        public bool Remove(EQuery value)
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
                key.ArgumentValidType(typeof(int), nameof(key));
                return this[(int) key];
            }
            set
            {
                key.ArgumentNotNull(nameof(key));
                key.ArgumentValidType(typeof(int), nameof(key));
                this[(int) key] = value;
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
            return this[(int) row].AccessTo(ePath);
        }

        #endregion
    }
}