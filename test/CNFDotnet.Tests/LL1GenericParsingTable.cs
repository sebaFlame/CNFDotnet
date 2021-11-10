using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Globalization;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LL;

namespace CNFDotnet.Tests
{
    public class LL1GenericParsingTable : BaseGenericParsingTable<LL1Action>
    {
        public LL1GenericParsingTable(params GenericParsingTableItem[] items)
            : base(items)
        { }

        public LL1GenericParsingTable(string json)
            : base(json)
        { }

        public override void Verify
            (CNFGrammar cnfGrammar, IParsingTable<LL1Action> generatedTable)
        {
            LL1Action foundAction;
            int productionIndex;
            GenericParsingTableItem foundItem;

            foreach(GenericParsingTableItem item in this.Items)
            {
                productionIndex = int.Parse
                (
                    item.Action,
                    CultureInfo.InvariantCulture
                );

                foundAction = generatedTable.SingleOrDefault
                (
                    x => string.Equals
                        (
                            x.Terminal,
                            item.Column,
                            StringComparison.Ordinal
                        )
                        && string.Equals
                        (
                            x.NonTerminal,
                            item.Row,
                            StringComparison.Ordinal
                        )
                        && x.Production.Index == productionIndex
                );

                if(foundAction is null)
                {
                    throw new VerifcationException($"{item} not found!");
                }
            }

            foreach(LL1Action action in generatedTable)
            {
                foundItem = this.Items.SingleOrDefault
                (
                    x => string.Equals
                        (
                            action.Terminal,
                            x.Column,
                            StringComparison.Ordinal
                        )
                        && string.Equals
                        (
                            action.NonTerminal,
                            x.Row,
                            StringComparison.Ordinal
                        )
                        && action.Production.Index == int.Parse
                            (
                                x.Action,
                                CultureInfo.InvariantCulture
                            )
                );

                if(foundItem is null)
                {
                    throw new VerifcationException(action.ToString());
                }
            }
        }

        protected override GenericParsingTableItem[] ParseJSON
            (string json)
        {
            List<GenericParsingTableItem> lst
                = new List<GenericParsingTableItem>();

            string row, column, index;
            using(JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;

                foreach(JsonProperty tableRow in root.EnumerateObject())
                {
                    row = tableRow.Name;
                    foreach(JsonProperty cell
                            in tableRow.Value.EnumerateObject())
                    {
                        column = cell.Name;
                        if(string.Equals
                        (
                            column,
                            "Grammar.END",
                            StringComparison.Ordinal
                        ))
                        {
                            column = "$";
                        }

                        foreach(JsonElement prod
                                in cell.Value.EnumerateArray())
                        {
                            index = prod
                                .GetInt32()
                                .ToString(CultureInfo.InvariantCulture);
                            lst.Add((row, column, index));

                            //yield return (row, column, index);
                        }
                    }
                }
            }

            return lst.ToArray();
        }
    }
}
