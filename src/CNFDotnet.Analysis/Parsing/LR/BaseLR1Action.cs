using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public abstract class BaseLR1Action : IDictionary<Token, LR1ActionItem>, IAction
    {
        public ICollection<Token> Keys => this._items.Keys;
        public ICollection<LR1ActionItem> Values => this._items.Values;
        public int Count => this._items.Count;
        public bool IsReadOnly => false;
        public LR1ActionItem this[Token key] { get => this._items[key]; set => this._items[key] = value; }

        private Dictionary<Token, LR1ActionItem> _items;

        public BaseLR1Action()
        {
            this._items = new Dictionary<Token, LR1ActionItem>();
        }

        public void Add (Token key, LR1ActionItem value) => this._items.Add(key, value);
        public bool ContainsKey (Token key) => this._items.ContainsKey(key);
        public bool Remove (Token key) => this._items.Remove(key);
        public bool TryGetValue (Token key, [MaybeNullWhen(false)] out LR1ActionItem value) => this._items.TryGetValue(key, out value);
        public void Add (KeyValuePair<Token, LR1ActionItem> item) => this._items.Add(item.Key, item.Value);
        public void Clear () => this._items.Clear();
        public bool Contains (KeyValuePair<Token, LR1ActionItem> item) => this._items.ContainsKey(item.Key);
        public void CopyTo (KeyValuePair<Token, LR1ActionItem>[] array, int arrayIndex) => throw new NotImplementedException();
        public bool Remove (KeyValuePair<Token, LR1ActionItem> item) => this._items.Remove(item.Key);
        public IEnumerator<KeyValuePair<Token, LR1ActionItem>> GetEnumerator () => this._items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator () => this._items.GetEnumerator();
    }
}