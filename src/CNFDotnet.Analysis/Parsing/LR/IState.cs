using System;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public interface IState<out TKernelItem>
        : IEquatable<IState<BaseLR0KernelItem>>
        where TKernelItem : BaseLR0KernelItem, IEquatable<TKernelItem>
    {
        IKernel<TKernelItem> Kernel { get; }
        IKernel<TKernelItem> Items { get; }

        void AddKernel(BaseLR0KernelItem item);
        void AddItem(BaseLR0KernelItem item);
    }
}

