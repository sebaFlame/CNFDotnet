using System;
using System.Collections.Generic;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public interface IAutomaton<out TKernelItem>
        : IEnumerable<IState<TKernelItem>>
        where TKernelItem : BaseLR0KernelItem, IEquatable<TKernelItem>
    {
        int Count { get; }

        int IndexOf(IState<BaseLR0KernelItem> item);
    }
}
