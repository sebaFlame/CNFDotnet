﻿using System.Collections;
using System.Collections.Generic;

namespace CNFDotnet.Analysis.Parsing
{
    //A parsing table is a list of actions
    public class ParsingTable<TAction> : IParsingTable<TAction>, IList<TAction>
        where TAction : class, IAction
    {
        private readonly List<TAction> _items;

        public TAction this[int index]
        {
            get => this._items[index];
            set => this._items[index] = value;
        }
        public int Count => this._items.Count;
        public bool IsReadOnly => false;

        public ParsingTable()
        {
            this._items = new List<TAction>();
        }

        public void Add(TAction item) => this._items.Add(item);

        public void Clear() => this._items.Clear();

        public bool Contains(TAction item) => this._items.Contains(item);

        public void CopyTo(TAction[] array, int arrayIndex)
            => this._items.CopyTo(array, arrayIndex);

        public IEnumerator<TAction> GetEnumerator()
            => this._items.GetEnumerator();

        public int IndexOf(TAction item)
            => this._items.IndexOf(item);

        public void Insert(int index, TAction item)
            => this._items.Insert(index, item);

        public bool Remove(TAction item)
            => this._items.Remove(item);

        public void RemoveAt(int index)
            => this._items.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator()
            => this._items.GetEnumerator();
    }
}