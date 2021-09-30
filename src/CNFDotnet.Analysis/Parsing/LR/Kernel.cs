using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public class Kernel<TKernelItem> : IList<TKernelItem>, IEquatable<Kernel<TKernelItem>>
        where TKernelItem : BaseKernelItem
    {
        private IList<TKernelItem> _items;

        public TKernelItem this[int index] { get => this._items[index]; set => this._items[index] = value; }
        public int Count => this._items.Count;
        public bool IsReadOnly => this._items.IsReadOnly;

        public Kernel ()
        {
            this._items = new List<TKernelItem>();
        }

        public void Add (TKernelItem item)
        {
            this._items.Add(item);
        }

        public void Clear ()
        {
            this._items.Clear();
        }

        public bool Contains (TKernelItem item)
        {
            return this._items.Contains(item);
        }

        public void CopyTo (TKernelItem[] array, int arrayIndex)
        {
            this._items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TKernelItem> GetEnumerator ()
        {
            return this._items.GetEnumerator();
        }

        public int IndexOf (TKernelItem item)
        {
            return this._items.IndexOf(item);
        }

        public void Insert (int index, TKernelItem item)
        {
            this._items.Insert(index, item);
        }

        public bool Remove (TKernelItem item)
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

        public bool Equals (Kernel<TKernelItem> other)
        {
            if(other is null)
            {
                return false;
            }

            //Kernel sequences can be in random order
//            Dictionary<TKernelItem, int> cnt = new Dictionary<TKernelItem, int>();
//            foreach(TKernelItem item in this)
//            {
//                if(cnt.ContainsKey(item))
//                {
//                    cnt[item]++;
//                }
//                else
//                {
//                    cnt.Add(item, 1);
//                }
//            }
//
//            foreach(TKernelItem item in other)
//            {
//                if(cnt.ContainsKey(item))
//                {
//                    cnt[item]--;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//
//            return cnt.Values.All(c => c == 0);
        
            if(this.Count != other.Count)
            {
                return false;
            }

            int i, j;

            for(i = 0; i < this.Count; i++)
            {
                for(j = 0; j < other.Count; j++)
                {
                    if(this[i].Equals(other[j]))
                    {
                        break;
                    }
                }

                if(j == other.Count)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object? obj)
        {
            if(obj is not Kernel<TKernelItem> other)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
            => this._items.GetHashCode();
    }
}