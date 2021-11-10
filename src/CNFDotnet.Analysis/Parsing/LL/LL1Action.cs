using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing.LL
{
    //Represents an action in an LL(1) parsing table, consisting of a terminal,
    //non-terminal and a production to compute the next item in the parsing
    //table
    public class LL1Action : IAction
    {
        public Token NonTerminal { get; private set; }
        public Token Terminal { get; private set; }
        public Production Production { get; private set; }

        public LL1Action
            (Token nonTerminal, Token terminal, Production production)
        {
            this.NonTerminal = nonTerminal;
            this.Terminal = terminal;
            this.Production = production;
        }
    }
}
