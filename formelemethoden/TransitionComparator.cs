using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace formelemethoden
{
    public class TransitionComparator<T> : IComparer<Transition<T>> where T : IComparable<T>

    {
        public int Compare(Transition<T> x, Transition<T> y)
        {
            int fromCmp = x.fromState.CompareTo(y.fromState);
            int symbolCmp = x.symbol.CompareTo(y.symbol);
            int toCmp = x.toState.CompareTo(y.toState);

            return (fromCmp != 0 ? fromCmp : (symbolCmp != 0 ? symbolCmp : toCmp));
        }
    }
}