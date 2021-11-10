using System.Collections.Generic;

namespace CNFDotnet.Analysis.Parsing
{
    //A parsing table is a list of actions
    public interface IParsingTable<out TAction> : IEnumerable<TAction>
        where TAction : class, IAction
    {
        int Count { get; }
    }
}
