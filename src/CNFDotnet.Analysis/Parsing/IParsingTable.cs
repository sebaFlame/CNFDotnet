using System;
using System.Collections.Generic;

namespace CNFDotnet.Analysis.Parsing
{
    public interface IParsingTable<out TAction> : IEnumerable<TAction>
        where TAction : class, IAction
    { }
}
