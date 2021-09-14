using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public class Kernel : IList<BaseKernelItem>, IEquatable<Kernel>
    {
        private IList<BaseKernelItem> _items;

        public BaseKernelItem this[int index] { get => this._items[index]; set => this._items[index] = value; }
        public int Count => this._items.Count;
        public bool IsReadOnly => this._items.IsReadOnly;

        public Kernel ()
        {
            this._items = new List<BaseKernelItem>();
        }

        public void Add (BaseKernelItem item)
        {
            this._items.Add(item);
        }

        public void Clear ()
        {
            this._items.Clear();
        }

        public bool Contains (BaseKernelItem item)
        {
            return this._items.Contains(item);
        }

        public void CopyTo (BaseKernelItem[] array, int arrayIndex)
        {
            this._items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<BaseKernelItem> GetEnumerator ()
        {
            return this._items.GetEnumerator();
        }

        public int IndexOf (BaseKernelItem item)
        {
            return this._items.IndexOf(item);
        }

        public void Insert (int index, BaseKernelItem item)
        {
            this._items.Insert(index, item);
        }

        public bool Remove (BaseKernelItem item)
        {
            return this._items.Remove(item);
        }

        public void RemoveAt (int index)
        {
            this._items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return this._items.GetEnumerator();
        }

        public bool Equals (Kernel other)
        {
            if(other is null)
            {
                return false;
            }

            return Enumerable.SequenceEqual(this, other);
        }

        public override bool Equals(object? obj)
        {
            if(obj is not Kernel other)
            {
                return false;
            }

            return this.Equals(other);
        }
    }
}