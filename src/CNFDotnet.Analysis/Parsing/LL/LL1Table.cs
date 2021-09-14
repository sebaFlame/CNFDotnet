using System;
using System.Collections.Generic;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LL
{
    public class LL1Table
    {
        private CNFGrammar _grammar;

        public LL1Table(CNFGrammar grammar)
        {
            this._grammar = grammar;
        }
        
        public Dictionary<Token, Dictionary<Token, List<Production>>> GenerateTable()
        {
            Dictionary<Token, Dictionary<Token, List<Production>>> table = new Dictionary<Token, Dictionary<Token, List<Production>>>();
            HashSet<Token> terminals = this._grammar.Terminals;
            HashSet<Token> nonTerminals = this._grammar.NonTerminals;
            Dictionary<Token, HashSet<Token>> firstSet = this._grammar.FirstSet;
            Dictionary<Token, HashSet<Token>> followSet = this._grammar.FollowSet;

            Token end = new Token(TokenType.EOF);

            foreach(Token nonTerminal in nonTerminals)
            {
                table.Add(nonTerminal, new Dictionary<Token, List<Production>>());

                foreach(Token terminal in terminals)
                {
                    table[nonTerminal].Add(terminal, new List<Production>());
                }

                table[nonTerminal].Add(end, new List<Production>());
            }

            Token head;
            IList<Token> body;

            foreach(Production production in this._grammar.Productions)
            {
                head = production.Left;
                body = production.Right;

                foreach(Token s in this._grammar.GetFirst(body))
                {
                    table[head][s].Add(production);
                }

                if(this._grammar.IsNullable(body))
                {
                    foreach(Token s in followSet[head])
                    {
                        table[head][s].Add(production);
                    }
                }
            }

            return table;
        }
    }
}
