using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LR.SLR1
{
    public class SLR1Parsing : BaseLR0Parsing<SLR1Action>
    {
        public SLR1Parsing(CNFGrammar cnfGrammar)
            : base(cnfGrammar)
        { }

        public override void Classify () 
        {
            this.ClassifyLR1(this.CreateParsingTable());
        }

        public override IParsingTable<SLR1Action> CreateParsingTable ()
        {
            if(this.ParsingTable is not null)
            {
                return this.ParsingTable;
            }

            IList<State<LR0KernelItem>> automaton = this.CreateAutomaton();
            Dictionary<Token, HashSet<Token>> follow = this.CNFGrammar.FollowSet;
            Token end = new Token(TokenType.EOF);
            SLR1Action actions;
            ParsingTable<SLR1Action> table = new ParsingTable<SLR1Action>();

            foreach(State<LR0KernelItem> state in automaton)
            {
                actions = new SLR1Action();

                foreach(KeyValuePair<Token, State<LR0KernelItem>> kv in state.Transitions)
                {
                    actions.Add(kv.Key, new LR1ActionItem<LR0KernelItem>(kv.Value));
                }

                foreach(LR0KernelItem item in state.Items)
                {
                    if(item.Production.Equals(Production.Null))
                    {
                        if(item.Index == 1)
                        {
                            this.AddReduceAction(actions, end, item.Production);
                        }
                    }
                    else
                    {
                        if(item.Index == item.Production.Right.Count)
                        {
                            foreach(Token token in follow[item.Production.Left])
                            {
                                this.AddReduceAction(actions, token, item.Production);
                            }
                        }
                    }
                }

                table.Add(actions);
            }
            
            this.ParsingTable = table;

            return table;
        }
    }
}

