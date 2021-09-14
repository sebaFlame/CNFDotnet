using System;

using CNFDotnet.Analysis.Grammar;

namespace CNFDotnet.Analysis.Parsing
{
    public interface IParsing<out TAction>
        where TAction : class, IAction
    {
        CNFGrammar CNFGrammar { get; }
        void Classify ();
        IParsingTable<TAction> CreateParsingTable ();
    }
}
