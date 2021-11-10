using System.Collections.Generic;
using System.Linq;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;

namespace CNFDotnet.Tests
{
    public abstract class BaseGenericParsingTable<TAction>
        where TAction : class, IAction
    {
        public IEnumerable<GenericParsingTableItem> Items => this._items;

        private readonly GenericParsingTableItem[] _items;

        public BaseGenericParsingTable(params GenericParsingTableItem[] items)
        {
            this._items = items;
        }

        public BaseGenericParsingTable(string json)
        {
            this._items = this.ParseJSON(json);
        }

        public abstract void Verify
            (CNFGrammar cnfGrammar, IParsingTable<TAction> generatedTable);
        protected abstract GenericParsingTableItem[]
            ParseJSON(string json);

        public override string ToString()
            => string.Join
            (
                '\n',
                this._items.Select
                (
                    x => $"(\"{x.Row}\", \"{x.Column}\", \"{x.Action}\"),"
                )
            );
    }

    public class GenericParsingTableItem
    {
        public string Row { get; private set; }
        public string Column { get; private set; }
        public string Action { get; private set; }

        private GenericParsingTableItem(string row, string column, string action)
        {
            this.Row = row;
            this.Column = column;
            this.Action = action;
        }

        public GenericParsingTableItem
            ((string Row, string Column, string Action) element)
            : this(element.Row, element.Column, element.Action)
        { }

        public static implicit operator GenericParsingTableItem
            ((string Row, string Column, string Action) element)
            => new GenericParsingTableItem(element);

        public override string ToString()
            => $"{this.Row}:{this.Column} -> {this.Action}";
    }
}
