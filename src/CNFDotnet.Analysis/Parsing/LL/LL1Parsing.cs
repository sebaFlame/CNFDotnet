using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LL
{
    public class LL1Parsing : BaseParsing<LL1Action>
    {
        public LL1Parsing (CNFGrammar cnfGrammar)
            : base (cnfGrammar)
        { }

        public override void Classify ()
        {
            Dictionary<Token, HashSet<Token>> table = new Dictionary<Token, HashSet<Token>>();

            foreach(Token nonTerminal in this.CNFGrammar.NonTerminals)
            {
                table.Add(nonTerminal, new HashSet<Token>());
            }

            Token head;
            IList<Token> body;

            foreach(Production production in this.CNFGrammar.Productions)
            {
                head = production.Left;
                body = production.Right;

                foreach(Token s in this.CNFGrammar.GetFirst(body))
                {
                    if(table[head].Contains(s))
                    {
                        throw new LL1ClassificationException($"Grammar contains a first set clash on non-terminal {head.Value} ({s.Value})");
                    }

                    table[head].Add(s);
                }
            }

            Dictionary<Token, HashSet<Token>> firstSet = this.CNFGrammar.FirstSet;
            Dictionary<Token, HashSet<Token>> followSet = this.CNFGrammar.FollowSet;

            foreach(Token k in this.CNFGrammar.Nullable)
            {
                foreach(Token f in firstSet[k])
                {
                    if(followSet[k].Contains(f))
                    {
                        throw new LL1ClassificationException($"Grammar contains a first/follow clash on {f.Value}");
                    }
                }
            }
        }

        public override ParsingTable<LL1Action> CreateParsingTable ()
        {
            if(this.ParsingTable is not null)
            {
                return this.ParsingTable;
            }

            ParsingTable<LL1Action> table = new ParsingTable<LL1Action>();
            HashSet<Token> terminals = this.CNFGrammar.Terminals;
            HashSet<Token> nonTerminals = this.CNFGrammar.NonTerminals;
            Dictionary<Token, HashSet<Token>> firstSet = this.CNFGrammar.FirstSet;
            Dictionary<Token, HashSet<Token>> followSet = this.CNFGrammar.FollowSet;

            Token head;
            IList<Token> body;

            foreach (Production production in this.CNFGrammar.Productions)
            {
                head = production.Left;
                body = production.Right;

                foreach (Token s in this.CNFGrammar.GetFirst(body))
                {
                    table.Add(new LL1Action(head, s, production));
                }

                if (this.CNFGrammar.IsNullable(body))
                {
                    foreach (Token s in followSet[head])
                    {
                        table.Add(new LL1Action(head, s, production));
                    }
                }
            }

            this.ParsingTable = table;

            return table;
        }
    }
}