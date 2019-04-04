using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace formelemethoden
{
    public class Transition<T>: IEquatable<Transition<T>> where T :  IComparable<T>
    {
        public const char EPSILON = '$';

        public T fromState { get;}
        public T toState { get;}
        public char symbol { get;}

        public Transition(T fromOrTo, char s): this(fromOrTo, s, fromOrTo)
        {
            
        }

        public Transition(T from, T to): this(from, EPSILON, to)
        {

        }

        public Transition(T from, char s, T to)
        {
            this.fromState = from;
            this.toState = to;
            this.symbol = s;
        }

        public T GetfromState()
        {
            return this.fromState;
        }

        public T GetToState()
        {
            return this.toState;
        }

        public char GetSymbol()
        {
            return this.symbol;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Transition<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EqualityComparer<T>.Default.GetHashCode(fromState);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(toState);
                hashCode = (hashCode * 397) ^ symbol.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(Transition<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(fromState, other.fromState) && EqualityComparer<T>.Default.Equals(toState, other.toState) && symbol == other.symbol;
        }

        public override string ToString()
        {
            return "(" + this.fromState + ", " + this.symbol+ ")" + "-->" + this.toState;
        }
    }
}