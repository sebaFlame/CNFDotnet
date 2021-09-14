using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LL
{
    public class LL1Classification
    {
        private CNFGrammar _grammar;

        public LL1Classification(CNFGrammar grammar)
        {
            this._grammar = grammar;
        }

        public void ValidateGrammar()
        {
            Dictionary<Token, HashSet<Token>> table = new Dictionary<Token, HashSet<Token>>();

            foreach(Token nonTerminal in this._grammar.NonTerminals)
            {
                table.Add(nonTerminal, new HashSet<Token>());

                //foreach(Token terminal in this._grammar.Terminals)
                //{
                //    table[nonTerminal].Add(terminal);
                //}
            }

            Token head;
            IList<Token> body;

            foreach(Production production in this._grammar.Productions)
            {
                head = production.Left;
                body = production.Right;

                foreach(Token s in this._grammar.GetFirst(body))
                {
                    if(table[head].Contains(s))
                    {
                        throw new LL1ClassificationException($"Grammar contains a first set clash on {head.Value} - {s.Value}");
                    }

                    table[head].Add(s);
                }
            }

            Dictionary<Token, HashSet<Token>> firstSet = this._grammar.FirstSet;
            Dictionary<Token, HashSet<Token>> followSet = this._grammar.FollowSet;

            foreach(Token k in this._grammar.Nullable)
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
    }
}