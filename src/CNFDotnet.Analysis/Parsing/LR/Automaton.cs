using System;
using System.Collections;
using System.Collections.Generic;

namespace CNFDotnet.Analysis.Parsing.LR
{
    /* Represents a set of one or more kernel items (production with an index in
     * that production)  which signify a state in an LR automaton */
    public class Automaton<TKernelItem>
        : IAutomaton<TKernelItem>, IList<State<TKernelItem>>
        where TKernelItem : BaseLR0KernelItem, IEquatable<TKernelItem>
    {
        private readonly IList<State<TKernelItem>> _items;

        public State<TKernelItem> this[int index]
        {
            get => this._items[index];
            set => this._items[index] = value;
        }
        public int Count => this._items.Count;
        public bool IsReadOnly => this._items.IsReadOnly;

        public Automaton()
        {
            this._items = new List<State<TKernelItem>>();
        }

        public void Add(State<TKernelItem> item) => this._items.Add(item);

        public void Clear() => this._items.Clear();

        public bool Contains(State<TKernelItem> item)
            => this._items.Contains(item);

        public void CopyTo(State<TKernelItem>[] array, int arrayIndex)
            => this._items.CopyTo(array, arrayIndex);

        public IEnumerator<State<TKernelItem>> GetEnumerator()
            => this._items.GetEnumerator();

        IEnumerator<IState<TKernelItem>>
            IEnumerable<IState<TKernelItem>>.GetEnumerator()
            => this._items.GetEnumerator();

        public int IndexOf(State<TKernelItem> item)
            => this._items.IndexOf(item);

        public int IndexOf(IState<BaseLR0KernelItem> item)
        {
            for(int i = 0; i < this._items.Count; i++)
            {
                if(item.Equals(this._items[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, State<TKernelItem> item)
            => this._items.Insert(index, item);

        public bool Remove(State<TKernelItem> item)
            => this._items.Remove(item);

        public void RemoveAt(int index)
            => this._items.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator()
            => this._items.GetEnumerator();
    }
}