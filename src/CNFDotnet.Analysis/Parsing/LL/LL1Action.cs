using System;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LL
{
    public class LL1Action : IAction
    {
        public Token NonTerminal { get; private set; }
        public Token Terminal { get; private set; }
        public Production Production { get; private set; }

        public LL1Action (Token nonTerminal, Token terminal, Production production)
        {
            this.NonTerminal = nonTerminal;
            this.Terminal = terminal;
            this.Production = production;
        }
    }
}
