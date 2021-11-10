using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Globalization;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LR;
using CNFDotnet.Analysis.Parsing.LR.LR0;

namespace CNFDotnet.Tests
{
    public class LR0GenericParsingTable : BaseGenericParsingTable<LR0Action>
    {
        public LR0GenericParsingTable(params GenericParsingTableItem[] items)
            : base(items)
        { }

        public LR0GenericParsingTable(string json)
            : base(json)
        { }

        public override void Verify
            (CNFGrammar cnfGrammar, IParsingTable<LR0Action> generatedTable)
        {
            LR0Action foundAction;
            int rowNumber, newState, productionIndex;
            Token itemToken;
            string itemAction;
            GenericParsingTableItem foundItem;

            foreach(GenericParsingTableItem item in this.Items)
            {
                rowNumber = int.Parse
                (
                    item.Row,
                    CultureInfo.InvariantCulture
                );

                if(rowNumber >= generatedTable.Count)
                {
                    throw new VerifcationException
                        ($"State {rowNumber} not found!");
                }

                foundAction = generatedTable.ElementAt(rowNumber);

                if(item.Action.StartsWith
                   ("s", StringComparison.OrdinalIgnoreCase))
                {
                    itemToken = item.Column;

                    if(foundAction.Shift is not null)
                    {
                        if(!foundAction.Shift.ContainsKey(itemToken))
                        {
                            throw new VerifcationException
                                ($"Shift {item.Action} not found");
                        }
                        else
                        {
                            newState = int.Parse
                            (
                                item.Action.Substring(1),
                                CultureInfo.InvariantCulture
                            );

                            if(foundAction.Shift[itemToken].Index != newState)
                            {
                                throw new VerifcationException
                                    ($"A shift to {item.Action} was expected,"
                                     + $" but got"
                                     + foundAction.Shift[itemToken].Index);
                            }
                        }
                    }
                    else
                    {
                        throw new VerifcationException
                            ($"A shift to {item.Action} was expected");
                    }
                }
                else if(item.Action.StartsWith
                        ("r", StringComparison.OrdinalIgnoreCase))
                {
                    if(foundAction.Reduce is not null
                       && foundAction.Reduce.Count == 1)
                    {
                        productionIndex = int.Parse
                        (
                            item.Action.Substring(1),
                            CultureInfo.InvariantCulture
                        );

                        if(foundAction.Reduce[0].Index != productionIndex)
                        {
                            throw new VerifcationException
                                ($"A reduction to {item.Action} was expected,"
                                + $" but got {foundAction.Reduce[0].Index}");
                        }
                    }
                    else
                    {
                        throw new VerifcationException
                            ($"A reduce to {item.Action} was expected");
                    }
                }
                else if(string.Equals
                (
                    item.Action,
                    "a",
                    StringComparison.Ordinal)
                )
                {
                    if(foundAction.Reduce is not null
                       && foundAction.Reduce.Count == 1)
                    {
                        //should be -1
                        if(foundAction.Reduce[0].Index != -1)
                        {
                            throw new VerifcationException
                                ($"An accept was expected, but got a reduction"
                                 + $" to {foundAction.Reduce[0].Index}");
                        }
                    }
                    else
                    {
                        throw new VerifcationException
                            ("An accept was expected");
                    }
                }
            }

            rowNumber = 0;
            foreach(LR0Action action in generatedTable)
            {
                if(action.Shift is not null)
                {
                    foreach(KeyValuePair<Token, State<LR0KernelItem>> kv
                            in action.Shift)
                    {
                        itemAction = $"s{kv.Value.Index}";

                        foundItem = this.Items.SingleOrDefault
                        (
                            x => string.Equals
                                (
                                    x.Row,
                                    rowNumber
                                        .ToString(CultureInfo.InvariantCulture),
                                    StringComparison.Ordinal
                                )
                                && string.Equals
                                (
                                    x.Column,
                                    kv.Key,
                                    StringComparison.Ordinal
                                )
                                && string.Equals
                                (
                                    x.Action,
                                    itemAction,
                                    StringComparison.Ordinal
                                )
                        );

                        if(foundItem is null)
                        {
                            throw new VerifcationException
                                ("Invalid state in parsing table");
                        }
                    }
                }
                else if(action.Reduce is not null
                        && action.Reduce.Count == 1)
                {
                    if(action.Reduce[0].Index == -1)
                    {
                        itemAction = "a";
                    }
                    else
                    {
                        itemAction = $"r{action.Reduce[0].Index}";
                    }

                    foundItem = this.Items.SingleOrDefault
                    (
                        x => string.Equals
                            (
                                x.Row,
                                rowNumber
                                    .ToString(CultureInfo.InvariantCulture),
                                StringComparison.Ordinal
                            )
                            && string.Equals
                            (
                                x.Column,
                                string.Empty,
                                StringComparison.Ordinal
                            )
                            && string.Equals
                            (
                                x.Action,
                                itemAction,
                                StringComparison.Ordinal
                            )
                    );

                    if(foundItem is null)
                    {
                        throw new VerifcationException
                            ("Invalid state in parsing table");
                    }
                }
                else
                {
                    throw new VerifcationException
                        ($"Invalid state {rowNumber}");
                }

                rowNumber++;
            }
        }

        protected override GenericParsingTableItem[] ParseJSON
            (string json)
        {
            List<GenericParsingTableItem> lst
                = new List<GenericParsingTableItem>();

            string row, column, action;
            JsonElement shift, reduce;
            int stateIndex = 0, productionIndex;
            using(JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                foreach(JsonElement tableRow in root.EnumerateArray())
                {
                    row = stateIndex.ToString(CultureInfo.InvariantCulture);

                    shift = tableRow.GetProperty("shift");
                    foreach(JsonProperty prop in shift.EnumerateObject())
                    {
                        column = prop.Name;
                        action = $"s{prop.Value.GetInt32()}";

                        lst.Add((row, column, action));
                    }

                    reduce = tableRow.GetProperty("reduce");
                    foreach(JsonElement element in reduce.EnumerateArray())
                    {
                        column = string.Empty;
                        productionIndex = element.GetInt32();
                        if(productionIndex == -1)
                        {
                            action = "a";
                        }
                        else
                        {
                            action = $"r{productionIndex}";
                        }

                        lst.Add((row, column, action));
                    }

                    stateIndex++;
                }
            }

            return lst.ToArray();
        }
    }
}
