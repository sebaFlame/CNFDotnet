using System;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing
{
    public abstract class BaseParsing<TAction> : IParsing<TAction>
        where TAction : class, IAction
    {
        public CNFGrammar CNFGrammar { get; private set; }
        public ParsingTable<TAction> ParsingTable { get; protected set; }

        protected BaseParsing (CNFGrammar cnfGrammar)
        {
            this.CNFGrammar = cnfGrammar;
        }

        public abstract void Classify ();

        public abstract IParsingTable<TAction> CreateParsingTable ();
    }
}
