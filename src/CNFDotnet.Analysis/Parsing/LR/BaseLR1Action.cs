using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    //Represents an LR(1) action in a parsing table, consider each dictionary a
    //row in a parsing table
    public abstract class BaseLR1ActionDictionary<TKernelItem>
        : IDictionary<Token, LR1ActionItem<TKernelItem>>, IAction
        where TKernelItem : BaseLR0KernelItem, IEquatable<TKernelItem>
    {
        public ICollection<Token> Keys => this._items.Keys;
        public ICollection<LR1ActionItem<TKernelItem>> Values
            => this._items.Values;
        public int Count => this._items.Count;
        public bool IsReadOnly => false;

        public LR1ActionItem<TKernelItem> this[Token key]
        {
            get => this._items[key];
            set => this._items[key] = value;
        }

        private readonly Dictionary<Token, LR1ActionItem<TKernelItem>> _items;

        public BaseLR1ActionDictionary()
        {
            this._items = new Dictionary<Token, LR1ActionItem<TKernelItem>>();
        }

        public void Add(Token key, LR1ActionItem<TKernelItem> value)
            => this._items.Add(key, value);

        public bool ContainsKey(Token key) => this._items.ContainsKey(key);

        public bool Remove(Token key) => this._items.Remove(key);

        public bool TryGetValue
        (
            Token key,
            [MaybeNullWhen(false)] out LR1ActionItem<TKernelItem> value
        )
            => this._items.TryGetValue(key, out value);

        public void Add(KeyValuePair<Token, LR1ActionItem<TKernelItem>> item)
            => this._items.Add(item.Key, item.Value);

        public void Clear() => this._items.Clear();

        public bool Contains
        (
            KeyValuePair<Token, LR1ActionItem<TKernelItem>> item
        )
            => this._items.ContainsKey(item.Key);

        public void CopyTo
        (
            KeyValuePair<Token, LR1ActionItem<TKernelItem>>[] array,
            int arrayIndex
        )
            => throw new NotImplementedException();

        public bool Remove(KeyValuePair<Token, LR1ActionItem<TKernelItem>> item)
            => this._items.Remove(item.Key);

        public IEnumerator<KeyValuePair<Token, LR1ActionItem<TKernelItem>>>
            GetEnumerator()
            => this._items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._items.GetEnumerator();
    }
}