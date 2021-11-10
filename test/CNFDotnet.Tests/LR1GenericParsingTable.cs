using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Globalization;

using CNFDotnet.Analysis.Grammar;
using CNFDotnet.Analysis.Parsing;
using CNFDotnet.Analysis.Parsing.LR;

namespace CNFDotnet.Tests
{
    public class LR1GenericParsingTable<TKernelItem>
        : BaseGenericParsingTable<BaseLR1ActionDictionary<TKernelItem>>
        where TKernelItem : BaseLR0KernelItem, IEquatable<TKernelItem>
    {
        public LR1GenericParsingTable(params GenericParsingTableItem[] items)
            : base(items)
        { }

        public LR1GenericParsingTable(string json)
            : base(json)
        { }

        public override void Verify
        (
            CNFGrammar cnfGrammar,
            IParsingTable<BaseLR1ActionDictionary<TKernelItem>> generatedTable
        )
        {
            BaseLR1ActionDictionary<TKernelItem> foundRow;
            LR1ActionItem<TKernelItem> foundAction;
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

                foundRow = generatedTable.ElementAt(rowNumber);
                itemToken = item.Column;

                if(!foundRow.TryGetValue(itemToken, out foundAction))
                {
                    throw new VerifcationException
                        ($"State {rowNumber} does not contain {item.Column}");
                }

                if(item.Action.StartsWith
                   ("s", StringComparison.OrdinalIgnoreCase))
                {
                    if(foundAction.Shift is not null)
                    {
                        newState = int.Parse
                        (
                            item.Action.Substring(1),
                            CultureInfo.InvariantCulture
                        );

                        if(foundAction.Shift.Index != newState)
                        {
                            throw new VerifcationException
                                ($"A shift to {item.Action} was expected, but"
                                    + $" got {foundAction.Shift.Index}");
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
                    StringComparison.Ordinal
                ))
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
            foreach(BaseLR1ActionDictionary<TKernelItem> action
                    in generatedTable)
            {
                foreach(KeyValuePair<Token, LR1ActionItem<TKernelItem>> kv
                        in action)
                {
                    if(kv.Value.Shift is not null)
                    {
                        itemAction = $"s{kv.Value.Shift.Index}";
                    }
                    else if(kv.Value.Reduce is not null
                            && kv.Value.Reduce.Count == 1)
                    {
                        if(kv.Value.Reduce[0].Index == -1)
                        {
                            itemAction = "a";
                        }
                        else
                        {
                            itemAction = $"r{kv.Value.Reduce[0].Index}";
                        }
                    }
                    else
                    {
                        throw new VerifcationException
                            ($"Invalid state {rowNumber}");
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

                    foreach(JsonProperty prop in tableRow.EnumerateObject())
                    {
                        column = prop.Name;
                        if(string.Equals
                        (
                            column,
                            "Grammar.END",
                            StringComparison.Ordinal
                        ))
                        {
                            column = "$";
                        }

                        if(prop.Value.TryGetProperty
                           ("shift", out shift))
                        {
                            action = $"s{shift.GetInt32()}";

                            lst.Add((row, column, action));
                        }
                        else if(prop.Value.TryGetProperty("reduce", out reduce))
                        {
                            //this should always be 1
                            foreach(JsonElement reduction
                                    in reduce.EnumerateArray())
                            {
                                productionIndex = reduction.GetInt32();

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
                        }
                    }

                    stateIndex++;
                }
            }

            return lst.ToArray();
        }
    }
}
