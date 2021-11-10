
using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing
{
    //Base class representing a type of parser, consisting of the (CNF) grammar
    //and a parsing table.
    public abstract class BaseParsing<TAction> : IParsing<TAction>
        where TAction : class, IAction
    {
        public CNFGrammar CNFGrammar { get; private set; }
        public ParsingTable<TAction> ParsingTable { get; protected set; }

        protected BaseParsing(CNFGrammar cnfGrammar)
        {
            this.CNFGrammar = cnfGrammar;
        }

        /// Classify a grammar as this type of parser
        public abstract void Classify();

        /// Generate the parsing table for this type of parser
        public abstract IParsingTable<TAction> CreateParsingTable();
    }
}
