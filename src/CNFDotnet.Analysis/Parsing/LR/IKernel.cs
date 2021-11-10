using System;
using System.Collections.Generic;

namespace CNFDotnet.Analysis.Parsing.LR
{
    public interface IKernel<out TKernelItem> : IEnumerable<TKernelItem>
        where TKernelItem : BaseLR0KernelItem, IEquatable<TKernelItem>
    {
        int Count { get; }

        //int IndexOf(BaseLR0KernelItem item);
        //bool Equals(IKernel<BaseLR0KernelItem> kernel);
    }

    public static class KernelExtensions
    {
        public static bool OrderlessSequenceEqual<TKernelItem>
            (this IKernel<TKernelItem> left, IKernel<TKernelItem> right)
            where TKernelItem : BaseLR0KernelItem, IEquatable<TKernelItem>
        {
            if(left.Count != right.Count)
            {
                return false;
            }

            IEquatable<TKernelItem> current;

            using IEnumerator<TKernelItem> leftEnum = left.GetEnumerator();
            using IEnumerator<TKernelItem> rightEnum = right.GetEnumerator();

            while(leftEnum.MoveNext())
            {
                current = leftEnum.Current;

                while(rightEnum.MoveNext())
                {
                    if(current.Equals(rightEnum.Current))
                    {
                        break;
                    }
                }

                //not found
                if(!current.Equals(rightEnum.Current))
                {
                    return false;
                }

                rightEnum.Reset();
            }

            return true;
        }

        public static bool Contains<TKernelItemSource, TKernelItemTarget>
            (
                this IKernel<TKernelItemSource> kernel,
                TKernelItemTarget item,
                out TKernelItemSource found
            )
            where TKernelItemTarget : BaseLR0KernelItem,
                  IEquatable<TKernelItemTarget>
            where TKernelItemSource : TKernelItemTarget,
                IEquatable<TKernelItemSource>
        {
            TKernelItemSource currentItem;

            using(IEnumerator<TKernelItemSource> enumerator
                  = kernel.GetEnumerator())
            {
                while(enumerator.MoveNext())
                {
                    currentItem = enumerator.Current;

                    if(currentItem is null)
                    {
                        continue;
                    }

                    if(item.Equals(currentItem))
                    {
                        found = currentItem;
                        return true;
                    }
                }
            }

            found = null;
            return false;
        }
    }
}
